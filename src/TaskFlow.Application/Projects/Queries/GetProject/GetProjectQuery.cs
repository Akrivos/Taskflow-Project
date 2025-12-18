using MediatR;
using TaskFlow.Application.DTOs;
namespace TaskFlow.Application.Projects.Queries;
public sealed record GetProjectQuery(Guid id) : IRequest<ProjectResponseDto>;
