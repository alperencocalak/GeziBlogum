using GeziBlogum.Entity;

namespace GeziBlogum.Data.Abstract{

    public interface ITagRepository{
        IQueryable<Tag> Tags {get;}
        void CreateTag(Tag Tag);
    }
}