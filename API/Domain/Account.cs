using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace API.Domain;

public class Account(Guid id)
{
    [Key]
    public Guid Id { get; set; } = id;
    public decimal Balance { get; set; }
    
    [JsonIgnore]
    public Guid UserId { get; set; }
}