#!/usr/bin/env bash

echo "Building..."

# TODO run the build script on client

echo "Server: Compiling TypeScript into JavaScript..."
npm run build:compile

echo "Build Complete!"