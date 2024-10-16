namespace API.Util;

public readonly struct Result<T>(T? value, IError error)
    where T : class
{
    private T? Value { get; } = value;
    private IError Err { get; } = error;

    public static Result<T> Ok(T left) => new(left, null!);
    public static Result<T> Error(IError right) => new(default, right);
    
    public bool IsOk => Value != null;
    public bool IsError => Value == null;
    
    public Result<TR> Map<TR>(Func<T, TR> f) where TR: class
    {
        return Value != null ? Result<TR>.Ok(f(Value)) : Result<TR>.Error(Err);
    }
    
    public Result<TR> MapOrError<TR>(Func<T, Result<TR>> f) where TR: class
    {
        return Value != null ? f(Value) : Result<TR>.Error(Err);
    }
    
    public T OrElse(Func<IError, T> f)
    {
        return Value ?? f(Err);
    }
    
    public T OrElseThrow()
    {
        if (Value == null)
        {
            throw new Exception(Err.Message);
        }
        
        return Value;
    }
}