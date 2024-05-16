import path from "path"
import express from "express"

const app = express()
const port = 3000

// Global Middleware
// app.use(cors()); // Enable all origins

// Static File Server
const publicFilesPath = path.join(__dirname, "..", "..", "client", "dist")
app.use(express.static(publicFilesPath))

app.listen(port, () => {
    console.log(`Example app listening on port ${port}`)
})