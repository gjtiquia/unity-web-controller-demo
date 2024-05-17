import crypto from "crypto"

export function generateID() {
    // return crypto.randomBytes(16).toString("hex");
    return crypto.randomUUID();
}