using UnityEngine;

public class AgentBoid : MonoBehaviour
{
    
    public AgentBoidData AgentBoidData;
    public float CurrentDistanceToPlayer { get; set; }

    private const int MIN_NEIGHBORS_TO_BE_SURROUNDED = 3;
    
    
    public bool IsSurrounded(Vector3 surroundedVector, int neighbourCount)
    {
        if (neighbourCount < MIN_NEIGHBORS_TO_BE_SURROUNDED) return false;
        return surroundedVector.magnitude <= AgentBoidData.SurroundedThreshold;
    }

    public Vector3 GetAppliedAgentAccelerationDirection(Vector3 direction)
    {
        return direction.normalized * AgentBoidData.AgentAcceleration;
    }
    
    
    
    public Vector3 GetEscapeFromGroupForce()
    {
        // Force to separate from nearby agents, prioritizing the closest ones
        Vector3 separationForce = FlockUtils.Separation(transform.position,
            DetectionUtils.GetNearbyAgents(AgentBoidData.SeparationDistance, transform),
            AgentBoidData.SeparationDistance) * AgentBoidData.SeparationWeight;
        
        // Force to move away from the center of the nearby agent group
        Vector3 antiCohesionForce = -FlockUtils.CohesionVector(transform.position,
            DetectionUtils.GetNearbyAgents(AgentBoidData.CohesionDistance, transform),
            AgentBoidData.CohesionDistance) * AgentBoidData.CohesionWeight;

        Vector3 combinedForce = separationForce + antiCohesionForce;
    
        combinedForce = Vector3.ClampMagnitude(combinedForce, AgentBoidData.MaxSteering);
    
        return combinedForce;
    }

    public Vector3 GetDirectionWithEscapeFromGroupForce(Vector3 direction)
    {
        // Force to separate from nearby agents, prioritizing the closest ones
        Vector3 separationForce = FlockUtils.Separation(transform.position,
            DetectionUtils.GetNearbyAgents(AgentBoidData.SeparationDistance, transform),
            AgentBoidData.SeparationDistance) * AgentBoidData.SeparationWeight;
        
        // Force to move away from the center of the nearby agent group
        Vector3 antiCohesionForce = -FlockUtils.CohesionVector(transform.position,
            DetectionUtils.GetNearbyAgents(AgentBoidData.CohesionDistance, transform),
            AgentBoidData.CohesionDistance) * AgentBoidData.CohesionWeight;

        Vector3 combinedForce = separationForce + antiCohesionForce + direction;
    
        combinedForce = Vector3.ClampMagnitude(combinedForce, AgentBoidData.MaxSteering);
    
        return combinedForce;
    }
    

    public Vector3 GetNormalizedCohesionVector()
    {
        Vector3 CohesionForce = FlockUtils.CohesionVector(transform.position,
            DetectionUtils.GetNearbyAgents(AgentBoidData.CohesionDistance, transform),
            AgentBoidData.CohesionDistance) * AgentBoidData.CohesionWeight;

        return CohesionForce.normalized;
    }
}