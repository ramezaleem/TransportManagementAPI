using Microsoft.EntityFrameworkCore;
using TransportManagement.Domain.Entities;
using TransportManagement.Infrastructure.Data;

namespace TransportManagement.Application.Services
{
    public class DirectionsService : IDirectionsService
    {
        private readonly TransportDbContext _context;

        public DirectionsService ( TransportDbContext context )
        {
            _context = context;
        }

        public async Task<(int TotalItems, IEnumerable<Direction> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize )
        {
            var query = _context.Directions.AsNoTracking();

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalItems, items);
        }

        public async Task<Direction> GetByIdAsync ( int id )
        {
            return await _context.Directions.FindAsync(id);
        }

        public async Task<Direction> AddAsync ( Direction direction )
        {
            _context.Directions.Add(direction);
            await _context.SaveChangesAsync();
            return direction;
        }

        public async Task<Direction> UpdateAsync ( Direction direction )
        {
            _context.Directions.Update(direction);
            await _context.SaveChangesAsync();
            return direction;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            var direction = await _context.Directions.FindAsync(id);
            if (direction == null)
                return false;
            _context.Directions.Remove(direction);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
