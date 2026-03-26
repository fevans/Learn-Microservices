#!/bin/bash

# Health Check Script for Microservices

echo "🔍 Microservices Health Check"
echo "============================="
echo ""

# Color codes
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m'

PROJECT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$PROJECT_DIR"

# Test function
test_service() {
    local name=$1
    local port=$2
    local endpoint=$3
    local use_https=$4

    if [ "$use_https" = "true" ]; then
        response=$(curl -s -k -o /dev/null -w "%{http_code}" "https://localhost:$port$endpoint" 2>/dev/null)
    else
        response=$(curl -s -o /dev/null -w "%{http_code}" "http://localhost:$port$endpoint" 2>/dev/null)
    fi

    if [ "$response" = "200" ] || [ "$response" = "302" ] || [ "$response" = "401" ]; then
        echo -e "${GREEN}✅${NC} $name (Port $port) - OK"
        return 0
    else
        echo -e "${RED}❌${NC} $name (Port $port) - FAILED (HTTP $response)"
        return 1
    fi
}

# Test function for TCP connection
test_tcp() {
    local name=$1
    local host=$2
    local port=$3

    timeout 2 bash -c "</dev/tcp/$host/$port" 2>/dev/null
    if [ $? -eq 0 ]; then
        echo -e "${GREEN}✅${NC} $name ($host:$port) - OK"
        return 0
    else
        echo -e "${RED}❌${NC} $name ($host:$port) - FAILED"
        return 1
    fi
}

echo -e "${BLUE}1. Checking Docker Containers${NC}"
echo ""

if ! command -v docker &> /dev/null; then
    echo -e "${RED}❌${NC} Docker not installed"
else
    if docker ps &>/dev/null; then
        echo -e "${GREEN}✅${NC} Docker is running"
        
        echo ""
        echo "Container Status:"
        docker-compose ps
        echo ""
        
        # Check MongoDB
        test_tcp "MongoDB" "localhost" "27017"
        
        # Check RabbitMQ
        test_tcp "RabbitMQ (AMQP)" "localhost" "5672"
    else
        echo -e "${RED}❌${NC} Docker daemon not responding"
    fi
fi

echo ""
echo -e "${BLUE}2. Checking .NET Services${NC}"
echo ""

# Check Identity.Service
test_service "Identity.Service" "5005" "/.well-known/openid-configuration" "true"

# Check Catalog.Service
test_service "Catalog.Service" "5001" "/swagger/ui" "true"

# Check Inventory.Service
test_service "Inventory.Service" "5002" "/swagger/ui" "true"

echo ""
echo -e "${BLUE}3. Checking Infrastructure Services${NC}"
echo ""

# Check RabbitMQ Management UI
test_service "RabbitMQ Management UI" "15672" "/" "false"

echo ""
echo -e "${BLUE}4. Summary${NC}"
echo ""

# Count healthy services
healthy=0
total=6

test_service "Identity.Service" "5005" "/.well-known/openid-configuration" "true" && ((healthy++)) || true
test_service "Catalog.Service" "5001" "/swagger/ui" "true" && ((healthy++)) || true
test_service "Inventory.Service" "5002" "/swagger/ui" "true" && ((healthy++)) || true
test_tcp "MongoDB" "localhost" "27017" && ((healthy++)) || true
test_tcp "RabbitMQ" "localhost" "5672" && ((healthy++)) || true
test_service "RabbitMQ UI" "15672" "/" "false" && ((healthy++)) || true

echo ""
echo "Services Healthy: $healthy/$total"

if [ $healthy -eq $total ]; then
    echo -e "${GREEN}✅ All services are healthy!${NC}"
    exit 0
else
    echo -e "${YELLOW}⚠️  Some services are not responding${NC}"
    echo ""
    echo "Troubleshooting tips:"
    echo "  1. Ensure Docker containers are running: docker-compose up -d"
    echo "  2. Ensure .NET services are started in separate terminals"
    echo "  3. Check service logs for errors"
    echo "  4. See TROUBLESHOOTING.md for more help"
    exit 1
fi

