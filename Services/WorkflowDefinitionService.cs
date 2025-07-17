using WorkflowEngine.Models;
using WorkflowEngine.Storage;

namespace WorkflowEngine.Services;

public class WorkflowDefinitionService
{
    private readonly IWorkflowRepository _repository;

    public WorkflowDefinitionService(IWorkflowRepository repository)
    {
        _repository = repository;
    }

    public bool ValidateAndAddDefinition(WorkflowDefinition definition, out string errorMessage)
    {
        if (_repository.GetDefinition(definition.Id) is not null)
        {
            errorMessage = $"Workflow with ID '{definition.Id}' already exists.";
            return false;
        }

        // Check for duplicate state IDs
        var stateIds = definition.States.Select(s => s.Id).ToList();
        if (stateIds.Count != stateIds.Distinct().Count())
        {
            errorMessage = "Duplicate state IDs found.";
            return false;
        }

        // Check for exactly one initial state
        var initialStates = definition.States.Where(s => s.IsInitial).ToList();
        if (initialStates.Count != 1)
        {
            errorMessage = "Workflow must contain exactly one initial state.";
            return false;
        }

        // Check that all actions point to valid states
        var allStateIds = definition.States.Select(s => s.Id).ToHashSet();
        foreach (var action in definition.Actions)
        {
            if (!allStateIds.Contains(action.ToState) ||
                action.FromStates.Any(f => !allStateIds.Contains(f)))
            {
                errorMessage = $"Action '{action.Id}' references unknown state(s).";
                return false;
            }
        }

        // All good
        _repository.AddDefinition(definition);
        errorMessage = string.Empty;
        return true;
    }

    public WorkflowDefinition? GetDefinition(string id)
    {
        return _repository.GetDefinition(id);
    }

    public IEnumerable<WorkflowDefinition> GetAllDefinitions()
    {
        return _repository.GetAllDefinitions();
    }
}
