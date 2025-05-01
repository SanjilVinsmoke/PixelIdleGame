using System;
using UnityEngine;

namespace Utils
{
    // Attribute to specify the color for state debug visualization
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class StateDebugColorAttribute : System.Attribute
    {
        public Color Color { get; }
        public UnityColor ColorType { get; private set; }

        // Constructor accepting our enum color type
        public StateDebugColorAttribute(UnityColor color)
        {
            ColorType = color;  // Store the enum value
            Color = GetUnityColor(color);  // Convert to actual Color
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
        public string Description { get; }

        public StateDescriptionAttribute(string description)
        {
            this.Description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class AutoRequireAttribute : Attribute
    {
    }
}