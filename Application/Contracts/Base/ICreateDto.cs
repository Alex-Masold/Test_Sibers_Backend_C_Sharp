namespace Application.Contracts.Base;

public interface ICreateDto<T>
{
    public T ToEntity();
}