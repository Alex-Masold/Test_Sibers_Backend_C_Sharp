namespace Application.Contracts.Base;

public interface IUpdateDto<T>
{
    public bool ApplyTo(T entity);
}