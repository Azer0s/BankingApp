using API.Repository;
using API.Repository.Impl;
using API.Service;
using API.Service.Impl;
using Microsoft.EntityFrameworkCore;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<BankingContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("postgres");
    options.UseNpgsql(connectionString);
});

builder.Services.AddScoped<ITransactionService, TransactionServiceImpl>();

builder.Services.AddScoped<IAccountRepository, AccountRepositoryImpl>();
builder.Services.AddScoped<IAccountService, AccountServiceImpl>();

builder.Services.AddScoped<IUserRepository, UserRepositoryImpl>();
builder.Services.AddScoped<IUserService, UserServiceImpl>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
} else {
    app.UseHsts();
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();