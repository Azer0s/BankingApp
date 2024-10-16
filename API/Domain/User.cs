using System.ComponentModel.DataAnnotations;
using API.Util;
using API.Util.Stereotype;

namespace API.Domain;

public class User(Guid id) : DomainObject
{
    [Key]
    public Guid Id { get; set; } = id;
    public List<Account> Accounts { get; set; } = [];
    public decimal PersonalBalance { get; set; } = 0m;
}