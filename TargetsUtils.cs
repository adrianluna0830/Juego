using System;
using System.Collections.Generic;
using UnityEngine;

public static class TargetsUtils
{
    /// <summary>
    /// Encuentra el objeto más cercano a una posición específica de una lista de objetos que implementan IHitManager.
    /// </summary>
    /// <param name="objects">Lista de objetos a verificar.</param>
    /// <param name="referencePosition">Posición de referencia.</param>
    /// <returns>El objeto más cercano a la posición de referencia o lanza una excepción si la lista está vacía.</returns>
    public static IHitManager GetClosestObjectByDistance(List<IHitManager> objects, Vector3 referencePosition)
    {
        if (objects == null || objects.Count == 0)
        {
            throw new ArgumentException("La lista de objetos está vacía o es nula.");
        }

        IHitManager closestObject = null;
        float closestDistance = float.MaxValue;

        foreach (IHitManager obj in objects)
        {
            float distance = Vector3.Distance(referencePosition, obj.Transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    /// <summary>
    /// Encuentra el objeto más cercano a un ángulo específico de una lista de objetos que implementan IHitManager.
    /// </summary>
    /// <param name="objects">Lista de objetos a verificar.</param>
    /// <param name="referencePosition">Posición de referencia.</param>
    /// <param name="referenceDirection">Dirección de referencia.</param>
    /// <returns>El objeto más cercano al ángulo de referencia o <c>null</c> si no hay uno válido.</returns>
    public static IHitManager GetClosestObjectByAngle(List<IHitManager> objects, Vector3 referencePosition,
        Vector3 referenceDirection)
    {
        if (objects == null || objects.Count == 0)
        {
            throw new ArgumentException("La lista de objetos está vacía o es nula.");
        }

        IHitManager closestObject = null;
        float closestAngle = float.MaxValue;

        foreach (IHitManager obj in objects)
        {
            Vector3 directionToObject = (obj.Transform.position - referencePosition).normalized;
            float angleToObject = Vector3.Angle(referenceDirection, directionToObject);

            if (angleToObject < closestAngle)
            {
                closestAngle = angleToObject;
                closestObject = obj;
            }
        }

        return closestObject;
    }

    /// <summary>
    /// Obtiene objetos que implementen IHitManager dentro de un radio desde una posición específica.
    /// </summary>
    /// <param name="position">Posición central para la búsqueda.</param>
    /// <param name="radius">Radio de búsqueda.</param>
    /// <returns>Lista de objetos dentro del radio especificado.</returns>
    public static List<IHitManager> GetObjectsInRadius(Vector3 position, float radius)
    {
        Collider[] colliders = Physics.OverlapSphere(position, radius);
        List<IHitManager> result = new List<IHitManager>();

        foreach (Collider collider in colliders)
        {
            IHitManager hitManager = collider.GetComponent<IHitManager>();
            if (hitManager != null)
            {
                result.Add(hitManager);
            }
        }

        return result;
    }

    /// <summary>
    /// Filtra una lista de objetos que implementen IHitManager basándose en si están dentro de un ángulo de visión específico.
    /// </summary>
    /// <param name="objects">Lista de objetos a filtrar.</param>
    /// <param name="position">Posición del observador.</param>
    /// <param name="forward">Dirección de visión del observador.</param>
    /// <param name="angle">Ángulo de visión en grados.</param>
    /// <returns>Lista de objetos dentro del ángulo de visión especificado.</returns>
    public static List<IHitManager> GetObjectsInViewAngle(List<IHitManager> objects, Vector3 position,
        Vector3 forward, float angle)
    {
        List<IHitManager> result = new List<IHitManager>();
        float halfAngle = angle / 2f;

        foreach (IHitManager obj in objects)
        {
            Vector3 directionToObject = (obj.Transform.position - position).normalized;
            float angleToObject = Vector3.Angle(forward, directionToObject);

            if (angleToObject <= halfAngle)
            {
                result.Add(obj);
            }
        }

        return result;
    }

    /// <summary>
    /// Método combinado que obtiene objetos que implementen IHitManager dentro de un radio y luego los filtra por ángulo de visión.
    /// </summary>
    /// <param name="position">Posición del observador.</param>
    /// <param name="forward">Dirección de visión del observador.</param>
    /// <param name="radius">Radio de búsqueda.</param>
    /// <param name="angle">Ángulo de visión en grados.</param>
    /// <returns>Lista de objetos dentro del radio y ángulo de visión especificados.</returns>
    public static List<IHitManager> GetObjectsInRadiusAndAngle(Vector3 position, Vector3 forward, float radius,
        float angle)
    {
        List<IHitManager> objectsInRadius = GetObjectsInRadius(position, radius);
        return GetObjectsInViewAngle(objectsInRadius, position, forward, angle);
    }
}