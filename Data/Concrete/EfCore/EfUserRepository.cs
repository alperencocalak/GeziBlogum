using GeziBlogum.Data.Abstract;
using GeziBlogum.Data.Concrete.EfCore;
using GeziBlogum.Entity;

namespace GeziBlogum.Data.Concrete{

    public class EfUserRepository : IUserRepository
    {
        private BlogContext _context;
        public EfUserRepository(BlogContext context){
            _context = context;
        }
        public IQueryable<User> Users => _context.Users;

        public void CreateUser(User User)
        {
            _context.Users.Add(User);
            _context.SaveChanges();
        }
        public void UpdateUser(User user)
        {
            _context.Users.Update(user);
            _context.SaveChanges();
         }

    }
}