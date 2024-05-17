# Unity Web Controller Demo

A demo project demonstrating using controllers on mobile browsers controlling characters in a Unity game.

Essentially a "local multiplayer game" but communicating over "non-local" network.

Inspired by HappyFunTimes.net. While HappyFunTimes.net is designed to work over the local network, this project demonstrates the feasibility of using mobile data to connect to the Unity game, without the need for all clients to connect to the same Wi-Fi.

## Prerequisites

### NodeJS

NodeJS is used for building the web client and running the web server.

Run the following command to check if you have NodeJS installed.

```bash
node -v
```

If you do not have NodeJS installed, it is recommended to install NodeJS via [nvm: Node Version Manager](https://github.com/nvm-sh/nvm). Here is a [guide for installing on Windows, Mac or Linux](https://www.freecodecamp.org/news/node-version-manager-nvm-install-guide/)


### Ngrok

Ngrok is used to forward your local port to a public URL.

Run the following command to check if you have Ngrok installed.

```bash
ngrok help
```

If you do not have Ngrok installed, you may reference their [Quick Start guide to install it on Windows, Mac or Linux](https://ngrok.com/docs/getting-started/).


### Unity

Ensure you have Unity installed. The Unity Editor version used for this demo is `2022.3.21f1`.


## Commands to run the demo

Run the following commands in a new terminal to build the client web app.

```bash

# Enter the /client directory
cd client

# Install dependencies
npm install

# Build the HTML, CSS and JavaScript to be served from the web server
npm run build

```

Run the following commands in a new terminal to build and run the web server.

```bash

# Enter the /server directory
cd server

# Install dependencies
npm install

# Compiles the code into JavaScript
npm run build

# Starts the web server on port 3000
npm run start
```

Run the following commands in a new terminal to forward port 3000 to a URL generated by Ngrok.

```bash
ngrok http 3000
```


Open the Unity project in `/unity` and enter Play Mode.

Enter the URL generated by Ngrok on any mobile device. You should now see the web controller on your mobile device and can control your character in Unity.

## Development Commands

### Client

Client is developed with Vite.

```bash
cd client

# Runs the development server with hot reloading
npm run dev
```

Run the build command to update the static files to be served by the server.

```bash
# Builds into the /dist directory, which is served by the server
npm run build
```

### Server

Server is a TypeScript Express project.

```bash
cd server

# Compiles the TypeScript into JavaScript and runs the Express server
# Watches for any changes to the source files, and re-compiles and re-run the server if necessary
npm run dev
```

## TODO

- [x] Basic server and client websocket connection.
- [x] Test ngrok
- [x] Assign client ID
- [x] Refactor to communicate over JSON
- [x] Basic Unity project
- [x] Websocket setup in Unity
- [x] Test over ngrok
- [ ] Simple platformer
- [ ] OnConnect / OnDisconnect handling
- [ ] Cache network input
- [ ] Update docs
