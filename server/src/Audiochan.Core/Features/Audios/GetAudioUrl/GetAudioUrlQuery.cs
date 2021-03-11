﻿using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models.Responses;
using Audiochan.Core.Common.Options;
using Audiochan.Core.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Audiochan.Core.Features.Audios.GetAudioUrl
{
    public record GetAudioUrlQuery : IRequest<IResult<AudioUrlViewModel>>
    {
        public long Id { get; init; }
    }

    public class GetAudioUrlQueryHandler : IRequestHandler<GetAudioUrlQuery, IResult<AudioUrlViewModel>>
    {
        private readonly IApplicationDbContext _dbContext;
        private readonly string _audioStorageUrl;

        public GetAudioUrlQueryHandler(IApplicationDbContext dbContext, IOptions<AudiochanOptions> options)
        {
            _dbContext = dbContext;
            _audioStorageUrl = $"{options.Value.StorageUrl}/{options.Value.AudioStorageOptions.Container}/";
        }


        public async Task<IResult<AudioUrlViewModel>> Handle(GetAudioUrlQuery request, CancellationToken cancellationToken)
        {
            var info = await _dbContext.Audios
                .AsNoTracking()
                .Where(a => a.Id == request.Id)
                .Select(a => new {a.UploadId, a.FileExt})
                .SingleOrDefaultAsync(cancellationToken);

            return info is not null 
                ? Result<AudioUrlViewModel>.Success(new AudioUrlViewModel(_audioStorageUrl + info.UploadId + info.FileExt))
                : Result<AudioUrlViewModel>.Fail(ResultError.NotFound);
        }
    }
}