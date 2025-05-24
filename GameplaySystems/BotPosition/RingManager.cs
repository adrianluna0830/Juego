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

    private Dictionary<Transform, int> transformRingIndex = new Dictionary<Transform, int>();
    private List<Transform> todosLosObjetos = new List<Transform>();

    public void AgregarTransform(Transform objeto)
    {
        if (!todosLosObjetos.Contains(objeto))
        {
            todosLosObjetos.Add(objeto);
        }
    }

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

    public void Update()
    {
        for (int i = 0; i < 10; i++)
        {
            DebugExtension.DebugCircle(centro.transform.position + new Vector3(0, 1, 0), Color.red, distanciaInicialDelPrimerRing + (i * separacionEntreRings));
        }

        if (centro != null)
        {
            todosLosObjetos.Sort((t1, t2) =>
            {
                float dist1 = Vector3.Distance(t1.position, centro.position);
                float dist2 = Vector3.Distance(t2.position, centro.position);
                return dist1.CompareTo(dist2);
            });
        }

        transformRingIndex.Clear();
        int indiceGlobal = 0;
        int ringActual = 0;
        int numObjetos = todosLosObjetos.Count;

        while (indiceGlobal < numObjetos)
        {
            int capacidadRing = slotsInicialPorRing + (ringActual * slotStepPorRing);
            int limite = Mathf.Min(indiceGlobal + capacidadRing, numObjetos);
            for (int i = indiceGlobal; i < limite; i++)
            {
                transformRingIndex[todosLosObjetos[i]] = ringActual;
            }
            indiceGlobal += capacidadRing;
            ringActual++;
        }
    }

    public float ObtenerRadioDelRing(Transform objeto)
    {
        if (!transformRingIndex.ContainsKey(objeto))
        {
            return 0f;
        }

        int ringIndex = transformRingIndex[objeto];
        return distanciaInicialDelPrimerRing + (ringIndex * separacionEntreRings);
    }

    public Vector3 ObtenerPosicionEnRing(Transform objeto)
    {
        if (!transformRingIndex.ContainsKey(objeto) || centro == null)
        {
            return objeto.position;
        }

        int ringIndex = transformRingIndex[objeto];
        float radio = ObtenerRadioDelRing(objeto);
        Vector3 direccion = (objeto.position - centro.position).normalized;
        Vector3 posicionRing = centro.position + (direccion * radio);

        return posicionRing;
    }
}