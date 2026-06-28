using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MaydonozDoner.Data;
using MaydonozDoner.Controllers;
using MaydonozDoner.Models;


var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(builder.Configuration
    .GetConnectionString("DefaultConnection")));

builder.Services.AddDefaultIdentity<AppUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider
        .GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider
        .GetRequiredService<UserManager<AppUser>>();

    foreach (var role in new[] { "Admin", "User" })
    {
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
    }
    
    var adminUsername = "admin";
    var adminUser = await userManager.FindByNameAsync(adminUsername);
    if (adminUser == null) {
        adminUser = new AppUser
        {
            UserName = adminUsername,
            Email = "admin@maydonoz.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, "1234");
        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
    }

    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    var productCount = await dbContext.Products.CountAsync();
    if (productCount < 20)
    {
        var existing = await dbContext.Products.ToListAsync();
        if (existing.Any())
        {
            dbContext.Products.RemoveRange(existing);
            await dbContext.SaveChangesAsync();
        }

        var defaultProducts = new List<Product>
        {
            // === DÖNERLER ===
            new Product {
                Name = "🌿 Maydonoz Özel Soslu Tavuk Döner", Price = 120.00, Category = "Döner",
                Description = "Özel maydonozlu yeşil sosumuz, çıtır patates ve tavuk etiyle taze dürüm lavaşında.",
                ImageUrl = "https://images.unsplash.com/photo-1626082927389-6cd097cdc6ec?w=500"
            },
            new Product {
                Name = "🥩 Maydonoz Özel Soslu Et Döner", Price = 175.00, Category = "Döner",
                Description = "Yaprak et döner, domates, turşu ve özel baharatlı sosumuz eşliğinde lavaş dürüm.",
                ImageUrl = "https://images.unsplash.com/photo-1529193591184-b1d58069ecdd?w=500"
            },
            new Product {
                Name = "🧀 Kaşarlı Tavuk Döner Dürüm", Price = 135.00, Category = "Döner",
                Description = "Eriyen bol kaşar peyniri, özel maydonoz sosu ve çıtır patatesli tavuk dürüm.",
                ImageUrl = "https://images.unsplash.com/photo-1565299624946-b28f40a0ae38?w=500"
            },
            new Product {
                Name = "🧀 Kaşarlı Et Döner Dürüm", Price = 190.00, Category = "Döner",
                Description = "Eriyen nefis kaşar peyniri, yaprak et döner ve soslu patatesli et dürüm.",
                ImageUrl = "https://images.unsplash.com/photo-1555939594-58d7cb561ad1?w=500"
            },
            new Product {
                Name = "🍔 Tombik Ekmek Tavuk Döner", Price = 115.00, Category = "Döner",
                Description = "Ağızda dağılan taze tombik ekmeğe bol tavuk döner, yeşillik ve özel sos.",
                ImageUrl = "https://images.unsplash.com/photo-1594212699903-ec8a3eca50f5?w=500"
            },
            new Product {
                Name = "🍔 Tombik Ekmek Et Döner", Price = 165.00, Category = "Döner",
                Description = "Tombik ekmek arasında yaprak et döner, domates, turşu ve özel sos.",
                ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=500"
            },
            new Product {
                Name = "🥖 Yarım Ekmek Tavuk Döner", Price = 105.00, Category = "Döner",
                Description = "Çıtır yarım ekmek arasında soslu tavuk döner, patates ve marul.",
                ImageUrl = "https://images.unsplash.com/photo-1594212699903-ec8a3eca50f5?w=500"
            },
            new Product {
                Name = "🥖 Yarım Ekmek Et Döner", Price = 155.00, Category = "Döner",
                Description = "Çıtır taze yarım ekmek arasında özel et döner, turşu ve domates.",
                ImageUrl = "https://images.unsplash.com/photo-1504674900247-0877df9cc836?w=500"
            },
            new Product {
                Name = "📦 Maydonoz Tavuk Dürüm Combo Menü", Price = 195.00, Category = "Döner",
                Description = "Tavuk Döner Dürüm, baharatlı patates kızartması ve soğuk içecek ile birlikte indirimli menü.",
                ImageUrl = "https://images.unsplash.com/photo-1594212699903-ec8a3eca50f5?w=500"
            },
            new Product {
                Name = "📦 Maydonoz Et Dürüm Combo Menü", Price = 250.00, Category = "Döner",
                Description = "Et Döner Dürüm, baharatlı patates kızartması ve soğuk içecek ile birlikte indirimli menü.",
                ImageUrl = "https://images.unsplash.com/photo-1529193591184-b1d58069ecdd?w=500"
            },

            // === SOSLAR ===
            new Product {
                Name = "🌿 Maydonoz Özel Yeşil Sos", Price = 15.00, Category = "Sos",
                Description = "Taze maydonoz ve gizli baharat formülümüzle hazırlanan efsane yeşil sos.",
                ImageUrl = "https://images.unsplash.com/photo-1570544820287-0b51a7d6118d?w=500"
            },
            new Product {
                Name = "🧄 Sarımsaklı Mayonez", Price = 12.00, Category = "Sos",
                Description = "Taze sarımsak özleriyle zenginleştirilmiş yoğun kıvamlı nefis mayonez.",
                ImageUrl = "https://images.unsplash.com/photo-1570544820287-0b51a7d6118d?w=500"
            },
            new Product {
                Name = "🌶️ Acılı Meksika Sosu", Price = 15.00, Category = "Sos",
                Description = "Hafif acı severler için özenle hazırlanan jalapeño biberli dip sos.",
                ImageUrl = "https://images.unsplash.com/photo-1570544820287-0b51a7d6118d?w=500"
            },
            new Product {
                Name = "🍅 Klasik Ketçap", Price = 10.00, Category = "Sos",
                Description = "Güneşte olgunlaşmış domateslerden üretilen klasik lezzet sosu.",
                ImageUrl = "https://images.unsplash.com/photo-1570544820287-0b51a7d6118d?w=500"
            },
            new Product {
                Name = "🍗 Barbekü Sos", Price = 12.00, Category = "Sos",
                Description = "Et dönerin yanına yakışan tütsü aromalı karamelize barbekü sos.",
                ImageUrl = "https://images.unsplash.com/photo-1570544820287-0b51a7d6118d?w=500"
            },
            new Product {
                Name = "🧴 Ranch Sos", Price = 12.00, Category = "Sos",
                Description = "Yoğurt, sarımsak ve çeşitli taze otlarla zenginleştirilmiş krema kıvamlı sos.",
                ImageUrl = "https://images.unsplash.com/photo-1570544820287-0b51a7d6118d?w=500"
            },

            // === İÇECEKLER ===
            new Product {
                Name = "🥤 Buz Gibi Ayran (Büyük)", Price = 30.00, Category = "İçecek",
                Description = "Dönerin yanına en yakışan, bol köpüklü soğuk yayık ayran.",
                ImageUrl = "/images/ayran.jpg"
            },
            new Product {
                Name = "🥤 Coca-Cola (Kutu)", Price = 35.00, Category = "İçecek",
                Description = "Buz gibi serinletici klasik kola lezzeti.",
                ImageUrl = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=500"
            },
            new Product {
                Name = "🥤 Fanta (Kutu)", Price = 35.00, Category = "İçecek",
                Description = "Portakal aromalı gazlı içecek.",
                ImageUrl = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=500"
            },
            new Product {
                Name = "🥤 Sprite (Kutu)", Price = 35.00, Category = "İçecek",
                Description = "Limon aromalı ferahlatıcı gazlı içecek.",
                ImageUrl = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=500"
            },
            new Product {
                Name = "🥤 Fuse Tea Şeftali (Kutu)", Price = 35.00, Category = "İçecek",
                Description = "Şeftali aromalı soğuk çay keyfi.",
                ImageUrl = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=500"
            },
            new Product {
                Name = "🍷 Şalgam Suyu (Büyük)", Price = 30.00, Category = "İçecek",
                Description = "Adana usulü acılı veya acısız taze şalgam suyu.",
                ImageUrl = "https://images.unsplash.com/photo-1622483767028-3f66f32aef97?w=500"
            }
        };

        dbContext.Products.AddRange(defaultProducts);
        await dbContext.SaveChangesAsync();
    }
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "defeault",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
