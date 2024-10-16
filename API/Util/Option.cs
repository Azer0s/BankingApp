using API.Domain;

namespace API.Util;

public class Option<T>
{
    private readonly T? _value;
    
    private Option(T? value)
    {
        this._value = value;
    }
    
    public static Option<T> Some(T value) => new Option<T>(value);
    public static Option<T> None() => new Option<T>(default);
    
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