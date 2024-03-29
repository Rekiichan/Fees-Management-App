using FeeCollectorApplication.Repository.IRepository;
using FeeCollectorApplication.Repository;
using FeeCollectorApplication.DataAccess;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using FeeCollectorApplication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using FeeCollectorApplication.Utility;
using FeeCollectorApplication.Service.IService;
using FeeCollectorApplication.Service;
using FeeCollectorApplication.Settings;

var builder = WebApplication.CreateBuilder(args);

// Service use when in product environment

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("DefaultConnection")));

//Service use when in develop environment

//builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
//    builder.Configuration.GetConnectionString("DevelopEnvironment"))
//);

builder.Services.AddIdentity<ApplicationUser, IdentityRole>().AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireDigit = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 5;
});
var secretKey = builder.Configuration.GetValue<string>("ApiSettings:Secret");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(u =>
{
    u.RequireHttpsMetadata = false;
    u.SaveToken = true;
    u.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secretKey)),
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true
    };
});

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IMailService, MailService>();

builder.Services.Configure<MailSettings>(builder.Configuration.GetSection(nameof(MailSettings)));
builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ContractResolver = new DefaultContractResolver();
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
           new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

//services cors
builder.Services.AddCors(p => p.AddPolicy("AllowAllHeadersPolicy", builder =>
{
    builder.WithOrigins("*").AllowAnyMethod().AllowAnyHeader();
}));

// add policy
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(SD.Policy_RoleManager, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Admin);
    });

    options.AddPolicy(SD.Policy_SuperAdmin, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        //policyBuilder.RequireRole(SD.Role_Admin);
        policyBuilder.RequireClaim("Name", "Dao Trong Nhan");
    });

    options.AddPolicy(SD.Policy_AccountManager, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Employee, SD.Role_Admin);
    });

    options.AddPolicy(SD.Policy_BillManager, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Employee, SD.Role_Admin);
    });

    options.AddPolicy(SD.Policy_RoleManager, policyBuilder =>
    {
        policyBuilder.RequireAuthenticatedUser();
        policyBuilder.RequireRole(SD.Role_Admin);
    });
});


var app = builder.Build();
app.UseCors("AllowAllHeadersPolicy");

// Configure the HTTP request pipeline.

app.UseSwagger();
app.UseSwaggerUI();


//app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
