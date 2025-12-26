using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebStore.DAL.EF; 
using WebStore.Model.DataModels; 
using WebStore.Services.ConcreteServices; 
using WebStore.Services.Interfaces; 
using WebStore.ViewModels.VM;
// Dodaj ten using, aby widzieć klasę MainProfile (z Lab 1) [cite: 106]
using WebStore.Services.Configuration.Profiles; 

var builder = WebApplication.CreateBuilder(args);

// 1. Konfiguracja bazy danych i serwisów
builder.Services.AddDbContext<WebStoreDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Rejestracja serwisu produktów (niezbędne dla ProductApiController)
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IStoreService, StoreService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>(); 

// Rejestracja AutoMappera (niezbędne dla AccountApiController i innych) [cite: 111]
builder.Services.AddAutoMapper(typeof(MainProfile));
// ---------------------------------------------

// 2. Pobranie ustawień JWT
builder.Services.Configure<JwtOptionsVm>(options => builder.Configuration.GetSection("JwtOptions").Bind(options));

// 3. Konfiguracja Identity
builder.Services.AddIdentity<User, IdentityRole<int>>(o =>
{
    o.Password.RequireDigit = false;
    o.Password.RequireUppercase = false;
    o.Password.RequireLowercase = false;
    o.Password.RequireNonAlphanumeric = false;
    o.User.RequireUniqueEmail = false;
})
.AddEntityFrameworkStores<WebStoreDbContext>()
.AddDefaultTokenProviders();

// 4. Konfiguracja uwierzytelniania JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false,
        ValidateIssuer = false,
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SecretKey"])),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(5)
    };
});

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
);

// 5. Swagger (Dokumentacja API)
builder.Services.AddSwaggerGen(opt =>
{
    opt.SwaggerDoc("v1", new OpenApiInfo { Title = "WebStore API", Version = "v1" });
    opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    opt.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            new string[] { }
        }
    });
});

// --- DODAJ TO ABY NAPRAWIĆ JSON W SWAGGERZE ---
builder.Services.AddSwaggerGenNewtonsoftSupport();
// ----------------------------------------------

var app = builder.Build();

// 6. Pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebStore API v1"));

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");

app.Run();