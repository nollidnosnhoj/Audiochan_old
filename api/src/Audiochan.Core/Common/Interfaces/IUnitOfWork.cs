﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Repositories;

namespace Audiochan.Core.Common.Interfaces
{
    public interface IUnitOfWork
    {
        // DbSet<Audio> Audios { get; }
        // DbSet<FavoriteAudio> FavoriteAudios { get; }
        // DbSet<FollowedUser> FollowedUsers { get; }
        // DbSet<Tag> Tags { get; }
        // DbSet<User> Users { get; }
        IAudioRepository Audios { get; }
        ITagRepository Tags { get; }
        IUserRepository Users { get; }
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}