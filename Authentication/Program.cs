
using Authentication;
using Authentication.Repository;
using Authentication.Security;
using Authentication.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;


var builder = WebApplication.CreateBuilder(args);

var refreshSecret = Environment.GetEnvironmentVariable("REFRESH_TOKEN_SECRET");

if (string.IsNullOrWhiteSpace(refreshSecret))
{
    throw new InvalidOperationException("REFRESH_TOKEN_SECRET is not set in the environment.");
}


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MySqlServerVersion(new Version(8, 0, 0)) 
    )
);

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
builder.Services.AddScoped<RefreshTokenService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ITransactionRepository,TransactionRepository>();
builder.Services.AddScoped<PasswordEncoder>();
builder.Services.AddSingleton(new TokenHasher(refreshSecret));


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

app.UseRouting();
app.UseRateLimiter();
app.MapControllers().RequireRateLimiting("auth-limit");
app.UseExceptionHandler("/error");
app.UseHttpsRedirection();
app.MapControllers();


app.Run();

