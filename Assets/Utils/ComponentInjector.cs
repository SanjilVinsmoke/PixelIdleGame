using System;
using System.Reflection;
using UnityEngine;
using Utils;


// Custom attribute to automatically require components
public static class ComponentInjector
{
    public static void InjectComponents(MonoBehaviour mono)
    {
        FieldInfo[] fields = mono.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        foreach (FieldInfo field in fields)
        {
            if (Attribute.IsDefined(field, typeof(AutoRequireAttribute)))
            {
                System.Type fieldType = field.FieldType;
                UnityEngine.Component comp = mono.GetComponent(fieldType);
                if (comp == null)
                {
                    comp = mono.gameObject.AddComponent(fieldType);
                    Debug.Log($"[AutoRequire] Added missing component {fieldType.Name} to {mono.name}");
                }
                field.SetValue(mono, comp);
            }
        }
    }
}