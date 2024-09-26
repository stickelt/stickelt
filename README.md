# BlazorWebApiDemo
 
# Blazor WebAssembly & .NET Core API Project

## Overview
This project demonstrates a Blazor WebAssembly front-end application integrated with a .NET Core Web API. The API handles user authentication using JWT, while the Blazor app communicates with the API to manage user sessions.

## Features
- User authentication with JWT
- API with Entity Framework and database migrations
- Middleware setup for authentication and CORS
- Components with data binding and routing in Blazor
- Unit testing for both API and Blazor components

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)

## Setup Instructions
1. Clone the repository.
2. Navigate to the API project and run the following commands to apply migrations:
   ```bash
   dotnet ef database update



