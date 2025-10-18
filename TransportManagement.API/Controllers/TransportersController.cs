using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TransportManagement.Application.Services;
using TransportManagement.Domain.Entities;

namespace TransportManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TransportersController : ControllerBase
    {
        private readonly ITransporterService _service;

        public TransportersController ( ITransporterService service )
        {
            _service = service;
        }

        /// <summary>
        /// Gets a paginated list of all transporters in the system.
        /// </summary>
        /// <param name="pageNumber">Page number (default is 1).</param>
        /// <param name="pageSize">Number of items per page (default is 10).</param>
        /// <returns>Returns total items, current page info, and the list of transporters.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAll ( int pageNumber = 1, int pageSize = 10 )
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return BadRequest("pageNumber and pageSize must be greater than zero.");

            var (totalItems, transporters) = await _service.GetAllPaginatedAsync(pageNumber, pageSize);

            var result = new
            {
                TotalItems = totalItems,
                PageNumber = pageNumber,
                PageSize = pageSize,
                Items = transporters
            };

            return Ok(result);
        }

        /// <summary>
        /// Gets details of a specific transporter by their ID.
        /// </summary>
        /// <param name="id">Transporter's unique ID.</param>
        /// <returns>Returns transporter details if found, or 404 if not found.</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById ( int id )
        {
            var result = await _service.GetByIdAsync(id);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Adds a new transporter to the system.
        /// </summary>
        /// <param name="transporter">Transporter object with all required details.</param>
        /// <returns>Returns the created transporter data and its ID.</returns>
        [HttpPost]
        public async Task<IActionResult> Add ( [FromBody] Transporter transporter )
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { Errors = errors });
            }

            // You can add password hashing here if needed for security
            // transporter.Password = HashPassword(transporter.Password);

            var result = await _service.AddAsync(transporter);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing transporter's information.
        /// </summary>
        /// <param name="id">Transporter's unique ID.</param>
        /// <param name="transporter">Updated transporter object.</param>
        /// <returns>Returns the updated transporter data.</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update ( int id, [FromBody] Transporter transporter )
        {
            if (id != transporter.Id)
                return BadRequest();

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                                              .Select(e => e.ErrorMessage)
                                              .ToList();
                return BadRequest(new { Errors = errors });
            }

            var result = await _service.UpdateAsync(transporter);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a transporter from the system by their ID.
        /// </summary>
        /// <param name="id">Transporter's unique ID.</param>
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
