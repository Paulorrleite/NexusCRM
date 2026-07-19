using FluentValidation;
using NexusCRM.Application;
using NexusCRM.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.Use(async (context, next) =>
{
    try
    {
        await next(context);
    }
    catch (ValidationException exception)
    {
        var errors = exception.Errors
            .GroupBy(error => error.PropertyName)
            .ToDictionary(
                group => group.Key,
                group => group.Select(error => error.ErrorMessage).ToArray());

        await Results.ValidationProblem(errors).ExecuteAsync(context);
    }
});

app.UseAuthorization();

app.MapControllers();

app.Run();
