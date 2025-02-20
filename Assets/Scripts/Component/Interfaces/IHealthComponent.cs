namespace Component.Interfaces
{
    public interface IHealthComponent
    {
        public float Health { get; set; }
        public void TakeDamage(float damage);
    }
}