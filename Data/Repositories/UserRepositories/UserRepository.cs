using Common;
using Common.Exception;
using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Data.Repositories.UserRepositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DataContext dbContext) : base(dbContext)
        {
        }

        public Task<User?> GetByUserAndPass(string username, string password, CancellationToken cancellationToken)
        {
            var passwordHash = SecurityHelper.GetSha256Hash(password);
            return Table.Where(p => p.UserName == username && p.Password == passwordHash).SingleOrDefaultAsync(cancellationToken);
        }

        public Task UpdateLastLoginDateAsync(User user, CancellationToken cancellationToken)
        {
            user.LastLoginDate = DateTimeOffset.Now;
            return UpdateAsync(user, cancellationToken);
        }

        public async Task AddAsync(User user, string password, CancellationToken cancellationToken)
        {
            var exists = await TableNoTracking.AnyAsync(p => p.UserName == user.UserName);
            if (exists)
                throw new BadRequestException("UserName is exist!");

            var passwordHash = SecurityHelper.GetSha256Hash(password);
            user.Password = passwordHash;
            await base.AddAsync(user, cancellationToken);
        }
    }

}