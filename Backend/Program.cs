using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Repository;
using FeeCollectorApplication.Service;
using FeeCollectorApplication.Settings;
using Microsoft.EntityFrameworkCore;

namespace FeeCollectorApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

            builder.Services.AddControllers().AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("DefaultConnection"))
            );
            builder.Services.Configure<FeeCollectorDatabaseSettings>(
                builder.Configuration.GetSection("MongoDB"));

            builder.Services.AddSingleton<FeeCollectorService>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

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