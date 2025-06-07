

using FestivalFlatform.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

namespace FF.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            #region JWT
            var jwtSettings = builder.Configuration.GetSection("JwtAuth");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);


            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidateAudience = true,
                    ValidAudience = jwtSettings["Audience"],
                    ValidateLifetime = true,
                };
            });
            #endregion
            // 1. Đọc chuỗi cấu hình JWT từ appsettings.json
            

            // 2. Đăng ký Authentication với JwtBearer
           

            // Thêm Authorization
            builder.Services.AddAuthorization();

            // Thêm các service khác (Repositories, Services, v.v.)
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register ApplicationDbContext
            builder.Services.AddDbContext<FestivalFlatformDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Thêm MemoryCache
            builder.Services.AddMemoryCache();


            // Inject IWebHostEnvironment: giúp acc update ảnh đại diện
            builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    policy => policy.AllowAnyOrigin()
                                    .AllowAnyMethod()
                                    .AllowAnyHeader());
            });

            builder.Services.AddAutoMapper(typeof(Program));
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
            .AddEntityFrameworkStores<FestivalFlatformDbContext>()
            .AddDefaultTokenProviders();



            builder.Services.AddControllers();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "LSAPI", Version = "v1" });


                // Thêm khai báo bảo mật Bearer
                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                c.AddSecurityDefinition("Bearer", securitySchema
                   );

                c.AddSecurityRequirement(new OpenApiSecurityRequirement {
                {
                        securitySchema,
                    new string[] { "Bearer" }
                    }
                }

                );


            });
            // Register IService and Service


            var app = builder.Build();

            app.UseCors("AllowAll");
            app.UseAuthorization();
            app.MapControllers();

            // Lấy IWebHostEnvironment từ app.Services
            var env = app.Services.GetRequiredService<IWebHostEnvironment>();

            // Debug thông tin môi trường
            Console.WriteLine($"Environment: {env.EnvironmentName}");
            Console.WriteLine($"WebRootPath: {env.WebRootPath}");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseStaticFiles(); // Cho phép truy cập ảnh đã upload

            app.MapControllers();

            app.Run();


        }
    }
}
