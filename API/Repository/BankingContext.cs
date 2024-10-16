using API.Domain;
using Microsoft.EntityFrameworkCore;

namespace API.Repository;

public class BankingContext(DbContextOptions<BankingContext> options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Account> Accounts { get; set; }
}