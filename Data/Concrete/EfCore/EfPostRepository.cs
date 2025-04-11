using GeziBlogum.Data.Abstract;
using GeziBlogum.Data.Concrete.EfCore;
using GeziBlogum.Entity;

namespace GeziBlogum.Data.Concrete
{

    public class EfPostRepository : IPostRepository
    {
        private BlogContext _context;
        public EfPostRepository(BlogContext context)
        {
            _context = context;
        }
        public IQueryable<Post> Posts => _context.Posts;

        public void CreatePost(Post post)
        {
            _context.Posts.Add(post);
            _context.SaveChanges();
        }
        public void EditPost(Post post)
        {
            var entity = _context.Posts.FirstOrDefault(i => i.PostId == post.PostId);

            if (entity != null)
            {
                entity.Title = post.Title;
                entity.Description = post.Description;
                entity.Content = post.Content;
                entity.Url = post.Url;
                entity.IsActive = post.IsActive;

                Console.WriteLine($"[EF] Güncelleniyor: PostId={entity.PostId}, IsActive={entity.IsActive}");

                _context.SaveChanges();
            }
        }
        public void DeletePost(int id)
        {
            var entity = _context.Posts.FirstOrDefault(p => p.PostId == id);
            if (entity != null)
            {
                if (!string.IsNullOrEmpty(entity.Image))
                {
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", entity.Image.TrimStart('/'));
                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }
                }

                _context.Posts.Remove(entity);
                _context.SaveChanges();
            }
        }

    }
}