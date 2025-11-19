using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskFlow.Domain.Entities;

namespace TaskFlow.Application.Common.Interfaces;

public interface ICommentRepository : IRepository<Comment> 
{
    Task DeleteAsync(Comment entity, CancellationToken ct);
}