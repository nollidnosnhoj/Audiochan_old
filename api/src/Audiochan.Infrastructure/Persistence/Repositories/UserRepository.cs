﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Entities;
using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<User?> LoadUserWithFollowers(long targetId, long observerId, CancellationToken cancellationToken = default)
        {
            IQueryable<User> queryable = Queryable;
            
            if (observerId > 0)
            {
                queryable = queryable.Include(a =>
                    a.Followers.Where(fa => fa.ObserverId == observerId));
            }
            else
            {
                queryable = queryable.Include(a => a.Followers);
            }

            return await queryable.SingleOrDefaultAsync(a => a.Id == targetId, cancellationToken);
        }
    }
}