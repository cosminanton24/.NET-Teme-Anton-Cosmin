using System.Collections.Concurrent;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();

builder.Services.AddValidatorsFromAssemblyContaining<CreateBookCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<UpdateBookCommandValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PaginationParamsValidator>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapPost("/books", async Task<Results<Created<Book>, ValidationProblem>> (
    CreateBookCommand cmd,
    IBookRepository repo,
    IValidator<CreateBookCommand> validator) =>
{
    var vr = await validator.ValidateAsync(cmd);
    if (!vr.IsValid) return TypedResults.ValidationProblem(vr.ToDictionary());

    var created = repo.Create(cmd.Title.Trim(), cmd.Author.Trim(), cmd.Year);
    return TypedResults.Created($"/books/{created.Id}", created);
});

app.MapGet("/books/{id:int}", Results<Ok<Book>, NotFound> (int id, IBookRepository repo) =>
{
    var b = repo.GetById(id);
    return b is null ? TypedResults.NotFound() : TypedResults.Ok(b);
});

app.MapGet("/books", async Task<Results<Ok<PagedResult<Book>>, ValidationProblem>> (
    int? page,
    int? pageSize,
    IBookRepository repo,
    IValidator<PaginationParams> validator) =>
{
    var p = new PaginationParams(page ?? 1, pageSize ?? 10);
    var vr = await validator.ValidateAsync(p);
    if (!vr.IsValid) return TypedResults.ValidationProblem(vr.ToDictionary());

    var (items, total) = repo.GetPaged(p.Page, p.PageSize);
    var totalPages = (int)Math.Ceiling(total / (double)p.PageSize);
    var result = new PagedResult<Book>(items, total, p.Page, p.PageSize, totalPages);
    return TypedResults.Ok(result);
});

app.MapPut("/books/{id:int}", async Task<Results<Ok<Book>, NotFound, ValidationProblem>> (
    int id,
    UpdateBookCommand body,
    IBookRepository repo,
    IValidator<UpdateBookCommand> validator) =>
{
    var cmd = body with { Id = id };
    var vr = await validator.ValidateAsync(cmd);
    if (!vr.IsValid) return TypedResults.ValidationProblem(vr.ToDictionary());

    if (repo.GetById(cmd.Id) is null) return TypedResults.NotFound();

    repo.Update(cmd.Id, cmd.Title.Trim(), cmd.Author.Trim(), cmd.Year);
    var updated = repo.GetById(cmd.Id)!;
    return TypedResults.Ok(updated);
});

app.MapDelete("/books/{id:int}", Results<NoContent, NotFound> (int id, IBookRepository repo) =>
{
    var ok = repo.Delete(id);
    return ok ? TypedResults.NoContent() : TypedResults.NotFound();
});

// optional seed
var seed = app.Services.GetRequiredService<IBookRepository>();
seed.Create("Clean Code", "Robert C. Martin", 2008);
seed.Create("The Pragmatic Programmer", "Andrew Hunt", 1999);
seed.Create("Design Patterns", "Erich Gamma", 1994);

app.Run();