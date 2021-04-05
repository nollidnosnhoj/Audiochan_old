﻿using System;
using Audiochan.Core.Common.Enums;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Common.Models.Responses;

namespace Audiochan.Core.Features.Audios.GetAudioList
{
    public record AudioViewModel : IBaseViewModel
    {
        public long Id { get; init; }
        public string Title { get; init; }
        public Visibility Visibility { get; init; }
        public int Duration { get; init; }
        public string Picture { get; init; }
        public DateTime Uploaded { get; init; }
        public string AudioUrl { get; init; }
        public MetaAuthorDto Author { get; init; }
    }
}