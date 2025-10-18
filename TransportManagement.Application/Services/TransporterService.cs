using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using TransportManagement.Domain.Entities;
using TransportManagement.Infrastructure.Data;

namespace TransportManagement.Application.Services
{
    public class TransporterService : ITransporterService
    {
        private readonly TransportDbContext _context;

        public TransporterService ( TransportDbContext context )
        {
            _context = context;
        }

        public async Task<(int TotalItems, IEnumerable<Transporter> Items)> GetAllPaginatedAsync ( int pageNumber, int pageSize )
        {
            var query = _context.Transporters.AsNoTracking();

            var totalItems = await query.CountAsync();
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (totalItems, items);
        }

        public async Task<Transporter> GetByIdAsync ( int id )
        {
            return await _context.Transporters.FindAsync(id);
        }

        public async Task<Transporter> AddAsync ( Transporter transporter )
        {
            transporter.Password = HashPassword(transporter.Password);
            _context.Transporters.Add(transporter);
            await _context.SaveChangesAsync();
            return transporter;
        }

        public async Task<Transporter> UpdateAsync ( Transporter transporter )
        {
            if (!string.IsNullOrWhiteSpace(transporter.Password))
            {
                transporter.Password = HashPassword(transporter.Password);
            }

            _context.Transporters.Update(transporter);
            await _context.SaveChangesAsync();
            return transporter;
        }

        public async Task<bool> DeleteAsync ( int id )
        {
            var transporter = await _context.Transporters.FindAsync(id);
            if (transporter == null)
                return false;

            _context.Transporters.Remove(transporter);
            await _context.SaveChangesAsync();
            return true;
        }

        private string HashPassword ( string password )
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
    }
}
