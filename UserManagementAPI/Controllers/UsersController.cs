using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserManagementAPI.Data;
using UserManagementAPI.Models;

namespace UserManagementAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UserManagementDbContext _context;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UserManagementDbContext context, ILogger<UsersController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET: Retrieve a list of all users
        /// </summary>
        /// <returns>List of users</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            try
            {
                _logger.LogInformation("Retrieving all users");
                var users = await _context.Users.ToListAsync();
                
                if (!users.Any())
                {
                    _logger.LogWarning("No users found in database");
                    return Ok(new List<User>());
                }

                return Ok(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving users");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { error = "Failed to retrieve users", details = ex.Message });
            }
        }

        /// <summary>
        /// GET: Retrieve a specific user by ID
        /// </summary>
        /// <param name="id">User ID</param>
        /// <returns>User object</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    _logger.LogWarning("Invalid user ID requested: {UserId}", id);
                    return BadRequest(new { error = "User ID must be greater than 0" });
                }

                _logger.LogInformation("Retrieving user with ID: {UserId}", id);
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User not found with ID: {UserId}", id);
                    return NotFound(new { error = $"User with ID {id} not found" });
                }

                return Ok(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving user with ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { error = "Failed to retrieve user", details = ex.Message });
            }
        }

        /// <summary>
        /// POST: Add a new user
        /// </summary>
        /// <param name="user">User object to create</param>
        /// <returns>Created user object</returns>
        [HttpPost]
        public async Task<ActionResult<User>> CreateUser([FromBody] User user)
        {
            try
            {
                // Validate user input
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid user data provided for creation");
                    return BadRequest(ModelState);
                }

                if (user == null)
                {
                    return BadRequest(new { error = "User data cannot be null" });
                }

                // Additional validation for required fields
                if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email) || 
                    string.IsNullOrWhiteSpace(user.Department))
                {
                    return BadRequest(new { error = "Name, Email, and Department are required" });
                }

                // Check for duplicate email
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
                if (existingUser != null)
                {
                    _logger.LogWarning("Attempt to create user with duplicate email: {Email}", user.Email);
                    return Conflict(new { error = "User with this email already exists" });
                }

                user.CreatedDate = DateTime.UtcNow;
                user.LastModifiedDate = DateTime.UtcNow;

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User created successfully with ID: {UserId}", user.Id);
                return CreatedAtAction(nameof(GetUserById), new { id = user.Id }, user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating user");
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { error = "Failed to create user", details = ex.Message });
            }
        }

        /// <summary>
        /// PUT: Update an existing user
        /// </summary>
        /// <param name="id">User ID to update</param>
        /// <param name="user">Updated user object</param>
        /// <returns>Updated user object</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> UpdateUser(int id, [FromBody] User user)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { error = "User ID must be greater than 0" });
                }

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid user data provided for update");
                    return BadRequest(ModelState);
                }

                var existingUser = await _context.Users.FindAsync(id);

                if (existingUser == null)
                {
                    _logger.LogWarning("User not found for update with ID: {UserId}", id);
                    return NotFound(new { error = $"User with ID {id} not found" });
                }

                // Validate required fields
                if (string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email) || 
                    string.IsNullOrWhiteSpace(user.Department))
                {
                    return BadRequest(new { error = "Name, Email, and Department are required" });
                }

                // Check for duplicate email (excluding current user)
                var userWithEmail = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email && u.Id != id);
                if (userWithEmail != null)
                {
                    _logger.LogWarning("Attempt to update user with duplicate email: {Email}", user.Email);
                    return Conflict(new { error = "Another user with this email already exists" });
                }

                // Update properties
                existingUser.Name = user.Name;
                existingUser.Email = user.Email;
                existingUser.Department = user.Department;
                existingUser.PhoneNumber = user.PhoneNumber;
                existingUser.LastModifiedDate = DateTime.UtcNow;

                _context.Users.Update(existingUser);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User updated successfully with ID: {UserId}", id);
                return Ok(existingUser);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating user with ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { error = "Failed to update user", details = ex.Message });
            }
        }

        /// <summary>
        /// DELETE: Remove a user by ID
        /// </summary>
        /// <param name="id">User ID to delete</param>
        /// <returns>No content</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteUser(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { error = "User ID must be greater than 0" });
                }

                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    _logger.LogWarning("User not found for deletion with ID: {UserId}", id);
                    return NotFound(new { error = $"User with ID {id} not found" });
                }

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                _logger.LogInformation("User deleted successfully with ID: {UserId}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting user with ID: {UserId}", id);
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    new { error = "Failed to delete user", details = ex.Message });
            }
        }
    }
}
