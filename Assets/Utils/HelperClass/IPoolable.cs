

// Interface for objects that need to perform actions when spawned/despawned
public interface IPoolable
{
    void OnSpawn();
    void OnDespawn();
}