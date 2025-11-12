using MediatR;
using TaskFlow.Application.DTOs;
namespace TaskFlow.Application.Projects.Queries;
public record GetProjectsQuery() : IRequest<IEnumerable<ProjectDto>>;
