export type WebSocketMessage = {
    origin: "unity" | "web-controller",
    message: string,
}

export type RelayMessage = {
    id: string,
    message: string,
}