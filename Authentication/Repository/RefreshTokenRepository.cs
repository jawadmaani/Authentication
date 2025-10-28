using Authentication.Model;
using Microsoft.EntityFrameworkCore;

namespace Authentication.Repository;

public class RefreshTokenRepository : IRefreshTokenRepository


{
    private readonly AppDbContext _context;

    public RefreshTokenRepository(AppDbContext context)
    {
        _context = context;

    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        await _context.RefreshTokens.AddAsync(refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string refreshTokenHash)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.RefreshTokenHash == refreshTokenHash);
    }
    
    public async Task<RefreshToken?> GetByTokenHashForUpdateAsync(string tokenHash)
    {
        return await _context.RefreshTokens
            .FromSqlRaw("SELECT * FROM RefreshTokens WHERE RefreshTokenHash = {0} FOR UPDATE", tokenHash)
            .AsTracking()
            .FirstOrDefaultAsync();
    }
    public async Task SaveAsync()
    {
        await _context.SaveChangesAsync();
    }

}