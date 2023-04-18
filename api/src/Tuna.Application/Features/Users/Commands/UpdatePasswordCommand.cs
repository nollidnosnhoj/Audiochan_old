﻿using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using OneOf;
using Tuna.Application.Features.Users.Errors;
using Tuna.Application.Persistence;
using Tuna.Application.Services;
using Tuna.Shared.Errors;
using Tuna.Shared.Mediatr;

namespace Tuna.Application.Features.Users.Commands;

public class UpdatePasswordCommand : AuthCommandRequest<UpdatePasswordCommandResult>
{
    public UpdatePasswordCommand(string newPassword, string currentPassword, ClaimsPrincipal user) : base(user)
    {
        CurrentPassword = currentPassword;
        NewPassword = newPassword;
    }

    public string CurrentPassword { get; }
    public string NewPassword { get; }
}

[GenerateOneOf]
public partial class UpdatePasswordCommandResult : OneOfBase<Unit, Unauthorized, IdentityServiceError>
{
}

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommand, UpdatePasswordCommandResult>
{
    private readonly IIdentityService _identityService;
    private readonly IUnitOfWork _unitOfWork;

    public UpdatePasswordCommandHandler(IUnitOfWork unitOfWork, IIdentityService identityService)
    {
        _unitOfWork = unitOfWork;
        _identityService = identityService;
    }

    public async Task<UpdatePasswordCommandResult> Handle(UpdatePasswordCommand command,
        CancellationToken cancellationToken)
    {
        var userId = command.GetUserId();
        var user = await _unitOfWork.Users.FindAsync(userId, cancellationToken);

        if (user is null) return new Unauthorized();

        var result = await _identityService.UpdatePasswordAsync(
            user.IdentityId,
            command.CurrentPassword,
            command.NewPassword,
            cancellationToken);

        if (!result.Succeeded) return new IdentityServiceError(result.Errors);

        // TODO: Remove session

        return Unit.Value;
    }
}