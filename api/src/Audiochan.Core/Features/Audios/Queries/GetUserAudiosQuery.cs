﻿using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Shared.Mediatr;
using Audiochan.Shared.Models;
using Audiochan.Core.Features.Audios.Mappings;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Persistence;
using HotChocolate.Types.Pagination;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Audios.Queries;

public record GetUserAudiosQuery(long UserId, int? Skip, int? Take) 
    : OffsetPagedQuery(Skip, Take), IQueryRequest<CollectionSegment<AudioDto>>;

public class GetUserAudiosQueryHandler : IRequestHandler<GetUserAudiosQuery, CollectionSegment<AudioDto>>
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;

    public GetUserAudiosQueryHandler(IDbContextFactory<ApplicationDbContext> dbContextFactory)
    {
        _dbContextFactory = dbContextFactory;
    }
    
    public async Task<CollectionSegment<AudioDto>> Handle(GetUserAudiosQuery request, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Audios
            .Where(x => x.UserId == request.UserId)
            .OrderByDescending(x => x.CreatedAt)
            .ProjectToDto()
            .ApplyOffsetPaginationAsync(request.Skip, request.Take, cancellationToken);
    }
}