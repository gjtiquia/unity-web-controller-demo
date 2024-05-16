import "./style.css"

console.log("Client Running...")

console.log("Creating socket...");
let socket = new WebSocket("ws://localhost:3000/");

socket.onopen = function () {
    console.log("Socket open.");

    // TODO : Better send JSON messages instead
    socket.send("Client socket opened!");

    console.log("Message sent.")
};

socket.onmessage = function (message) {
    console.log("Received server message:", message.data);
};

const buttonElement = document.getElementById("message-button");
if (!buttonElement) {
    throw new Error("No element with id 'message-button'!");
}

buttonElement.addEventListener("click", () => {

    // TODO : Better send JSON messages instead
    socket.send("Client button clicked!");

    console.log("Message sent with button.")
});
