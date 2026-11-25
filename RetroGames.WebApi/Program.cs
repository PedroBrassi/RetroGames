using RetroGames.WebApi.Configuration;
using RetroGames.WebApi.Middlewares;
using FluentValidation;
using RetroGames.WebApi.Filters;

DotNetEnv.Env.TraversePath().Load();

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("Default");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Configure ConnectionStrings__Default no arquivo .env da raiz do repositório.");
}

builder.Services.AddRepositorios(connectionString);
builder.Services.AddServicosDeNegocio();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

builder.Services
    .AddControllers(opcoes => opcoes.Filters.Add<FluentValidationFilter>())
    .ConfigureApiBehaviorOptions(opcoes =>
    {
        opcoes.InvalidModelStateResponseFactory = contexto =>
            FluentValidationFilter.CriarResposta(
                contexto.ModelState.SelectMany(campo =>
                    campo.Value!.Errors.Select(erro => (
                        Campo: campo.Key,
                        Mensagem: erro.Exception is null &&
                                  !string.IsNullOrWhiteSpace(erro.ErrorMessage)
                            ? erro.ErrorMessage
                            : "Valor inválido."))));
    });
    
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(opcoes =>
    {
        opcoes.SwaggerEndpoint("/openapi/v1.json", "RetroGames v1");
    });
}

app.MapControllers();

app.Run();