using UnityEngine;
using UnityEngine.AI;

// Note: This will be mostly used for enemies, but we could add it to an ally npc that follow us, etc.
public class NavMeshMovementHandler : IMovable
{
    private NavMeshAgent agent;
    public NavMeshMovementHandler(NavMeshAgent agent)
    {
        this.agent = agent;
    }

    bool isActive = true;
    public bool IsActive { get => isActive; set => isActive = value; }

    public void MoveTowards(Vector3 target, float speed)
    {
        if (!isActive)
            return;

        agent.isStopped = false;
        agent.SetDestination(target);
    }

    public void SetRotation(Vector3 direction)
    {
        // We do nothing since NavMeshAgents already handle rotations, but we can override it.
    }

    public void Stop() => agent.isStopped = true;
}
