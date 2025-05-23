using UnityEngine;

public class PhysicsMovementNoMono
{
    private readonly CharacterController characterController;

    public PhysicsMovementNoMono(CharacterController characterController)
    {
        if (characterController == null)
        {
            throw new System.ArgumentNullException(nameof(characterController),
                "CharacterController cannot be null.");
        }

        this.characterController = characterController;
    }

    private float momentumBonus = 0.1f;

    // Velocidad actual (expuesta)
    public Vector3 Velocity { get; private set; } = Vector3.zero;

    // Propiedades para ajustar parámetros físicos
    public float Drag { get; set; } = 0;
    public float Gravity { get; set; } = 0;

    /// <summary>
    /// Llama a este método desde un MonoBehaviour con el ciclo de vida `Update`.
    /// </summary>
    public void Tick()
    {
        // Aplicamos gravedad y drag a la velocidad
        Velocity = ApplyGravity(Velocity, Gravity);
        Velocity = ApplyVelocityDrag(Velocity, Drag);

        // Movemos el objeto usando el CharacterController
        Move(Velocity);
    }

    /// <summary>
    /// Aplica fuerza al movimiento.
    /// </summary>
    public void ApplyForce(Vector3 force, float speedLimit)
    {
        if (force == Vector3.zero) return;

        // Ajustamos el impulso con base en la dirección y la fuerza
        float k = MomentumFactor(Velocity, force);

        // Aumentamos la velocidad respetando el impulso
        Velocity += force * (Mathf.Min(1, k + momentumBonus) * Time.deltaTime);

        // Limitar la velocidad horizontal
        Velocity = LimitHorizontalSpeed(Velocity, speedLimit);
    }

    /// <summary>
    /// Aplica el efecto de gravedad.
    /// </summary>
    private Vector3 ApplyGravity( Vector3 velocity, float gravity)
    {
        velocity.y += gravity * Time.deltaTime;
        return velocity;
    }

    /// <summary>
    /// Aplica drag o "arrastre" para reducir la velocidad progresivamente.
    /// </summary>
    private Vector3 ApplyVelocityDrag( Vector3 vel, float drag)
    {
        vel *= Mathf.Pow(1f - drag, Time.deltaTime); // Drag disminuye la velocidad
        return vel;
    }

    /// <summary>
    /// Limita la velocidad horizontal según un límite establecido.
    /// </summary>
    private Vector3 LimitHorizontalSpeed( Vector3 vel, float speedLimit)
    {
        var flatVel = new Vector3(vel.x, 0f, vel.z);
        if (flatVel.magnitude > speedLimit)
        {
            var limitedVel = flatVel.normalized * speedLimit;
            vel = new Vector3(limitedVel.x, vel.y, limitedVel.z);
        }
        return vel;

    }

    /// <summary>
    /// Mueve el CharacterController usando la velocidad actual.
    /// </summary>
    private void Move(Vector3 velocity)
    {
        // Mover al objeto usando el CharacterController
        characterController.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Calcula el factor de impulso o momentum en base a la dirección actual del movimiento.
    /// </summary>
    private float MomentumFactor(Vector3 velocity, Vector3 forceDir)
    {
        // Si el objeto está casi detenido, devolvemos un impulso fijo
        if (velocity.sqrMagnitude < 1) return 1f;

        // Calculamos cuánto afecta la fuerza en base al ángulo (dirección)
        float dot = Vector3.Dot(velocity.normalized, forceDir.normalized);
        return Mathf.Max(dot, 0f);
    }
}