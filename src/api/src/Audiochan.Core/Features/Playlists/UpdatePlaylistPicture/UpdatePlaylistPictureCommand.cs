﻿using System.Threading;
using System.Threading.Tasks;
using Audiochan.Core.Common.Constants;
using Audiochan.Core.Common.Interfaces;
using Audiochan.Core.Common.Models;
using Audiochan.Core.Entities;
using Audiochan.Core.Interfaces;
using Audiochan.Core.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Audiochan.Core.Features.Playlists.UpdatePlaylistPicture
{
    public record UpdatePlaylistPictureCommand : IImageData, IRequest<Result<ImageUploadResponse>>
    {
        public long Id { get; init; }
        public string Data { get; init; } = null!;

        public UpdatePlaylistPictureCommand(long id, ImageUploadRequest request)
        {
            Id = id;
            Data = request.Data;
        }
    }
    
    
    public class UpdatePlaylistPictureCommandHandler : IRequestHandler<UpdatePlaylistPictureCommand, Result<ImageUploadResponse>>
    {
        private readonly long _currentUserId;
        private readonly INanoidGenerator _nanoidGenerator;
        private readonly IImageUploadService _imageUploadService;
        private readonly ApplicationDbContext _unitOfWork;

        public UpdatePlaylistPictureCommandHandler(IImageUploadService imageUploadService,
            ApplicationDbContext unitOfWork, 
            ICurrentUserService currentUserService, 
            INanoidGenerator nanoidGenerator)
        {
            _imageUploadService = imageUploadService;
            _unitOfWork = unitOfWork;
            _nanoidGenerator = nanoidGenerator;
            _currentUserId = currentUserService.GetUserId();
        }
        
        public async Task<Result<ImageUploadResponse>> Handle(UpdatePlaylistPictureCommand request, CancellationToken cancellationToken)
        {
            var playlist = await _unitOfWork.Playlists
                .Include(p => p.User)
                .SingleOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

            if (playlist == null)
                return Result<ImageUploadResponse>.NotFound<Playlist>();

            if (playlist.UserId != _currentUserId)
                return Result<ImageUploadResponse>.Forbidden();
            
            var blobName = string.Empty;
            if (string.IsNullOrEmpty(request.Data))
            {
                await RemoveOriginalPicture(playlist.Picture, cancellationToken);
                playlist.Picture = null;
            }
            else
            {
                blobName = $"{await _nanoidGenerator.GenerateAsync(size: 15)}.jpg";
                await _imageUploadService.UploadImage(request.Data, AssetContainerConstants.PlaylistPictures, blobName, cancellationToken);
                await RemoveOriginalPicture(playlist.Picture, cancellationToken);
                playlist.Picture = blobName;
            }
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Result<ImageUploadResponse>.Success(new ImageUploadResponse
            {
                Url = string.Format(MediaLinkInvariants.PlaylistPictureUrl, blobName)
            });
        }
        
        private async Task RemoveOriginalPicture(string? picture, CancellationToken cancellationToken = default)
        {
            if (!string.IsNullOrEmpty(picture))
            {
                await _imageUploadService.RemoveImage(AssetContainerConstants.PlaylistPictures, picture, cancellationToken);
            }
        }
    }
}