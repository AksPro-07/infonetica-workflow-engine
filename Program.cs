using WorkflowEngine.Models;
using WorkflowEngine.Services;
using WorkflowEngine.Storage;

var builder = WebApplication.CreateBuilder(args);

// ✅ Register services BEFORE building the app
builder.Services.AddSingleton<IWorkflowRepository, InMemoryWorkflowRepository>();
builder.Services.AddSingleton<WorkflowDefinitionService>();
builder.Services.AddSingleton<WorkflowExecutionService>();

var app = builder.Build();

// Endpoints

app.MapPost("/definitions", (WorkflowDefinition def, WorkflowDefinitionService definitionService) =>
{
    var result = definitionService.ValidateAndAddDefinition(def, out var error);
    if (!result)
        return Results.BadRequest(new { error });

    return Results.Created($"/definitions/{def.Id}", def);
});

app.MapGet("/definitions/{id}", (string id, WorkflowDefinitionService definitionService) =>
{
    var def = definitionService.GetDefinition(id);
    return def is not null ? Results.Ok(def) : Results.NotFound();
});

app.MapGet("/definitions", (WorkflowDefinitionService definitionService) =>
{
    return Results.Ok(definitionService.GetAllDefinitions());
});

app.MapPost("/instances", (string definitionId, WorkflowExecutionService executionService) =>
{
    try
    {
        var instance = executionService.StartInstance(definitionId);
        return Results.Created($"/instances/{instance.Id}", instance);
    }
    catch (Exception ex)
    {
        return Results.BadRequest(new { error = ex.Message });
    }
});

app.MapPost("/instances/{id}/execute", (string id, string actionId, WorkflowExecutionService executionService) =>
{
    var (success, error) = executionService.ExecuteAction(id, actionId);
    if (!success)
        return Results.BadRequest(new { error });

    var instance = executionService.GetInstance(id);
    return Results.Ok(instance);
});

app.MapGet("/instances/{id}", (string id, WorkflowExecutionService executionService) =>
{
    var instance = executionService.GetInstance(id);
    return instance is not null ? Results.Ok(instance) : Results.NotFound();
});

app.MapGet("/instances", (WorkflowExecutionService executionService) =>
{
    return Results.Ok(executionService.GetAllInstances());
});

app.Run();
