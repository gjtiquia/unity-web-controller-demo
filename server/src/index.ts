import path from "path"
import http from "http"
import express from "express"
import ws from "ws"
import { generateID } from "./utils"
import { RelayMessage, WebSocketMessage } from "./types"

const app = express()
const PORT = 3000

// Static File Server
const publicFilesPath = path.join(__dirname, "..", "..", "client", "dist")
app.use(express.static(publicFilesPath))

// Setup Web Socket
const WSServer = ws.Server;
const server = http.createServer();
const wss = new WSServer({
    server: server,

    // https://github.com/websockets/ws?tab=readme-ov-file#websocket-compression
    perMessageDeflate: false
})

// Mount Express App on HTTP Server
server.on('request', app);

// In-memory variables
const unityWebSockets: Map<string, ws> = new Map();

type CustomWs = ws & { id: string }

// Listen to WebSocket events
wss.on('connection', (ws: CustomWs) => {

    ws.id = generateID();
    console.log(`${ws.id} connected`)

    ws.on('message', (rawData) => {
        try {
            const parsedMessage: WebSocketMessage = JSON.parse(rawData.toString());
            console.log(`${ws.id} message:`, parsedMessage);

            // Unity client initialization
            if (parsedMessage.origin === "unity" && parsedMessage.message === "connected") {
                unityWebSockets.set(ws.id, ws);
                console.log(`${ws.id} added to Unity Web Sockets`);
            }

            // Relay message to Unity
            if (parsedMessage.origin === "web-controller") {
                unityWebSockets.forEach((unityWebSocket) => {
                    const relayMessage: RelayMessage = { id: ws.id, message: parsedMessage.message }
                    unityWebSocket.send(JSON.stringify(relayMessage));
                })
            }
        }
        catch (e: unknown) {
            console.error(e);
            console.error("rawData:", rawData.toString());
        }
    });

    ws.on('close', () => {
        console.log(`${ws.id} disconnected`);

        // Unity websocket disconnect handling
        if (unityWebSockets.has(ws.id)) {
            unityWebSockets.delete(ws.id);
            console.log(`${ws.id} removed from Unity Web Sockets`);
        }

        // Relay disconnnect message to Unity websocket
        else {
            unityWebSockets.forEach((unityWebSocket) => {
                const relayMessage: RelayMessage = { id: ws.id, message: "disconnected" }
                unityWebSocket.send(JSON.stringify(relayMessage));
            })
        }
    });

    ws.on('error', console.error);
});

// Start listening on PORT
server.listen(PORT, () => {
    console.log(`Server listening on port ${PORT}`)
});