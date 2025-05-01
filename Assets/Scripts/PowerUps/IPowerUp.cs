// Defines the common behavior for all power-ups
public interface IPowerUp
{
    /// <summary>
    /// Applies the power-up effect to the player.
    /// </summary>
    /// <param name="player">The player instance to apply the effect to.</param>
    void Apply(Player player);
}
public enum PowerUpType
{
    None,
    EnableDash,
    EnableDoubleJump,
    IncreaseHealth
    // Add other power-up types here
}