using MediatR;

namespace ELearning.Application.Features.Authentication.Commands.Login;

public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<LoginResponse>;

public sealed record LoginResponse(
    bool Succeeded,
    Guid? UserId,
    string? AccessToken,
    IReadOnlyList<string> Errors);
