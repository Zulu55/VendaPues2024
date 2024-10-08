using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using VendaPues.Backend.Data;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.Repositories.Implementations;
using VendaPues.Backend.Repositories.Interfaces;
using VendaPues.Backend.UnitsOfWork.Implementations;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.Entities;

using VendaPues.Backend.Data;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder
            .Services
            .AddControllers()
            .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "Orders Backend", Version = "v1" });
            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = @"JWT Authorization header using the Bearer scheme. <br /> <br />
                      Enter 'Bearer' [space] and then your token in the text input below.<br /> <br />
                      Example: 'Bearer 12345abcdef'<br /> <br />",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });
            c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
              In = ParameterLocation.Header,
            },
            new List<string>()
          }
                });
        });

        builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));
        builder.Services.AddTransient<SeedDb>();
        builder.Services.AddScoped<IFileStorage, FileStorage>();
        builder.Services.AddScoped<IMailHelper, MailHelper>();
        builder.Services.AddScoped<IOrdersHelper, OrdersHelper>();
        builder.Services.AddScoped<IPurchaseHelper, PurchaseHelper>();
        builder.Services.AddScoped<ISmtpClient, SmtpClientWrapper>();
        builder.Services.AddScoped<IBlobContainerClientFactory, BlobContainerClientFactory>();
        builder.Services.AddScoped<IRuntimeInformationWrapper, RuntimeInformationWrapper>();

        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));

        builder.Services.AddScoped<IBanksRepository, BanksRepository>();
        builder.Services.AddScoped<IBanksUnitOfWork, BanksUnitOfWork>();
        builder.Services.AddScoped<ICategoriesRepository, CategoriesRepository>();
        builder.Services.AddScoped<ICategoriesUnitOfWork, CategoriesUnitOfWork>();
        builder.Services.AddScoped<ICitiesRepository, CitiesRepository>();
        builder.Services.AddScoped<ICitiesUnitOfWork, CitiesUnitOfWork>();
        builder.Services.AddScoped<ICountriesRepository, CountriesRepository>();
        builder.Services.AddScoped<ICountriesUnitOfWork, CountriesUnitOfWork>();
        builder.Services.AddScoped<IInventoriesRepository, InventoriesRepository>();
        builder.Services.AddScoped<IInventoriesUnitOfWork, InventoriesUnitOfWork>();
        builder.Services.AddScoped<IInventoryDetailsRepository, InventoryDetailsRepository>();
        builder.Services.AddScoped<IInventoryDetailsUnitOfWork, InventoryDetailsUnitOfWork>();
        builder.Services.AddScoped<IKardexRepository, KardexRepository>();
        builder.Services.AddScoped<IKardexUnitOfWork, KardexUnitOfWork>();
        builder.Services.AddScoped<INewsRepository, NewsRepository>();
        builder.Services.AddScoped<INewsUnitOfWork, NewsUnitOfWork>();
        builder.Services.AddScoped<IOrdersRepository, OrdersRepository>();
        builder.Services.AddScoped<IOrdersUnitOfWork, OrdersUnitOfWork>();
        builder.Services.AddScoped<IProductsRepository, ProductsRepository>();
        builder.Services.AddScoped<IProductsUnitOfWork, ProductsUnitOfWork>();
        builder.Services.AddScoped<IPurchaseRepository, PurchaseRepository>();
        builder.Services.AddScoped<IPurchaseUnitOfWork, PurchaseUnitOfWork>();
        builder.Services.AddScoped<IPurchaseDetailRepository, PurchaseDetailRepository>();
        builder.Services.AddScoped<IPurchaseDetailUnitOfWork, PurchaseDetailUnitOfWork>();
        builder.Services.AddScoped<IStatesRepository, StatesRepository>();
        builder.Services.AddScoped<IStatesUnitOfWork, StatesUnitOfWork>();
        builder.Services.AddScoped<ISuppliersRepository, SuppliersRepository>();
        builder.Services.AddScoped<ISuppliersUnitOfWork, SuppliersUnitOfWork>();
        builder.Services.AddScoped<ITemporalOrdersRepository, TemporalOrdersRepository>();
        builder.Services.AddScoped<ITemporalOrdersUnitOfWork, TemporalOrdersUnitOfWork>();
        builder.Services.AddScoped<ITemporalPurchasesRepository, TemporalPurchasesRepository>();
        builder.Services.AddScoped<ITemporalPurchasesUnitOfWork, TemporalPurchasesUnitOfWork>();
        builder.Services.AddScoped<IUsersRepository, UsersRepository>();
        builder.Services.AddScoped<IUsersUnitOfWork, UsersUnitOfWork>();

        builder.Services.AddIdentity<User, IdentityRole>(x =>
        {
            x.Tokens.AuthenticatorTokenProvider = TokenOptions.DefaultAuthenticatorProvider;
            x.SignIn.RequireConfirmedEmail = true;
            x.User.RequireUniqueEmail = true;
            x.Password.RequireDigit = false;
            x.Password.RequiredUniqueChars = 0;
            x.Password.RequireLowercase = false;
            x.Password.RequireNonAlphanumeric = false;
            x.Password.RequireUppercase = false;
            x.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
            x.Lockout.MaxFailedAccessAttempts = 3;
            x.Lockout.AllowedForNewUsers = true;
        })
            .AddEntityFrameworkStores<DataContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(x => x.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["jwtKey"]!)),
                ClockSkew = TimeSpan.Zero
            });

        var app = builder.Build();
        SeedData(app);

        void SeedData(WebApplication app)
        {
            var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

            using (var scope = scopedFactory!.CreateScope())
            {
                var service = scope.ServiceProvider.GetService<SeedDb>();
                service!.SeedAsync().Wait();
            }
        }

        app.UseCors(x => x
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => true)
            .AllowCredentials());

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