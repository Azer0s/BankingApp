namespace API.Util;

public abstract class Lockable
{
    public (string, string) AcquireLock()
    {
        var id = GetType().GetProperty("Id")?.GetValue(this) ?? throw new Exception("Id field not found");
        var typeName = !GetType().Name.EndsWith("s") ? GetType().Name + "s" : GetType().Name;

        return (id switch
        {
            Guid guid => (typeName, $"uuid('{guid}')"),
            string str => (typeName, $"'{str}'"),
            _ => (typeName, id.ToString())!
        })!;
    }
}