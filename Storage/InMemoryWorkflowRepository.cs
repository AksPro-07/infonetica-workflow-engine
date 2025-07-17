using WorkflowEngine.Models;

namespace WorkflowEngine.Storage;

public class InMemoryWorkflowRepository : IWorkflowRepository
{
    private readonly Dictionary<string, WorkflowDefinition> _definitions = new();
    private readonly Dictionary<string, WorkflowInstance> _instances = new();

    public void AddDefinition(WorkflowDefinition definition)
    {
        _definitions[definition.Id] = definition;
    }

    public WorkflowDefinition? GetDefinition(string id)
    {
        _definitions.TryGetValue(id, out var def);
        return def;
    }

    public IEnumerable<WorkflowDefinition> GetAllDefinitions()
    {
        return _definitions.Values;
    }

    public WorkflowInstance CreateInstance(string definitionId)
    {
        var instance = new WorkflowInstance
        {
            DefinitionId = definitionId,
            Id = Guid.NewGuid().ToString(),
        };

        if (_definitions.TryGetValue(definitionId, out var definition))
        {
            var initialState = definition.States.FirstOrDefault(s => s.IsInitial && s.Enabled);
            if (initialState == null)
                throw new InvalidOperationException("No valid initial state found.");

            instance.CurrentStateId = initialState.Id;
        }
        else
        {
            throw new KeyNotFoundException("Workflow definition not found.");
        }

        _instances[instance.Id] = instance;
        return instance;
    }

    public WorkflowInstance? GetInstance(string id)
    {
        _instances.TryGetValue(id, out var inst);
        return inst;
    }

    public void UpdateInstance(WorkflowInstance instance)
    {
        _instances[instance.Id] = instance;
    }

    public IEnumerable<WorkflowInstance> GetAllInstances()
    {
        return _instances.Values;
    }
}
