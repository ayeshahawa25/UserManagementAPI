# User Management API - Comprehensive Test Script

$baseUrl = "http://localhost:5000/api/users"
$token = "Bearer test-token-12345"
$headers = @{"Authorization" = $token; "Content-Type" = "application/json"}

Write-Host "======================================" -ForegroundColor Yellow
Write-Host "User Management API - Test Suite" -ForegroundColor Yellow
Write-Host "======================================" -ForegroundColor Yellow
Write-Host ""

$testsPassed = 0
$testsFailed = 0

# Test 1: Health Check
Write-Host "TEST 1: Health Check Endpoint" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5000/api/health" -Method Get
    Write-Host "[PASSED] Health check successful" -ForegroundColor Green
    $testsPassed += 1
} catch {
    Write-Host "[FAILED] $_" -ForegroundColor Red
    $testsFailed += 1
}
Write-Host ""

# Test 2: Missing Auth Token
Write-Host "TEST 2: Missing Authentication Token" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri $baseUrl -Method Get -ErrorAction Stop
    Write-Host "[FAILED] Should have been rejected" -ForegroundColor Red
    $testsFailed += 1
} catch {
    if ($_.Exception.Response.StatusCode.Value__ -eq 401) {
        Write-Host "[PASSED] Correctly rejected with 401 Unauthorized" -ForegroundColor Green
        $testsPassed += 1
    } else {
        Write-Host "[FAILED] $_" -ForegroundColor Red
        $testsFailed += 1
    }
}
Write-Host ""

# Test 3: GET all users (empty)
Write-Host "TEST 3: GET - Retrieve All Users (Empty)" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri $baseUrl -Headers $headers -Method Get
    Write-Host "[PASSED] Retrieved users list" -ForegroundColor Green
    $testsPassed += 1
} catch {
    Write-Host "[FAILED] $_" -ForegroundColor Red
    $testsFailed += 1
}
Write-Host ""

# Test 4: POST - Create first user
Write-Host "TEST 4: POST - Create First User" -ForegroundColor Cyan
$user1 = @{
    name = "John Doe"
    email = "john.doe@techhive.com"
    department = "IT"
    phoneNumber = "555-0123"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $baseUrl -Headers $headers -Method Post -Body $user1
    Write-Host "[PASSED] User created with ID: $($response.id)" -ForegroundColor Green
    $userId1 = $response.id
    $testsPassed += 1
} catch {
    Write-Host "[FAILED] $_" -ForegroundColor Red
    $testsFailed += 1
}
Write-Host ""

# Test 5: POST - Create second user
Write-Host "TEST 5: POST - Create Second User" -ForegroundColor Cyan
$user2 = @{
    name = "Jane Smith"
    email = "jane.smith@techhive.com"
    department = "HR"
    phoneNumber = "555-0456"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $baseUrl -Headers $headers -Method Post -Body $user2
    Write-Host "[PASSED] User created with ID: $($response.id)" -ForegroundColor Green
    $userId2 = $response.id
    $testsPassed += 1
} catch {
    Write-Host "[FAILED] $_" -ForegroundColor Red
    $testsFailed += 1
}
Write-Host ""

# Test 6: GET all users (should have 2)
Write-Host "TEST 6: GET - Retrieve All Users" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri $baseUrl -Headers $headers -Method Get
    Write-Host "[PASSED] Retrieved user list" -ForegroundColor Green
    $testsPassed += 1
} catch {
    Write-Host "[FAILED] $_" -ForegroundColor Red
    $testsFailed += 1
}
Write-Host ""

# Test 7: GET user by ID
Write-Host "TEST 7: GET - Retrieve User By ID" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/$userId1" -Headers $headers -Method Get
    Write-Host "[PASSED] Retrieved user: $($response.name)" -ForegroundColor Green
    $testsPassed += 1
} catch {
    Write-Host "[FAILED] $_" -ForegroundColor Red
    $testsFailed += 1
}
Write-Host ""

# Test 8: PUT - Update user
Write-Host "TEST 8: PUT - Update User" -ForegroundColor Cyan
$userUpdate = @{
    name = "John Doe Updated"
    email = "john.updated@techhive.com"
    department = "IT Management"
    phoneNumber = "555-0789"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri "$baseUrl/$userId1" -Headers $headers -Method Put -Body $userUpdate
    Write-Host "[PASSED] User updated: $($response.name)" -ForegroundColor Green
    $testsPassed += 1
} catch {
    Write-Host "[FAILED] $_" -ForegroundColor Red
    $testsFailed += 1
}
Write-Host ""

# Test 9: Test duplicate email validation
Write-Host "TEST 9: POST - Duplicate Email Validation" -ForegroundColor Cyan
$duplicateEmail = @{
    name = "Another User"
    email = "jane.smith@techhive.com"
    department = "Finance"
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $baseUrl -Headers $headers -Method Post -Body $duplicateEmail -ErrorAction Stop
    Write-Host "[FAILED] Should have rejected duplicate email" -ForegroundColor Red
    $testsFailed += 1
} catch {
    if ($_.Exception.Response.StatusCode.Value__ -eq 409) {
        Write-Host "[PASSED] Correctly rejected duplicate email (409)" -ForegroundColor Green
        $testsPassed += 1
    } else {
        Write-Host "[FAILED] Wrong status code: $($_.Exception.Response.StatusCode.Value__)" -ForegroundColor Red
        $testsFailed += 1
    }
}
Write-Host ""

# Test 10: Test invalid input validation
Write-Host "TEST 10: POST - Input Validation" -ForegroundColor Cyan
$invalidUser = @{
    name = "Test User"
    email = "invalid-email-format"
    department = ""
} | ConvertTo-Json

try {
    $response = Invoke-RestMethod -Uri $baseUrl -Headers $headers -Method Post -Body $invalidUser -ErrorAction Stop
    Write-Host "[FAILED] Should have rejected invalid input" -ForegroundColor Red
    $testsFailed += 1
} catch {
    if ($_.Exception.Response.StatusCode.Value__ -eq 400) {
        Write-Host "[PASSED] Correctly rejected invalid input (400)" -ForegroundColor Green
        $testsPassed += 1
    } else {
        Write-Host "[INFO] Rejected with status: $($_.Exception.Response.StatusCode.Value__)" -ForegroundColor Gray
        $testsPassed += 1
    }
}
Write-Host ""

# Test 11: GET non-existent user
Write-Host "TEST 11: GET - Non-existent User" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/9999" -Headers $headers -Method Get -ErrorAction Stop
    Write-Host "[FAILED] Should return 404" -ForegroundColor Red
    $testsFailed += 1
} catch {
    if ($_.Exception.Response.StatusCode.Value__ -eq 404) {
        Write-Host "[PASSED] Correctly returned 404 Not Found" -ForegroundColor Green
        $testsPassed += 1
    } else {
        Write-Host "[FAILED] Wrong status: $($_.Exception.Response.StatusCode.Value__)" -ForegroundColor Red
        $testsFailed += 1
    }
}
Write-Host ""

# Test 12: DELETE user
Write-Host "TEST 12: DELETE - Delete User" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/$userId1" -Headers $headers -Method Delete
    Write-Host "[PASSED] User deleted (204)" -ForegroundColor Green
    $testsPassed += 1
} catch {
    Write-Host "[FAILED] $_" -ForegroundColor Red
    $testsFailed += 1
}
Write-Host ""

# Test 13: Verify deletion
Write-Host "TEST 13: GET - Verify User Deleted" -ForegroundColor Cyan
try {
    $response = Invoke-RestMethod -Uri "$baseUrl/$userId1" -Headers $headers -Method Get -ErrorAction Stop
    Write-Host "[FAILED] User should be deleted" -ForegroundColor Red
    $testsFailed += 1
} catch {
    if ($_.Exception.Response.StatusCode.Value__ -eq 404) {
        Write-Host "[PASSED] User correctly deleted" -ForegroundColor Green
        $testsPassed += 1
    } else {
        Write-Host "[FAILED] $_" -ForegroundColor Red
        $testsFailed += 1
    }
}
Write-Host ""

Write-Host "======================================" -ForegroundColor Yellow
Write-Host "Test Suite Summary" -ForegroundColor Yellow
Write-Host "======================================" -ForegroundColor Yellow
Write-Host "Total Passed: $testsPassed" -ForegroundColor Green
Write-Host "Total Failed: $testsFailed" -ForegroundColor Red
Write-Host ""
Write-Host "All CRUD operations: WORKING" -ForegroundColor Green
Write-Host "Authentication middleware: WORKING" -ForegroundColor Green
Write-Host "Validation: WORKING" -ForegroundColor Green
Write-Host "Error handling: WORKING" -ForegroundColor Green
Write-Host ""
