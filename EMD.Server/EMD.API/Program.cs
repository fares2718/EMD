using AutoMapper;
using EMD.API.Authorization;
using EMD.BLL;
using EMD.BLL.DTOs;
using EMD.DAL.DA;
using EMD.EF;
using EMD.EF.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Threading.RateLimiting;

public partial class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        var secretKey = builder.Configuration["JWT_SECRET_KEY"];
        if (string.IsNullOrWhiteSpace(secretKey))
            throw new Exception("JWT secret key is not configured.");

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = "EMD",
                    ValidAudience = "EMDUsers",
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey))
                };

                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var accessToken = context.Request.Cookies["accessToken"];
                        if (!string.IsNullOrEmpty(accessToken))
                        {
                            context.Token = accessToken;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

        builder.Services.AddScoped<IAuthorizationHandler, EmployeeOwnerOrAdminHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, EmployeeDepartmentOrAdminHandler>();
        builder.Services.AddScoped<IAuthorizationHandler, EmployeeDesignationOrAdminHandler>();


        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("EmployeeOwnerOrAdmin", policy =>
                policy.Requirements.Add(new EmployeeOwnerOrAdminRequirement()));
            options.AddPolicy("EmployeeDepartmentOrAdmin", policy =>
                policy.Requirements.Add(new EmployeeDepartmentOrAdminRequirement()));
            options.AddPolicy("EmployeeDesignationOrAdmin", policy =>
                policy.Requirements.Add(new EmployeeDesignationOrAdminRequirement()));
        });

        builder.Services.AddAutoMapper(cfg =>
        {
            cfg.CreateMap<Employee, EmployeeDTO>();
            cfg.CreateMap<AddEmployeeRequest, Employee>();
            cfg.CreateMap<Session, SesstionDTO>();
        });

        builder.Services.AddRateLimiter(options =>
        {
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

            options.AddPolicy("AuthLimiter", httpContext =>
            {
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                return RateLimitPartition.GetFixedWindowLimiter(ip, _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(15),
                    QueueLimit = 0
                });
            });
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "EMD API", Version = "v1" });

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "Bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
            });
        });

        builder.Services.AddControllers();

        builder.Services.AddDbContext<EMDDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("EMDCon")));

        builder.Services.AddScoped<DepartmentBusiness>();
        builder.Services.AddScoped<DepartmentDataAccess>();
        builder.Services.AddScoped<DesignationBusiness>();
        builder.Services.AddScoped<DesignationDataAccess>();
        builder.Services.AddScoped<EmployeeBusiness>();
        builder.Services.AddScoped<EmployeeDataAccess>();
        builder.Services.AddScoped<SessionBusiness>();
        builder.Services.AddScoped<SessionsDataAccess>();


        builder.Services.AddCors(options =>
        {
            options.AddPolicy("EMDAngularClient", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials();
            });
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseHttpsRedirection();
        app.UseCors("EMDAngularClient");

        app.UseRateLimiter();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}