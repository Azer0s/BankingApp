namespace API.Util;

public abstract class Lockable
{
    public (string, string) AcquireLock()
    {
        var id = GetType().GetProperty("Id")?.GetValue(this) ?? throw new Exception("Id field not found");
        var typeName = !GetType().Name.EndsWith("s") ? GetType().Name + "s" : GetType().Name;
        
        if (id is Guid guid)
        {
            return (typeName, $"uuid('{guid}')");
        }
        
        return (typeName, id.ToString())!;
    }
}