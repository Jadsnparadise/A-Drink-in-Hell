namespace Enemies.DemonFrog
{
    public class DemonFrogController : EnemyController
    {
        private void Awake()
        {
            base.Awake();
            SpriteRenderer.flipX = true;
        }
    }
}