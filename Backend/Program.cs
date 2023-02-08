
using FeeCollectorApplication.DataAccess;
using FeeCollectorApplication.Models.IModels;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver.Core.Configuration;
using MySql.EntityFrameworkCore.Extensions;

namespace FeeCollectorApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Add Database

            builder.Services.AddEntityFrameworkMySQL().AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            //services cors
            builder.Services.AddCors(p => p.AddPolicy("AllowAllHeadersPolicy", builder =>
            {
                builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
            }));
            var app = builder.Build();
            app.UseCors("AllowAllHeadersPolicy");
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}