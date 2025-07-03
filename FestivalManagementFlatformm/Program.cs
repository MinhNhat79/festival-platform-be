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
using FestivalFlatform.Service.Helpers;
using FestivalFlatform.Service.Services.Implement;
using FestivalFlatform.Service.Services.Interface;
using FestivalFlatform.Data.UnitOfWork;
using System.Security.Claims;

namespace FF.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Lấy cấu hình JWT từ appsettings.json
            var jwtSettings = builder.Configuration.GetSection("JwtAuth");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"]);

            #region Đăng ký Authentication với JWT Bearer
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

            // Đăng ký Authorization với các policy theo Roles
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy(Roles.Admin, policy =>
                    policy.RequireRole(Roles.Admin));
                options.AddPolicy(Roles.SchoolManager, policy =>
                    policy.RequireRole(Roles.SchoolManager));
                options.AddPolicy(Roles.Teacher, policy =>
                    policy.RequireRole(Roles.Teacher));
                options.AddPolicy(Roles.Student, policy =>
                    policy.RequireRole(Roles.Student));
                options.AddPolicy(Roles.Supplier, policy =>
                    policy.RequireRole(Roles.Supplier));
            });

            // Đăng ký DbContext với SQL Server
            builder.Services.AddDbContext<FestivalFlatformDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Đăng ký Identity (nếu bạn có sử dụng)
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<FestivalFlatformDbContext>()
                .AddDefaultTokenProviders();

            // Các service khác
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
            builder.Services.AddScoped<ILoginService, LoginService>();
            builder.Services.AddScoped<IAccountService, AccountService>();
            builder.Services.AddScoped<IAccountPointsService, AccountPointsService>();
            builder.Services.AddScoped<IFestivalService, FestivalService>();
            builder.Services.AddScoped<IBoothService, BoothService>();
            builder.Services.AddScoped<ISupplierService, SupplierService>();
            builder.Services.AddScoped<ISchoolService, SchoolService>();
  
            builder.Services.AddScoped<IStudentGroupService, StudentGroupService>();
            builder.Services.AddScoped<IFestivalMapService, FestivalMapService>();
            builder.Services.AddScoped<IMapLocationService, MapLocationService>();
            builder.Services.AddScoped<IFestivalMenuService, FestivalMenuService>();
            builder.Services.AddScoped<IBoothMenuItemService, BoothMenuItemService>();
            builder.Services.AddScoped<IMenuItemService, MenuItemService>();
            builder.Services.AddScoped<IIngredientService, IngredientService>();
            builder.Services.AddScoped<IFestivalIngredientService, FestivalIngredientService>();
            builder.Services.AddScoped<IFestivalSchoolService, FestivalSchoolService>();
            builder.Services.AddScoped<IOrderService, OrderService>();
            builder.Services.AddScoped<IGroupMemberService, GroupMemberService>();
            builder.Services.AddScoped<IImageService, ImageService>();
            builder.Services.AddScoped<IMenuItemIngredientService, MenuItemIngredientService>();
            builder.Services.AddScoped<IMiniGameService, MiniGameService>();
            builder.Services.AddScoped<IOrderItemService, OrderItemService>();
            builder.Services.AddScoped<IPointsTransactionService, PointsTransactionService>();
            builder.Services.AddScoped<IQuestionService, QuestionService>();
            builder.Services.AddScoped<IChatSessionService, ChatSessionService>();
            builder.Services.AddScoped<IRoleService, RoleService>();





            // Các cấu hình bổ sung
            builder.Services.AddControllers()
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "FF.API", Version = "v1" });

                // Cấu hình Swagger để hỗ trợ JWT Bearer Authentication
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
                c.AddSecurityDefinition("Bearer", securitySchema);
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, new string[] { "Bearer" } }
                });
            });

            builder.Services.AddMemoryCache();

            builder.Services.AddSingleton<IWebHostEnvironment>(builder.Environment);

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policy =>
                    policy.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
            });

            builder.Services.AddAutoMapper(typeof(Program));

            var app = builder.Build();

            // Môi trường phát triển

            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "FF.API v1"));


            app.UseDeveloperExceptionPage();

            app.UseRouting();

            app.UseCors("AllowAll");

            app.UseAuthentication(); // Phải trước Authorization
            app.UseAuthorization();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.MapControllers();

            app.Run();
        }
    }
}
