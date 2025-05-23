using System;

public interface IHitStatus
{
    public bool IsHitting { get; }
    public event Action OnHitStartEvent;
    public event Action OnHitEndEvent;

}