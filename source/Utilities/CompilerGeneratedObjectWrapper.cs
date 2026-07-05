using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;

namespace TownOfUs.Utilities;

/// <summary>
/// A safe wrapper around compiler generated classes like DisplayClass or IEnumerator state machines.
/// </summary>
public class CompilerGeneratedObjectWrapper
{
    /// <summary>
    /// Gets a reference to the compiler generated object.
    /// </summary>
    public object GeneratedObject { get; }

    /// <summary>
    /// Gets the type of the compiler generated object.
    /// </summary>
    protected Type GeneratedType { get; }

    /// <summary>
    /// Gets the property info cache for faster lookups.
    /// </summary>
    protected Dictionary<string, PropertyInfo> PropertyCache { get; }

    /// <summary>
    /// Gets the getter cache for faster property access.
    /// </summary>
    protected Dictionary<string, Delegate> GetterCache { get; }

    /// <summary>
    /// Gets the setter cache for faster property access.
    /// </summary>
    protected Dictionary<string, Delegate> SetterCache { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompilerGeneratedObjectWrapper"/> class.
    /// </summary>
    /// <param name="generatedObject">An instance of the compiler generated object.</param>
    public CompilerGeneratedObjectWrapper(object generatedObject)
    {
        GeneratedObject = generatedObject;
        GeneratedType = generatedObject.GetType();

        PropertyCache = [];
        GetterCache = [];
        SetterCache = [];
    }

    public PropertyInfo CacheProperty<T>(string fieldName)
    {
        var propertyInfo = AccessTools.Property(GeneratedType, fieldName)
                           ?? throw new MissingMemberException(
                               $"Could not find field '{fieldName}' in type '{GeneratedType}'.");

        if (propertyInfo.PropertyType != typeof(T))
        {
            throw new InvalidCastException(
                $"Field '{fieldName}' is of type '{propertyInfo.PropertyType}', not '{typeof(T)}'.");
        }

        PropertyCache[fieldName] = propertyInfo;

        var funcType = typeof(Func<T>);
        GetterCache[fieldName] = propertyInfo.GetMethod!.CreateDelegate(funcType, GeneratedObject);

        var actionType = typeof(Action<T>);
        SetterCache[fieldName] = propertyInfo.SetMethod!.CreateDelegate(actionType, GeneratedObject);

        return propertyInfo;
    }

    /// <summary>
    /// Gets the value of a field in the compiler generated object.
    /// </summary>
    /// <param name="fieldName">The name of the field to get.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <returns>>The value of the field.</returns>
    /// <exception cref="MissingMemberException">Thrown if the field does not exist.</exception>
    public TField GetField<TField>(string fieldName)
    {
        if (!PropertyCache.TryGetValue(fieldName, out var propertyInfo))
        {
            propertyInfo = CacheProperty<TField>(fieldName);
        }

        if (GetterCache.TryGetValue(fieldName, out var getter))
        {
            return ((Func<TField>) getter)();
        }

        return (TField) propertyInfo.GetValue(GeneratedObject)!;
    }

    /// <summary>
    /// Sets the value of a field in the compiler generated object.
    /// </summary>
    /// <param name="fieldName">The name of the field to set.</param>
    /// <param name="value">The value to set.</param>
    /// <typeparam name="TField">The type of the field.</typeparam>
    /// <exception cref="MissingMemberException">Thrown if the field does not exist.</exception>
    public void SetField<TField>(string fieldName, TField value)
    {
        if (!PropertyCache.TryGetValue(fieldName, out var propertyInfo))
        {
            propertyInfo = CacheProperty<TField>(fieldName);
        }

        if (SetterCache.TryGetValue(fieldName, out var setter))
        {
            ((Action<TField>) setter)(value);
            return;
        }

        propertyInfo.SetValue(GeneratedObject, value);
    }
}

