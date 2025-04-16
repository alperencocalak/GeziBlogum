using GeziBlogum.Entity;
using Microsoft.EntityFrameworkCore;

namespace GeziBlogum.Data.Concrete.EfCore{

    public class BlogContext:DbContext{

        public BlogContext(DbContextOptions<BlogContext> options):base(options){}
        public DbSet<Post> Posts => Set<Post>();
        public DbSet<Tag> Tags => Set<Tag>();
        public DbSet<Comment> Comments => Set<Comment>();
        public DbSet<User> Users => Set<User>();
        public DbSet<CommentVote> CommentVotes { get; set; }

    }
}