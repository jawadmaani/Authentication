using System.Security.Cryptography;
using Authentication.Exceptions;
using Authentication.Model;
using Authentication.Repository;
using Authentication.Security;
using Report_System_Backend.middleware.RefreshTokenExceptions;

namespace Authentication.Service;

public class RefreshTokenService
{
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly TokenHasher _tokenHasher;
    private readonly IUserRepository _userRepository;
    private readonly ITransactionRepository _transactionRepository;

    public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository,
        ITransactionRepository transactionRepository, TokenHasher tokenHasher, IUserRepository userRepository)
    {
        _refreshTokenRepository = refreshTokenRepository;
        _tokenHasher = tokenHasher;
        _userRepository = userRepository;
        _transactionRepository = transactionRepository;
    }

    public async Task<string> CreateRefreshTokenAsync(int userId, bool saveChanges = true)
    {
        var user = await _userRepository.GetUserByIdAsync(userId);
        if (user == null)
            throw new EmptyDataBaseFromUsers("User not found");

        var plainToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var hashedToken = _tokenHasher.HashToken(plainToken);

        var refreshToken = new RefreshToken
        {
            UserId = userId,
            RefreshTokenHash = hashedToken,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
        };

        await _refreshTokenRepository.CreateAsync(refreshToken);

        if (saveChanges)
            await _refreshTokenRepository.SaveAsync();

        return plainToken;
    }

    public async Task<RefreshToken> ValidateRefreshTokenAsync(string plainToken, bool forUpdate = false)
    {
        
        var hashed = _tokenHasher.HashToken(plainToken);
        RefreshToken? storedToken;
        if (forUpdate)
            storedToken = await _refreshTokenRepository.GetByTokenHashForUpdateAsync(hashed);
        else
            storedToken = await _refreshTokenRepository.GetByTokenHashAsync(hashed);

        if (storedToken == null)
            throw new RefreshTokenNotFoundException("The refresh token does not exist or is invalid.");

        if (storedToken.RevokedAt.HasValue)
            throw new RefreshTokenRevokedException("The refresh token has been revoked and cannot be used.");

        if (storedToken.ExpiresAt < DateTime.UtcNow)
            throw new RefreshTokenExpiredException("The refresh token has expired and can no longer be used.");
        return storedToken;
    }


    public async Task RevokeRefreshTokenAsync(string plainToken)
    {
        var hashed = _tokenHasher.HashToken(plainToken);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(hashed);

        if (storedToken == null)
            return;

        storedToken.RevokedAt = DateTime.UtcNow;
        await _refreshTokenRepository.SaveAsync();
    }
    
    public async Task<(string newToken, int userId)> RotateRefreshTokenAsync(string oldPlainToken)
    {
        await using var transaction = await _transactionRepository.BeginTransactionAsync();
        try
        {
            var storedToken = await ValidateRefreshTokenAsync(oldPlainToken, forUpdate: true);

            storedToken.RevokedAt = DateTime.UtcNow;
            var newPlainToken = await CreateRefreshTokenAsync(storedToken.UserId, saveChanges: false);
       
            await _refreshTokenRepository.SaveAsync();
            await _transactionRepository.CommitTransactionAsync();
        
            return (newPlainToken, storedToken.UserId); 
        }
        catch
        {
            await _transactionRepository.RollbackTransactionAsync();
            throw;
        }
    }
}



