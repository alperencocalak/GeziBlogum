using GeziBlogum.Entity;
using Microsoft.EntityFrameworkCore;

namespace GeziBlogum.Data.Abstract{
    
    public interface ICommentRepository{
        IQueryable<Comment> Comments {get;}
        void CreateComment(Comment Comment);
        void UpdateComment(Comment Comment);
    }


}