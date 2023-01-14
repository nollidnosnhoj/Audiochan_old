﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Persistence;
using Audiochan.Core.Persistence.Repositories;
using Audiochan.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Infrastructure.Persistence.Repositories
{
    public class AudioRepository : EfRepository<Audio>, IAudioRepository
    {
        public AudioRepository(ApplicationDbContext dbContext) : base(dbContext)
        {
        }

        public async Task<Audio?> LoadAudioWithFavorites(long audioId, long observerId = 0, 
            CancellationToken cancellationToken = default)
        {
            IQueryable<Audio> queryable = Queryable;

            if (observerId > 0)
            {
                queryable = queryable.Include(a => 
                    a.FavoriteAudios.Where(fa => fa.UserId == observerId));
            }
            else
            {
                queryable = queryable.Include(a => a.FavoriteAudios);
            }

            return await queryable.SingleOrDefaultAsync(a => a.Id == audioId, cancellationToken);
        }
    }
}