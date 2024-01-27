# URL Shortener in .NET 8

## Overview

This example demonstrates how to create an URL shortener in ASP.NET Core 8 using MinimalAPI, Entity Framework Core and a Postgres Database with Docker.
This repository is based on this blog post but makes use of the far superior Postgres database.

## Prerequisites
- .NET 8 SDK
- Docker

## Getting Started

1.  Configure PostgreSQL Docker Container:

Install Docker and make sure the Docker daemon is running on your machine.

Run the following command to pull and run a PostgreSQL Docker container:

```bash
docker pull postgres
docker run --name url-shortener-db -e POSTGRES_PASSWORD=your-postgres-password -p 5432:5432 -d postgres
``` 

2. Apply the latest migration
```bash
dotnet ef database update
```

3. Run the application
```bash
dotnet run
```

