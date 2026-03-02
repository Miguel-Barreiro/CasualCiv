using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Calcatz.AnyObjectFinder {

    public class ExampleComponent : MonoBehaviour {
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
        public string[] stringArray;
        public ExampleAsset exampleAsset;

        [System.Serializable]
        public class NestedExample {
            public string string2Val;
            public int int2Val;
            public float float2Val;
        }

    }

}
