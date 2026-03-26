@echo off
REM Microservices Startup Script for Windows
REM This script starts Docker containers

setlocal enabledelayedexpansion

echo ===============================================
echo Microservices Environment - Startup Script
echo ===============================================
echo.

REM Check if Docker is running
docker ps >nul 2>&1
if errorlevel 1 (
    echo ERROR: Docker is not running. Please start Docker Desktop.
    pause
    exit /b 1
)

echo [OK] Docker is running
echo.

REM Get the project directory
set PROJECT_DIR=%~dp0

REM Start Docker containers
echo Starting Docker containers...
docker-compose up -d

REM Wait for containers to start
timeout /t 10 /nobreak

REM Check if containers are running
docker-compose ps | findstr "mongodb" >nul
if errorlevel 1 (
    echo ERROR: Failed to start Docker containers
    docker-compose logs
    pause
    exit /b 1
)

echo.
echo ===============================================
echo DOCKER CONTAINERS STARTED
echo ===============================================
echo.
docker-compose ps
echo.

echo ===============================================
echo NEXT STEPS
echo ===============================================
echo.
echo Open 3 new terminals/PowerShell windows and run:
echo.
echo Terminal 1 - Identity.Service:
echo   cd "%PROJECT_DIR%src\Identity.Service"
echo   dotnet run
echo   (Runs on https://localhost:5005)
echo.
echo Terminal 2 - Catalog.Service:
echo   cd "%PROJECT_DIR%src\Catalog.Service"
echo   dotnet run
echo   (Runs on https://localhost:5001)
echo.
echo Terminal 3 - Inventory.Service:
echo   cd "%PROJECT_DIR%src\Inventory.Service"
echo   dotnet run
echo   (Runs on https://localhost:5002)
echo.

echo ===============================================
echo USEFUL LINKS
echo ===============================================
echo RabbitMQ Management: http://localhost:15672
echo Username: guest
echo Password: guest
echo.

pause

