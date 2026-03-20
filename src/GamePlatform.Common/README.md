# GamePlatform.Common

A shared library providing common infrastructure components for the GamePlatform microservices architecture.

## Overview

This library contains reusable components, interfaces, and utilities used across multiple microservices in the GamePlatform ecosystem.

## Features

### Entities
- **`IEntity`** - Base interface for domain entities with GUID-based identifiers
- **`CatalogItem`** - Catalog item entity representing game items in the platform

### Repositories
- **`IRepository<T>`** - Generic repository interface for CRUD operations
- **`MongoRepository<T>`** - MongoDB implementation of the repository pattern

### Extensions
- **`ServiceCollectionExtensions`** - Dependency injection extensions for:
  - MongoDB configuration (`AddMongo`)
  - Catalog repository registration (`AddCatalogRepositories`)

### Settings
- **`MongoDbSettings`** - MongoDB connection configuration

## Installation

Add a project reference to your microservice:

```xml
<ItemGroup>
  <ProjectReference Include="..\src\GamePlatform.Common\GamePlatform.Common.csproj" />
</ItemGroup>
```

## Usage

### Configure MongoDB

In your `Program.cs`:

```csharp
using GamePlatform.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddMongo(builder.Configuration)
    .AddCatalogRepositories();
```

### Configure appsettings.json

```json
{
  "MongoDbSettings": {
    "Host": "localhost",
    "Port": 27017
  }
}
```

### Using the Repository

```csharp
using GamePlatform.Common.Entities;
using GamePlatform.Common.Repositories;

public class ItemsController(IRepository<CatalogItem> repository) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<CatalogItem>>> GetAllAsync()
        => Ok(await repository.GetAllAsync());
}
```

## Dependencies

- MongoDB.Driver
- Microsoft.AspNetCore.App
- Microsoft.Azure.Functions.Worker (for Azure Functions support)

## Target Framework

- .NET 10.0
