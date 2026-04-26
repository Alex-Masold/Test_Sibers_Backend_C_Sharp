namespace Application.Contracts.Base;

public interface ICreateDto<T>
{
    public T ToEntity();
}

public interface ICreateDto<T, in TParam>
{
    public T ToEntity(TParam param);
}

