using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportManagement.Application.Services;
using TransportManagement.Domain.Entities;

namespace TransportManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DirectionsController : ControllerBase
    {
        private readonly IDirectionsService _service;

        public DirectionsController ( IDirectionsService service )
        {
            _service = service;
        }

        /// <summary>
        /// Gets a paginated list of all directions in the system.
        /// </summary>
        /// <param name="pageNumber">Page number (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 10).</param>
        /// <returns>Returns total items, current page info, and the list of directions.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll ( int pageNumber = 1, int pageSize = 10 )
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("pageNumber and pageSize must be greater than zero.");

            var (totalItems, directions) = await _service.GetAllPaginatedAsync(pageNumber, pageSize);

            var result = new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = directions
            };

            return Ok(result);
        }

        /// <summary>
        /// Gets details of a specific direction by its ID.
        /// </summary>
        /// <param name="id">Direction's unique ID.</param>
        /// <returns>Returns direction details if found, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById ( int id )
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Adds a new direction to the system.
        /// </summary>
        /// <param name="direction">Direction object with all required details.</param>
        /// <returns>Returns the created direction data and its ID.</returns>
        [HttpPost]
        public async Task<IActionResult> Add ( [FromBody] Direction direction )
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { Errors = errors });
            }

            var result = await _service.AddAsync(direction);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing direction's information.
        /// </summary>
        /// <param name="id">Direction's unique ID.</param>
        /// <param name="direction">Updated direction object.</param>
        /// <returns>Returns the updated direction data.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update ( int id, [FromBody] Direction direction )
        {
            if (id != direction.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { Errors = errors });
            }

            var result = await _service.UpdateAsync(direction);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a direction from the system by its ID.
        /// </summary>
        /// <param name="id">Direction's unique ID.</param>
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
