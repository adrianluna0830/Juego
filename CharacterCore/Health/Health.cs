using System.Collections;
using DefaultNamespace;
using Unity.Cinemachine;
using UnityEngine;

public class Health : MonoBehaviour, IHitProcessor, IHitReact
{
    [Header("Health Configuration")] [SerializeField]
    private int maxHits = 5;

    [SerializeField] private float regenerationInterval = 3f; // Tiempo entre cada ciclo de regeneración.

    [SerializeField]
    private float waitBeforeRegeneration = 2f; // Tiempo que espera antes de empezar a regenerarse tras recibir daño.

    [SerializeField] public int _currentHits;
    private Coroutine _regenerationCoroutine;
    private Coroutine _damageCooldownCoroutine;
    private bool _isRegenerating = false;
    [SerializeField] private CinemachineImpulseSource cinemachineImpulseSource;

    public bool IsDead { get; private set; } = false;

    public event System.Action OnDamaged;
    public event System.Action OnDeath;
    public event System.Action OnFullHealth;

    private void Awake()
    {
        _currentHits = maxHits;
    }

    private void OnDisable()
    {
        StopRegeneration();
    }

    public void DoDamage()
    {
        if (IsDead) return; // Si está muerto, no procesar el daño.
        cinemachineImpulseSource.GenerateImpulse();
        _currentHits--;
        OnDamaged?.Invoke();

        // Si la vida está entre 1 y el máximo, detener regeneración y reiniciar lógica.
        if (_currentHits > 0 && _currentHits < maxHits)
        {
            StopRegeneration(); // Detener la regeneración actual.
            StopDamageCooldown(); // Detener cualquier cooldown en curso.

            // Iniciar un nuevo cooldown que activará la regeneración después del tiempo configurado.
            _damageCooldownCoroutine = StartCoroutine(RestartRegenerationAfterDelay());
        }

        if (_currentHits <= 0)
        {
            _currentHits = 0;
            IsDead = true;
            OnDeath?.Invoke();
            StopRegeneration(); // Detener regeneración al morir.
        }
    }

    public void Heal(int amount = 1)
    {
        if (IsDead) return;

        int previousHits = _currentHits;
        _currentHits = Mathf.Min(_currentHits + amount, maxHits);

        // Si alcanzamos la vida máxima, detener regeneración
        if (_currentHits == maxHits)
        {
            OnFullHealth?.Invoke();
            StopRegeneration();
        }
    }

    public void StartRegeneration()
    {
        // Si ya está regenerando, no hacer nada.
        if (_isRegenerating) return;

        // Si tiene la vida completa, no iniciar regeneración.
        if (_currentHits >= maxHits) return;

        _isRegenerating = true;
        StopRegeneration(); // Por seguridad, detener cualquier corutina existente.

        _regenerationCoroutine = StartCoroutine(RegenerateHealthRoutine());
    }

    public void StopRegeneration()
    {
        if (_regenerationCoroutine != null)
        {
            StopCoroutine(_regenerationCoroutine);
            _regenerationCoroutine = null;
        }

        _isRegenerating = false;
    }

    private IEnumerator RegenerateHealthRoutine()
    {
        WaitForSeconds waitTime = new WaitForSeconds(regenerationInterval);

        while (enabled && _currentHits < maxHits && !IsDead)
        {
            yield return waitTime;

            if (_currentHits < maxHits && !IsDead)
            {
                Heal(1);

                // Si después de sanar alcanzamos vida completa, detenemos la corutina.
                if (_currentHits >= maxHits)
                {
                    break;
                }
            }
        }

        // Al salir del bucle, marcar que ya no está regenerando.
        _isRegenerating = false;
        _regenerationCoroutine = null;
    }

    private IEnumerator RestartRegenerationAfterDelay()
    {
        // Esperar el tiempo configurado antes de empezar a regenerar.
        yield return new WaitForSeconds(waitBeforeRegeneration);

        // Solo reiniciar regeneración si no está muerto y no tiene vida completa.
        if (!IsDead && _currentHits < maxHits)
        {
            StartRegeneration();
        }
    }

    private void StopDamageCooldown()
    {
        if (_damageCooldownCoroutine != null)
        {
            StopCoroutine(_damageCooldownCoroutine);
            _damageCooldownCoroutine = null;
        }
    }

    public IHitReact HitReact => this;

    public bool HitCanBeProcessed()
    {
        if (IsDead) return false;
        return true;
    }

    public HitContext ProccesssHit(HitContext context)
    {
        return context;
    }

    public void ReactToHit(HitContext proccessedHit)
    {
        DoDamage(); // Procesa el daño cuando se recibe un impacto.
    }
}