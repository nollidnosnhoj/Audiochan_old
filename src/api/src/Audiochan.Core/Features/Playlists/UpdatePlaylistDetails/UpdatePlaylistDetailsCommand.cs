﻿using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Features.Playlists.GetPlaylist;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using Audiochan.Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.UpdatePlaylistDetails
{
    public record UpdatePlaylistDetailsCommand : IRequest<Result<PlaylistDto>>
    {
        public long Id { get; init; }
        public string? Title { get; init; }
        public string? Description { get; init; }
        public List<string>? Tags { get; init; }

        public UpdatePlaylistDetailsCommand(long id, UpdatePlaylistDetailsRequest request)
        {
            Id = id;
            Title = request.Title;
            Description = request.Description;
            Tags = request.Tags;
        }
    }
    
    public class UpdatePlaylistDetailsCommandValidator : AbstractValidator<UpdatePlaylistDetailsCommand>
    {
        public UpdatePlaylistDetailsCommandValidator()
        {
            When(req => req.Title is not null, () =>
            {
                RuleFor(req => req.Title)
                    .NotEmpty()
                    .WithMessage("Title is required.")
                    .MaximumLength(30)
                    .WithMessage("Title cannot be no more than 30 characters long.");
            });

            When(req => req.Description is not null, () =>
            {
                RuleFor(req => req.Description)
                    .MaximumLength(500)
                    .WithMessage("Description cannot be more than 500 characters long.");
            });

            When(req => req.Tags is not null, () =>
            {
                RuleFor(req => req.Tags)
                    .Must(u => u!.Count <= 10)
                    .WithMessage("Can only have up to 10 tags per audio upload.")
                    .ForEach(tagsRule =>
                    {
                        tagsRule
                            .NotEmpty()
                            .WithMessage("Each tag cannot be empty.")
                            .Length(3, 15)
                            .WithMessage("Each tag must be between 3 and 15 characters long.");
                    });
            });
        }
    }
    
    public class UpdatePlaylistDetailsCommandHandler 
        : IRequestHandler<UpdatePlaylistDetailsCommand,Result<PlaylistDto>>
    {
        private readonly long _currentUserId;
        private readonly ApplicationDbContext _unitOfWork;

        public UpdatePlaylistDetailsCommandHandler(ICurrentUserService currentUserService, ApplicationDbContext unitOfWork)
        {
            _currentUserId = currentUserService.GetUserId();
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<PlaylistDto>> Handle(UpdatePlaylistDetailsCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .Include(p => p.User)
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (playlist is null)
                return Result<PlaylistDto>.NotFound<Playlist>();

            if (playlist.UserId != _currentUserId)
                return Result<PlaylistDto>.Forbidden();

            if (!string.IsNullOrWhiteSpace(request.Title))
                playlist.Title = request.Title;

            if (request.Description is not null)
                playlist.Description = request.Description;

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            
            return Result<PlaylistDto>.Success(PlaylistMaps.PlaylistToDetailFunc.Compile().Invoke(playlist));
        }
    }
}