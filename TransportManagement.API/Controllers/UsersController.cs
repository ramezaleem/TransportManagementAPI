using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportManagement.Application.Services;
using TransportManagement.Domain.Entities;

namespace TransportManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _service;

        public UsersController ( IUserService service )
        {
            _service = service;
        }

        /// <summary>
        /// Gets a list of all users in the system.
        /// </summary>
        /// <returns>Returns a list of user objects.</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetAll ()
        {
            var users = await _service.GetAllAsync();
            return Ok(users);
        }

        /// <summary>
        /// Gets details of a specific user by their ID.
        /// </summary>
        /// <param name="id">User's unique ID.</param>
        /// <returns>Returns user details if found, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetById ( int id )
        {
            var user = await _service.GetByIdAsync(id);
            if (user == null)
                return NotFound();
            return Ok(user);
        }

        /// <summary>
        /// Adds a new user to the system.
        /// </summary>
        /// <param name="user">User object with all required details.</param>
        /// <returns>Returns the created user data and its ID.</returns>
        [HttpPost]
        public async Task<ActionResult<User>> Add ( [FromBody] User user )
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var createdUser = await _service.AddAsync(user);
            return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
        }

        /// <summary>
        /// Updates an existing user's information.
        /// </summary>
        /// <param name="id">User's unique ID.</param>
        /// <param name="user">Updated user object.</param>
        /// <returns>Returns the updated user data.</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<User>> Update ( int id, [FromBody] User user )
        {
            if (id != user.Id)
                return BadRequest();
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var updatedUser = await _service.UpdateAsync(user);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Deletes a user from the system by their ID.
        /// </summary>
        /// <param name="id">User's unique ID.</param>
        /// <returns>Returns 204 No Content if deleted, or 404 if not found.</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete ( int id )
        {
            var success = await _service.DeleteAsync(id);
            if (!success)
                return NotFound();
            return NoContent();
        }
    }
}
