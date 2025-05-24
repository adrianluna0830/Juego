public class EnemyDeathState : BaseState
{
    public OnDeath OnDeath;
    public EnemyDeathState(OnDeath onDeath)
    {
        OnDeath = onDeath;
    }

    public override void OnEnter()
    {
        OnDeath.PlayDeathAnimation();
        OnDeath.PlayDeathSound();
        
        OnDeath.Destroy(OnDeath.gameObject,3);
    }
}