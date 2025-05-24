namespace DefaultNamespace
{
    public interface IHitProcessor
    {
        public bool HitCanBeProcessed();
        public HitContext ProccesssHit(HitContext context);

    }
}