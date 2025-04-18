using GeziBlogum.Data.Abstract;
using GeziBlogum.Data.Concrete;
using GeziBlogum.Data.Concrete.EfCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();


builder.Services.AddDbContext<BlogContext>(options =>
{
    options.UseSqlite(builder.Configuration["ConnectionStrings:Sql_connection"]);
});


builder.Services.AddScoped<IPostRepository, EfPostRepository>();
builder.Services.AddScoped<ITagRepository, EfTagRepository>();
builder.Services.AddScoped<ICommentRepository, EfCommentRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();
builder.Services.AddScoped<ICommentVoteRepository, EfCommentVoteRepository>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.AccessDeniedPath = "/Users/AccessDenied";
        options.LoginPath = "/Users/Login";
        options.LogoutPath = "/Users/Logout";
    });

var app = builder.Build();



app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

SeedData.TestVerileriniDoldur(app);

app.MapControllerRoute(
    name: "post_details",
    pattern: "blogs/{url}",
    defaults: new { controller = "Posts", action = "Details" }
);


app.MapControllerRoute(
    name: "posts_by_tag",
    pattern: "posts/tag/{id:int}",
    defaults: new { controller = "Posts", action = "Tag" }
);


app.MapControllerRoute(
    name: "user_profile",
    pattern: "profile/{username}",
    defaults: new { controller = "Users", action = "Profile" }
);



app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Posts}/{action=Index}/{id?}"
);

app.Run();
