using BookApi.Data;
using BookApi.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Text;

public class Startup
{
    /// <summary>
    /// Configures services for the application.
    /// </summary>
    /// <param name="services">The collection of service descriptors.</param>
    public void ConfigureServices(IServiceCollection services)
    {
        // Adds the DbContext for the application using SQLite as the database provider.
        var connectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING");
        services.AddDbContext<BookContext>(options => options.UseSqlite(connectionString));

        // Adds controller services to the application.
        services.AddControllers();

        // Add JWT authentication services
        var key = Encoding.ASCII.GetBytes("YourSecretKeyHere");
        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.RequireHttpsMetadata = false;
            x.SaveToken = true;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        // Register the repository service
        services.AddScoped<IBookRepository, BookRepository>();
    }

    /// <summary>
    /// Configures the HTTP request pipeline.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <param name="env">The web hosting environment.</param>
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        // Enables developer exception page in development environment.
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Home/Error");
            app.UseHsts();
        }

        // Adds routing middleware to the request pipeline.
        app.UseRouting();

        // Add authentication middleware
        app.UseAuthentication();

        // Adds authorization middleware to the request pipeline.
        app.UseAuthorization();

        // Add HTTPS redirection middleware
        app.UseHttpsRedirection();

        // Configures the endpoints for the application.
        app.UseEndpoints(endpoints =>
        {
            // Maps controller actions to endpoints.
            endpoints.MapControllers();
        });
    }

}