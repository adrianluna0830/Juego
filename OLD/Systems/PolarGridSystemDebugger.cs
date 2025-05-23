using System;
using UnityEngine;

public class PolarGridSystemDebugger : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] private Transform debugTransform;
    [SerializeField] private float ringStep;
    [SerializeField] private float sectorStep;
    [SerializeField] private int ringCount;
    [SerializeField] private float yValue = 1;
    
    private PolarGridSystem polarGridSystem;

    private void Awake()
    {
        polarGridSystem = new PolarGridSystem(ringStep, sectorStep,player.transform);
    }

    private void Update()
    {
        DebugTile();
        DebugCordinates(player.position);
    }
    

    private void DebugTile()
    {
        var position = polarGridSystem.GetTilePosition(polarGridSystem.GetTileCoordinate(debugTransform.position));
        var center = new Vector3(position.x, yValue, position.z);
        var (foward,right,up) = polarGridSystem.GetTileRelativeCoordinateSystem(center);

        //
        var backPos = polarGridSystem.GetTilePosition(polarGridSystem.GetBackwardNeighbour(position));
        var backCenter = new Vector3(backPos.x, yValue, backPos.z);
        var (fowardBack,rightBack,upBack) = polarGridSystem.GetTileRelativeCoordinateSystem(backPos);

        
        DebugCordinates(center, up, foward, right);
        DebugCordinates(backPos, upBack, fowardBack, rightBack);

    }

    private static void DebugCordinates(Vector3 center, Vector3 up, Vector3 foward, Vector3 right)
    {
        DebugExtension.DebugArrow(center,up,Color.green);
        DebugExtension.DebugArrow(center,foward,Color.blue);
        DebugExtension.DebugArrow(center,right,Color.red);
    }

    private void DebugCordinates(Vector3 origin)
    {
        for (int i = 0; i < ringCount; i++)
        {
            float startRadius = i * ringStep;
            float endRadius = (i + 1) * ringStep;
            float avgRadius = (startRadius + endRadius) / 2;

            DebugExtension.DebugCircle(origin + new Vector3(0, 1, 0), Color.red, endRadius, 0.01f);
            DebugExtension.DebugCircle(origin + new Vector3(0, 1, 0), Color.red, startRadius, 0.01f);

            if (sectorStep <= 0)
            {
                Debug.LogError("sectorStep must be greater than 0");
                return;
            }

            float circumference = 2 * Mathf.PI * avgRadius;
            int sectors = Mathf.Max(4, Mathf.RoundToInt(circumference / sectorStep));

            float angleStep = 360f / sectors;
            float currentAngle = 0;

            for (int j = 0; j < sectors; j++)
            {
                Vector3 direction = new Vector3(
                    Mathf.Sin(Mathf.Deg2Rad * currentAngle),
                    0,
                    Mathf.Cos(Mathf.Deg2Rad * currentAngle)
                );
                Vector3 start = origin + new Vector3(0, yValue, 0) + direction * startRadius;
                Vector3 end = origin + new Vector3(0, yValue, 0) + direction * endRadius;

                Debug.DrawLine(start, end, Color.red);
                currentAngle += angleStep;
            }
        }
    }
}