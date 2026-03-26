#!/bin/bash

# Cleanup Script for Microservices
# This script helps clean up containers, volumes, and build artifacts

echo "🧹 Microservices Cleanup Utility"
echo "================================"
echo ""

RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m'

PROJECT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
cd "$PROJECT_DIR"

# Display options
echo "What would you like to clean up?"
echo ""
echo "1) Stop Docker containers (keeps data)"
echo "2) Stop and remove Docker containers (keeps data)"
echo "3) Remove Docker containers and volumes (DELETES DATA)"
echo "4) Remove .NET build artifacts"
echo "5) Full clean (containers, volumes, and build artifacts)"
echo "6) Exit"
echo ""
read -p "Choose option (1-6): " choice

case $choice in
    1)
        echo ""
        echo -e "${YELLOW}Stopping Docker containers...${NC}"
        docker-compose stop
        echo -e "${GREEN}✅ Containers stopped${NC}"
        echo ""
        echo "To start again: docker-compose up -d"
        ;;
    2)
        echo ""
        echo -e "${YELLOW}Stopping and removing Docker containers...${NC}"
        docker-compose down
        echo -e "${GREEN}✅ Containers removed${NC}"
        echo ""
        echo "To start again: docker-compose up -d"
        ;;
    3)
        echo ""
        echo -e "${RED}WARNING: This will delete all volumes and data!${NC}"
        read -p "Are you sure? (yes/no): " confirm
        if [ "$confirm" = "yes" ]; then
            echo -e "${YELLOW}Removing Docker containers and volumes...${NC}"
            docker-compose down -v
            echo -e "${GREEN}✅ Containers and volumes removed${NC}"
            echo ""
            echo "Data has been deleted. To start fresh: docker-compose up -d"
        else
            echo "Cancelled"
        fi
        ;;
    4)
        echo ""
        echo -e "${YELLOW}Cleaning .NET build artifacts...${NC}"
        
        for service in Identity.Service Catalog.Service Inventory.Service GamePlatform.Common; do
            if [ -d "src/$service" ]; then
                cd "src/$service"
                echo "  Cleaning $service..."
                dotnet clean 2>/dev/null
                cd "$PROJECT_DIR"
            fi
        done
        
        echo -e "${GREEN}✅ Build artifacts cleaned${NC}"
        ;;
    5)
        echo ""
        echo -e "${RED}WARNING: This will perform a complete cleanup!${NC}"
        echo "This will:"
        echo "  • Remove Docker containers and volumes (DELETES DATA)"
        echo "  • Delete .NET build artifacts"
        echo ""
        read -p "Are you sure? (yes/no): " confirm
        if [ "$confirm" = "yes" ]; then
            echo ""
            echo -e "${YELLOW}Removing Docker containers and volumes...${NC}"
            docker-compose down -v
            echo -e "${GREEN}✅ Containers and volumes removed${NC}"
            
            echo ""
            echo -e "${YELLOW}Cleaning .NET build artifacts...${NC}"
            for service in Identity.Service Catalog.Service Inventory.Service GamePlatform.Common; do
                if [ -d "src/$service" ]; then
                    cd "src/$service"
                    echo "  Cleaning $service..."
                    dotnet clean 2>/dev/null
                    cd "$PROJECT_DIR"
                fi
            done
            echo -e "${GREEN}✅ Build artifacts cleaned${NC}"
            
            echo ""
            echo -e "${GREEN}✅ Full cleanup complete!${NC}"
            echo ""
            echo "Next steps:"
            echo "  1. docker-compose up -d     (Start containers)"
            echo "  2. dotnet build              (Rebuild services)"
            echo "  3. See QUICK_START.md"
        else
            echo "Cancelled"
        fi
        ;;
    6)
        echo "Exiting..."
        exit 0
        ;;
    *)
        echo -e "${RED}Invalid option${NC}"
        exit 1
        ;;
esac

echo ""

