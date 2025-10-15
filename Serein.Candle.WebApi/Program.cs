using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serein.Candle.Application.Interfaces;
using Serein.Candle.Application.MappingProfiles;
using Serein.Candle.Application.Services;
using Serein.Candle.Domain.Interfaces;
using Serein.Candle.Domain.Settings;
using Serein.Candle.Infrastructure.Interfaces;
using Serein.Candle.Infrastructure.Persistence.Models;
using Serein.Candle.Infrastructure.Persistence.Repositories;
using Serein.Candle.Infrastructure.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// =======================================================
// THAY ĐỔI 1: Đọc Chuỗi Kết nối Azure SQL từ Biến Môi trường mới
// =======================================================
// Đọc biến môi trường mới: AZURE_DB_CONNECTION_STRING
var dbConnectionString = Environment.GetEnvironmentVariable("AZURE_DB_CONNECTION_STRING");

if (string.IsNullOrEmpty(dbConnectionString))
{
    // Nếu biến môi trường bị thiếu, ứng dụng nên báo lỗi
    throw new InvalidOperationException("Chuỗi kết nối DB (AZURE_DB_CONNECTION_STRING) bị thiếu trên môi trường triển khai.");
}

builder.Services.AddDbContext<Serein.Candle.Infrastructure.Persistence.Models.CandleShopDbContext>(options =>
    options.UseSqlServer(dbConnectionString,
    sqlServerOptions => sqlServerOptions.MigrationsAssembly("Serein.Candle.Infrastructure")));

// =======================================================
// THAY ĐỔI 2: Cấu hình CloudinarySettings
// =======================================================
builder.Services.Configure<CloudinarySettings>(options =>
{
    // Đọc các giá trị không nhạy cảm từ appsettings.json
    builder.Configuration.GetSection("CloudinarySettings").Bind(options);

    // Đọc các giá trị nhạy cảm từ Biến Môi trường mới
    options.ApiKey = Environment.GetEnvironmentVariable("CLOUDINARY_API_KEY");
    options.ApiSecret = Environment.GetEnvironmentVariable("CLOUDINARY_API_SECRET");

    if (string.IsNullOrEmpty(options.ApiKey) || string.IsNullOrEmpty(options.ApiSecret))
    {
        throw new InvalidOperationException("Cấu hình Cloudinary (API Key/Secret) bị thiếu trên môi trường triển khai.");
    }
});
builder.Services.AddTransient<IImageService, CloudinaryImageService>();

// =======================================================
// THAY ĐỔI 3: Cấu hình SmtpSettings
// =======================================================
builder.Services.Configure<SmtpSettings>(options =>
{
    // Đọc các giá trị không nhạy cảm từ appsettings.json
    builder.Configuration.GetSection("SmtpSettings").Bind(options);

    // Đọc mật khẩu nhạy cảm từ Biến Môi trường mới
    options.Password = Environment.GetEnvironmentVariable("SMTP_PASSWORD");

    if (string.IsNullOrEmpty(options.Password))
    {
        throw new InvalidOperationException("Mật khẩu SMTP (SMTP_PASSWORD) bị thiếu trên môi trường triển khai.");
    }
});
builder.Services.AddTransient<Serein.Candle.Application.Interfaces.IEmailService, Serein.Candle.Infrastructure.Services.EmailService>();


// Cấu hình CORS
var _MyAllowSpecificOrigins = "MyCORS";
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: _MyAllowSpecificOrigins,
        policy =>
        {
            policy.WithOrigins("http://localhost:8080")
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .AllowAnyHeader()
                    .AllowCredentials();
        });
});

// Đăng ký các Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IRoleRepository, RoleRepository>();
builder.Services.AddScoped<IStaffRepository, StaffRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductImageRepository, ProductImageRepository>();
builder.Services.AddScoped<IProductAttributeValueRepository, ProductAttributeValueRepository>();
builder.Services.AddScoped<IInventoryRepository, InventoryRepository>();
builder.Services.AddScoped<ICartItemRepository, CartItemRepository>();
builder.Services.AddScoped<ICartRepository, CartRepository>();
builder.Services.AddScoped<IWishlistRepository, WishlistRepository>();
builder.Services.AddScoped<IProductReviewRepository, ProductReviewRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Đăng ký AutoMapper
builder.Services.AddAutoMapper(typeof(GeneralMappingProfile));

// Đăng ký các lớp cụ thể cho ProductAttribute
builder.Services.AddScoped<IProductAttributeRepository, ProductAttributeRepository>();
builder.Services.AddScoped<IProductAttributeService, ProductAttributeService>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IVoucherRepository, VoucherRepository>();
builder.Services.AddScoped<IVoucherService, VoucherService>();
builder.Services.AddScoped<IRoleTypeRepository, RoleTypeRepository>();
builder.Services.AddScoped<IRoleTypeService, RoleTypeService>();
builder.Services.AddScoped<IProductReviewService, ProductReviewService>();

// Đăng ký Generic Services và Controllers 
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(IGenericService<,>), typeof(GenericService<,>));
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IWishlistService, WishlistService>();

// Đăng ký dịch vụ bộ nhớ đệm
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IOrderService, OrderService>();


// =======================================================
// THAY ĐỔI 4: Cấu hình JWT Settings
// =======================================================
var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
jwtSettings.Key = Environment.GetEnvironmentVariable("JWT_SECRET_KEY") ?? jwtSettings.Key;

if (string.IsNullOrEmpty(jwtSettings.Key))
{
    throw new InvalidOperationException("JWT_SECRET_KEY bị thiếu.");
}

builder.Services.AddSingleton(jwtSettings);


var key = Encoding.ASCII.GetBytes(jwtSettings.Key);

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
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors(_MyAllowSpecificOrigins);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();