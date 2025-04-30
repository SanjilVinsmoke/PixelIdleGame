using UnityEngine;

public static class PowerUpFactory
{
    public static IPowerUp CreatePowerUp(PowerUpType type)
    {
        switch (type)
        {
            case PowerUpType.EnableDash:
                return new DashEnablePowerUp();
            case PowerUpType.EnableDoubleJump:
                return new DoubleJumpEnablePowerUp();
            case PowerUpType.IncreaseHealth:
                // You could potentially pass parameters from the factory if needed
                return new HealthIncreasePowerUp(25); // Example fixed amount
            case PowerUpType.None:
            default:
                Debug.LogWarning($"PowerUp type '{type}' not recognized or is None.");
                return null;
        }
    }
}