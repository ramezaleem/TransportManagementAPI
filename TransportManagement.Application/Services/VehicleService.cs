using Microsoft.EntityFrameworkCore;
using TransportManagement.Domain.Entities;
using TransportManagement.Infrastructure.Data;

namespace TransportManagement.Application.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly TransportDbContext _context;

        public VehicleService ( TransportDbContext context )
        {
            _context = context;
        }

        public async Task<(int TotalItems, IEnumerable<Vehicle> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize )
        {
            var query = _context.Vehicles.AsNoTracking()
                                         .Include(v => v.Transporter);

            var totalItems = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalItems, items);
        }

        public async Task<Vehicle> GetByIdAsync ( int id )
        {
            return await _context.Vehicles.Include(v => v.Transporter)
                                          .FirstOrDefaultAsync(v => v.Id == id);
        }

        public async Task<Vehicle> AddAsync ( Vehicle vehicle )
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<Vehicle> UpdateAsync ( Vehicle vehicle )
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle == null)
                return false;

            _context.Vehicles.Remove(vehicle);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
