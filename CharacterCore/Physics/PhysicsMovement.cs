using UnityEngine;

public class PhysicsMovement : MonoBehaviour
{
    
    [Range(0,1)]
    [SerializeField] private float momentumBonus;

    [SerializeField] private CharacterController characterController;
    
    public Vector3 _velocity = Vector3.zero;
    private float drag;
    private float gravity;
    private void Awake()
    
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        ApplyGravity(ref _velocity,gravity);
        ApplyVelocityDrag(ref _velocity,drag);
        Move(_velocity);
    }
    

    private void ApplyVelocityDrag(ref Vector3 vel, float drag)
    {
        vel *= Mathf.Pow(1f - drag, Time.deltaTime);
        if (vel.magnitude < 0.1f)
        {
            vel = Vector3.zero;
        }
    }
    
    private void Move(Vector3 velocity)
    {
        characterController.Move(velocity * Time.deltaTime);
    }


    public Transform CharacterTransform => transform;

    public Vector3 GetVelocity()
    {
        return _velocity;
    }

    public void SetVelocity(Vector3 velocity)
    {
        _velocity = velocity;
    }

    public void SetDrag(float drag)
    {
        this.drag = drag;
    }

    public void ApplyForce(Vector3 force, float speedLimit = 0)
    {
        float k = MomentumFactor(_velocity, force);

        _velocity += force * (Mathf.Min(1,k + momentumBonus) * Time.deltaTime);

        if (speedLimit != 0)
        {
            LimitHorizontalSpeed(ref _velocity, speedLimit);
        }
        
        DebugExtension.DebugArrow(transform.position + new Vector3(0,1,0),_velocity.normalized,Color.black);
    }

    private void ApplyGravity(ref Vector3 velocity, float gravity)
    {
        _velocity.y += gravity * Time.deltaTime;
    }

    public void SetGravity(float gravity)
    {
        this.gravity = gravity;
    }
    
    private float MomentumFactor(Vector3 velocity, Vector3 forceDir)
    {
        if (velocity.sqrMagnitude < 1) return 1f;

        float dot = Vector3.Dot(velocity.normalized, forceDir.normalized);
        return Mathf.Max(dot, 0f);       
    }

    private void LimitHorizontalSpeed(ref Vector3 vel, float _speedLimit)
    {
        var flatVel = new Vector3(vel.x, 0, vel.z);
        if (flatVel.magnitude > _speedLimit)
        {
            var limitedVel = flatVel.normalized * _speedLimit;
            vel = new Vector3(limitedVel.x, vel.y, limitedVel.z);
        }
    }
    
    [SerializeField] private float velocitySmoothTime = 0.15f; // Ajusta este valor según tus necesidades

    private Vector3 _targetVelocity = Vector3.zero; // La velocidad objetivo antes del suavizado
    private Vector3 _smoothDampVelocity; // Variable de referencia para SmoothDamp
    
    public void ApplySmoothForce(Vector3 force, float speedLimit = 0, float customSmoothTime = -1)
    {
        float actualSmoothTime = customSmoothTime > 0 ? customSmoothTime : velocitySmoothTime;

        // Calculamos la nueva velocidad objetivo sumando la fuerza al vector velocidad actual
        Vector3 targetVelocity = _velocity + force * Time.deltaTime;

        // Suavizamos la velocidad actual hacia la velocidad objetivo
        _velocity = Vector3.SmoothDamp(
            _velocity,         // current: velocidad actual
            targetVelocity,    // target: velocidad objetivo
            ref _smoothDampVelocity,  // referencia para la velocidad de suavizado interna
            actualSmoothTime
        );

        // Limitar la velocidad horizontal si es necesario
        if (speedLimit != 0)
        {
            LimitHorizontalSpeed(ref _velocity, speedLimit);
        }

        _targetVelocity = _velocity;

        DebugExtension.DebugArrow(transform.position + new Vector3(0,1,0), _velocity, Color.black);
    }
}


