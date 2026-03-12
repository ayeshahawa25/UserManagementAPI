# User Management API - Documentation

## Project Overview
This is a complete User Management API built with ASP.NET Core that allows the HR and IT departments at TechHive Solutions to efficiently create, update, retrieve, and delete user records.

## Architecture

### Database Layer
- **Technology**: Entity Framework Core with In-Memory Database
- **Entity**: User model with validation
- **DbContext**: UserManagementDbContext

### API Endpoints
Base URL: `http://localhost:5000/api/users` (or `https://localhost:5001` for HTTPS)

#### 1. GET /api/users
**Description**: Retrieve a list of all users

**Response Example**:
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "email": "john.doe@techhive.com",
    "department": "IT",
    "phoneNumber": "555-0123",
    "createdDate": "2026-03-12T10:30:00Z",
    "lastModifiedDate": "2026-03-12T10:30:00Z"
  }
]
```

**Status Codes**:
- 200 OK: Successfully retrieved users
- 500 Internal Server Error: Server error

---

#### 2. GET /api/users/{id}
**Description**: Retrieve a specific user by ID

**Parameters**:
- `id` (path parameter): User ID (integer > 0)

**Response Example**:
```json
{
  "id": 1,
  "name": "John Doe",
  "email": "john.doe@techhive.com",
  "department": "IT",
  "phoneNumber": "555-0123",
  "createdDate": "2026-03-12T10:30:00Z",
  "lastModifiedDate": "2026-03-12T10:30:00Z"
}
```

**Error Response**:
```json
{
  "error": "User with ID 999 not found"
}
```

**Status Codes**:
- 200 OK: User found
- 400 Bad Request: Invalid ID
- 404 Not Found: User not found
- 500 Internal Server Error: Server error

---

#### 3. POST /api/users
**Description**: Create a new user

**Request Body**:
```json
{
  "name": "Jane Smith",
  "email": "jane.smith@techhive.com",
  "department": "HR",
  "phoneNumber": "555-0456"
}
```

**Required Fields**:
- `name` (string, 2-100 characters)
- `email` (string, valid email format)
- `department` (string, 2-50 characters)

**Optional Fields**:
- `phoneNumber` (string, valid phone format)

**Response Example** (201 Created):
```json
{
  "id": 2,
  "name": "Jane Smith",
  "email": "jane.smith@techhive.com",
  "department": "HR",
  "phoneNumber": "555-0456",
  "createdDate": "2026-03-12T10:35:00Z",
  "lastModifiedDate": "2026-03-12T10:35:00Z"
}
```

**Error Response** (400 Bad Request):
```json
{
  "error": "Name, Email, and Department are required"
}
```

**Conflict Response** (409 Conflict):
```json
{
  "error": "User with this email already exists"
}
```

**Status Codes**:
- 201 Created: User successfully created
- 400 Bad Request: Invalid input data
- 409 Conflict: Email already exists
- 500 Internal Server Error: Server error

---

#### 4. PUT /api/users/{id}
**Description**: Update an existing user

**Parameters**:
- `id` (path parameter): User ID to update

**Request Body**:
```json
{
  "name": "Jane Smith Updated",
  "email": "jane.updated@techhive.com",
  "department": "HR",
  "phoneNumber": "555-0789"
}
```

**Response Example** (200 OK):
```json
{
  "id": 2,
  "name": "Jane Smith Updated",
  "email": "jane.updated@techhive.com",
  "department": "HR",
  "phoneNumber": "555-0789",
  "createdDate": "2026-03-12T10:35:00Z",
  "lastModifiedDate": "2026-03-12T10:40:00Z"
}
```

**Status Codes**:
- 200 OK: User successfully updated
- 400 Bad Request: Invalid input data
- 404 Not Found: User not found
- 409 Conflict: Email already exists for another user
- 500 Internal Server Error: Server error

---

#### 5. DELETE /api/users/{id}
**Description**: Remove a user by ID

**Parameters**:
- `id` (path parameter): User ID to delete

**Response**: No content (204 No Content)

**Error Response** (404 Not Found):
```json
{
  "error": "User with ID 999 not found"
}
```

**Status Codes**:
- 204 No Content: User successfully deleted
- 400 Bad Request: Invalid ID
- 404 Not Found: User not found
- 500 Internal Server Error: Server error

---

## Middleware Implementation

### 1. Error Handling Middleware
- **Purpose**: Catches unhandled exceptions and returns consistent JSON error responses
- **Features**:
  - Logs all exceptions
  - Returns 500 status code with error details
  - Consistent error response format

### 2. Authentication Middleware
- **Purpose**: Validates authentication tokens on incoming requests
- **Features**:
  - Checks for Authorization header with "Bearer" token
  - Returns 401 Unauthorized for missing or invalid tokens
  - Allows public access to Swagger and health check endpoints
  - Logs authentication attempts

### 3. Logging Middleware
- **Purpose**: Logs all HTTP requests and responses for auditing
- **Features**:
  - Records HTTP method and request path
  - Tracks response status code
  - Measures request processing duration
  - Timestamps all log entries

### Middleware Pipeline Order (Correct Implementation)
1. **Error Handling Middleware** (First) - Catches exceptions from all subsequent middleware
2. **Authentication Middleware** (Second) - Validates tokens before processing requests
3. **Logging Middleware** (Last) - Logs all requests/responses

---

## Testing the API

### Using Swagger UI
1. Start the API: `dotnet run`
2. Navigate to: `http://localhost:5000/swagger`
3. Explore and test endpoints directly from the UI

### Using Postman

#### Step 1: GET all users (initially empty)
```
GET http://localhost:5000/api/users
Headers:
  Authorization: Bearer test-token-123
```

#### Step 2: Create a new user
```
POST http://localhost:5000/api/users
Headers:
  Authorization: Bearer test-token-123
  Content-Type: application/json

Body:
{
  "name": "John Doe",
  "email": "john.doe@techhive.com",
  "department": "IT",
  "phoneNumber": "555-0123"
}
```

#### Step 3: Get user by ID
```
GET http://localhost:5000/api/users/1
Headers:
  Authorization: Bearer test-token-123
```

#### Step 4: Update user
```
PUT http://localhost:5000/api/users/1
Headers:
  Authorization: Bearer test-token-123
  Content-Type: application/json

Body:
{
  "name": "John Doe Updated",
  "email": "john.updated@techhive.com",
  "department": "IT",
  "phoneNumber": "555-0124"
}
```

#### Step 5: Delete user
```
DELETE http://localhost:5000/api/users/1
Headers:
  Authorization: Bearer test-token-123
```

### Testing Edge Cases

#### 1. Missing Authentication Token
```
GET http://localhost:5000/api/users
(No Authorization header)
Expected: 401 Unauthorized
```

#### 2. Invalid Email Format
```
POST http://localhost:5000/api/users
Headers: Authorization: Bearer test-token-123

Body:
{
  "name": "Test User",
  "email": "invalid-email",
  "department": "IT"
}
Expected: 400 Bad Request (Validation error)
```

#### 3. Duplicate Email
```
POST http://localhost:5000/api/users
Headers: Authorization: Bearer test-token-123

Body:
{
  "name": "Another User",
  "email": "john.doe@techhive.com",
  "department": "HR"
}
Expected: 409 Conflict
```

#### 4. Non-existent User ID
```
GET http://localhost:5000/api/users/9999
Headers: Authorization: Bearer test-token-123
Expected: 404 Not Found
```

#### 5. Invalid User ID Format
```
GET http://localhost:5000/api/users/0
Headers: Authorization: Bearer test-token-123
Expected: 400 Bad Request
```

#### 6. Health Check (No Auth Required)
```
GET http://localhost:5000/api/health
Expected: 200 OK with health status
```

---

## How Microsoft Copilot Assisted in This Project

### Activity 1: Code Generation and Enhancement
1. **Project Scaffolding**: Copilot generated the initial ASP.NET Core Web API project structure and configuration in Program.cs
2. **CRUD Endpoint Generation**: Copilot provided templates for all CRUD operations (GET, POST, PUT, DELETE) with proper HTTP verbs and route attributes
3. **Model Creation**: Generated the User model with appropriate data annotations and validation attributes
4. **DbContext Setup**: Created Entity Framework Core context with proper entity configuration

### Activity 2: Debugging and Validation
1. **Input Validation**: Copilot enhanced the API with comprehensive validation including:
   - Required field checks
   - Email format validation
   - String length constraints
   - Phone number format validation
2. **Error Handling**: Implemented try-catch blocks in all controller methods
3. **Database Lookups**: Added null checking and proper error responses for non-existent users
4. **Data Integrity**: Implemented duplicate email checking with conflict responses

### Activity 3: Middleware Implementation
1. **Logging Middleware**: Generated middleware to log HTTP methods, paths, status codes, and response times
2. **Error Handling Middleware**: Created centralized exception handling with consistent JSON error responses
3. **Authentication Middleware**: Implemented token validation with Bearer token support
4. **Middleware Configuration**: Correctly ordered middleware in the pipeline for optimal performance and security

### Key Copilot Contributions
- **Code Quality**: Generated clean, maintainable code following ASP.NET Core best practices
- **Consistency**: Ensured consistent error response formats across all endpoints
- **Performance**: Optimized database queries with proper Entity Framework usage
- **Security**: Implemented proper authentication and authorization patterns
- **Documentation**: Generated comprehensive XML comments and endpoint descriptions
- **Testing Guidance**: Provided examples for testing all CRUD operations and edge cases

---

## Running the Application

```bash
# Navigate to project directory
cd "d:\Coursera sem 5\UserManagementAPI"

# Restore packages
dotnet restore

# Build the project
dotnet build

# Run the application
dotnet run

# Application will be available at:
# http://localhost:5000 (HTTP)
# https://localhost:5001 (HTTPS)
# Swagger UI: http://localhost:5000/swagger
```

## Project Structure
```
UserManagementAPI/
├── Controllers/
│   └── UsersController.cs          # CRUD endpoints
├── Data/
│   └── UserManagementDbContext.cs  # Entity Framework context
├── Middleware/
│   ├── LoggingMiddleware.cs        # Request/response logging
│   ├── ErrorHandlingMiddleware.cs  # Exception handling
│   └── AuthenticationMiddleware.cs # Token validation
├── Models/
│   └── User.cs                     # User entity with validation
├── Program.cs                       # Application configuration
└── UserManagementAPI.csproj        # Project file

```

## Dependencies
- Microsoft.EntityFrameworkCore (9.0.0)
- Microsoft.EntityFrameworkCore.InMemory (9.0.0)
- Swashbuckle.AspNetCore (6.4.0)
- ASP.NET Core Runtime 9.0+

## Validation Rules

### User Entity
- **Name**: Required, 2-100 characters
- **Email**: Required, valid email format, unique across database
- **Department**: Required, 2-50 characters
- **PhoneNumber**: Optional, valid phone format if provided
- **CreatedDate**: Automatically set when user is created
- **LastModifiedDate**: Automatically updated when user is modified

## Status Codes Used
- **200 OK**: Successful GET or PUT request
- **201 Created**: Successful POST request with new resource location
- **204 No Content**: Successful DELETE request
- **400 Bad Request**: Invalid input or validation error
- **401 Unauthorized**: Missing or invalid authentication token
- **404 Not Found**: Resource not found
- **409 Conflict**: Email already exists
- **500 Internal Server Error**: Unhandled server exception

---

## Notes
- The API uses in-memory database, so data will be lost when the application restarts
- For production use, replace InMemoryDatabase with SQL Server or other persistent storage
- The authentication middleware uses simple Bearer token validation for demo purposes
- In production, implement JWT tokens with proper signature verification
- All timestamps are in UTC format

---

**Project Created**: March 12, 2026
**Version**: 1.0.0
**Status**: Complete and Tested
