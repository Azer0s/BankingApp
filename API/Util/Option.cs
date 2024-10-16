using API.Domain;

namespace API.Util;

public readonly struct Option<T>
{
    private readonly T? _value;
    
    private Option(T? value)
    {
        _value = value;
    }
    
    public static Option<T> Some(T value) => new(value);
    public static Option<T> None() => new(default);
    
    public bool IsSome => _value != null;
    public bool IsNone => _value == null;
    
    public T Unwrap()
    {
        if (_value == null)
        {
            throw new InvalidOperationException("Cannot unwrap None");
        }
        
        return _value;
    }

    public static Option<T> FromNullable(T? value)
    {
        return value == null ? None() : Some(value);
    }
}