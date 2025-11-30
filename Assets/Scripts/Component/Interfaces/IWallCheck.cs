namespace Component.Interfaces
{
    /// <summary>
    /// Interface for wall detection functionality.
    /// Allows components to check for wall collisions in different directions.
    /// </summary>
    public interface IWallCheck
    {
        /// <summary>
        /// Returns true if touching a wall on the right side.
        /// </summary>
        bool IsTouchingWallRight { get; }
        
        /// <summary>
        /// Returns true if touching a wall on the left side.
        /// </summary>
        bool IsTouchingWallLeft { get; }
        
        /// <summary>
        /// Returns true if touching any wall (left or right).
        /// </summary>
        bool IsTouchingAnyWall { get; }
        
        /// <summary>
        /// Checks if there's a wall in the specified movement direction.
        /// </summary>
        /// <param name="moveDirection">Horizontal movement direction (-1 for left, +1 for right, 0 for none)</param>
        /// <returns>True if there's a wall in that direction, false otherwise</returns>
        bool IsWallInDirection(float moveDirection);
    }
}

