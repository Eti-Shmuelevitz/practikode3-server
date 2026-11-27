
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("ToDoDB");

builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.Parse("8.0.46-mysql")));

builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseCors();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/items", (ToDoDbContext db) => db.Items.ToList());

app.MapPost("/items", (ToDoDbContext db, Item item) =>
{
    db.Items.Add(item);
    db.SaveChanges();
    return item;
});
app.MapPut("/items/{id}", (ToDoDbContext db, int id, Item newItem) =>
{
    var item = db.Items.Find(id);
    if (item == null) return Results.NotFound();

    item.IsComplete = newItem.IsComplete;
    db.SaveChanges();
    return Results.Ok(item);
});

app.MapDelete("/items/{id}",(ToDoDbContext db, int id)=>
{
    var item = db.Items.Find(id);
    if (item == null) return Results.NotFound();

    db.Items.Remove(item);
    db.SaveChanges();
    return Results.Ok();
});

app.Run();
