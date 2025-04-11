using GeziBlogum.Data.Abstract;
using GeziBlogum.Data.Concrete.EfCore;
using GeziBlogum.Entity;

namespace GeziBlogum.Data.Concrete{

    public class EfCommentRepository : ICommentRepository
    {
        private BlogContext _context;
        public EfCommentRepository(BlogContext context){
            _context = context;
        }
        public IQueryable<Comment> Comments => _context.Comments;

        public void CreateComment(Comment Comment)
        {
            _context.Comments.Add(Comment);
            _context.SaveChanges();
        }
    }
}