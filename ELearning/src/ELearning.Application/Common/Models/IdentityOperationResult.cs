using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ELearning.Application.Common.Models
{
    public sealed record IdentityOperationResult( bool Succeeded,
    Guid? UserId,
    IReadOnlyList<string> Roles
    ,IReadOnlyList<string> Errors);
}