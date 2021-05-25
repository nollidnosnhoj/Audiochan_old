﻿using Ardalis.Specification;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public sealed class GetFollowingIdsSpecification : Specification<FollowedUser, string>
    {
        public GetFollowingIdsSpecification(string userId)
        {
            Query.Select(user => user.TargetId)
                .AsNoTracking()
                .Where(user => user.ObserverId == userId);
        }
    }
}