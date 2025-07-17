using WorkflowEngine.Models;
using WorkflowEngine.Storage;

namespace WorkflowEngine.Services;

public class WorkflowExecutionService
{
    private readonly IWorkflowRepository _repository;

    public WorkflowExecutionService(IWorkflowRepository repository)
    {
        _repository = repository;
    }

    public WorkflowInstance StartInstance(string definitionId)
    {
        return _repository.CreateInstance(definitionId);
    }

    public (bool Success, string ErrorMessage) ExecuteAction(string instanceId, string actionId)
    {
        var instance = _repository.GetInstance(instanceId);
        if (instance == null)
            return (false, "Instance not found.");

        var definition = _repository.GetDefinition(instance.DefinitionId);
        if (definition == null)
            return (false, "Workflow definition not found.");

        var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentStateId);
        if (currentState == null)
            return (false, "Current state not found.");

        if (currentState.IsFinal)
            return (false, "Cannot execute actions on a final state.");

        var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
        if (action == null)
            return (false, $"Action '{actionId}' does not exist in the workflow.");

        if (!action.Enabled)
            return (false, "Action is disabled.");

        if (!action.FromStates.Contains(currentState.Id))
            return (false, $"Action '{action.Id}' is not allowed from current state '{currentState.Id}'.");

        var nextState = definition.States.FirstOrDefault(s => s.Id == action.ToState && s.Enabled);
        if (nextState == null)
            return (false, "Target state is disabled or not found.");

        // All good: perform the transition
        instance.CurrentStateId = nextState.Id;
        instance.History.Add(new ExecutionLog
        {
            ActionId = action.Id,
            Timestamp = DateTime.UtcNow
        });

        _repository.UpdateInstance(instance);
        return (true, "");
    }

    public WorkflowInstance? GetInstance(string instanceId)
    {
        return _repository.GetInstance(instanceId);
    }

    public IEnumerable<WorkflowInstance> GetAllInstances()
    {
        return _repository.GetAllInstances();
    }
}
