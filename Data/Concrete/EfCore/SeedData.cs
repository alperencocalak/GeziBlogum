using Microsoft.EntityFrameworkCore;
using GeziBlogum.Entity;

namespace GeziBlogum.Data.Concrete.EfCore
{
    public static class SeedData
    {
        public static void TestVerileriniDoldur(IApplicationBuilder app)
        {
            using var scope = app.ApplicationServices.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BlogContext>();

            Console.WriteLine("🟢 BlogContext başarıyla alındı.");

            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
                Console.WriteLine("📦 Migration uygulandı.");
            }

            if (!context.Tags.Any())
            {
                Console.WriteLine("🏷️ Tag verileri ekleniyor...");
                context.Tags.AddRange(
                    new Tag { Text = "Amerika", Url = "amerika", Color = TagColors.primary },
                    new Tag { Text = "İtalya", Url = "italya", Color = TagColors.danger },
                    new Tag { Text = "Hollanda", Url = "hollanda", Color = TagColors.secondary },
                    new Tag { Text = "İspanya", Url = "ispanya", Color = TagColors.success },
                    new Tag { Text = "Çekya", Url = "cekya", Color = TagColors.warning },
                    new Tag { Text = "Macaristan", Url = "macaristan", Color = TagColors.info }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("❗Tag verileri zaten var.");
            }

            if (!context.Users.Any())
            {
                Console.WriteLine("👤 Kullanıcı verileri ekleniyor...");
                context.Users.AddRange(
                    new User
                    {
                        UserName = "alperencocalak",
                        Name = "Alperen Çocalak",
                        Email = "info@alperencocalak.com",
                        Password = "123456",
                        Image = "/img/p1.jpg"
                    },
                    new User
                    {
                        UserName = "bilgecocalak",
                        Name = "Bilge Çocalak",
                        Email = "info@bilgecocalak.com",
                        Password = "123456",
                        Image = "/img/p2.jpg"
                    }
                );
                context.SaveChanges();
            }
            else
            {
                Console.WriteLine("❗Kullanıcı verileri zaten var.");
            }

            if (!context.Posts.Any())
            {
                Console.WriteLine("📝 Post verileri ekleniyor...");
                var tags = context.Tags.ToList();
                var user1 = context.Users.FirstOrDefault(u => u.UserName == "alperencocalak");
                var user2 = context.Users.FirstOrDefault(u => u.UserName == "bilgecocalak");

                if (user1 != null && user2 != null)
                {
                    context.Posts.AddRange(
                        new Post
                        {
                            Title = "Amerika Gezisi",
                            Content = "Amerika",
                            Description = "New York Gezisi",
                            Url = "amerika",
                            IsActive = true,
                            Image = "/img/newyork.jpg",
                            PublishedOn = DateTime.Now.AddDays(-10),
                            Tags = tags.Where(t => t.Url == "amerika").ToList(),
                            UserId = user1.UserId
                        },
                        new Post
                        {
                            Title = "İtalya Gezisi",
                            Content = "İtalya",
                            Description = "Roma Gezisi",
                            Url = "italya",
                            IsActive = true,
                            Image = "/img/roma.jpg",
                            PublishedOn = DateTime.Now.AddDays(-20),
                            Tags = tags.Where(t => t.Url == "italya").ToList(),
                            UserId = user1.UserId
                        },
                        new Post
                        {
                            Title = "Hollanda Gezisi",
                            Content = "Hollanda",
                            Description = "Amsterdam Gezisi",
                            Url = "hollanda",
                            IsActive = true,
                            Image = "/img/amsterdam.jpg",
                            PublishedOn = DateTime.Now.AddDays(-25),
                            Tags = tags.Where(t => t.Url == "hollanda").ToList(),
                            UserId = user2.UserId
                        }
                    );
                    context.SaveChanges();
                }
                else
                {
                    Console.WriteLine("❌ Kullanıcılar bulunamadı, postlar eklenemedi.");
                }
            }
            else
            {
                Console.WriteLine("❗Post verileri zaten var.");
            }

            Console.WriteLine("✅ Seed işlemi tamamlandı.");
        }
    }
}
