using System.Collections.Generic;
using UnityEngine;

public static class FlockUtils
{
    public static Vector3 Separation<T>(Vector3 selfPosition, List<T> neighbors, float maxDistance) where T : Component
    {
        var steer = Vector3.zero; // Vector de separación acumulado
        var closeCount = 0; // Número de vecinos considerados

        foreach (var neighbor in neighbors)
        {
            var distance = Vector3.Distance(selfPosition, neighbor.transform.position);
            if (distance > maxDistance) continue; // Filtra según la distancia máxima

            // Dirección desde el vecino hacia el agente
            var diff = selfPosition - neighbor.transform.position;
            diff.y = 0; // Mantén el movimiento en plano horizontal

            if (diff != Vector3.zero)
            {
                diff.Normalize(); // Normaliza la dirección
                steer += diff; // Acumula la dirección de separación
                closeCount++;
            }
        }

        if (closeCount > 0) steer /= closeCount; // Promedia la acumulación si hay vecinos

        // Devuelve la dirección normalizada si no está en Vector3.zero
        return steer != Vector3.zero ? steer.normalized : Vector3.zero;
    }

    public static Vector3 CohesionVector<T>( Vector3 agentPosition,List<T> neighbors, float maxDistance)
        where T : Component
    {
        if (neighbors == null || neighbors.Count == 0)
            return Vector3.zero;

        var center = Vector3.zero;
        var closeCount = 0;

        foreach (var neighbor in neighbors)
        {
            var distance = Vector3.Distance(agentPosition, neighbor.transform.position);
            if (distance > maxDistance) continue; // Filtra según la distancia máxima

            var pos = neighbor.transform.position;
            pos.y = 0; // Plano X-Z (opcional)
            center += pos;
            closeCount++;
        }

        if (closeCount == 0)
            return Vector3.zero;

        center /= closeCount; // Centroide (promedio de las posiciones cercanas)

        // Vector desde el agente hacia el centroide
        var dir = center - new Vector3(agentPosition.x, 0f, agentPosition.z);

        // Devuelve la dirección (normalizada) para combinarla con un peso externo
        return dir == Vector3.zero ? Vector3.zero : dir.normalized;
    }

    public static Vector3 SurroundedVector<T>(List<T> neighbors, Vector3 agentPosition, float maxDistance)
        where T : Component
    {
        var totalVector = Vector3.zero;
        var closeCount = 0;

        if (neighbors == null || neighbors.Count == 0)
            return Vector3.zero;

        foreach (var neighbor in neighbors)
        {
            var distance = Vector3.Distance(agentPosition, neighbor.transform.position);
            if (distance > maxDistance) continue; // Filtra según la distancia máxima

            var toNeighbor = neighbor.transform.position - agentPosition;
            toNeighbor.y = 0;

            totalVector += toNeighbor.normalized;
            closeCount++;
        }

        if (closeCount == 0)
            return Vector3.zero;

        return totalVector;
    }
    
}