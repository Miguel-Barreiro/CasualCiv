using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.AnyObjectFinder {

    public static class VisualElementExtension {

        public static VisualElement AOF_FixedWidth(this VisualElement elm, float width) {
            elm.style.minWidth = width;
            elm.style.maxWidth = width;
            elm.style.width = width;
            return elm;
        }

        public static VisualElement AOF_FixedHeight(this VisualElement elm, float height) {
            elm.style.minHeight = height;
            elm.style.maxHeight = height;
            elm.style.height = height;
            return elm;
        }

        public static VisualElement AOF_Margin(this VisualElement elm, float margin) {
            return AOF_Margin(elm, margin, margin, margin, margin);
        }

        public static VisualElement AOF_Margin(this VisualElement elm, float top, float right, float bottom, float left) {
            elm.style.marginTop = top;
            elm.style.marginRight = right;
            elm.style.marginBottom = bottom;
            elm.style.marginLeft = left;
            return elm;
        }

        public static VisualElement AOF_Padding(this VisualElement elm, float padding) {
            return AOF_Padding(elm, padding, padding, padding, padding);
        }

        public static VisualElement AOF_Padding(this VisualElement elm, float top, float right, float bottom, float left) {
            elm.style.paddingTop = top;
            elm.style.paddingRight = right;
            elm.style.paddingBottom = bottom;
            elm.style.paddingLeft = left;
            return elm;
        }

        public static VisualElement AOF_BorderRadius(this VisualElement elm, float borderRadius) {
            return AOF_BorderRadius(elm, borderRadius, borderRadius, borderRadius, borderRadius);
        }

        public static VisualElement AOF_BorderRadius(this VisualElement elm, float topRight, float bottomRight, float bottomLeft, float topLeft) {
            elm.style.borderTopRightRadius = topRight;
            elm.style.borderBottomRightRadius = bottomRight;
            elm.style.borderBottomLeftRadius = bottomLeft;
            elm.style.borderTopLeftRadius = topLeft;
            return elm;
        }

        public static VisualElement AOF_BorderWidth(this VisualElement elm, float borderWidth) {
            return AOF_BorderWidth(elm, borderWidth, borderWidth, borderWidth, borderWidth);
        }

        public static VisualElement AOF_BorderWidth(this VisualElement elm, float top, float right, float bottom, float left) {
            elm.style.borderTopWidth = top;
            elm.style.borderRightWidth = right;
            elm.style.borderBottomWidth = bottom;
            elm.style.borderLeftWidth = left;
            return elm;
        }

        public static VisualElement AOF_BackgroundColor(this VisualElement elm, Color backgroundColor) {
            elm.style.backgroundColor = backgroundColor;
            return elm;
        }

        public static VisualElement AOF_Vertical(this VisualElement elm, bool reversed = false) {
            if (reversed) elm.style.flexDirection = FlexDirection.ColumnReverse;
            else elm.style.flexDirection = FlexDirection.Column;
            return elm;
        }

        public static VisualElement AOF_Horizontal(this VisualElement elm, bool reversed = false) {
            if (reversed) elm.style.flexDirection = FlexDirection.RowReverse;
            else elm.style.flexDirection = FlexDirection.Row;
            return elm;
        }

        public static VisualElement AOF_Grow(this VisualElement elm, float flexGrow = 1f) {
            elm.style.flexGrow = flexGrow;
            return elm;
        }

        public static VisualElement AOF_Shrink(this VisualElement elm, float flexShrink = 1f) {
            elm.style.flexShrink = flexShrink;
            return elm;
        }

        public static VisualElement AOF_TextAlignment(this VisualElement elm, TextAnchor textAnchor) {
            elm.style.unityTextAlign = textAnchor;
            return elm;
        }

        public static VisualElement AOF_FontStyle(this VisualElement elm, FontStyle fontStyle) {
            elm.style.unityFontStyleAndWeight = fontStyle;
            return elm;
        }

    }

}