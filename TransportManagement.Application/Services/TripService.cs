using Microsoft.EntityFrameworkCore;
using TransportManagement.Domain.Entities;
using TransportManagement.Infrastructure.Data;

namespace TransportManagement.Application.Services
{
    public class TripService : ITripService
    {
        private readonly TransportDbContext _context;

        public TripService ( TransportDbContext context )
        {
            _context = context;
        }

        public async Task<(int TotalItems, IEnumerable<Trip> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize )
        {
            var query = _context.Trips.AsNoTracking();

            var totalItems = await query.CountAsync();
            var items = await query
                .Include(t => t.Client)
                .Include(t => t.Vehicle)
                .Include(t => t.Direction)
                .Include(t => t.Transporter)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalItems, items);
        }

        public async Task<Trip> GetByIdAsync ( int id )
        {
            return await _context.Trips
                .Include(t => t.Client)
                .Include(t => t.Vehicle)
                .Include(t => t.Direction)
                .Include(t => t.Transporter)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<Trip> AddAsync ( Trip trip )
        {
            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();
            return trip;
        }

        public async Task<Trip> UpdateAsync ( Trip trip )
        {
            _context.Trips.Update(trip);
            await _context.SaveChangesAsync();
            return trip;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
                return false;
            _context.Trips.Remove(trip);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
