using System.ComponentModel.DataAnnotations;

namespace API.Util;

// This is basically a `derive` to get the table name and id field of a class
public abstract class Lockable
{
    private static readonly Dictionary<string, (string tableName, string idField)> TableNames = new();
    
    private static string FormatId(object id)
    {
        return (id switch
        {
            Guid guid => $"uuid('{guid}')",
            string str => $"'{str}'",
            _ => id.ToString()
        })!;
    }
    
    public (string tableName, string idField, string idValue) AcquireLock()
    {
        // look up if we have already cached the table name and id field
        if (TableNames.TryGetValue(GetType().Name, out var value))
        {
            var idValue = GetType().GetProperty(value.idField)?.GetValue(this) ?? throw new Exception("Id field not found");
            return (value.tableName, value.idField, FormatId(idValue));
        }
        
        // get the field with [Key] attribute
        var keyField = GetType().GetProperties().FirstOrDefault(p => p.GetCustomAttributes(typeof(KeyAttribute), true).Length > 0);
        if (keyField == null)
        {
            throw new Exception("No field with [Key] attribute found");
        }
        
        var id = keyField.GetValue(this) ?? throw new Exception("Id field not found");
        var idName = keyField.Name;
        
        var typeName = !GetType().Name.EndsWith("s") ? GetType().Name + "s" : GetType().Name;
        
        // cache for later
        TableNames[GetType().Name] = (typeName, idName);

        return (typeName, idName, FormatId(id));
    }
}