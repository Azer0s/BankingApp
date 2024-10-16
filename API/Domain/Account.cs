using System.ComponentModel.DataAnnotations;
using API.Util;
using API.Util.Stereotype;
using Newtonsoft.Json;

namespace API.Domain;

public class Account(Guid id) : DomainObject
{
    [Key]
    public Guid Id { get; set; } = id;
    public decimal Balance { get; set; }
    
    [JsonIgnore]
    public Guid UserId { get; set; }
}