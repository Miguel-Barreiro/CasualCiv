using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.AnyObjectFinder {

    public static class DrawingUtils {

        public static Button CreateSearchButton(System.Action onClick) {
            Button searchButton = new Button(onClick);
            searchButton.text = "";

            Texture2D searchIcon = EditorGUIUtility.FindTexture("d_Search Icon");

            Image icon = new Image() {
                image = searchIcon,
                scaleMode = ScaleMode.ScaleToFit,
                style =
                {
                width = 14,
                height = 14,
                //marginLeft = 2,
                marginTop = 2,
                marginRight = 2,
                marginBottom = 2,
                alignSelf = Align.Center
            }
            };

            VisualElement container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.alignItems = Align.Center;

            container.Add(icon);

            var label = new Label("Search");
            label.style.fontSize = 11f;
            container.Add(label);

            searchButton.Add(container);
            return searchButton;
        }

    }

}
