using System;
using TaskFlow.Domain.Enums;
using DomainTaskStatus = TaskFlow.Domain.Enums.TaskStatus;

namespace TaskFlow.Application.DTOs;

public record TaskDto(
    Guid Id,
    string Title,
    string? Description,
    DomainTaskStatus Status,
    Guid ProjectId
);