import path from "path"
import http from "http"
import express from "express"
import ws from "ws"
import { generateID } from "./utils"
import { WebSocketMessage } from "./types"

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

type CustomWs = ws & { id: string }

// Listen to WebSocket events
wss.on('connection', (ws: CustomWs) => {

    ws.id = generateID();
    console.log(`${ws.id} connected`)

    ws.on('message', (rawData) => {
        try {
            const message: WebSocketMessage = JSON.parse(rawData.toString());
            console.log(`${ws.id} message:`, message);

            if (message.origin === "unity") {
                // TODO : if message === connected, save unity ws externally (as an array)
            }

            if (message.origin === "web-controller") {
                // TODO : Relay to unity ws
            }
        }
        catch (e: unknown) {
            console.error(e);
            console.error("rawData:", rawData.toString());
        }
    });

    ws.on('close', () => {
        console.log(`${ws.id} disconnected`);
    });

    ws.on('error', console.error);
});

// Start listening on PORT
server.listen(PORT, () => {
    console.log(`Server listening on port ${PORT}`)
});