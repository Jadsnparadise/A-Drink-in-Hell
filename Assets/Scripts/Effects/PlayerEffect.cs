namespace Effects
{
    public abstract class PlayerEffect
    {
        public float Duration { get; protected set; }
        public virtual bool IsStackable => false;
        
        protected PlayerEffect(float duration)
        {
            Duration = duration;
        }

        public abstract void Apply(PlayerController controller);

        public abstract void Remove(PlayerController controller);
    }
}