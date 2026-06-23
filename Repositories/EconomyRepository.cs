using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; 
using Alexander.Models;
using Alexander.Data;               

namespace Alexander.Repositories
{
    public class EconomyRepository : IEconomyRepository
    {
        private readonly BotDbContext _context;

        public EconomyRepository(BotDbContext context)
        {
            _context = context;
        }

        public async Task<List<EconomyProfile>> LoadAllAsync()
        {
            return await _context.Perfiles.ToListAsync();
        }

        public async Task SaveAllAsync(List<EconomyProfile> profiles)
        {
            _context.Perfiles.UpdateRange(profiles);
            await _context.SaveChangesAsync();
        }

        public async Task<EconomyProfile> GetByUserIdAsync(ulong userId)
        {
            return await _context.Perfiles.FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task AddAsync(EconomyProfile profile)
        {
            if (Exists(profile.UserId))
                throw new InvalidOperationException($"El usuario {profile.UserId} ya existe");

            await _context.Perfiles.AddAsync(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(EconomyProfile profile)
        {
            var rastreado = _context.Perfiles.Local.FirstOrDefault(p => p.UserId == profile.UserId);
            
            if (rastreado != null)
            {
                _context.Entry(rastreado).CurrentValues.SetValues(profile);
            }
            else
            {
                _context.Perfiles.Update(profile);
            }
            
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(ulong userId)
        {
            var profile = await GetByUserIdAsync(userId);

            if (profile == null)
                throw new KeyNotFoundException($"Usuario {userId} no encontrado");

            _context.Perfiles.Remove(profile);
            await _context.SaveChangesAsync();
        }

        public bool Exists(ulong userId)
        {
            return _context.Perfiles.Any(p => p.UserId == userId);
        }
    }
}