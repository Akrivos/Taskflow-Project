using MediatR;
using TaskFlow.Application.DTOs;
namespace TaskFlow.Application.Projects.Queries;
public record GetProjectQuery(Guid id) : IRequest<ProjectDto>;
