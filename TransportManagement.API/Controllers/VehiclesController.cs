using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportManagement.Application.Services;
using TransportManagement.Domain.Entities;

namespace TransportManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class VehiclesController : ControllerBase
    {
        private readonly IVehicleService _service;

        public VehiclesController ( IVehicleService service )
        {
            _service = service;
        }

        /// <summary>
        /// Gets a paginated list of all vehicles in the system.
        /// </summary>
        /// <param name="pageNumber">Page number (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 10).</param>
        /// <returns>Returns total items, current page info, and the list of vehicles.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll ( int pageNumber = 1, int pageSize = 10 )
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("pageNumber and pageSize must be greater than zero.");

            var (totalItems, vehicles) = await _service.GetAllPaginatedAsync(pageNumber, pageSize);

            var result = new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = vehicles
            };

            return Ok(result);
        }

        /// <summary>
        /// Gets details of a specific vehicle by its ID.
        /// </summary>
        /// <param name="id">Vehicle's unique ID.</param>
        /// <returns>Returns vehicle details if found, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById ( int id )
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Adds a new vehicle to the system.
        /// </summary>
        /// <param name="vehicle">Vehicle object with all required details.</param>
        /// <returns>Returns the created vehicle data and its ID.</returns>
        [HttpPost]
        public async Task<IActionResult> Add ( [FromBody] Vehicle vehicle )
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { Errors = errors });
            }

            var result = await _service.AddAsync(vehicle);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing vehicle's information.
        /// </summary>
        /// <param name="id">Vehicle's unique ID.</param>
        /// <param name="vehicle">Updated vehicle object.</param>
        /// <returns>Returns the updated vehicle data.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update ( int id, [FromBody] Vehicle vehicle )
        {
            if (id != vehicle.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { Errors = errors });
            }

            var result = await _service.UpdateAsync(vehicle);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a vehicle from the system by its ID.
        /// </summary>
        /// <param name="id">Vehicle's unique ID.</param>
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
