namespace Effects.Healing
{
    public class HealingEffect : PlayerEffect
    {
        private readonly int _amount;
        
        public HealingEffect(float duration, int amount) : base(duration)
        {
            _amount = amount;
        }

        public override void Apply(PlayerController controller)
        {
            GameManager.Instance.HealPlayer(_amount);
        }

        public override void Remove(PlayerController controller)
        {
            // Nothing to remove
        }
    }
}