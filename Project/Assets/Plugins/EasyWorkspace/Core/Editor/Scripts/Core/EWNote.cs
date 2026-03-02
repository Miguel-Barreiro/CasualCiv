using System;
using UnityEditor;
using UnityEngine;

namespace EasyWorkspace
{
    [EWCustom("Note", 200f, 200f)]
    [EWResizable(100f, 100f, 700f, 700f)]
    public class EWNote : EWCustom
    {
        [SerializeField] private string _text;
        [SerializeField] private int _fontSize = 16;

        private static GUIStyle _style;
        private static GUIStyle Style
        {
            get
            {
                if (_style == null || _style.normal.background == null)
                {
                    _style = new GUIStyle(GUI.skin.textArea) { wordWrap = true, font = EWContainer.Instance.Font };

                    Texture2D texture = new(1, 1);
                    texture.SetPixel(0, 0, UnityEngine.Color.clear);
                    texture.Apply();

                    _style.normal.background = texture;
                    _style.normal.scaledBackgrounds = Array.Empty<Texture2D>();
                }

                return _style;
            }
        }

        protected override void DrawGUI()
        {
            GUIStyle style = new(Style) { fontSize = _fontSize };
            string newText = EditorGUILayout.TextArea(_text, style, GUILayout.ExpandHeight(true));
            if (newText != _text)
            {
                Undo.RecordObject(this, "Change Note Text");

                _text = newText;
            }
        }

        protected override void DrawInspectorGUI()
        {
            EditorGUILayout.LabelField("Font Size");
            _fontSize = EditorGUILayout.IntSlider("", _fontSize, 8, 48);
        }

        protected override string GetCollapseInfo()
        {
            return _text;
        }
    }
}