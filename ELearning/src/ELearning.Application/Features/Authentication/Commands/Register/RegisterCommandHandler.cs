using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELearning.Application.Common.Interfaces;
using MediatR;

namespace ELearning.Application.Features.Authentication.Commands.Register
{
    public sealed class RegisterCommandHandler : IRequestHandler<RegisterCommand, RegisterResponse>
    {
        private readonly IIdentityService _identityService;
        private readonly ITokenService _tokenService;

        public RegisterCommandHandler(IIdentityService identityService, ITokenService tokenService)
        {
            _identityService = identityService;
            _tokenService = tokenService;
        }
        public async Task<RegisterResponse> Handle(
          RegisterCommand request,
          CancellationToken cancellationToken)
        {
            var result = await _identityService.RegisterAsync(
                request.FullName.Trim(),
                request.Email.Trim(),
                request.Password);

            if (!result.Succeeded || result.UserId is null)
            {
                return new RegisterResponse(
                    false,
                    null,
                    null,
                    result.Errors);
            }

            var accessToken = await _tokenService.GenerateAccessTokenAsync(
                result.UserId.Value,
                request.Email.Trim(),
                result.Roles.ToArray());

            return new RegisterResponse(
                true,
                result.UserId.Value,
                accessToken,
                Array.Empty<string>());
                
        }

    }

}