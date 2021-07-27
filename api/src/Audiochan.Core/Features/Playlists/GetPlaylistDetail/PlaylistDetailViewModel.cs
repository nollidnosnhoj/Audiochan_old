﻿using System;
using System.Collections.Generic;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities.Enums;
using Audiochan.Core.Features.Audios.GetAudio;
using Audiochan.Core.Features.Audios.GetLatestAudios;

namespace Audiochan.Core.Features.Playlists.GetPlaylistDetail
{
    public record PlaylistDetailViewModel
    {
        public Guid Id { get; init; }
        public string Title { get; init; } = string.Empty;
        public string Description { get; init; } = string.Empty;
        public string? Picture { get; init; }
        public Visibility Visibility { get; init; }
        public List<string> Tags { get; init; } = new();
        public List<AudioViewModel> Audios { get; init; } = new();
        public MetaAuthorDto User { get; init; } = null!;
    }
}