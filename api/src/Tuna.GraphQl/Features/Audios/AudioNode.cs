﻿using System.Threading;
using System.Threading.Tasks;
using HotChocolate.Types;
using HotChocolate.Types.Relay;
using Tuna.Application.Features.Audios.DataLoaders;
using Tuna.Application.Features.Audios.Models;

namespace Tuna.GraphQl.Features.Audios;

[Node]
[ExtendObjectType(typeof(AudioDto))]
public sealed class AudioNode
{
    [NodeResolver]
    public static Task<AudioDto> GetAudioAsync(long id, GetAudioDataLoader dataLoader,
        CancellationToken cancellationToken)
    {
        return dataLoader.LoadAsync(id, cancellationToken);
    }
}