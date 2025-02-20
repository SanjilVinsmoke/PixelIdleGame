namespace Component.Interfaces
{
    public interface IBaseAttackComponent
    {
            bool CanAttack { get; }
            void Attack();

    }
}