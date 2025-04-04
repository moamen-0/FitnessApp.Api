

using FitnessApp.BLL.Services;
using FitnessApp.Core.Entities;
using FitnessApp.Core.Interfaces;
using FitnessApp.Core.Interfaces.IRepository;
using FitnessApp.Core.Interfaces.IService;
using FitnessApp.DAL;
using FitnessApp.DAL.Data;
using FitnessApp.DAL.Repositories;
using FitnessApp.DAL.Utilities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FitnessApp.Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
			builder.Services.AddEndpointsApiExplorer();
			builder.Services.AddSwaggerGen();
			builder.Services.AddDbContext<AppDbContext>(options =>
			options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
		
			builder.Services.AddScoped<IExerciseRepository, ExerciseRepository>();
			builder.Services.AddScoped<IWorkoutPlanRepository, WorkoutPlanRepository>();
			builder.Services.AddScoped<IUserRepository, UserRepository>();
			builder.Services.AddScoped<IWorkoutPlanGenerationService, WorkoutPlanGenerationService>();
			builder.Services.AddScoped<IInBodyResultRepository, InBodyResultRepository>();
			builder.Services.AddScoped<IUserProgressService, UserProgressService>();
			builder.Services.AddScoped<IUserProgressRepository, UserProgressRepository>();
			builder.Services.AddScoped<IEmailService, EmailService>();
			builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IUserService, UserService>();




			builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidateIssuer = false,
						ValidateAudience = false,
						ValidateLifetime = true,
						ValidateIssuerSigningKey = true,
						IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]))
					};
				});
  builder.Services.AddAuthorization();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();




			var app = builder.Build();

			using (var scope = app.Services.CreateScope())
			{
				var services = scope.ServiceProvider;
				try
				{
					var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

					// Create roles if they don't exist
					if (!await roleManager.RoleExistsAsync(Roles.Admin))
						await roleManager.CreateAsync(new IdentityRole(Roles.Admin));

					if (!await roleManager.RoleExistsAsync(Roles.User))
						await roleManager.CreateAsync(new IdentityRole(Roles.User));

					// You can add more roles here if needed
				}
				catch (Exception ex)
				{
					var logger = services.GetRequiredService<ILogger<Program>>();
					logger.LogError(ex, "An error occurred while creating roles.");
				}
			}

			// Configure the HTTP request pipeline.
			if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
				app.UseSwagger();
				app.UseSwaggerUI();
			}

           
			app.UseAuthentication();


			app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
