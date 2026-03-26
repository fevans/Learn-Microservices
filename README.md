# 🎯 Learn Microservices - Complete Setup & Configuration

## Overview

This is a complete microservices learning platform with:
- **3 ASP.NET Core services** - Identity, Catalog, and Inventory
- **MongoDB** - Data persistence
- **RabbitMQ** - Event messaging
- **React Frontend** - User interface

All services run locally with only infrastructure (MongoDB, RabbitMQ) in Docker containers.

---

## ✅ Problem Solved

### Issue
```
Bearer error="invalid_token", 
error_description="The issuer 'https://localhost:5005' is invalid"
```

### Root Cause
Services running in Docker containers couldn't correctly validate tokens from Identity.Service at `https://localhost:5005` due to network hostname resolution issues.

### Solution
Removed all microservice containers. Services now run locally with direct access to infrastructure services. This eliminates network translation issues while maintaining separation of concerns.

---

## 🏗️ Architecture

```
┌─────────────────────────────────────────────────────────┐
│                LOCAL DEVELOPMENT MACHINE                 │
├─────────────────────────────────────────────────────────┤
│                                                         │
│  DOCKER CONTAINERS (docker-compose.yaml)               │
│  ├── MongoDB (localhost:27017)                          │
│  └── RabbitMQ (localhost:5672, 15672)                   │
│                                                         │
│  LOCAL .NET SERVICES (dotnet run)                       │
│  ├── Identity.Service (https://localhost:5005)          │
│  ├── Catalog.Service (https://localhost:5001)           │
│  └── Inventory.Service (https://localhost:5002)         │
│                                                         │
│  FRONTEND (npm start)                                   │
│  └── React App (http://localhost:3000)                  │
│                                                         │
└─────────────────────────────────────────────────────────┘
```

---

## 🚀 Quick Start (5 Minutes)

### Prerequisites
- Docker & Docker Compose
- .NET 8 SDK or later
- Node.js (for frontend)

### Step 1: Start Infrastructure
```bash
./start-all.sh
```
Or manually:
```bash
docker-compose up -d
```

### Step 2: Start Services (3 Separate Terminals)

**Terminal 1 - Identity.Service**
```bash
./start-identity.sh
```
Or manually:
```bash
cd src/Identity.Service && dotnet run
# Runs on https://localhost:5005
```

**Terminal 2 - Catalog.Service**
```bash
./start-catalog.sh
```
Or manually:
```bash
cd src/Catalog.Service && dotnet run
# Runs on https://localhost:5001
```

**Terminal 3 - Inventory.Service**
```bash
./start-inventory.sh
```
Or manually:
```bash
cd src/Inventory.Service && dotnet run
# Runs on https://localhost:5002
```

### Step 3: Verify Everything Works
```bash
./health-check.sh
```

---

## 📋 Service Configuration

| Service | URL | Role | Authentication |
|---------|-----|------|-----------------|
| **Identity.Service** | https://localhost:5005 | Auth server, User management | N/A (Issuer) |
| **Catalog.Service** | https://localhost:5001 | Product catalog API | Required (JWT) |
| **Inventory.Service** | https://localhost:5002 | Inventory management API | Required (JWT) |

---

## 🔐 Authentication Flow

1. **Authenticate with Identity.Service**
   ```bash
   POST https://localhost:5005/connect/token
   Content-Type: application/x-www-form-urlencoded
   
   grant_type=password&username=user@example.com&password=password&scope=catalog
   ```

2. **Use Token to Access Protected Services**
   ```bash
   GET https://localhost:5001/api/items
   Authorization: Bearer {jwt_token}
   ```

3. **Token Validation**
   - Catalog.Service receives request with token
   - Validates JWT signature and claims
   - Confirms issuer: `https://localhost:5005` ✅
   - Allows access if valid

---

## 📚 Helper Scripts

### start-all.sh
Start Docker containers and show setup instructions
```bash
./start-all.sh
```

### start-identity.sh, start-catalog.sh, start-inventory.sh
Start individual services
```bash
./start-identity.sh
./start-catalog.sh
./start-inventory.sh
```

### health-check.sh
Verify all services are running and healthy
```bash
./health-check.sh
```

### cleanup.sh
Clean up containers, volumes, or build artifacts
```bash
./cleanup.sh
```

---

## 📖 Documentation

### Quick References
- **[INDEX.md](INDEX.md)** - Documentation index and navigation
- **[QUICK_START.md](QUICK_START.md)** - 5-minute setup guide
- **[README.md](README.md)** - This file

### Comprehensive Guides
- **[SETUP_GUIDE.md](SETUP_GUIDE.md)** - Complete setup instructions with details
- **[TROUBLESHOOTING.md](TROUBLESHOOTING.md)** - Common issues and solutions

### Technical Reference
- **[CHANGES_SUMMARY.md](CHANGES_SUMMARY.md)** - What was changed and why
- **[VALIDATION_REPORT.md](VALIDATION_REPORT.md)** - Technical verification

---

## 🔧 Manual Configuration

### Docker Containers
**File:** `compose.yaml`
```yaml
services:
  mongodb:
    image: mongo
    ports:
      - "27017:27017"
  rabbitmq:
    image: rabbitmq:3-management
    ports:
      - "5672:5672"      # AMQP
      - "15672:15672"    # Management UI
```

### appsettings Configuration
All services configured with:
```json
{
  "ServiceSettings": {
    "Authority": "https://localhost:5005"
  },
  "MongoDbSettings": {
    "Host": "localhost",
    "Port": 27017
  },
  "RabbitMqSettings": {
    "Host": "localhost"
  }
}
```

### Launch Configuration
Services launch on:
- Identity.Service: `https://localhost:5005`
- Catalog.Service: `https://localhost:5001`
- Inventory.Service: `https://localhost:5002`

---

## 💾 Databases

### MongoDB Collections
- **Identity.Database** - Users, roles, claims
- **CatalogDb** - Products/items
- **Inventory** - Stock levels

### Connection String
```
mongodb://localhost:27017
```

---

## 📬 Message Broker

### RabbitMQ
- **AMQP Endpoint:** `amqp://localhost:5672/`
- **Management UI:** http://localhost:15672
- **Username:** guest
- **Password:** guest

### Event Flow
1. Catalog.Service publishes `CatalogItemCreated` event
2. RabbitMQ routes to Inventory.Service
3. Inventory.Service consumer processes the event

---

## 📊 API Documentation

### Swagger UI
Access API documentation:
- **Identity.Service:** https://localhost:5005/swagger
- **Catalog.Service:** https://localhost:5001/swagger
- **Inventory.Service:** https://localhost:5002/swagger

### Health Endpoints
```bash
# Identity.Service discovery
curl -k https://localhost:5005/.well-known/openid-configuration

# Catalog Service swagger
curl -k https://localhost:5001/swagger/ui

# Inventory Service swagger
curl -k https://localhost:5002/swagger/ui
```

---

## 🧪 Testing

### Health Check
```bash
./health-check.sh
```

### Manual Port Testing
```bash
# Test MongoDB
telnet localhost 27017

# Test RabbitMQ
telnet localhost 5672

# Test HTTP Services
curl -k https://localhost:5005/.well-known/openid-configuration
```

### API Testing with cURL
```bash
# Get token
TOKEN=$(curl -s -k -X POST https://localhost:5005/connect/token \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=password&username=user@example.com&password=password&scope=catalog" \
  | jq -r '.access_token')

# Use token to access API
curl -k https://localhost:5001/api/items \
  -H "Authorization: Bearer $TOKEN"
```

---

## 🔄 Typical Workflow

### 1. Development Setup
```bash
# First time setup
./start-all.sh
```

### 2. Start Services
Open 3 terminals and run:
```bash
./start-identity.sh
./start-catalog.sh
./start-inventory.sh
```

### 3. Verify Startup
```bash
./health-check.sh
```

### 4. Access Applications
- **API Documentation:** https://localhost:5005/swagger
- **RabbitMQ Dashboard:** http://localhost:15672 (guest/guest)
- **Frontend:** http://localhost:3000 (if running)

### 5. Make API Calls
Use Postman or similar tool with your JWT token

### 6. Cleanup When Done
```bash
./cleanup.sh
```

---

## 🛠️ Troubleshooting

### "Invalid Issuer" Error
✅ **Fixed** - All services now run on localhost with centralized Authority at `https://localhost:5005`

### Port Already in Use
```bash
# Find process using port
lsof -i :5005

# Kill process
kill -9 <PID>
```

### MongoDB Won't Connect
```bash
# Restart container
docker-compose restart mongodb

# Verify connection
telnet localhost 27017
```

### Services Won't Start
```bash
# Check if ports are available
lsof -i :5005 && lsof -i :5001 && lsof -i :5002

# Rebuild project
cd src/Catalog.Service
dotnet clean && dotnet build && dotnet run
```

### More Issues?
See [TROUBLESHOOTING.md](TROUBLESHOOTING.md) for comprehensive troubleshooting guide.

---

## 📦 Project Structure

```
learn-microservices/
├── src/
│   ├── Identity.Service/           # Auth server
│   ├── Catalog.Service/            # Product catalog API
│   ├── Inventory.Service/          # Inventory API
│   ├── GamePlatform.Common/        # Shared utilities
│   ├── GamePlatform.Catalog.Contracts/  # Message contracts
│   └── frontend/                   # React application
├── compose.yaml                    # Docker Compose for MongoDB & RabbitMQ
├── start-all.sh                    # Start Docker containers
├── start-identity.sh               # Start Identity.Service
├── start-catalog.sh                # Start Catalog.Service
├── start-inventory.sh              # Start Inventory.Service
├── health-check.sh                 # Verify all services
├── cleanup.sh                      # Cleanup utility
├── QUICK_START.md                  # 5-minute setup
├── SETUP_GUIDE.md                  # Comprehensive guide
├── TROUBLESHOOTING.md              # Problem solving
└── README.md                       # This file
```

---

## 🎓 Learning Objectives

This project teaches:
- **Microservices Architecture** - Multiple independent services
- **Authentication & Authorization** - JWT tokens with IdentityServer
- **Data Persistence** - MongoDB with different databases per service
- **Event-Driven Communication** - RabbitMQ message broker
- **API Design** - RESTful APIs with Swagger documentation
- **Container Technology** - Docker & Docker Compose
- **Development Workflow** - Local development with infrastructure in containers

---

## 📝 Files Modified

1. **compose.yaml**
   - Removed service containers
   - Kept MongoDB and RabbitMQ

2. **src/Catalog.Service/Properties/launchSettings.json**
   - Changed from HTTP to HTTPS

## 📁 Files Created

1. **Helper Scripts**
   - start-all.sh, start-identity.sh, start-catalog.sh, start-inventory.sh
   - health-check.sh, cleanup.sh

2. **Documentation**
   - QUICK_START.md, SETUP_GUIDE.md, TROUBLESHOOTING.md
   - CHANGES_SUMMARY.md, VALIDATION_REPORT.md, INDEX.md

---

## ✅ Configuration Status

| Component | Status | Details |
|-----------|--------|---------|
| Docker Compose | ✅ Ready | Infrastructure only |
| Identity.Service | ✅ Ready | https://localhost:5005 |
| Catalog.Service | ✅ Ready | https://localhost:5001 |
| Inventory.Service | ✅ Ready | https://localhost:5002 |
| MongoDB | ✅ Ready | localhost:27017 |
| RabbitMQ | ✅ Ready | localhost:5672 |
| Documentation | ✅ Complete | 6 guides provided |
| Helper Scripts | ✅ Complete | 6 scripts provided |

---

## 🎯 Next Steps

1. **First Time?** → Read [QUICK_START.md](QUICK_START.md)
2. **Want Details?** → Read [SETUP_GUIDE.md](SETUP_GUIDE.md)
3. **Got Issues?** → Read [TROUBLESHOOTING.md](TROUBLESHOOTING.md)
4. **Ready to Go?** → Run `./start-all.sh`

---

## 💡 Tips

- **Keep 3 terminals open** during development - one for each service
- **Use `./health-check.sh`** to verify everything is working
- **Check logs** in each terminal for debugging
- **Read [TROUBLESHOOTING.md](TROUBLESHOOTING.md)** if something breaks
- **Run `./cleanup.sh`** before doing a complete reset

---

## 📞 Support

For issues and questions, check:
1. [TROUBLESHOOTING.md](TROUBLESHOOTING.md) - Common issues
2. [SETUP_GUIDE.md](SETUP_GUIDE.md) - Comprehensive guide
3. Service logs in terminal windows
4. Docker logs: `docker-compose logs <service>`

---

## 🎉 Ready to Start!

Everything is configured and ready to go.

**Start with:** `./start-all.sh`

Then follow the on-screen instructions to start each service.

---

*Configuration Status: Complete & Verified ✅*
*Last Updated: March 26, 2026*

