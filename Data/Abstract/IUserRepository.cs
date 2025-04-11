using GeziBlogum.Entity;

namespace GeziBlogum.Data.Abstract{

    public interface IUserRepository{
        IQueryable<User> Users {get;}
        void CreateUser(User User);
    }
}