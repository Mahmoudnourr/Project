using ELearning.Application.Common.Interfaces;
using MediatR;

namespace ELearning.Application.Features.Authentication.Commands.Login;

public sealed class LoginCommandHandler
    : IRequestHandler<LoginCommand, LoginResponse>
{
    private readonly IIdentityService _identityService;
    private readonly ITokenService _tokenService;

    public LoginCommandHandler(
        IIdentityService identityService,
        ITokenService tokenService)
    {
        _identityService = identityService;
        _tokenService = tokenService;
    }

    public async Task<LoginResponse> Handle(
        LoginCommand request,
        CancellationToken cancellationToken)
    {
        var email = request.Email.Trim();
        var result = await _identityService.LoginAsync(
            email,
            request.Password);

        if (!result.Succeeded || result.UserId is null)
        {
            return new LoginResponse(
                false,
                null,
                null,
                result.Errors);
        }

        var accessToken = await _tokenService.GenerateAccessTokenAsync(
            result.UserId.Value,
            email,
            result.Roles.ToArray());

        return new LoginResponse(
            true,
            result.UserId.Value,
            accessToken,
            Array.Empty<string>());
    }
}
