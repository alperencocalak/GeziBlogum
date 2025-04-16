namespace GeziBlogum.Entity
{
    public class Comment{
    public int CommentId {get;set;}
    public string? Text {get;set;}
    public DateTime PublishedOn {get;set;}
    public int PostId {get;set;}
    public Post Post {get;set;} = null!;
    public int UserId {get;set;}
    public User User {get;set;} = null!;
    public int LikeCount { get; set; } = 0;
    public int DislikeCount { get; set; } = 0;

}
}