using ArtVenue.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ArtVenue.Hubs;
using ArtVenue.Models;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer("Server=tcp:artvenue.database.windows.net,1433;Initial Catalog=ArtVenue_db;Persist Security Info=False;User ID=CloudSA42b1aa45;Password=Dragomanoit15:D;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=False;Connection Timeout=60;"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = false)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var _db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    string[] roles = new string[]{"admin", "moderator"};
    foreach (var role in roles)
    {
        bool exists = await roleManager.RoleExistsAsync(role);
        if (!exists)
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    Category[] categories = new Category[] {
        new Category
        {
            CategoryName= "Painting",
            CategoryImage= "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1675854424/painter_kaghle.png",
            CategoryDescription= "The expression of ideas and emotions, with the creation of certain aesthetic qualities, in a two-dimensional visual language."
        },
        new Category
        {
            CategoryName= "Music",
            CategoryImage= "https://res.cloudinary.com/ddo3vrwcb/image/upload/v1675855498/music_ldx44e.png",
            CategoryDescription= "Music is different from all other artforms because it alone is an expression of itself rather than something else. Notes and melodies, unlike phrases and colors, do not try to represent anything but can instead be appreciated simply for what they are."
        },
        new Category
        {
            CategoryName= "Dancing",
            CategoryImage= "https://images.unsplash.com/photo-1519925610903-381054cc2a1c?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxzZWFyY2h8OHx8ZGFuY2VyfGVufDB8fDB8fA%3D%3D&w=1000&q=80",
            CategoryDescription= "Dance is a natural form of self-expression: the body expresses itself naturally and so therefore does the spirit. We are all free willed beings, no matter what our personal situation may be. Through dance, our body expresses how free we actually are, while also highlighting the restrictions our minds impose on it."
        },
        new Category
        {
            CategoryName= "Poetry",
            CategoryImage= "https://images.unsplash.com/photo-1473186505569-9c61870c11f9?ixlib=rb-4.0.3&ixid=MnwxMjA3fDB8MHxzZWFyY2h8Mnx8cG9ldHJ5fGVufDB8fDB8fA%3D%3D&w=1000&q=80",
            CategoryDescription= "Literature that evokes a concentrated imaginative awareness of experience or a specific emotional response through language chosen and arranged for its meaning, sound, and rhythm."
        },
        new Category
        {
            CategoryName= "Photography",
            CategoryImage= "https://images.hindustantimes.com/img/2021/08/19/1600x900/patrick-dozkVhDyvhQ-unsplash_1628163163817_1629346849962.jpg",
            CategoryDescription= "Photography is the art, application, and practice of creating durable images by recording light, either electronically by means of an image sensor, or chemically by means of a light-sensitive material such as photographic film."
        },
        new Category
        {
            CategoryName= "Cooking",
            CategoryImage= "https://assets.bonappetit.com/photos/5e7a6c79edf206000862e452/16:9/w_2580,c_limit/Cooking-Home-Collection.jpg",
            CategoryDescription= "Cooking is the art, science and craft of using heat to prepare food for consumption. Cooking techniques and ingredients vary widely, from grilling food over an open fire to using electric stoves, to baking in various types of ovens, reflecting local conditions."
        },
        new Category
        {
            CategoryName= "Woodcarving",
            CategoryImage= "https://ronixtools.com/en/blog/wp-content/uploads/2021/02/woodcarving-1-1024x690.jpg",
            CategoryDescription= "Wood Carving is a form of woodworking by means of a cutting tool (knife) in one hand or a chisel by two hands or with one hand on a chisel and one hand on a mallet, resulting in a wooden figure or figurine, or in the sculptural ornamentation of a wooden object."
        },
    };
    foreach (var category in categories)
    {
        bool exists = await _db.Categories.Where(x=>x.CategoryName == category.CategoryName).AnyAsync();
        if (!exists)
        {
            await _db.Categories.AddAsync(category);
        }
    }
    await _db.SaveChangesAsync();
}

app.UseExceptionHandler("/error/servererror");
app.UseStatusCodePagesWithRedirects("/error/statuscode/{0}");

app.UseHsts();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();
app.MapHub<ChatHub>("/chatHub");
app.Run();
