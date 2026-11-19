using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;

namespace MyLittleRPG_Etape4
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Connexion à la base de données
            builder.Services.AddDbContext<Data.Context.RPGContext>(options =>
            {
                string connectionString = builder.Configuration.GetConnectionString("Default");

                options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 46)));
            });

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllLocal", policy =>
                {
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader();
                });
            });

            //anti brute force sur login/register block nb tries
            builder.Services.AddRateLimiter(options =>
            {
                //error code to display
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

                //what to do when error triggered
                options.OnRejected = async (context, token) =>
                    await context.HttpContext.Response.WriteAsJsonAsync(new { message = "Trop de tentatives. Veuillez réessayer ultérieurement." }, token);

                //define ratelimiter details : limit of 5, block for 1min.
                options.AddPolicy("auth", httpContext =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                        _ => new FixedWindowRateLimiterOptions { PermitLimit = 5, Window = TimeSpan.FromMinutes(1) }));
            });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration.GetSection("Jwt")["Issuer"],
                        ValidateAudience = true,
                        ValidAudience = builder.Configuration.GetSection("Jwt")["Audience"],
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration.GetSection("Jwt")["Key"]!))
                    };
                });

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            //avant error handler pour que les 500 restent avec header CORS
            app.UseCors("AllowAllLocal");

            //unhandled exception = 500
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(new { message = "Erreur du serveur. Réessayez plus tard." });
                });
            });

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();

            app.Run();
        }
    }
}
