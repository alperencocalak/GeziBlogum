using GeziBlogum.Entity;

namespace GeziBlogum.Data.Abstract{
    
    public interface ICommentRepository{
        IQueryable<Comment> Comments {get;}
        void CreateComment(Comment Comment);
    }


}