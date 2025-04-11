using GeziBlogum.Entity;

namespace GeziBlogum.Data.Abstract{

    public interface IPostRepository{
        IQueryable<Post> Posts {get;}
        void CreatePost(Post post);
        void EditPost(Post post);
        void DeletePost(int id); // Silerken sadece post id yeterli bu yüzden int id şeklinde yaptım.

    }
}