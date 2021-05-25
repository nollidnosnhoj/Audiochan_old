﻿using System.Collections.Generic;
using Ardalis.Specification;
using Audiochan.Core.Common.Extensions.MappingExtensions;
using Audiochan.Core.Entities;

namespace Audiochan.Core.Features.Audios.GetAudioFeed
{
    public sealed class GetAudioFeedSpecification : Specification<Audio, AudioViewModel>
    {
        public GetAudioFeedSpecification(List<string> followedIds, int size = 15)
        {
            Query.Select(AudioMappingExtensions.AudioToListProjection())
                .AsNoTracking()
                .Include(a => a.User)
                .Where(a => a.IsPublic)
                .Where(a => followedIds.Contains(a.UserId))
                .OrderByDescending(a => a.Created)
                .Take(size);
        }
    }
}