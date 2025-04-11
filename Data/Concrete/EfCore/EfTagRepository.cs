using GeziBlogum.Data.Abstract;
using GeziBlogum.Data.Concrete.EfCore;
using GeziBlogum.Entity;

namespace GeziBlogum.Data.Concrete{

    public class EfTagRepository : ITagRepository
    {
        private BlogContext _context;
        public EfTagRepository(BlogContext context){
            _context = context;
        }
        public IQueryable<Tag> Tags => _context.Tags;

        public void CreateTag(Tag Tag)
        {
            _context.Tags.Add(Tag);
            _context.SaveChanges();
        }
    }
}