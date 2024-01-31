using dhofarAPI.Data;
using dhofarAPI.InterFaces;
using dhofarAPI.Model;
using dhofarAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace dhofarAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers().AddNewtonsoftJson(options =>
            options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
          );

            // Add services to the container.
            string? connString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services
                .AddDbContext<dhofarDBContext>
                (option => option.UseSqlServer(connString));


            //builder.Services.AddTransient<ICateogry, CategoryService>();

            builder.Services.AddTransient<IComplaint, ComplaintServices>();
            builder.Services.AddScoped<JWTTokenService>();

            // Auth setup 
            builder.Services.AddIdentity<User, IdentityRole>
               (options =>
               {
                   options.User.RequireUniqueEmail = true;
               }).AddEntityFrameworkStores<dhofarDBContext>();

            // Add Services here 
            builder.Services.AddTransient<IIDentityUser, IdentityUserService>();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = JWTTokenService.GetValidationParamerts(builder.Configuration);
            });


            // Permissions Setup
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Create", policy => policy.RequireClaim("Permissions", "Create"));
                options.AddPolicy("Read", policy => policy.RequireClaim("Permissions", "Read"));
                options.AddPolicy("Update", policy => policy.RequireClaim("Permissions", "Update"));
                options.AddPolicy("Delete", policy => policy.RequireClaim("Permissions", "Delete"));
            });


            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI
                    ();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}