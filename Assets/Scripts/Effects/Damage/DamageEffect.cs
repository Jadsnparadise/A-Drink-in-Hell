namespace Effects.Damage
{
    public class DamageEffect : PlayerEffect
    {
        private readonly int _damage;
        
        public DamageEffect(float duration, int damage) : base(duration)
        {
            _damage = damage;
        }

        public override void Apply(PlayerController controller)
        {
            GameManager.Instance.DamagePlayer(_damage);
        }
    }
}