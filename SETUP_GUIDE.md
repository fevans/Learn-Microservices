# Microservices Setup Guide

## Overview
This project uses a mixed deployment model where only MongoDB and RabbitMQ run in Docker containers, while the .NET services run locally.

## Service Configuration

| Service | URL | Role | Deployment |
|---------|-----|------|-----------|
| Identity.Service | https://localhost:5005 | Auth Server + Users API | Local (.NET) |
| Catalog.Service | https://localhost:5001 | Protected Resource Server | Local (.NET) |
| Inventory.Service | https://localhost:5002 | Protected Resource Server | Local (.NET) |
| MongoDB | localhost:27017 | Data store | Docker Container |
| RabbitMQ | localhost:5672 / 15672 | Message broker | Docker Container |

## Prerequisites
- .NET 8 SDK or later
- Docker & Docker Compose
- Node.js (for frontend)

## Startup Instructions

### Step 1: Start Docker Containers
```bash
cd /Users/effevans/Dev2/learn-microservices
docker-compose up -d
```

This will start:
- **MongoDB** on port 27017
- **RabbitMQ** on ports 5672 (AMQP) and 15672 (Management UI)

Verify containers are running:
```bash
docker-compose ps
```

### Step 2: Start Services Locally

Open separate terminals for each service:

#### Terminal 1: Identity.Service (Auth Server)
```bash
cd /Users/effevans/Dev2/learn-microservices/src/Identity.Service
dotnet run
```
- Runs on: **https://localhost:5005**
- Swagger: https://localhost:5005/swagger/index.html

#### Terminal 2: Catalog.Service
```bash
cd /Users/effevans/Dev2/learn-microservices/src/Catalog.Service
dotnet run
```
- Runs on: **https://localhost:5001**
- Swagger: https://localhost:5001/swagger/index.html

#### Terminal 3: Inventory.Service
```bash
cd /Users/effevans/Dev2/learn-microservices/src/Inventory.Service
dotnet run
```
- Runs on: **https://localhost:5002**
- Swagger: https://localhost:5002/swagger/index.html

### Step 3: Start Frontend (Optional)
```bash
cd /Users/effevans/Dev2/learn-microservices/src/frontend
npm install
npm start
```
- Runs on: **http://localhost:3000**

## Configuration Details

### MongoDB
- **Connection String (from services):** `mongodb://localhost:27017`
- **Databases:**
  - `IdentityDb` - Identity.Service
  - `CatalogDb` - Catalog.Service
  - `Inventory` - Inventory.Service

### RabbitMQ
- **AMQP Connection:** `amqp://guest:guest@localhost:5672/`
- **Management UI:** http://localhost:15672
- **Credentials:** 
  - Username: `guest`
  - Password: `guest`

### Authentication Flow
1. **Obtain Token:** POST to `https://localhost:5005/connect/token` with credentials
2. **Use Token:** Include in Authorization header: `Bearer {token}`
3. **Validation:** Catalog.Service and Inventory.Service validate tokens with Identity.Service at `https://localhost:5005`

## Key appsettings Configuration

All services are configured with:
- **Authority:** `https://localhost:5005` (Identity.Service endpoint)
- **MongoDB Host:** `localhost`
- **RabbitMQ Host:** `localhost`
- **AllowedOrigins:** `http://localhost:3000` (Frontend)

## Troubleshooting

### "Invalid Issuer" Error
- Ensure Identity.Service is running on `https://localhost:5005`
- Verify all services' `appsettings.json` has `Authority: "https://localhost:5005"`
- Services validate the issuer from the Discovery endpoint

### MongoDB Connection Issues
```bash
# Check if MongoDB is running
docker-compose ps

# Verify connection
telnet localhost 27017
```

### RabbitMQ Connection Issues
```bash
# Check RabbitMQ health
docker-compose exec rabbitmq rabbitmq-diagnostics ping

# Access Management UI
# http://localhost:15672 (guest/guest)
```

### Service Won't Start
1. Check if ports are already in use:
   ```bash
   lsof -i :5005  # Identity.Service
   lsof -i :5001  # Catalog.Service
   lsof -i :5002  # Inventory.Service
   ```
2. Kill process if needed: `kill -9 <PID>`

## Development Certificate Setup (if needed)

If you encounter HTTPS certificate issues:

```bash
# Create development certificate (macOS)
dotnet dev-certs https --clean
dotnet dev-certs https --trust
```

## Cleanup

### Stop Services
- Press Ctrl+C in each service terminal

### Stop Containers
```bash
docker-compose down
```

### Remove Volumes (Clean slate)
```bash
docker-compose down -v
```

## Additional Resources

- [RabbitMQ Management UI](http://localhost:15672)
- Identity.Service Swagger: https://localhost:5005/swagger
- Catalog.Service Swagger: https://localhost:5001/swagger
- Inventory.Service Swagger: https://localhost:5002/swagger

