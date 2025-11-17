namespace TaskFlow.Application.Common.Interfaces;

public interface IReadRepository<T>
{
    IQueryable<T> Query(); 
}