using Microsoft.EntityFrameworkCore;
using System.Runtime.InteropServices;
using VendaPues.Backend.Helpers;
using VendaPues.Backend.UnitsOfWork.Interfaces;
using VendaPues.Shared.DTOs;
using VendaPues.Shared.Entities;
using VendaPues.Shared.Enums;

namespace VendaPues.Backend.Data;

public class SeedDb
{
    private readonly DataContext _context;
    private readonly IUsersUnitOfWork _usersUnitOfWork;
    private readonly IFileStorage _fileStorage;
    private readonly IRuntimeInformationWrapper _runtimeInformationWrapper;
    private readonly IPurchaseHelper _purchaseHelper;

    public SeedDb(DataContext context, IUsersUnitOfWork usersUnitOfWork, IFileStorage fileStorage, IRuntimeInformationWrapper runtimeInformationWrapper, IPurchaseHelper purchaseHelper)
    {
        _context = context;
        _usersUnitOfWork = usersUnitOfWork;
        _fileStorage = fileStorage;
        _runtimeInformationWrapper = runtimeInformationWrapper;
        _purchaseHelper = purchaseHelper;
    }

    public async Task SeedAsync()
    {
        await _context.Database.EnsureCreatedAsync();
        await CheckCountriesFullAsync();
        await CheckCountriesAsync();
        await CheckCategoriesAsync();
        await CheckRolesAsync();
        await CheckProductsAsync();
        await CheckUsersAsync();
        await CheckSuppiersAsync();
        await CheckBanksAsync();
        await CheckPromotionsAsync();

        //TODO: Remove in production evironments
        await CheckPurchaseAsync();
    }

    private async Task CheckPromotionsAsync()
    {
        if (!_context.NewsArticles.Any())
        {
            await AddPromotionAsync("Black Friday", "BlackFriday.jpg");
            await AddPromotionAsync("Ahora podrás pagar con PSE!", "PagoPSE.jpg");
            await AddPromotionAsync("Adidas con el 30% off", "PromoAdidas.jpg");
            await AddPromotionAsync("Adidas 2 x 1", "PromoAdidas2.jpg");
            await AddPromotionAsync("Promociones en sus pedidos de Burguer King", "PromoBK.jpg");
            await AddPromotionAsync("Promociones en los últimos iPhone", "PromoIPhone.jpg");
            await _context.SaveChangesAsync();
        }
    }

    private async Task AddPromotionAsync(string title, string image)
    {
        string filePath;
        if (_runtimeInformationWrapper.IsOSPlatform(OSPlatform.Windows))
        {
            filePath = $"{Environment.CurrentDirectory}\\Images\\promos\\{image}";
        }
        else
        {
            filePath = $"{Environment.CurrentDirectory}/Images/promos/{image}";
        }

        var fileBytes = File.ReadAllBytes(filePath);
        var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "promos");

        var newsArticle = new NewsArticle
        {
            Title = title,
            Active = true,
            ImageUrl = imagePath,
            Summary = "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Vitae proin sagittis nisl rhoncus mattis rhoncus urna. Risus at ultrices mi tempus imperdiet. Placerat vestibulum lectus mauris ultrices. Nam at lectus urna duis. Ac felis donec et odio pellentesque. Phasellus faucibus scelerisque eleifend donec pretium. A diam maecenas sed enim ut. Sapien faucibus et molestie ac feugiat sed lectus vestibulum mattis. Enim neque volutpat ac tincidunt vitae semper quis lectus. Nisl nunc mi ipsum faucibus vitae aliquet nec ullamcorper. Sagittis purus sit amet volutpat consequat mauris. Bibendum enim facilisis gravida neque convallis a.\r\n\r\nVolutpat ac tincidunt vitae semper quis lectus nulla at. Facilisis magna etiam tempor orci eu lobortis elementum nibh. Sed odio morbi quis commodo odio aenean. Ultricies mi eget mauris pharetra. Sit amet facilisis magna etiam tempor. At urna condimentum mattis pellentesque id nibh tortor id aliquet. Sed id semper risus in hendrerit gravida rutrum. Mi in nulla posuere sollicitudin aliquam. Egestas erat imperdiet sed euismod nisi. Nibh nisl condimentum id venenatis a. Tellus mauris a diam maecenas sed enim ut sem. Id eu nisl nunc mi ipsum faucibus vitae aliquet. Nullam eget felis eget nunc lobortis.",
        };
        _context.NewsArticles.Add(newsArticle);
    }

    private async Task CheckBanksAsync()
    {
        if (!_context.Banks.Any())
        {
            _context.Banks.Add(new Bank { Name = "Alianza Fiduciaria" });
            _context.Banks.Add(new Bank { Name = "Ban100" });
            _context.Banks.Add(new Bank { Name = "Bancamia S.A." });
            _context.Banks.Add(new Bank { Name = "Banco Agrario" });
            _context.Banks.Add(new Bank { Name = "Baco AV Villas" });
            _context.Banks.Add(new Bank { Name = "Banco BBVA Colombia S.A." });
            _context.Banks.Add(new Bank { Name = "Banco Caja Social" });
            _context.Banks.Add(new Bank { Name = "Banco Coopperativo Coopcentro" });
            _context.Banks.Add(new Bank { Name = "Banco Davivienda" });
            _context.Banks.Add(new Bank { Name = "Banco de Bogotá" });
            _context.Banks.Add(new Bank { Name = "Banco Falabella" });
            _context.Banks.Add(new Bank { Name = "Banco Finadina S.A. BIC" });
            _context.Banks.Add(new Bank { Name = "Banco Itau" });
            _context.Banks.Add(new Bank { Name = "Banco J.P. Morgan Colombia" });
            _context.Banks.Add(new Bank { Name = "Banco Mundo Mujer S.A." });
            _context.Banks.Add(new Bank { Name = "Banco Pichincha S.A." });
            _context.Banks.Add(new Bank { Name = "Banco Popular" });
            _context.Banks.Add(new Bank { Name = "Banco Santander Colombia" });
            _context.Banks.Add(new Bank { Name = "Banco Serfinanza" });
            _context.Banks.Add(new Bank { Name = "Bancolombia" });
            _context.Banks.Add(new Bank { Name = "CFA Cooperativa Financiera" });
            _context.Banks.Add(new Bank { Name = "Citybank" });
            _context.Banks.Add(new Bank { Name = "Coltefinanciera" });
            _context.Banks.Add(new Bank { Name = "Confiar Cooperativa Financiera" });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckPurchaseAsync()
    {
        if (!_context.Kardex.Any())
        {
            var supplier = await _context.Suppliers.FirstOrDefaultAsync();
            var user = await _context.Users.FirstOrDefaultAsync(x => x.UserType == UserType.Admin);

            for (int i = 1; i <= 4; i++)
            {
                var purchaseDTO = new PurchaseDTO
                {
                    Date = DateTime.UtcNow,
                    SupplierId = supplier!.Id,
                    Remarks = $"Compra {i} agregada por SeedDb para propositos de Testing.",
                    PurchaseDetails = []
                };

                var random = new Random();
                foreach (var product in _context.Products)
                {
                    decimal percentajeCost = 1M - random.Next(10, 41) / 100M;
                    purchaseDTO.PurchaseDetails.Add(new PurchaseDetailDTO
                    {
                        Cost = product.Price * percentajeCost,
                        ProductId = product.Id,
                        Quantity = random.Next(6, 25),
                        Remarks = $"Compra {i} agregada por SeedDb para propositos de Testing.",
                    });
                }

                await _purchaseHelper.ProcessPurchaseAsync(purchaseDTO, user!.UserName!);
            }
        }
    }

    private async Task CheckSuppiersAsync()
    {
        if (!_context.Suppliers.Any())
        {
            var city = await GetCityAsync();
            _context.Suppliers.Add(new Supplier
            {
                Address = "Calle 80A 54 14",
                City = city,
                ContactFirstName = "John",
                ContactLastName = "Doe",
                Document = "800456798-2",
                Email = "compras@grupoexito.com",
                Phone = "604 590 4232",
                SupplierName = "Grupo Exito",
            });
            _context.Suppliers.Add(new Supplier
            {
                Address = "Diagonal 45A 54 14",
                City = city,
                ContactFirstName = "Luisa",
                ContactLastName = "Sandoval",
                Document = "800456798-1",
                Email = "ventas@meli.com",
                Phone = "604 590 8080",
                SupplierName = "Mercado Libre",
            });
            _context.Suppliers.Add(new Supplier
            {
                Address = "Diagonal 45A 54 14",
                City = city,
                ContactFirstName = "Claudia",
                ContactLastName = "Carreño",
                Document = "800456798-0",
                Email = "ventas@todopets.com",
                Phone = "604 590 8080",
                SupplierName = "Todo PETs",
            });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckUsersAsync()
    {
        await CheckUserAsync("0001", "Juan", "Zuluaga", "zulu@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "JuanZuluaga.jpg", UserType.Admin);
        await CheckUserAsync("0002", "Ledys", "Bedoya", "ledys@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "LedysBedoya.jpg", UserType.User);
        await CheckUserAsync("0003", "Brad", "Pitt", "brad@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "Brad.jpg", UserType.User);
        await CheckUserAsync("0004", "Angelina", "Jolie", "angelina@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "Angelina.jpg", UserType.User);
        await CheckUserAsync("0005", "Bob", "Marley", "bob@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "bob.jpg", UserType.User);
        await CheckUserAsync("0006", "Celia", "Cruz", "celia@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "celia.jpg", UserType.Admin);
        await CheckUserAsync("0007", "Fredy", "Mercury", "fredy@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "fredy.jpg", UserType.User);
        await CheckUserAsync("0008", "Hector", "Lavoe", "hector@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "hector.jpg", UserType.User);
        await CheckUserAsync("0009", "Liv", "Taylor", "liv@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "liv.jpg", UserType.User);
        await CheckUserAsync("0010", "Otep", "Shamaya", "otep@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "otep.jpg", UserType.User);
        await CheckUserAsync("0011", "Ozzy", "Osbourne", "ozzy@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "ozzy.jpg", UserType.User);
        await CheckUserAsync("0012", "Selena", "Quintanilla", "selenba@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", "selena.jpg", UserType.User);
    }

    private async Task CheckCountriesFullAsync()
    {
        if (!_context.Countries.Any())
        {
            var countriesStatesCitiesSQLScript = File.ReadAllText("Data\\CountriesStatesCities.sql");
            await _context.Database.ExecuteSqlRawAsync(countriesStatesCitiesSQLScript);
        }
    }

    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, string image, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);
        if (user == null)
        {
            var city = await GetCityAsync();

            string filePath;
            if (_runtimeInformationWrapper.IsOSPlatform(OSPlatform.Windows))
            {
                filePath = $"{Environment.CurrentDirectory}\\Images\\users\\{image}";
            }
            else
            {
                filePath = $"{Environment.CurrentDirectory}/Images/users/{image}";
            }

            var fileBytes = File.ReadAllBytes(filePath);
            var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "users");

            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Address = address,
                Document = document,
                City = city,
                UserType = userType,
                Photo = imagePath,
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456");
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());

            var token = await _usersUnitOfWork.GenerateEmailConfirmationTokenAsync(user);
            await _usersUnitOfWork.ConfirmEmailAsync(user, token);
        }

        return user;
    }

    private async Task<City?> GetCityAsync()
    {
        var city = await _context.Cities.FirstOrDefaultAsync(x => x.Name == "Medellín");
        city ??= await _context.Cities.FirstOrDefaultAsync();
        return city;
    }

    private async Task CheckCategoriesAsync()
    {
        if (!_context.Categories.Any())
        {
            _context.Categories.Add(new Category { Name = "Apple" });
            _context.Categories.Add(new Category { Name = "Autos" });
            _context.Categories.Add(new Category { Name = "Belleza" });
            _context.Categories.Add(new Category { Name = "Calzado" });
            _context.Categories.Add(new Category { Name = "Comida" });
            _context.Categories.Add(new Category { Name = "Cosmeticos" });
            _context.Categories.Add(new Category { Name = "Deportes" });
            _context.Categories.Add(new Category { Name = "Gamer" });
            _context.Categories.Add(new Category { Name = "Jugetes" });
            _context.Categories.Add(new Category { Name = "Mascotas" });
            _context.Categories.Add(new Category { Name = "Nutrición" });
            _context.Categories.Add(new Category { Name = "Ropa" });
            _context.Categories.Add(new Category { Name = "Tecnología" });
            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckProductsAsync()
    {
        if (!_context.Products.Any())
        {
            await AddProductAsync("Adidas Barracuda", 270000M, ["Calzado", "Deportes"], ["adidas_barracuda.png"], 230000M, 0.3F);
            await AddProductAsync("Adidas Superstar", 250000M, ["Calzado", "Deportes"], ["Adidas_superstar.png"], 200000M, 0.3F);
            await AddProductAsync("Aguacate", 5000M, ["Comida"], ["Aguacate1.png", "Aguacate2.png", "Aguacate3.png"], 4000M, 0.3F);
            await AddProductAsync("AirPods", 1300000M, ["Tecnología", "Apple"], ["airpos.png", "airpos2.png"], 1000000M, 0.3F);
            await AddProductAsync("Akai APC40 MKII", 2650000M, ["Tecnología"], ["Akai1.png", "Akai2.png", "Akai3.png"], 2150000M, 0.3F);
            await AddProductAsync("Apple Watch Ultra", 4500000M, ["Apple", "Tecnología"], ["AppleWatchUltra1.png", "AppleWatchUltra2.png"], 4000000M, 0.3F);
            await AddProductAsync("Audifonos Bose", 870000M, ["Tecnología"], ["audifonos_bose.png"], 800000M, 0.3F);
            await AddProductAsync("Bicicleta Ribble", 12000000M, ["Deportes"], ["bicicleta_ribble.png"], 10000000M, 0.3F);
            await AddProductAsync("Camisa Cuadros", 56000M, ["Ropa"], ["camisa_cuadros.png"], 50000M, 0.3F);
            await AddProductAsync("Casco Bicicleta", 820000M, ["Deportes"], ["casco_bicicleta.png", "casco.png"], 750000M, 0.3F);
            await AddProductAsync("Gafas deportivas", 160000M, ["Deportes"], ["Gafas1.png", "Gafas2.png", "Gafas3.png"], 130000M, 0.3F);
            await AddProductAsync("Hamburguesa triple carne", 25500M, ["Comida"], ["Hamburguesa1.png", "Hamburguesa2.png", "Hamburguesa3.png"], 16500M, 0.3F);
            await AddProductAsync("iPad", 2300000M, ["Tecnología", "Apple"], ["ipad.png"], 200000M, 0.3F);
            await AddProductAsync("iPhone 13", 5200000M, ["Tecnología", "Apple"], ["iphone13.png", "iphone13b.png", "iphone13c.png", "iphone13d.png"], 4900000M, 0.3F);
            await AddProductAsync("Johnnie Walker Blue Label 750ml", 1266700M, ["Licores"], ["JohnnieWalker3.png", "JohnnieWalker2.png", "JohnnieWalker1.png"], 1000000M, 0.3F);
            await AddProductAsync("KOOY Disfraz inflable de gallo para montar", 150000M, ["Juguetes"], new List<string>() { "KOOY1.png", "KOOY2.png", "KOOY3.png" }, 100000M, 0.3F);
            await AddProductAsync("Mac Book Pro", 12100000M, ["Tecnología", "Apple"], ["mac_book_pro.png"], 11500000M, 0.3F);
            await AddProductAsync("Mancuernas", 370000M, ["Deportes"], ["mancuernas.png"], 300000M, 0.3F);
            await AddProductAsync("Mascarilla Cara", 26000M, ["Belleza"], ["mascarilla_cara.png"], 20000M, 0.3F);
            await AddProductAsync("New Balance 530", 180000M, ["Calzado", "Deportes"], ["newbalance530.png"], 140000M, 0.3F);
            await AddProductAsync("New Balance 565", 179000M, ["Calzado", "Deportes"], ["newbalance565.png"], 155000M, 0.3F);
            await AddProductAsync("Nike Air", 233000M, ["Calzado", "Deportes"], ["nike_air.png"], 200000M, 0.3F);
            await AddProductAsync("Nike Zoom", 249900M, ["Calzado", "Deportes"], ["nike_zoom.png"], 200000M, 0.3F);
            await AddProductAsync("Buso Adidas Mujer", 134000M, ["Ropa", "Deportes"], ["buso_adidas.png"], 100000M, 0.3F);
            await AddProductAsync("Suplemento Boots Original", 15600M, ["Nutrición"], ["Boost_Original.png"], 150000M, 0.3F);
            await AddProductAsync("Whey Protein", 252000M, ["Nutrición"], ["whey_protein.png"], 200000M, 0.3F);
            await AddProductAsync("Arnes Mascota", 25000M, ["Mascotas"], ["arnes_mascota.png"], 20000M, 0.3F);
            await AddProductAsync("Cama Mascota", 99000M, ["Mascotas"], ["cama_mascota.png"], 78000M, 0.3F);
            await AddProductAsync("Teclado Gamer", 67000M, ["Gamer", "Tecnología"], ["teclado_gamer.png"], 53000M, 0.3F);
            await AddProductAsync("Ring de Lujo 17", 1600000M, ["Autos"], ["Ring1.png", "Ring2.png"], 1350000M, 0.3F);
            await AddProductAsync("Silla Gamer", 980000M, ["Gamer", "Tecnología"], ["silla_gamer.png"], 715000M, 0.3F);
            await AddProductAsync("Mouse Gamer", 132000M, ["Gamer", "Tecnología"], ["mouse_gamer.png"], 99900M, 0.3F);
            await _context.SaveChangesAsync();
        }
    }

    private async Task AddProductAsync(string name, decimal price, List<string> categories, List<string> images, decimal cost, float desiredProfit)
    {
        Product prodcut = new()
        {
            Description = name,
            Name = name,
            Price = price,
            Cost = cost,
            DesiredProfit = desiredProfit,
            ProductCategories = [],
            ProductImages = []
        };

        foreach (var categoryName in categories)
        {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
            if (category != null)
            {
                prodcut.ProductCategories.Add(new ProductCategory { Category = category });
            }
        }

        foreach (string? image in images)
        {
            string filePath;
            if (_runtimeInformationWrapper.IsOSPlatform(OSPlatform.Windows))
            {
                filePath = $"{Environment.CurrentDirectory}\\Images\\products\\{image}";
            }
            else
            {
                filePath = $"{Environment.CurrentDirectory}/Images/products/{image}";
            }

            var fileBytes = File.ReadAllBytes(filePath);
            var imagePath = await _fileStorage.SaveFileAsync(fileBytes, "jpg", "products");
            prodcut.ProductImages.Add(new ProductImage { Image = imagePath });
        }

        _context.Products.Add(prodcut);
    }

    private async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            _ = _context.Countries.Add(new Country
            {
                Name = "Colombia",
                States =
                [
                    new State()
                        {
                            Name = "Antioquia",
                            Cities = [
                                new() { Name = "Medellín" },
                                new() { Name = "Itagüí" },
                                new() { Name = "Envigado" },
                                new() { Name = "Bello" },
                                new() { Name = "Rionegro" },
                                new() { Name = "Marinilla" },
                            ]
                        },
                        new State()
                        {
                            Name = "Bogotá",
                            Cities = [
                                new() { Name = "Usaquen" },
                                new() { Name = "Champinero" },
                                new() { Name = "Santa fe" },
                                new() { Name = "Useme" },
                                new() { Name = "Bosa" },
                            ]
                        },
                    ]
            });
            _context.Countries.Add(new Country
            {
                Name = "Estados Unidos",
                States =
                [
                    new State()
                        {
                            Name = "Florida",
                            Cities = [
                                new() { Name = "Orlando" },
                                new() { Name = "Miami" },
                                new() { Name = "Tampa" },
                                new() { Name = "Fort Lauderdale" },
                                new() { Name = "Key West" },
                            ]
                        },
                        new State()
                        {
                            Name = "Texas",
                            Cities = [
                                new() { Name = "Houston" },
                                new() { Name = "San Antonio" },
                                new() { Name = "Dallas" },
                                new() { Name = "Austin" },
                                new() { Name = "El Paso" },
                            ]
                        },
                    ]
            });

            await _context.SaveChangesAsync();
        }
    }
}