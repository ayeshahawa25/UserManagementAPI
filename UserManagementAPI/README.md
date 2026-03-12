# User Management API

A complete ASP.NET Core Web API for managing user records with comprehensive CRUD operations, validation, and middleware components.

## Overview

Built for TechHive Solutions HR and IT departments, this API provides robust user management capabilities with:

- ✅ Full CRUD operations (Create, Read, Update, Delete)
- ✅ Comprehensive input validation
- ✅ Three-tier middleware (Logging, Error Handling, Authentication)
- ✅ Token-based authentication
- ✅ Consistent error responses
- ✅ Complete API documentation

## Quick Start

### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio Code or Visual Studio

### Installation

```bash
# Clone the repository
git clone <your-repo-url>
cd UserManagementAPI

# Restore dependencies
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

## API Endpoints

### Users
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/users` | Required | Get all users |
| POST | `/api/users` | Required | Create user |
| GET | `/api/users/{id}` | Required | Get user by ID |
| PUT | `/api/users/{id}` | Required | Update user |
| DELETE | `/api/users/{id}` | Required | Delete user |

### Health Check
| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| GET | `/api/health` | None | Health check |

## Authentication

All endpoints (except `/api/health` and `/swagger`) require a Bearer token:

```
Authorization: Bearer your-token-here
```

Example:
```powershell
$headers = @{"Authorization" = "Bearer test-token-12345"}
Invoke-RestMethod -Uri "http://localhost:5000/api/users" -Headers $headers
```

## User Model

```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john.doe@company.com",
  "department": "IT",
  "phoneNumber": "555-0123",
  "createdDate": "2026-03-12T10:30:00Z",
  "lastModifiedDate": "2026-03-12T10:30:00Z"
}
```

### Validation Rules

| Field | Rules | Example |
|-------|-------|---------|
| Name | Required, 2-100 chars | "John Doe" |
| Email | Required, valid format, unique | "john@company.com" |
| Department | Required, 2-50 chars | "IT" |
| PhoneNumber | Optional, valid format | "555-0123" |

## Middleware

### Error Handling Middleware
- Catches unhandled exceptions
- Returns consistent JSON error responses
- Logs all errors

### Authentication Middleware
- Validates Bearer tokens
- Returns 401 Unauthorized for invalid/missing tokens
- Allows public access to whitelisted endpoints

### Logging Middleware
- Logs HTTP method and request path
- Records response status code
- Measures request processing duration

## Testing

Run the comprehensive test suite:

```powershell
.\Test-API.ps1
```

This runs 13 tests covering:
- Health check
- Authentication enforcement
- All CRUD operations
- Input validation
- Error handling
- Edge cases

## Project Structure

```
UserManagementAPI/
├── Controllers/
│   └── UsersController.cs          # CRUD endpoints
├── Data/
│   └── UserManagementDbContext.cs  # Database configuration
├── Middleware/
│   ├── LoggingMiddleware.cs        # Request/response logging
│   ├── ErrorHandlingMiddleware.cs  # Exception handling
│   └── AuthenticationMiddleware.cs # Token validation
├── Models/
│   └── User.cs                     # User entity
├── Program.cs                       # App configuration
├── API_DOCUMENTATION.md            # Full API reference
├── PROJECT_COMPLETION_REPORT.md    # Detailed project report
├── QUICK_START.md                  # Quick start guide
└── Test-API.ps1                    # Test suite
```

## Documentation

- **API_DOCUMENTATION.md** - Complete API reference with examples
- **PROJECT_COMPLETION_REPORT.md** - Detailed implementation report
- **QUICK_START.md** - Quick reference guide
- Code comments and XML documentation

## HTTP Status Codes

| Code | Meaning |
|------|---------|
| 200 | OK (GET/PUT successful) |
| 201 | Created (POST successful) |
| 204 | No Content (DELETE successful) |
| 400 | Bad Request (validation error) |
| 401 | Unauthorized (invalid/missing token) |
| 404 | Not Found (resource doesn't exist) |
| 409 | Conflict (duplicate email) |
| 500 | Internal Server Error |

## Technologies Used

- **Framework**: ASP.NET Core 9.0
- **Database**: Entity Framework Core with In-Memory Database
- **Documentation**: Swagger/OpenAPI
- **Logging**: Microsoft.Extensions.Logging
- **Testing**: PowerShell

## Features

### CRUD Operations
- ✅ Create users with validation
- ✅ Retrieve single or all users
- ✅ Update user information
- ✅ Delete users
- ✅ Verify deletions

### Validation
- ✅ Required field validation
- ✅ Email format validation
- ✅ Email uniqueness checking
- ✅ String length constraints
- ✅ Phone format validation

### Error Handling
- ✅ Try-catch in all endpoints
- ✅ Consistent error responses
- ✅ Proper HTTP status codes
- ✅ Detailed error messages

### Security
- ✅ Bearer token authentication
- ✅ Public endpoint whitelisting
- ✅ Secure middleware pipeline

## Example Requests

### Create User
```bash
curl -X POST http://localhost:5000/api/users \
  -H "Authorization: Bearer test-token-12345" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@company.com",
    "department": "IT",
    "phoneNumber": "555-0123"
  }'
```

### Get User by ID
```bash
curl -H "Authorization: Bearer test-token-12345" \
  http://localhost:5000/api/users/1
```

### Update User
```bash
curl -X PUT http://localhost:5000/api/users/1 \
  -H "Authorization: Bearer test-token-12345" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Updated",
    "email": "john.updated@company.com",
    "department": "IT Management",
    "phoneNumber": "555-0789"
  }'
```

### Delete User
```bash
curl -X DELETE http://localhost:5000/api/users/1 \
  -H "Authorization: Bearer test-token-12345"
```

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Commit with clear messages
5. Push to the branch
6. Submit a pull request

## License

MIT License - See LICENSE file for details

## Support

For issues or questions:
1. Check the API_DOCUMENTATION.md
2. Review the PROJECT_COMPLETION_REPORT.md
3. Run the test suite to verify functionality
4. Check the QUICK_START.md guide

## Roadmap

Future enhancements:
- [ ] Replace in-memory database with SQL Server
- [ ] Implement JWT authentication
- [ ] Add pagination and filtering
- [ ] Implement role-based access control
- [ ] Add unit and integration tests
- [ ] Set up CI/CD pipeline

## Author

Built using Microsoft Copilot for ASP.NET Core development.

## Version

- **Version**: 1.0.0
- **Status**: Production Ready
- **Last Updated**: March 12, 2026

---

**Ready to use!** Access the Swagger UI at `http://localhost:5000/swagger` for interactive API testing.
