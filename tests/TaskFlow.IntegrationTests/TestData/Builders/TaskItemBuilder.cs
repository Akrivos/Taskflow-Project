using System;
using TaskFlow.Domain.Entities;

namespace TaskFlow.IntegrationTests.TestData.Builders;

public sealed class TaskItemBuilder
{
    private string _title = "Task 1";
    private string _description = "Task Description";
    private Guid _projectId;

    public TaskItemBuilder ForProject(Guid projectId)
    {
        _projectId = projectId;
        return this;
    }

    public TaskItemBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public TaskItemBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public TaskItem Build()
    {
        if (_projectId == Guid.Empty)
        {
            throw new InvalidOperationException("ProjectId must be set.");
        }

        return new TaskItem(_title, _description, _projectId);
    }
}
