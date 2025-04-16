namespace GeziBlogum.Entity
{
    public class CommentVote
    {
        public int CommentVoteId { get; set; }
        public int CommentId { get; set; }
        public int UserId { get; set; }
        public bool IsLike { get; set; }

        public Comment Comment { get; set; } = null!;
        public User User { get; set; } = null!;
    }
}
