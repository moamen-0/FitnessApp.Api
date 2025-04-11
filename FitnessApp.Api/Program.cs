

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
using System.Text.Json.Serialization;

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
			builder.Services.AddScoped<IFavoritesRepository, FavoritesRepository>();
			builder.Services.AddScoped<IMealService, MealService>();
			builder.Services.AddScoped<IDietPlanService, DietPlanService>();


			builder.Services.AddControllers()
	.AddJsonOptions(options =>
	{
		options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
		options.JsonSerializerOptions.MaxDepth = 64; // Increase from the default 32 if needed
	});
			builder.Services.AddControllers()
	.AddJsonOptions(options => {
		options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
	});

			builder.Services.AddAuthentication(options =>
			{
				options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
			})
.AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateLifetime = true,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"])),
		ClockSkew = TimeSpan.Zero // Reduces the default 5-minute leeway
	};

	options.Events = new JwtBearerEvents
	{
		OnAuthenticationFailed = context =>
		{
			Console.WriteLine($"Authentication failed: {context.Exception.Message}");
			return Task.CompletedTask;
		},
		OnTokenValidated = context =>
		{
			Console.WriteLine("Token validated successfully");
			return Task.CompletedTask;
		},
		OnMessageReceived = context =>
		{
			var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
			Console.WriteLine($"Token received: {token?.Substring(0, 20)}...");
			return Task.CompletedTask;
		}
	};
});
			builder.Services.AddAuthorization();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
			builder.Services.AddScoped<IUserService, UserService>();
			builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
	.AddEntityFrameworkStores<AppDbContext>()
	.AddDefaultTokenProviders();

			builder.Services.AddCors(options =>
			{
				options.AddPolicy("AllowFlutterApp",
					policy => policy
						.AllowAnyOrigin() // Consider restricting to specific origins in production
						.AllowAnyMethod()
						.AllowAnyHeader());
			});

			builder.Services.Configure<IdentityOptions>(options =>
			{
				// Password settings
				options.Password.RequireDigit = false;
				options.Password.RequireLowercase = false;
				options.Password.RequireNonAlphanumeric = false;
				options.Password.RequireUppercase = false;
				options.Password.RequiredLength = 1; // Minimum length (set to whatever minimum you want)
				options.Password.RequiredUniqueChars = 0;
			});
			var app = builder.Build();
			using (var scope = app.Services.CreateScope())
			{
				var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
				dbContext.Database.Migrate();
			}
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
			app.UseStaticFiles();


			app.MapOpenApi();
				app.UseSwagger();
				app.UseSwaggerUI();
			app.UseHttpsRedirection();
			app.UseCors("AllowFlutterApp");
			app.UseAuthentication(); 
			app.UseAuthorization();
		

			app.MapControllers();

            app.Run();
        }
    }
}
