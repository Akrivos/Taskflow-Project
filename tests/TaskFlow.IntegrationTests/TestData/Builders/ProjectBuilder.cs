using System;
using TaskFlow.Domain.Entities;

namespace TaskFlow.IntegrationTests.TestData.Builders;

public sealed class ProjectBuilder
{
    private string _name = "Default Project";
    private string _description = "Default Description";

    public ProjectBuilder WithUniqueName(string prefix = "Project")
    {
        _name = $"{prefix}-{Guid.NewGuid():N}";
        return this;
    }

    public ProjectBuilder WithName(string name)
    {
        _name = name;

        return this;
    }

    public ProjectBuilder WithDescription(string description)
    {
        _description = description;
        return this;
    }

    public Project Build()
    {

        return new(_name, _description);
    }
}
