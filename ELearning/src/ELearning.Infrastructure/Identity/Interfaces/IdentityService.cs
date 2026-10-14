using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELearning.Application.Common.Interfaces;
using ELearning.Application.Common.Models;
using Microsoft.AspNetCore.Identity;

namespace ELearning.Infrastructure.Identity.Interfaces
{
    public sealed class IdentityService : IIdentityService
{
    private const string DefaultRole = "Student";
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole<Guid>> _roleManager;
    private readonly SignInManager<ApplicationUser> _signInManager;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole<Guid>> roleManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
    }

    public async Task<IdentityOperationResult> RegisterAsync(
        string fullName,
        string email,
        string password)
    {
        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
        {
            return new IdentityOperationResult(
                false,
                null,
                Array.Empty<string>(),
                new[] { "An account with this email already exists." });
        }
        if (!await _roleManager.RoleExistsAsync(DefaultRole))
        {
            var roleResult = await _roleManager.CreateAsync(
                new IdentityRole<Guid>(DefaultRole));

            if (!roleResult.Succeeded)
            {
                return new IdentityOperationResult(
                    false,
                    null,
                    Array.Empty<string>(),
                    roleResult.Errors
                        .Select(error => error.Description)
                        .ToArray());
            }
        }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = fullName.Trim(),
            UserName = email.Trim(),
            Email = email.Trim()
        };

        var result = await _userManager.CreateAsync(user, password);

        if (!result.Succeeded)
        {
            return new IdentityOperationResult(
                false,
                null,
                Array.Empty<string>(),
                result.Errors
                    .Select(error => error.Description)
                    .ToArray());
        }
         var addRoleResult = await _userManager.AddToRoleAsync(
            user,
            DefaultRole);
        if (!addRoleResult.Succeeded)
        {
            // Roll back user creation if role assignment fails.
            await _userManager.DeleteAsync(user);

            return new IdentityOperationResult(
                false,
                null,
                Array.Empty<string>(),
                addRoleResult.Errors
                    .Select(error => error.Description)
                    .ToArray());
        }
        var roles = await _userManager.GetRolesAsync(user);
        return new IdentityOperationResult(
            true,
            user.Id,
            roles.ToArray(),
            Array.Empty<string>());
    }

    public async Task<IdentityOperationResult> LoginAsync(
        string email,
        string password)
    {
        const string invalidCredentialsMessage = "Invalid email or password.";
        var user = await _userManager.FindByEmailAsync(email.Trim());

        if (user is null)
        {
            return new IdentityOperationResult(
                false,
                null,
                Array.Empty<string>(),
                new[] { invalidCredentialsMessage });
        }

        var signInResult = await _signInManager.CheckPasswordSignInAsync(
            user,
            password,
            lockoutOnFailure: true);

        if (!signInResult.Succeeded)
        {
            return new IdentityOperationResult(
                false,
                null,
                Array.Empty<string>(),
                new[] { invalidCredentialsMessage });
        }

        var roles = await _userManager.GetRolesAsync(user);
        return new IdentityOperationResult(
            true,
            user.Id,
            roles.ToArray(),
            Array.Empty<string>());
    }

}
}