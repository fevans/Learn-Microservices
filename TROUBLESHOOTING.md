# Troubleshooting Quick Reference

## Common Issues & Solutions

### Issue 1: "Invalid Issuer" Error ❌

```
Bearer error="invalid_token", 
error_description="The issuer 'https://localhost:5005' is invalid"
```

**Causes & Solutions:**

1. **Identity.Service not running**
   ```bash
   # Check if running
   curl -k https://localhost:5005/.well-known/openid-configuration
   
   # If error, start Identity.Service in a terminal
   cd src/Identity.Service && dotnet run
   ```

2. **Wrong Authority in appsettings**
   ```json
   // Check that ALL services have this:
   "ServiceSettings": {
     "Authority": "https://localhost:5005"
   }
   ```

3. **Identity.Service running on wrong port**
   ```bash
   # Verify port
   lsof -i :5005
   
   # Should show: Identity.Service listening on port 5005
   ```

4. **HTTPS certificate issues**
   ```bash
   # Trust development certificate
   dotnet dev-certs https --trust
   ```

✅ **Solution:** Follow QUICK_START.md startup sequence

---

### Issue 2: MongoDB Connection Error 🗄️

```
Error: Failed to connect to 'mongodb://localhost:27017'
```

**Causes & Solutions:**

1. **Container not running**
   ```bash
   # Check status
   docker-compose ps
   
   # Start if needed
   docker-compose up -d mongodb
   ```

2. **Port already in use**
   ```bash
   # Check what's using port 27017
   lsof -i :27017
   
   # Kill process if needed
   kill -9 <PID>
   
   # Try again
   docker-compose up -d mongodb
   ```

3. **MongoDB not responding**
   ```bash
   # Restart MongoDB
   docker-compose restart mongodb
   
   # Verify connection
   telnet localhost 27017
   ```

4. **Wrong host in appsettings**
   ```json
   // Verify in appsettings.json:
   "MongoDbSettings": {
     "Host": "localhost",  // NOT "mongodb"
     "Port": 27017
   }
   ```

✅ **Solution:** Run `docker-compose up -d` then verify with `docker-compose ps`

---

### Issue 3: RabbitMQ Connection Error 🐰

```
Error: Timeout connecting to '127.0.0.1:5672'
```

**Causes & Solutions:**

1. **Container not running**
   ```bash
   docker-compose ps rabbitmq
   docker-compose up -d rabbitmq
   ```

2. **Health check failing**
   ```bash
   # Check logs
   docker-compose logs rabbitmq
   
   # Restart
   docker-compose restart rabbitmq
   
   # Wait for health check to pass
   docker-compose ps  # Wait for "healthy" status
   ```

3. **Port already in use**
   ```bash
   lsof -i :5672
   kill -9 <PID>
   ```

4. **Wrong host in appsettings**
   ```json
   // Verify in appsettings.json:
   "RabbitMqSettings": {
     "Host": "localhost"  // NOT "rabbitmq"
   }
   ```

✅ **Solution:** Run `docker-compose restart rabbitmq && sleep 10`

---

### Issue 4: Port Already in Use 🔌

```
Address already in use: 0.0.0.0:5005
```

**Causes & Solutions:**

```bash
# Find what's using the port
lsof -i :5005   # For Identity.Service
lsof -i :5001   # For Catalog.Service
lsof -i :5002   # For Inventory.Service

# Kill the process
kill -9 <PID>

# Or if it's a .NET process
killall dotnet

# Then restart the service
cd src/Identity.Service && dotnet run
```

✅ **Solution:** Kill previous process and restart

---

### Issue 5: HTTPS Certificate Error 🔒

```
The remote certificate is invalid according to the validation procedure.
```

**Causes & Solutions:**

```bash
# Trust development certificate
dotnet dev-certs https --trust

# Or clear and recreate
dotnet dev-certs https --clean
dotnet dev-certs https

# For curl, bypass validation (dev only)
curl -k https://localhost:5005/.well-known/openid-configuration
```

✅ **Solution:** Run `dotnet dev-certs https --trust`

---

### Issue 6: Service Won't Start 🚫

```
Unhandled exception: System.IO.IOException: Only one usage of each socket address...
```

**Solutions:**

1. **Check for duplicate process**
   ```bash
   ps aux | grep dotnet
   kill -9 <PID>  # Kill any previous processes
   ```

2. **Check if port is in use**
   ```bash
   lsof -i :5001  # Or appropriate port
   ```

3. **Check appsettings.json for errors**
   ```bash
   cd src/Catalog.Service
   cat appsettings.json  # Check JSON syntax
   ```

4. **Rebuild project**
   ```bash
   cd src/Catalog.Service
   dotnet clean
   dotnet build
   dotnet run
   ```

✅ **Solution:** Check ports, kill processes, rebuild

---

### Issue 7: Services Can't Communicate 📡

```
Error contacting Identity.Service at https://localhost:5005
```

**Causes & Solutions:**

1. **Services on different machines**
   - All services must run on the **same machine** (localhost)
   - Don't mix local + containerized services for .NET apps

2. **Wrong Authority URL**
   ```json
   // CORRECT - all services
   "Authority": "https://localhost:5005"
   
   // WRONG - don't use container names
   "Authority": "https://identity.service:8080"
   ```

3. **Service not running**
   ```bash
   # Test connectivity
   curl -k https://localhost:5005/swagger
   
   # Should return Swagger UI
   ```

✅ **Solution:** Ensure all services run locally, check Authority config

---

### Issue 8: Frontend Can't Connect 🌐

```
Error: Failed to fetch from API
CORS error
```

**Causes & Solutions:**

1. **Service not running**
   ```bash
   curl -k https://localhost:5001/api/items
   curl -k https://localhost:5002/api/inventory-items
   ```

2. **CORS not configured**
   ```json
   // Check AllowedOrigins in appsettings.json
   "AllowedOrigins": "http://localhost:3000"
   ```

3. **Frontend on wrong port**
   ```bash
   # Frontend should be on http://localhost:3000
   cd src/frontend && npm start
   ```

✅ **Solution:** Ensure services running, CORS configured for port 3000

---

## Health Check Commands

### Quick Verification
```bash
# 1. Check all containers running
docker-compose ps

# 2. Check MongoDB
telnet localhost 27017

# 3. Check RabbitMQ
telnet localhost 5672

# 4. Check Identity.Service
curl -k https://localhost:5005/.well-known/openid-configuration

# 5. Check Catalog.Service
curl -k https://localhost:5001/swagger/ui

# 6. Check Inventory.Service
curl -k https://localhost:5002/swagger/ui

# All should return successfully ✅
```

### Complete System Check
```bash
#!/bin/bash

echo "🔍 System Health Check"
echo "====================="
echo ""

echo "1️⃣  Docker Containers:"
docker-compose ps

echo ""
echo "2️⃣  MongoDB Connection:"
telnet localhost 27017 2>/dev/null && echo "✅ MongoDB OK" || echo "❌ MongoDB Failed"

echo ""
echo "3️⃣  RabbitMQ Connection:"
telnet localhost 5672 2>/dev/null && echo "✅ RabbitMQ OK" || echo "❌ RabbitMQ Failed"

echo ""
echo "4️⃣  Identity.Service:"
curl -s -k https://localhost:5005/.well-known/openid-configuration > /dev/null && echo "✅ Identity.Service OK" || echo "❌ Identity.Service Failed"

echo ""
echo "5️⃣  Catalog.Service:"
curl -s -k https://localhost:5001/swagger/ui > /dev/null && echo "✅ Catalog.Service OK" || echo "❌ Catalog.Service Failed"

echo ""
echo "6️⃣  Inventory.Service:"
curl -s -k https://localhost:5002/swagger/ui > /dev/null && echo "✅ Inventory.Service OK" || echo "❌ Inventory.Service Failed"

echo ""
echo "✅ Health check complete"
```

---

## Reset Everything

### Start Fresh
```bash
# Stop all services (Ctrl+C in terminals)

# Stop and remove containers
docker-compose down

# Clean volumes (WARNING: Deletes data)
docker-compose down -v

# Remove build artifacts
cd src/Identity.Service && dotnet clean
cd src/Catalog.Service && dotnet clean
cd src/Inventory.Service && dotnet clean

# Start over
docker-compose up -d
cd src/Identity.Service && dotnet run  # Terminal 1
cd src/Catalog.Service && dotnet run   # Terminal 2
cd src/Inventory.Service && dotnet run # Terminal 3
```

---

## Getting Help

1. **Check logs in terminals** - Look at console output
2. **Check `SETUP_GUIDE.md`** - Comprehensive guide
3. **Check `QUICK_START.md`** - Fast reference
4. **Check Docker logs** - `docker-compose logs <service>`
5. **Check appsettings** - Verify configuration files

---

## Quick Command Reference

```bash
# Containers
docker-compose up -d          # Start containers
docker-compose down           # Stop containers
docker-compose ps             # List containers
docker-compose logs mongodb   # View MongoDB logs
docker-compose restart mongodb # Restart service

# MongoDB
mongosh mongodb://localhost:27017
use CatalogDb

# RabbitMQ UI
http://localhost:15672  # (guest/guest)

# Ports Check
lsof -i :5005   # Check who's using port 5005
lsof -i :5001
lsof -i :5002

# Services
dotnet run                   # Run current service
dotnet clean && dotnet build # Rebuild
dotnet dev-certs https --trust # Trust cert
```

---

**Most Issues Resolved By:** 
1. Checking if services are actually running
2. Restarting Docker containers
3. Checking port availability
4. Verifying configuration files

**If still stuck:** Follow the **SETUP_GUIDE.md** step-by-step from the beginning.

