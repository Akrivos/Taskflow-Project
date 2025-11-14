using MediatR;
namespace TaskFlow.Application.Projects.Commands;

public class UpdateProjectCommand : IRequest<Guid>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;
}
