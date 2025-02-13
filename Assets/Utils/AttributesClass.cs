namespace Utils
{
    using UnityEngine;
    using TMPro;

// Attribute to specify the color for state debug visualization
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class StateDebugColorAttribute : System.Attribute
    {
        public Color Color { get; }
    
        // Constructor accepting Unity's built-in colors
        public StateDebugColorAttribute(UnityColor color)
        {
            Color = GetUnityColor(color);
        }
    
        // Helper enum for common colors
        public enum UnityColor
        {
            Red,
            Green,
            Blue,
            Yellow,
            Cyan,
            Magenta,
            White,
            Black,
            Gray,
            Orange,
            Purple
        }
    
        private Color GetUnityColor(UnityColor color)
        {
            return color switch
            {
                UnityColor.Red => Color.red,
                UnityColor.Green => Color.green,
                UnityColor.Blue => Color.blue,
                UnityColor.Yellow => Color.yellow,
                UnityColor.Cyan => Color.cyan,
                UnityColor.Magenta => Color.magenta,
                UnityColor.White => Color.white,
                UnityColor.Black => Color.black,
                UnityColor.Gray => Color.gray,
                UnityColor.Orange => new Color(1f, 0.5f, 0f),
                UnityColor.Purple => new Color(0.5f, 0f, 0.5f),
                _ => Color.white
            };
        }
    }

    // Attribute to specify the description for state debug visualization
    
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class StateDescriptionAttribute : System.Attribute
    {
        public string description { get; }
        public StateDescriptionAttribute(string description)
        {
            this.description = description;
        }
    }
    
    
    
}