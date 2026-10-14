using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELearning.Application.Common.Interfaces
{
    public interface ITokenService
    {
        Task<string> GenerateAccessTokenAsync(
        Guid userId,
        string email,
        IList<string> roles);
    }
}