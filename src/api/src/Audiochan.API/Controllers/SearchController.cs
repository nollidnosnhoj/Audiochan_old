﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Audios.Queries;
using Audiochan.Core.Dtos;
using Audiochan.Core.Dtos.Wrappers;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Audiochan.API.Controllers
{
    [Route("search")]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SearchController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("audios")]
        [ProducesResponseType(200)]
        [SwaggerOperation(Summary = "Search for audios", OperationId = "SearchAudio", Tags = new[] {"search"})]
        public async Task<ActionResult<PagedListDto<AudioDto>>> SearchAudios(
            [FromQuery] SearchAudiosQuery query,
            CancellationToken cancellationToken)
        {
            var results = await _mediator.Send(query, cancellationToken);
            return new JsonResult(results);
        }
    }
}