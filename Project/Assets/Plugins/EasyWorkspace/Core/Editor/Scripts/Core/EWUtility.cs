using System;
using System.Linq;
using System.Reflection;

public static class EWUtility
{
    public static object GetFieldValue(this object obj, string fieldName)
    {
        if (obj.GetType().GetField(fieldName, (BindingFlags)62) is FieldInfo fieldInfo)
            return fieldInfo.GetValue(obj);
        
        throw new Exception($"Field {fieldName} not found in {obj.GetType().Name}");
    }
    
    public static T GetFieldValue<T>(this object obj, string fieldName)
    {
        return (T) obj.GetFieldValue(fieldName);
    }
    
    public static object GetPropertyValue(this object obj, string propertyName)
    {
        if (obj.GetType().GetProperty(propertyName, (BindingFlags)62) is PropertyInfo propertyInfo)
            return propertyInfo.GetValue(obj);
        
        throw new Exception($"Property {propertyName} not found in {obj.GetType().Name}");
    }
    
    public static T GetPropertyValue<T>(this object obj, string propertyName)
    {
        return (T) obj.GetPropertyValue(propertyName);
    }

    public static void InvokeMethod(this object obj, string methodName, params object[] parameters)
    {
        for (Type currentType = obj.GetType(); currentType != null; currentType = currentType.BaseType)
        {
            if (currentType.GetMethod(methodName, (BindingFlags)62, null, parameters.Select(p => p.GetType()).ToArray(), null) is MethodInfo methodInfo)
            {
                methodInfo.Invoke(obj, parameters);
                return;
            }
        }

        throw new Exception($"Method {methodName} not found in {obj.GetType().Name}");
    }
}