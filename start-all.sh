#!/bin/bash

# Microservices Startup Script
# This script starts all required services

echo "🚀 Starting Microservices Environment"
echo "===================================="
echo ""

# Color codes for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

PROJECT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"

# Function to print section headers
print_header() {
    echo -e "${BLUE}>>> $1${NC}"
}

# Function to print success messages
print_success() {
    echo -e "${GREEN}✅ $1${NC}"
}

# Function to print error messages
print_error() {
    echo -e "${RED}❌ $1${NC}"
}

# Function to print warning messages
print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

# Step 1: Check prerequisites
print_header "Checking Prerequisites"

# Check Docker
if ! command -v docker &> /dev/null; then
    print_error "Docker is not installed"
    exit 1
fi
print_success "Docker is installed"

# Check Docker Compose
if ! command -v docker-compose &> /dev/null; then
    print_error "Docker Compose is not installed"
    exit 1
fi
print_success "Docker Compose is installed"

# Check .NET SDK
if ! command -v dotnet &> /dev/null; then
    print_error ".NET SDK is not installed"
    exit 1
fi
print_success ".NET SDK is installed"

echo ""

# Step 2: Start Docker containers
print_header "Starting Docker Containers"

cd "$PROJECT_DIR"

# Check if containers are already running
if docker-compose ps | grep -q "Up"; then
    print_warning "Some containers are already running"
    read -p "Do you want to restart them? (y/n) " -n 1 -r
    echo
    if [[ $REPLY =~ ^[Yy]$ ]]; then
        docker-compose down
        print_success "Stopped existing containers"
    fi
fi

# Start containers
docker-compose up -d

# Wait for containers to be healthy
print_warning "Waiting for containers to start (10 seconds)..."
sleep 10

# Verify containers are running
if docker-compose ps | grep -q "mongodb.*Up" && docker-compose ps | grep -q "rabbitmq.*Up"; then
    print_success "Docker containers are running"
else
    print_error "Failed to start Docker containers"
    docker-compose logs
    exit 1
fi

echo ""

# Step 3: Display startup instructions for services
print_header "Services Ready to Start"
echo ""
echo "The Docker containers are running. Now start the .NET services in separate terminals:"
echo ""
echo -e "${YELLOW}Terminal 1 - Identity.Service (Auth Server)${NC}"
echo "  cd $PROJECT_DIR/src/Identity.Service && dotnet run"
echo "  Runs on: https://localhost:5005"
echo ""
echo -e "${YELLOW}Terminal 2 - Catalog.Service${NC}"
echo "  cd $PROJECT_DIR/src/Catalog.Service && dotnet run"
echo "  Runs on: https://localhost:5001"
echo ""
echo -e "${YELLOW}Terminal 3 - Inventory.Service${NC}"
echo "  cd $PROJECT_DIR/src/Inventory.Service && dotnet run"
echo "  Runs on: https://localhost:5002"
echo ""

# Step 4: Provide useful links
echo -e "${BLUE}Useful Links:${NC}"
echo ""
print_success "RabbitMQ Management UI: http://localhost:15672 (guest/guest)"
echo ""

# Step 5: Show health check options
echo -e "${BLUE}Verify Everything is Working:${NC}"
echo ""
echo "Run these commands in a new terminal:"
echo ""
echo "  # Check MongoDB"
echo "  telnet localhost 27017"
echo ""
echo "  # Check RabbitMQ"
echo "  telnet localhost 5672"
echo ""
echo "  # Check Identity.Service"
echo "  curl -k https://localhost:5005/.well-known/openid-configuration"
echo ""

# Step 6: Display status
echo ""
print_success "Docker containers started successfully!"
echo ""
echo -e "${BLUE}Container Status:${NC}"
docker-compose ps

echo ""
print_header "✨ Ready to go! Start the .NET services in separate terminals."
echo ""

