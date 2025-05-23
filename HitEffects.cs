using DefaultNamespace;
using UnityEngine;

public class HitEffects : IHitProcessor
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IHitReact HitReact { get; }
    public bool HitCanBeProcessed()
    {
        throw new System.NotImplementedException();
    }

    public HitContext ProccesssHit(HitContext context)
    {
        throw new System.NotImplementedException();
    }
}
