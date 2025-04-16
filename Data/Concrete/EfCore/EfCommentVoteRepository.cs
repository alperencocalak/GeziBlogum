using GeziBlogum.Data.Abstract;
using GeziBlogum.Entity;
using GeziBlogum.Data.Concrete.EfCore;
using System.Linq;

namespace GeziBlogum.Data.Concrete.EfCore
{
    public class EfCommentVoteRepository : ICommentVoteRepository
    {
        private readonly BlogContext _context;

        public EfCommentVoteRepository(BlogContext context)
        {
            _context = context;
        }

        public bool HasUserVoted(int commentId, int userId)
        {
            return _context.CommentVotes.Any(v => v.CommentId == commentId && v.UserId == userId);
        }

        public void AddVote(CommentVote vote)
        {
            _context.CommentVotes.Add(vote);
            _context.SaveChanges();
        }
    }
}