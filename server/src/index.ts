import path from "path"
import http from "http"
import express from "express"
import ws from "ws"

const app = express()
const PORT = 3000

// Static File Server
const publicFilesPath = path.join(__dirname, "..", "..", "client", "dist")
app.use(express.static(publicFilesPath))

// Setup Web Socket
let WSServer = ws.Server;
let server = http.createServer();
let wss = new WSServer({
    server: server,

    // https://github.com/websockets/ws?tab=readme-ov-file#websocket-compression
    perMessageDeflate: false
})

// Mount Express App on HTTP Server
server.on('request', app);

// Global variables
// TODO : Map of client ID => client data ({ws, name})

// Listen to WebSocket events
wss.on('connection', (ws) => {

    // TODO : Generate ID
    console.log("Client connected!")

    ws.on('message', (message) => {
        // TODO : Should be in JSON. expect a field of origin = "unity" | "web-controller" => save "unity" websocket + relay the "web-controller" messages to "unity"
        console.log(`Message received: ${message}`);
    });

    ws.on('close', () => {
        console.log('Client disconnected!');
    });

    ws.on('error', console.error);
});

// Start listening on PORT
server.listen(PORT, () => {
    console.log(`Server listening on port ${PORT}`)
});