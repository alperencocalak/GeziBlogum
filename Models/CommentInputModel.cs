namespace GeziBlogum.Models
{
    public class CommentInputModel
    {
        public int PostId { get; set; }
        public string Text { get; set; } = string.Empty;
    }
}