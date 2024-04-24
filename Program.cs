using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using Utilities.Database;
using Utilities.Database.Models;
using Utilities.Services;
using Utilities.Services.Services.Implementations;
using Utilities.Services.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<UtilitiesContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<User>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<UtilitiesContext>();

builder.Services.AddControllersWithViews();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

var res = IsConnectionStringValid(connectionString);

Console.ForegroundColor = res ? ConsoleColor.Green : ConsoleColor.Red;
Console.WriteLine("IsConnectionStringValid " + res);
Console.ForegroundColor = ConsoleColor.Black;

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
await AddAdminRoleUser();

app.Run();

bool IsConnectionStringValid(string connectionString)
{
    try
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            if (connection.State == System.Data.ConnectionState.Open)
            {
                Console.WriteLine("valid.");
                return true;
            }
            else
            {
                Console.WriteLine("connection isn't valid.");
                return false;
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Exception -> {ex.Message}");
        return false;
    }
}


async Task AddAdminRoleUser()
{
    using (var scope = app.Services.CreateScope())
    {
        var needServices = scope.ServiceProvider;
        var dataManager = needServices.GetRequiredService<UtilitiesContext>();
        try
        {
            #region RolesAndUsers
            if (!dataManager.Roles.Any(r => r.Name == "Default"))
            {
                var role = new IdentityRole()
                {
                    Name = "Default",
                    NormalizedName = "Default".ToUpper()
                };

                dataManager.Add(role);
                dataManager.SaveChanges();

                var checkRole = await dataManager.Roles.FirstOrDefaultAsync(r => r.Name == "Default");
                if (checkRole == null)
                {
                    throw new EndOfStreamException();
                }
            }

            if (!dataManager.Roles.Any(r => r.Name == "Admin"))
            {
                var role = new IdentityRole()
                {
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper()
                };

                dataManager.Add(role);
                dataManager.SaveChanges();

                var checkRole = await dataManager.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                if (checkRole == null)
                {
                    throw new EndOfStreamException();
                }
            }

            if (!dataManager.Users.Any(x => x.Id == "666"))
            {
                var admin = new User()
                {
                    UserName = "admin@admin.min",
                    NormalizedUserName = "admin@admin.min".ToUpper(),
                    FirstName = "Adminich",
                    LastName = "Admin",
                    Login = "Admin",
                    LockoutEnabled = true,
                    Email = "admin@admin.min",
                    PasswordHash =
                        "AQAAAAEAACcQAAAAEDPluyigsSGN0WazXT76DZYRvKST0XXrhaIotHqnerUNTwHoj5pFR5QxoVma7L5g+w==", // m8xY4yTM-2$jt?w
                    NormalizedEmail = "admin@admin.min".ToUpper(),
                    Id = 666.ToString(),
                    EmailConfirmed = true,
                    SecurityStamp = "GBGEJB4JVH44AKT23O6NW67HHEXY3O2J",
                    ConcurrencyStamp = "fe4e29c5-a3ea-412c-aada-db278715d6fa",
                };
                dataManager.Add(admin);

                var checkRole = await dataManager.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");
                dataManager.SaveChanges();

                var ad = dataManager.Users.FirstOrDefault(x => x.Id == admin.Id);

                if (ad == null)
                {
                    throw new EndOfStreamException();
                }

                dataManager.UserRoles.Add(new IdentityUserRole<string>() { RoleId = checkRole.Id, UserId = admin.Id });
                dataManager.SaveChanges();
            }

            if (!dataManager.Users.Any(x => x.Id == "777"))
            {
                var admin = new User()
                {
                    UserName = "default@default.def",
                    NormalizedUserName = "default@default.def".ToUpper(),
                    LockoutEnabled = true,
                    Email = "default@default.def",
                    FirstName = "Default",
                    LastName = "Default",
                    Login = "Default",
                    PasswordHash =
                        "AQAAAAEAACcQAAAAEDPluyigsSGN0WazXT76DZYRvKST0XXrhaIotHqnerUNTwHoj5pFR5QxoVma7L5g+w==", // m8xY4yTM-2$jt?w
                    NormalizedEmail = "default@default.def".ToUpper(),
                    Id = 777.ToString(),
                    EmailConfirmed = true,
                    SecurityStamp = "GBGEJB4JVH44AKT23O6NW67HHEXY3O2J",
                    ConcurrencyStamp = "fe4e29c5-a3ea-412c-aada-db278715d6fa",
                };
                dataManager.Add(admin);

                var checkRole = await dataManager.Roles.FirstOrDefaultAsync(r => r.Name == "Default");
                dataManager.SaveChanges();

                var ad = dataManager.Users.FirstOrDefault(x => x.Id == admin.Id);

                if (ad == null)
                {
                    throw new EndOfStreamException();
                }

                dataManager.UserRoles.Add(new IdentityUserRole<string>() { RoleId = checkRole.Id, UserId = admin.Id });
                dataManager.SaveChanges();
            }
            #endregion

            #region Utilities
            if (!dataManager.Utilities.Any(ut => ut.UtilityName.ToUpper() == "Электричество".ToUpper()))
            {
                var newUt = new Utility()
                {
                    UtilityName = "Электричество",
                    Description = "Происзвести расчёт стоимости электроэнергии исходя из показателей личных счётчиков."
                };

                dataManager.Add(newUt);
                dataManager.SaveChanges();
            }

            if (!dataManager.Utilities.Any(ut => ut.UtilityName.ToUpper() == "Водоснабжение".ToUpper()))
            {
                var newUt = new Utility()
                {
                    UtilityName = "Водоснабжение",
                    Description = "Холодное водоснабжение по фиксированным тарифам, обеспечивающим полное возмещение экономических обоснованных" +
                    "затрат и субсидируемые государством."
                };

                dataManager.Add(newUt);
                dataManager.SaveChanges();
            }

            if (!dataManager.Utilities.Any(ut => ut.UtilityName.ToUpper() == "Отопление".ToUpper()))
            {
                var newUt = new Utility()
                {
                    UtilityName = "Отопление",
                    Description = "Расчёт стоимости отопления по тарифам для нужд отопления и горячего водоснабжения жилых домов " +
                    "и квартир потребителей."
                };

                dataManager.Add(newUt);
                dataManager.SaveChanges();
            }
            #endregion

            #region Regions
            var regions = dataManager.Regions;
            if (!regions.Any(r => r.RegionName.ToUpper() == "Минская область".ToUpper()))
            {
                var reg = new Region()
                {
                    RegionName = "Минская область",
                };
                regions.Add(reg);
                dataManager.SaveChanges();
            }

            if (!regions.Any(r => r.RegionName.ToUpper() == "Гродненская область".ToUpper()))
            {
                var reg = new Region()
                {
                    RegionName = "Гродненская область",
                };
                regions.Add(reg);
                dataManager.SaveChanges();
            }
            #endregion

            if (!dataManager.Providers.Any(pr => pr.ProviderName.ToUpper() == "ЭлектроЭнергоМинск".ToUpper()))
            {
                var pr = new Provider()
                {
                    ProviderName = "ЭлектроЭнергоБрестАнус",
                    RegionId = (await dataManager.Regions.FirstOrDefaultAsync(r => r.RegionName == "Минская область"))!.RegionId,
                    ProviderDescription = "РЕСПУБЛИКАНСКОЕ УНИТАРНОЕ ПРЕДПРИЯТИЕ ЭЛЕКТРОЭНЕРГЕТИКИ"
                };

                dataManager.Providers.Add(pr);
                dataManager.SaveChanges();

                pr.Tariffs.Add(new Tariff
                {
                    TariffPrice = 0.1025M,
                    TariffName = "Одноставочный тариф",
                    UtilityId = (await dataManager.Utilities.Where(ut => ut.UtilityName == "Электричество").Select(ut => ut.UtilityId).FirstOrDefaultAsync()),
                    TariffDescription = "Электрическая энергия для использования для нужд отопления"
                });

                dataManager.SaveChanges();
            }
        }
        catch (Exception ex)
        {
            var logger = needServices.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred while seeding the database.");
        }
    }

}