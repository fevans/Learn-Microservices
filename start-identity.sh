#!/bin/bash

# Start Identity.Service
echo "🚀 Starting Identity.Service (https://localhost:5005)"
echo "=================================================="
echo ""
echo "This service:"
echo "  • Runs on port 5005 (HTTPS)"
echo "  • Acts as the authentication/authorization server"
echo "  • Issues JWT tokens"
echo "  • Manages users"
echo ""
echo "Prerequisites:"
echo "  • MongoDB must be running (docker-compose up -d)"
echo "  • .NET SDK installed"
echo ""
echo "Press Ctrl+C to stop the service"
echo ""
echo "=================================================="
echo ""

PROJECT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$PROJECT_DIR/src/Identity.Service"

# Check if dotnet is available
if ! command -v dotnet &> /dev/null; then
    echo "ERROR: .NET SDK is not installed"
    exit 1
fi

# Run the service
dotnet run

