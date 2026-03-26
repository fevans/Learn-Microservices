#!/bin/bash

# Start Inventory.Service
echo "🚀 Starting Inventory.Service (https://localhost:5002)"
echo "=================================================="
echo ""
echo "This service:"
echo "  • Runs on port 5002 (HTTPS)"
echo "  • Provides inventory API"
echo "  • Requires valid JWT token from Identity.Service"
echo "  • Connects to MongoDB and RabbitMQ"
echo "  • Listens to Catalog events via RabbitMQ"
echo ""
echo "Prerequisites:"
echo "  • MongoDB must be running (docker-compose up -d)"
echo "  • RabbitMQ must be running (docker-compose up -d)"
echo "  • Identity.Service must be running (on port 5005)"
echo "  • .NET SDK installed"
echo ""
echo "Press Ctrl+C to stop the service"
echo ""
echo "=================================================="
echo ""

PROJECT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$PROJECT_DIR/src/Inventory.Service"

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET SDK is not installed"
    exit 1
fi

# Run the service
dotnet run

