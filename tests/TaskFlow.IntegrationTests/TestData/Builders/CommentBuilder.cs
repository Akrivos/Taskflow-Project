using System;
using TaskFlow.Domain.Entities;

namespace TaskFlow.IntegrationTests.TestData.Builders;

public sealed class CommentBuilder
{
    private Guid _taskItemId;
    private string _content = "This is a comment.";
    private string _userId = "test-user-id";

    public CommentBuilder ForTask(Guid taskItemId)
    {
        _taskItemId = taskItemId;
        return this;
    }

    public CommentBuilder ByUser(string userId)
    {
        _userId = userId;
        return this;
    }

    public CommentBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public Comment Build()
    {
        if (_taskItemId == Guid.Empty)
        {
            throw new InvalidOperationException("TaskItemId must be set.");
        }


        return new Comment(_taskItemId, _content, _userId);
    }
}
