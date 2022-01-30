﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.API.Models;
using Audiochan.Core.Commons.Dtos.Wrappers;
using Audiochan.Core.Commons.Extensions;
using Audiochan.Core.Commons.Services;
using Audiochan.Core.Features.Audios.Models;
using Audiochan.Core.Features.Audios.Queries.GetAudioFeed;
using Audiochan.Core.Features.Users.Queries.GetUserAudios;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers.Me
{
    [Area("me")]
    [Authorize]
    [Route("[area]/audios")]
    [ProducesResponseType(401)]
    public class MyAudiosController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly long _currentUserId;
        private readonly string _currentUsername;

        public MyAudiosController(ICurrentUserService currentUserService, IMediator mediator)
        {
            _mediator = mediator;
            currentUserService.User.TryGetUserId(out _currentUserId);
            currentUserService.User.TryGetUserName(out _currentUsername);
        }
        
        [HttpGet(Name = "YourAudios")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Get authenticated user's uploaded audios.",
            Description = "Requires authentication",
            OperationId = "YourAudios",
            Tags = new [] {"me"}
        )]
        public async Task<ActionResult<OffsetPagedListDto<AudioDto>>> GetYourAudios(
            [FromQuery] OffsetPaginationQueryParams queryParams, 
            CancellationToken cancellationToken = default)
        {
            var result = await _mediator.Send(new GetUsersAudioQuery(_currentUsername)
            {
                Offset = queryParams.Offset,
                Size = queryParams.Size,
            }, cancellationToken);
            return Ok(result);
        }
        
        [HttpGet("feed", Name = "YourAudioFeed")]
        [Produces("application/json")]
        [ProducesResponseType(200)]
        [SwaggerOperation(
            Summary = "Returns a list of tracks uploaded by authenticated user's followings.",
            Description = "Requires authentication.",
            OperationId = "YourAudioFeed",
            Tags = new[] {"me"}
        )]
        public async Task<ActionResult<PagedListDto<AudioDto>>> GetYourFeed(
            [FromQuery] OffsetPaginationQueryParams queryParams, 
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(new GetAudioFeedQuery(_currentUserId)
            {
                Offset = queryParams.Offset,
                Size = queryParams.Size
            }, cancellationToken);
            return Ok(result);
        }
    }
}