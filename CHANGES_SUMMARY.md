# Configuration Changes Summary

## Changes Made

### 1. ✅ Docker Compose (compose.yaml)
**Removed:** All microservice containers (identity.service, catalog.service, inventory.service)
**Kept:** Only MongoDB and RabbitMQ containers
**Result:** Services now run locally, only infrastructure services in containers

### 2. ✅ Catalog.Service Launch Configuration
**File:** `src/Catalog.Service/Properties/launchSettings.json`
**Change:** Updated protocol from HTTP to HTTPS
- **Before:** `http://localhost:5001`
- **After:** `https://localhost:5001`
**Reason:** Matches Authority configuration and application expectations

### 3. ✅ Configuration Verification
**Identity.Service:**
- Launch URL: `https://localhost:5005` ✓
- Authority: `https://localhost:5005` ✓
- Role: Auth Server + Users API ✓

**Catalog.Service:**
- Launch URL: `https://localhost:5001` ✓
- Authority: `https://localhost:5005` ✓
- Role: Protected Resource Server ✓

**Inventory.Service:**
- Launch URL: `https://localhost:5002` ✓
- Authority: `https://localhost:5005` ✓
- Role: Protected Resource Server ✓

**MongoDB:**
- Container: mongodb ✓
- Port: 27017 ✓
- Connection String: `mongodb://localhost:27017` ✓

**RabbitMQ:**
- Container: rabbitmq ✓
- AMQP Port: 5672 ✓
- Management Port: 15672 ✓
- Connection String: `amqp://guest:guest@localhost:5672/` ✓

## Authentication Flow Fix

### Root Cause of "Invalid Issuer" Error
The services were running in containers with different hostnames, while the Authority was hardcoded to `https://localhost:5005`. This caused a mismatch when validating JWT tokens.

### Solution
By running all services locally:
1. Identity.Service acts as the issuer at `https://localhost:5005`
2. Catalog.Service and Inventory.Service can validate tokens using the same authority URL
3. No hostname/network translation issues
4. All services see the same issuer URL in tokens

## appsettings Configuration (Already Correct)
All services have proper configurations:
- `ServiceSettings.Authority`: `https://localhost:5005`
- `MongoDbSettings.Host`: `localhost`
- `RabbitMqSettings.Host`: `localhost`
- All use the same database and message broker

## Next Steps for User

1. **Start Docker containers:**
   ```bash
   docker-compose up -d
   ```

2. **Start each service locally (in separate terminals):**
   - Terminal 1: `cd src/Identity.Service && dotnet run`
   - Terminal 2: `cd src/Catalog.Service && dotnet run`
   - Terminal 3: `cd src/Inventory.Service && dotnet run`

3. **Verify Services:**
   - Identity.Service: https://localhost:5005/swagger
   - Catalog.Service: https://localhost:5001/swagger
   - Inventory.Service: https://localhost:5002/swagger

4. **Test Authentication:**
   - Obtain token from Identity.Service
   - Use token to access Catalog.Service and Inventory.Service

## Files Modified
1. `/Users/effevans/Dev2/learn-microservices/compose.yaml`
2. `/Users/effevans/Dev2/learn-microservices/src/Catalog.Service/Properties/launchSettings.json`

## Files Created
1. `/Users/effevans/Dev2/learn-microservices/SETUP_GUIDE.md` - Complete setup and troubleshooting guide

## Configuration Files (No Changes Needed)
These files are already correctly configured:
- `src/Identity.Service/appsettings.json`
- `src/Catalog.Service/appsettings.json`
- `src/Inventory.Service/appsettings.json`
- `src/Identity.Service/Properties/launchSettings.json`
- `src/Inventory.Service/Properties/launchSettings.json`

