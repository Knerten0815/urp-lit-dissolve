using UnityEngine;

namespace UnityEditor.Rendering.Universal.ShaderGUI
{
    internal class DissolveGUI
    {
        internal static class Styles
        {
            public static readonly GUIContent dissolveInputs = EditorGUIUtility.TrTextContent("Dissolve",
                "These settings control the dissolve effect on the surface.");

            public static readonly GUIContent dissolveColorText = EditorGUIUtility.TrTextContent("Color",
                "The color of the dissolve edge effect.");

            public static readonly GUIContent dissolveEmissionText = EditorGUIUtility.TrTextContent("Emission",
                "The emission intensity of the dissolve edge.");

            public static readonly GUIContent dissolveRadiusText = EditorGUIUtility.TrTextContent("Radius",
                "The radius of the dissolve effect from the origin point.");

            public static readonly GUIContent dissolveAreaText = EditorGUIUtility.TrTextContent("Area",
                "The width of the dissolve edge transition area.");

            public static readonly GUIContent dissolveOriginText = EditorGUIUtility.TrTextContent("Origin",
                "The world space origin point of the dissolve effect.");

            public static readonly GUIContent noiseMapText = EditorGUIUtility.TrTextContent("Noise Map",
                "The noise texture used to create the dissolve pattern.");

            public static readonly GUIContent dissolveShadowsText = EditorGUIUtility.TrTextContent("Dissolve Shadows",
                "Wether or not the dissolved parts of the object cast shadows.");
        }

        public struct DissolveProperties
        {
            public MaterialProperty dissolveColor;
            public MaterialProperty dissolveEmission;
            public MaterialProperty dissolveRadius;
            public MaterialProperty dissolveArea;
            public MaterialProperty dissolveOrigin;
            public MaterialProperty noiseMap;
            public MaterialProperty dissolveShadows;

            public DissolveProperties(MaterialProperty[] properties)
            {
                dissolveColor = BaseShaderGUI.FindProperty("_DissolveColor", properties, false);
                dissolveEmission = BaseShaderGUI.FindProperty("_DissolveEmission", properties, false);
                dissolveRadius = BaseShaderGUI.FindProperty("_DissolveRadius", properties, false);
                dissolveArea = BaseShaderGUI.FindProperty("_DissolveArea", properties, false);
                dissolveOrigin = BaseShaderGUI.FindProperty("_DissolveOrigin", properties, false);
                noiseMap = BaseShaderGUI.FindProperty("_NoiseMap", properties, false);
                dissolveShadows = BaseShaderGUI.FindProperty("_DissolveShadows", properties, false);
            }
        }

        public static void DoDissolveArea(DissolveProperties properties, MaterialEditor materialEditor)
        {
            if (properties.dissolveColor != null)
                materialEditor.ShaderProperty(properties.dissolveColor, Styles.dissolveColorText);

            if (properties.dissolveEmission != null)
                materialEditor.ShaderProperty(properties.dissolveEmission, Styles.dissolveEmissionText);

            if (properties.dissolveRadius != null)
                materialEditor.ShaderProperty(properties.dissolveRadius, Styles.dissolveRadiusText);

            if (properties.dissolveArea != null)
                materialEditor.ShaderProperty(properties.dissolveArea, Styles.dissolveAreaText);

            if (properties.dissolveOrigin != null)
                materialEditor.ShaderProperty(properties.dissolveOrigin, Styles.dissolveOriginText);

            if (properties.noiseMap != null)
            {
                materialEditor.TexturePropertySingleLine(Styles.noiseMapText, properties.noiseMap);
                materialEditor.TextureScaleOffsetProperty(properties.noiseMap);
            }

            if (properties.dissolveShadows != null)
            {
                materialEditor.ShaderProperty(properties.dissolveShadows, Styles.dissolveShadowsText);
            }
        }

        public static void SetMaterialKeywords(Material material)
        {
            if (material.HasProperty("_NoiseMap"))
            {
                bool hasNoiseMap = material.GetTexture("_NoiseMap");
                UnityEngine.Rendering.CoreUtils.SetKeyword(material, "_NOISE_MAP", hasNoiseMap);
            }
        }
    }
}
