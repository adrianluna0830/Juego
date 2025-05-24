using UnityEngine;

public class PlayerCameraRotation : MonoBehaviour
{
    [Header("Sensibilidad")]
    [SerializeField] private float sensitivity;

    [Header("Límites de rotación vertical (X)")]
    [SerializeField] private float upperLookLimit = -80f; 
    [SerializeField] private float lowerLookLimit = 80f;  

    float cameraX;
    float cameraY;

    public Vector2 cameraRotationDegree => new Vector2(cameraX, cameraY);

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void RotateWithInput(Vector2 input)
    {
        Vector2 rotation = input * sensitivity * Time.deltaTime;

        cameraX -= rotation.y;
        cameraX = Mathf.Clamp(cameraX, upperLookLimit, lowerLookLimit);

        cameraY += rotation.x;

        RotateCamera(new Vector2(cameraX, cameraY));
    }

    public void RotateCamera(Vector2 rotation)
    {
        transform.localRotation = Quaternion.Euler(rotation.x, rotation.y, 0f);
    }
}
