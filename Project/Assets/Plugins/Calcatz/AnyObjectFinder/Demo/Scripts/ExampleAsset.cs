using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.AnyObjectFinder {

    //[CreateAssetMenu(fileName = "Example Asset", menuName = "Any Object Finder/Example Asset", order = 0)]
    public class ExampleAsset : ScriptableObject {

        public enum EnumExample {
            EnumVal1, EnumVal2, EnumVal3
        }

        public string stringVal;
        public int intVal;
        public float floatVal;
        public bool boolVal;
        public EnumExample enumVal;
        public Texture2D texture2DRef;
        public AudioClip audioClipRef;
        public NestedExample nestedExample;
        public Vector3 structExample;
        public string[] stringArray;

        [System.Serializable]
        public class NestedExample {
            public string string2Val;
            public int int2Val;
            public float float2Val;
        }

    }

}
