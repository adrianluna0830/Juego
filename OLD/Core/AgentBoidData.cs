using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "AgentBoidData", menuName = "ScriptableObjects/AgentBoidData", order = 1)]
public class AgentBoidData : ScriptableObject
{
    [Header("Thresholds")] 
    public float SurroundedThreshold;

    [Header("Steering and Movement")] 
    public float MaxSteering;
    public float AgentAcceleration;
    public float AgentAccelerationSprint;

    [Header("DistanceToPlayer")] 
    public float MinDistanceToPlayer;
    public float MaxDistanceToPlayer;

    [Header("Flocking")] 
    public float SeparationDistance;
    public float CohesionDistance;

    [Header("Weights")] 
    public float SeparationWeight;
    public float CohesionWeight;

    [Header("Retreat")] 
    public float RetreatMinDistance;
    public float RetreatMaxDistance;

    [FormerlySerializedAs("DefaultAgentDetectionDistance")] [Header("Detection Distances")] 
    public float AgentDetectionDistance;

    [Header("Detection Angles")]
    public float DetectAgentsAngle;
    public float FleeHorizontalAngle;

    [Header("Strafing")]
    public float StrafeSpeed;
    public float MaxStrafeTime;
    public float MinStrafeTime;
}