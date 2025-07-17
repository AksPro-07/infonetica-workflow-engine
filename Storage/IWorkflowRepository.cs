using WorkflowEngine.Models;

namespace WorkflowEngine.Storage;

public interface IWorkflowRepository
{
    // Definitions
    void AddDefinition(WorkflowDefinition definition);
    WorkflowDefinition? GetDefinition(string id);
    IEnumerable<WorkflowDefinition> GetAllDefinitions();

    // Instances
    WorkflowInstance CreateInstance(string definitionId);
    WorkflowInstance? GetInstance(string id);
    void UpdateInstance(WorkflowInstance instance);
    IEnumerable<WorkflowInstance> GetAllInstances();
}
