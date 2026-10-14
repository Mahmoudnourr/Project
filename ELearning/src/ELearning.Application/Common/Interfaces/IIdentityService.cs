using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ELearning.Application.Common.Models;

namespace ELearning.Application.Common.Interfaces
{
    public interface IIdentityService
    {
        Task<IdentityOperationResult> RegisterAsync(
        string fullName,
        string email,
        string password);
    }
}