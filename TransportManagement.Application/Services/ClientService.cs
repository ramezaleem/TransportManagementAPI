using Microsoft.EntityFrameworkCore;
using TransportManagement.Domain.Entities;
using TransportManagement.Infrastructure.Data;

namespace TransportManagement.Application.Services
{
    public class ClientService : IClientService
    {
        private readonly TransportDbContext _context;

        public ClientService ( TransportDbContext context )
        {
            _context = context;
        }

        public async Task<(int TotalItems, IEnumerable<Client> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize )
        {
            var query = _context.Clients.AsNoTracking();

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalItems, items);
        }

        public async Task<Client> GetByIdAsync ( int id )
        {
            return await _context.Clients.FindAsync(id);
        }

        public async Task<Client> AddAsync ( Client client )
        {
            _context.Clients.Add(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<Client> UpdateAsync ( Client client )
        {
            _context.Clients.Update(client);
            await _context.SaveChangesAsync();
            return client;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            var client = await _context.Clients.FindAsync(id);
            if (client == null)
                return false;
            _context.Clients.Remove(client);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
