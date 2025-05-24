using System.Collections.Generic;
using UnityEngine;

public class RingManager : MonoBehaviour
{
    [Header("Configuración de Rings")]
    [SerializeField] private float distanciaInicialDelPrimerRing = 2f;
    [SerializeField] private float separacionEntreRings = 2f;
    [SerializeField] private int slotsInicialPorRing = 4;
    [SerializeField] private int slotStepPorRing = 2;
    [Header("Transform principal (centro)")]
    [SerializeField] private Transform centro;

    // Diccionario para asociar cada Transform a su índice de ring
    private Dictionary<Transform, int> transformRingIndex = new Dictionary<Transform, int>();

    // Lista completa de Transforms a gestionar
    private List<Transform> todosLosObjetos = new List<Transform>();

    /// <summary>
    /// Agrega un Transform al manager. Se recalcularán sus rings en la próxima actualización (Tick).
    /// </summary>
    /// <param name="objeto">Transform a agregar</param>
    public void AgregarTransform(Transform objeto)
    {
        if (!todosLosObjetos.Contains(objeto))
        {
            todosLosObjetos.Add(objeto);
        }
    }

    /// <summary>
    /// Elimina un Transform del manager. Se recalcularán sus rings en la próxima actualización (Tick).
    /// </summary>
    /// <param name="objeto">Transform a remover</param>
    public void RemoverTransform(Transform objeto)
    {
        if (todosLosObjetos.Contains(objeto))
        {
            todosLosObjetos.Remove(objeto);
        }

        if (transformRingIndex.ContainsKey(objeto))
        {
            transformRingIndex.Remove(objeto);
        }
    }

    /// <summary>
    /// Recalcula a qué ring pertenece cada Transform, ordenándolos primero 
    /// por cercanía al centro y asignándolos de menor ring a mayor.
    /// </summary>
    public void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            DebugExtension.DebugCircle(centro.transform.position + new Vector3(0, 1, 0), Color.red, distanciaInicialDelPrimerRing + (i * separacionEntreRings));

        }

        // Ordenar la lista por distancia al centro (menor a mayor).
        // Solo si existe un centro válido para calcular distancias.
        if (centro != null)
        {
            todosLosObjetos.Sort((t1, t2) =>
            {
                float dist1 = Vector3.Distance(t1.position, centro.position);
                float dist2 = Vector3.Distance(t2.position, centro.position);
                return dist1.CompareTo(dist2);
            });
        }

        // Limpiar la asignación previa
        transformRingIndex.Clear();

        // Índice global que recorre todosLosObjetos
        int indiceGlobal = 0;
        int ringActual = 0;
        int numObjetos = todosLosObjetos.Count;

        // Asignar rings hasta que hayamos ubicado todos los objetos
        while (indiceGlobal < numObjetos)
        {
            // Determina cuántos objetos caben en este ring
            int capacidadRing = slotsInicialPorRing + (ringActual * slotStepPorRing);

            // Para cada objeto que quepa en el ring actual:
            int limite = Mathf.Min(indiceGlobal + capacidadRing, numObjetos);
            for (int i = indiceGlobal; i < limite; i++)
            {
                transformRingIndex[todosLosObjetos[i]] = ringActual;
            }

            // Actualiza el índice global y pasa al siguiente ring
            indiceGlobal += capacidadRing;
            ringActual++;
        }
    }

    /// <summary>
    /// Obtiene la distancia desde el centro hasta el ring en el que se ubica el Transform dado.
    /// </summary>
    /// <param name="objeto">Transform cuyo ring se consulta</param>
    /// <returns>Distancia desde el centro hasta ese ring</returns>
    public float ObtenerRadioDelRing(Transform objeto)
    {
        if (!transformRingIndex.ContainsKey(objeto))
        {
            return 0f; // No está asignado
        }

        int ringIndex = transformRingIndex[objeto];
        // Cálculo del radio para este ring
        return distanciaInicialDelPrimerRing + (ringIndex * separacionEntreRings);
    }

    /// <summary>
    /// Devuelve la posición ideal en la circunferencia del ring para el Transform dado.
    /// Se asume que los objetos se distribuyen equitativamente alrededor del ring,
    /// tomando en cuenta su orden en la lista y su ring.
    /// </summary>
    /// <param name="objeto">Transform cuyo lugar en el ring se consulta</param>
    /// <returns>La posición estimada en el ring. Retorna la posición actual si no está asignado.</returns>
    public Vector3 ObtenerPosicionEnRing(Transform objeto)
    {
        if (!transformRingIndex.ContainsKey(objeto) || centro == null)
        {
            return objeto.position;
        }

        // Obtenemos el índice del ring
        int ringIndex = transformRingIndex[objeto];

        // Calculamos el radio para ese ring
        float radio = ObtenerRadioDelRing(objeto);

        // Obtenemos la dirección desde el centro hacia el objeto, la normalizamos y 
        // luego la multiplicamos por el radio para obtener la posición en el ring.
        Vector3 direccion = (objeto.position - centro.position).normalized;
        Vector3 posicionRing = centro.position + (direccion * radio);

        return posicionRing;
    }

}