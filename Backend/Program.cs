using FeeCollectorApplication.Service;
using FeeCollectorApplication.Settings;

namespace FeeCollectorApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
    .AddJsonOptions(
        options => options.JsonSerializerOptions.PropertyNamingPolicy = null);
            builder.Services.Configure<FeeCollectorDatabaseSettings>(
                builder.Configuration.GetSection("MongoDB"));
            builder.Services.AddSingleton<FeeCollectorService>();
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