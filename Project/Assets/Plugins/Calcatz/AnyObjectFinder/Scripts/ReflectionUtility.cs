using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Calcatz.AnyObjectFinder {

    public static class ReflectionUtility {

        public static FieldInfo[] GetFieldInfosIncludingBaseClasses(System.Type type, BindingFlags bindingFlags) {
            FieldInfo[] fieldInfos = type.GetFields(bindingFlags);

            if (type.BaseType == typeof(object)) { // class doesn't have a base
                return fieldInfos;
            }
            else {   // collect all types up to the basest class
                var currentType = type;
                var fieldComparer = new FieldInfoComparer();
                var fieldInfoList = new HashSet<FieldInfo>(fieldInfos, fieldComparer);
                while (currentType != typeof(object)) {
                    fieldInfos = currentType.GetFields(bindingFlags);
                    fieldInfoList.UnionWith(fieldInfos);
                    currentType = currentType.BaseType;
                }
                return fieldInfoList.ToArray();
            }
        }

        public static PropertyInfo[] GetPropertyInfosIncludingBaseClasses(System.Type type, BindingFlags bindingFlags) {
            PropertyInfo[] propertyInfos = type.GetProperties(bindingFlags);

            if (type.BaseType == typeof(object)) {
                return propertyInfos;
            }
            else {
                var currentType = type;
                var propertyComparer = new PropertyInfoComparer();
                var propertyInfoList = new HashSet<PropertyInfo>(propertyInfos, propertyComparer);
                while (currentType != typeof(object)) {
                    propertyInfos = currentType.GetProperties(bindingFlags);
                    propertyInfoList.UnionWith(propertyInfos);
                    currentType = currentType.BaseType;
                }
                return propertyInfoList.ToArray();
            }
        }

        public static bool IsExternProperty(PropertyInfo propertyInfo) {
            if (propertyInfo == null) {
                //throw new ArgumentNullException(nameof(propertyInfo));
                return false;
            }

            MethodInfo getMethod = propertyInfo.GetGetMethod();
            MethodInfo setMethod = propertyInfo.GetSetMethod();

            return (getMethod != null && IsExternMethod(getMethod)) ||
                   (setMethod != null && IsExternMethod(setMethod));
        }

        public static bool IsExternGetterProperty(PropertyInfo propertyInfo) {
            if (propertyInfo == null) {
                //throw new ArgumentNullException(nameof(propertyInfo));
                return false;
            }

            MethodInfo getMethod = propertyInfo.GetGetMethod();
            return (getMethod != null && IsExternMethod(getMethod));
        }

        private static bool IsExternMethod(MethodInfo method) {
            MethodImplAttributes methodImplAttributes = method.GetMethodImplementationFlags();
            return (methodImplAttributes & MethodImplAttributes.InternalCall) == MethodImplAttributes.InternalCall;
        }


        public class FieldInfoComparer : IEqualityComparer<FieldInfo> {
            public bool Equals(FieldInfo x, FieldInfo y) {
                return x.DeclaringType == y.DeclaringType && x.Name == y.Name;
            }

            public int GetHashCode(FieldInfo obj) {
                return obj.Name.GetHashCode() ^ obj.DeclaringType.GetHashCode();
            }
        }

        public class PropertyInfoComparer : IEqualityComparer<PropertyInfo> {
            public bool Equals(PropertyInfo x, PropertyInfo y) {
                return x.DeclaringType == y.DeclaringType && x.Name == y.Name;
            }

            public int GetHashCode(PropertyInfo obj) {
                return obj.Name.GetHashCode() ^ obj.DeclaringType.GetHashCode();
            }
        }

        public static object GetEnumValue(SerializedProperty property) {
            if (property.propertyType != SerializedPropertyType.Enum) {
                //throw new System.ArgumentException("SerializedProperty must be an enum type.");
                return null;
            }

            System.Type targetType = property.serializedObject.targetObject.GetType();
            var field = targetType.GetField(property.propertyPath, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static);
            if (field == null) {
                //throw new System.ArgumentException($"Field '{property.propertyPath}' not found in {targetType}");
                return null;
            }

            System.Type enumType = field.FieldType;

            // Check if the enum has the [Flags] attribute
            bool isFlagsEnum = System.Attribute.IsDefined(enumType, typeof(System.FlagsAttribute));

            if (isFlagsEnum) {
                // Use enumValueFlag for [Flags] enums
                int enumValueFlag = property.intValue;
                return System.Enum.ToObject(enumType, enumValueFlag);
            }
            else {
                // Use enumValueIndex for regular enums
                int enumValueIndex = property.enumValueIndex;
                System.Array enumValues = System.Enum.GetValues(enumType);
                if (enumValueIndex < 0 || enumValueIndex >= enumValues.Length) {
                    //throw new System.IndexOutOfRangeException($"Enum index {enumValueIndex} is out of range for {enumType}");
                    return null;
                }

                return enumValues.GetValue(enumValueIndex);
            }
        }

    }

}