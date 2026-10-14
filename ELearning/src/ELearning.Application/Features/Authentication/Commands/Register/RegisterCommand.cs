using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;

namespace ELearning.Application.Features.Authentication.Commands.Register;
public sealed record RegisterCommand(
    string FullName,
    string Email,
    string Password) : IRequest<RegisterResponse>;

public sealed record RegisterResponse(
    bool Succeeded,
    Guid? UserId,
    string? AccessToken,
    IReadOnlyList<string> Errors);