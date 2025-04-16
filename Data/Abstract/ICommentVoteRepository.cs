using GeziBlogum.Entity;

namespace GeziBlogum.Data.Abstract
{
    public interface ICommentVoteRepository
    {
        bool HasUserVoted(int commentId, int userId);
        void AddVote(CommentVote vote);
    }
}