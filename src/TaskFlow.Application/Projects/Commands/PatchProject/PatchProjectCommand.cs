using MediatR;
namespace TaskFlow.Application.Projects.Commands;

public class PatchProjectCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string? Name { get; set; } 
    public string? Description { get; set; }
}
