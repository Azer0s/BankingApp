using System.ComponentModel.DataAnnotations;

namespace API.Domain;

public class User(Guid id)
{
    [Key]
    public Guid Id { get; set; } = id;

    public List<Account> Accounts { get; set; } = [];
    
    public decimal PersonalBalance { get; set; } = 0m;
}