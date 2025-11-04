
using Authentication;
using Authentication.Repository;
using Authentication.Security;
using Authentication.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;
using  Authentication.Middleware;


var builder = WebApplication.CreateBuilder(args);

var refreshSecret = Environment.GetEnvironmentVariable("REFRESH_TOKEN_SECRET");

if (string.IsNullOrWhiteSpace(refreshSecret))
{
    throw new InvalidOperationException("REFRESH_TOKEN_SECRET is not set in the environment.");
}

builder.Services.Configure<JwtSettings>(
    builder.Configuration.GetSection("JwtSettings")
);
 

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)) 
    )
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<AccessTokenService>();
builder.Services.AddScoped<JwtTokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ITransactionRepository,TransactionRepository>();
builder.Services.AddScoped<PasswordHasher>();
builder.Services.AddScoped<ITokenHashStrategy, HmacSha512HashStrategy>();
builder.Services.AddSingleton(new TokenHasher(refreshSecret, new HmacSha512HashStrategy()));


builder.Services.AddOpenApi();
builder.Services.AddControllers();



builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth-limit", opt =>
    {
        opt.PermitLimit = 5; 
        opt.Window = TimeSpan.FromSeconds(30);
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 0;
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseExceptionHandler("/error");
app.UseHttpsRedirection();     
app.UseRouting();                 
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<AccessTokenMiddleware>(); 
app.UseRateLimiter();         
app.MapControllers();


app.Run();

