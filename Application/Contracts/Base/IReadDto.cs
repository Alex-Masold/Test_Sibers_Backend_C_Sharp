using System.Linq.Expressions;

namespace Application.Contracts.Base;

public interface IReadDto< TEntity,  TDto>
    where TDto : IReadDto<TEntity, TDto>
{
    static abstract Expression<Func<TEntity, TDto>> Projection { get; }
    static abstract TDto From(TEntity entity);
}

