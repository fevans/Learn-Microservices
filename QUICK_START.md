# Quick Start Guide

## Architecture Overview
```
┌─────────────────────────────────────────────────────┐
│           Local Development Environment             │
├─────────────────────────────────────────────────────┤
│                                                     │
│  Terminal 1: Identity.Service                       │
│  └─ https://localhost:5005                          │
│     └─ Connects to: MongoDB, RabbitMQ               │
│                                                     │
│  Terminal 2: Catalog.Service                        │
│  └─ https://localhost:5001                          │
│     └─ Connects to: MongoDB, RabbitMQ               │
│     └─ Validates tokens with: Identity.Service     │
│                                                     │
│  Terminal 3: Inventory.Service                      │
│  └─ https://localhost:5002                          │
│     └─ Connects to: MongoDB, RabbitMQ               │
│     └─ Validates tokens with: Identity.Service     │
│                                                     │
│  Docker Container: MongoDB                          │
│  └─ localhost:27017                                 │
│                                                     │
│  Docker Container: RabbitMQ                         │
│  └─ AMQP: localhost:5672                            │
│  └─ UI: http://localhost:15672                      │
│                                                     │
└─────────────────────────────────────────────────────┘
```

## 🚀 Quick Start (5 minutes)

### Step 1: Start Infrastructure (Terminal)
```bash
cd /Users/effevans/Dev2/learn-microservices
docker-compose up -d
```
✅ MongoDB and RabbitMQ now running in containers

### Step 2: Start Services

**Terminal 1 - Identity Server (Auth)**
```bash
cd src/Identity.Service && dotnet run
# Listens on https://localhost:5005
```

**Terminal 2 - Catalog Service**
```bash
cd src/Catalog.Service && dotnet run
# Listens on https://localhost:5001
```

**Terminal 3 - Inventory Service**
```bash
cd src/Inventory.Service && dotnet run
# Listens on https://localhost:5002
```

## 🔐 How Authentication Works

1. **Get Token** (in Postman or curl):
   ```
   POST https://localhost:5005/connect/token
   Content-Type: application/x-www-form-urlencoded
   
   grant_type=password&username=user@example.com&password=password&scope=catalog
   ```

2. **Use Token** (to call Catalog Service):
   ```
   GET https://localhost:5001/api/items
   Authorization: Bearer {your_token_here}
   ```

3. **Token Validation Flow**:
   - Catalog.Service receives request with token
   - Checks token issuer from metadata
   - Validates with Identity.Service at https://localhost:5005 ✓

## ✅ Verification Checklist

Run these commands to verify everything is working:

```bash
# Check containers
docker-compose ps

# Check MongoDB connection
telnet localhost 27017

# Check RabbitMQ connection
telnet localhost 5672

# Check Identity.Service health
curl -k https://localhost:5005/.well-known/openid-configuration

# Check Catalog.Service health
curl -k https://localhost:5001/swagger/ui

# Check Inventory.Service health
curl -k https://localhost:5002/swagger/ui
```

## 📝 Configuration Summary

| Component | URL | Type | Status |
|-----------|-----|------|--------|
| Identity.Service | https://localhost:5005 | Local | Running in Terminal 1 |
| Catalog.Service | https://localhost:5001 | Local | Running in Terminal 2 |
| Inventory.Service | https://localhost:5002 | Local | Running in Terminal 3 |
| MongoDB | localhost:27017 | Container | Running in Docker |
| RabbitMQ | localhost:5672 | Container | Running in Docker |
| RabbitMQ UI | http://localhost:15672 | Container | Running in Docker |

## 🔧 Common Issues

### "Invalid Issuer" Error
✅ **Fixed** - All services now connect to Identity.Service at `https://localhost:5005`

### Port Already in Use
```bash
# Find process using port
lsof -i :5005  # or :5001, :5002

# Kill it
kill -9 <PID>
```

### MongoDB Connection Issues
```bash
# Verify container is running
docker-compose ps mongodb

# Restart if needed
docker-compose restart mongodb
```

### HTTPS Certificate Issues
```bash
# Trust development certificate
dotnet dev-certs https --trust
```

## 📚 Useful Links

- **Swagger UI:**
  - Identity: https://localhost:5005/swagger
  - Catalog: https://localhost:5001/swagger
  - Inventory: https://localhost:5002/swagger

- **RabbitMQ Management:**
  - http://localhost:15672
  - Username: guest
  - Password: guest

- **MongoDB Connection String:**
  - `mongodb://localhost:27017`

## 🛑 Shutdown

```bash
# Stop all services
Ctrl+C in each terminal

# Stop containers
docker-compose down

# Clean everything (warning: removes volumes)
docker-compose down -v
```

## 📖 For More Details
See `SETUP_GUIDE.md` for comprehensive documentation including:
- Step-by-step setup
- Configuration details
- Troubleshooting guide
- Authentication flow details

