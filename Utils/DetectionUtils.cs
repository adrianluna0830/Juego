using System.Collections.Generic;
using UnityEngine;

public static class DetectionUtils
{
    
    public static List<Transform> GetNearbyAgentsInView(float viewAngle,float maxDistance,Transform physicsMovement)
    {
        // 1. Obtener los agentes cercanos (ya filtra por distancia y exclusiones básicas)
        List<Transform> nearby = GetNearbyAgents(maxDistance, physicsMovement.transform);

        // 2. Filtrar por campo de visión
        List<Transform> nearbyInView = new List<Transform>();

     
        Vector3 forward = physicsMovement.transform.forward;
        // Dirección en la que mira
        float halfView = viewAngle * 0.5f;                    // Ángulo “a cada lado” del forward

        foreach (Transform target in nearby)
        {
            Vector3 dir = target.position - physicsMovement.transform.position;
            dir.y = 0f;                                       // Ignorar diferencia de altura

            // Si el ángulo entre el forward y la dirección al objetivo es menor que el FOV/2,
            // el objetivo está dentro del cono de visión.
            if (Vector3.Angle(forward, dir) <= halfView)
            {
                nearbyInView.Add(target);
            }
        }

        return nearbyInView;
    } 

    
    public static List<Transform> GetNearbyAgents( float maxDistance,Transform ignoreTransform)
    {
        List<Transform> nearbyAgents = new List<Transform>();
        foreach (var transform in CombatContext.Instance.Enemies)
        {
            if (transform == ignoreTransform) continue;
            Vector3 distance = ignoreTransform.position - transform.position;
            distance.y = 0;
        
            if (distance.magnitude > maxDistance) continue;
            nearbyAgents.Add(transform);
        }
        return nearbyAgents;
    }
}