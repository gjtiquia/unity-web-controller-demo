import path from "path"
import http from "http"
import express from "express"
import ws from "ws"

const app = express()
const PORT = 3000

// Global Middleware
// app.use(cors()); // Enable all origins

// Static File Server
const publicFilesPath = path.join(__dirname, "..", "..", "client", "dist")
app.use(express.static(publicFilesPath))

let WSServer = ws.Server;
let server = http.createServer();
let wss = new WSServer({
    server: server,
    perMessageDeflate: false
})

// Mount Express App on HTTP Server
server.on('request', app);

// Listen to WebSocket events
wss.on('connection', function connection(ws) {
    console.log("Client connected!")

    ws.on('message', function incoming(message) {
        console.log(`Message received: ${message}`);

        // TODO : send JSON messages instead for extensibility
        ws.send(`Server received the message '${message}'`);
    });
});

// Start listening on PORT
server.listen(PORT, function () {
    console.log(`Example app listening on port ${PORT}`)
});