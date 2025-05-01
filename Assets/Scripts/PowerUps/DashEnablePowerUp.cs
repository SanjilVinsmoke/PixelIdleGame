using UnityEngine;

public class DashEnablePowerUp : IPowerUp
{
    public void Apply(Player player)
    {
        DashComponent dashComponent = player.GetComponent<DashComponent>();
        if (dashComponent != null)
        {
            // Assuming DashComponent has a method or property to enable/disable dashing
            // For example: dashComponent.SetDashEnabled(true);
            // Or maybe it controls the number of charges: dashComponent.AddCharge();
            Debug.Log("Dash Enabled/Enhanced PowerUp Applied!");
            // Replace Debug.Log with actual logic, e.g.:
            // dashComponent.EnableDashFeature(); // Hypothetical method
        }
        else
        {
            Debug.LogWarning("DashComponent not found on player.", player);
        }
    }
}