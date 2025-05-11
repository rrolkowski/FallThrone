Shader "Custom/OutlineLitURP"
{
Properties
{
    // G³ówne
    _BaseMap ("Base Map", 2D) = "white" {}
    _BaseColor ("Base Color", Color) = (1,1,1,1)

    // Outline
    _OutlineColor ("Outline Color", Color) = (1,0.5,0,1)
    _OutlineWidth ("Outline Width", Float) = 0.02

    // Metallic workflow
    _Metallic ("Metallic", Range(0.0, 1.0)) = 0.0
    _MetallicGlossMap("Metallic Map", 2D) = "white" {}

    // Smoothness
    _Smoothness ("Smoothness", Range(0.0, 1.0)) = 0.5
    _SmoothnessTextureChannel ("Smoothness texture channel", Float) = 0

    // Specular
    _SpecColor("Specular", Color) = (0.2, 0.2, 0.2)
    _SpecGlossMap("Specular Map", 2D) = "white" {}

    // Normal Map
    _BumpMap("Normal Map", 2D) = "bump" {}
    _BumpScale("Bump Scale", Float) = 1.0

    // Occlusion
    _OcclusionMap("Occlusion Map", 2D) = "white" {}
    _OcclusionStrength("Occlusion Strength", Range(0,1)) = 1.0

    // Emission
    [HDR] _EmissionColor("Emission Color", Color) = (0,0,0)
    _EmissionMap("Emission Map", 2D) = "white" {}

    // Detail
    _DetailMask("Detail Mask", 2D) = "white" {}
    _DetailAlbedoMap("Detail Albedo x2", 2D) = "linearGrey" {}
    _DetailAlbedoMapScale("Detail Albedo Scale", Range(0,2)) = 1.0
    _DetailNormalMap("Detail Normal Map", 2D) = "bump" {}
    _DetailNormalMapScale("Detail Normal Scale", Range(0,2)) = 1.0

    // Clipping, transparency
    _Cutoff ("Alpha Cutoff", Range(0,1)) = 0.5
    [ToggleUI] _AlphaClip("__clip", Float) = 0.0
    _Surface("__surface", Float) = 0.0
    _Blend("__blend", Float) = 0.0
    _Cull("__cull", Float) = 2.0
    [HideInInspector] _SrcBlend("__src", Float) = 1.0
    [HideInInspector] _DstBlend("__dst", Float) = 0.0
    [HideInInspector] _SrcBlendAlpha("__srcA", Float) = 1.0
    [HideInInspector] _DstBlendAlpha("__dstA", Float) = 0.0
    [HideInInspector] _ZWrite("__zw", Float) = 1.0

    // Other
    [ToggleUI] _ReceiveShadows("Receive Shadows", Float) = 1.0
    _QueueOffset("Queue offset", Float) = 0.0
}

SubShader
{
    Tags { "RenderPipeline"="UniversalPipeline" "RenderType"="Opaque" }

    // === OUTLINE PASS ===
    Pass
    {
        Name "Outline"
        Tags { "LightMode" = "SRPDefaultUnlit" }
        Cull Front
        ZWrite On

        HLSLPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct Attributes
        {
            float3 positionOS : POSITION;
            float3 normalOS : NORMAL;
        };

        struct Varyings
        {
            float4 positionHCS : SV_POSITION;
        };

        float _OutlineWidth;
        float4 _OutlineColor;

        Varyings vert(Attributes IN)
        {
            Varyings OUT;
            float3 normal = normalize(IN.normalOS);
            float3 offsetPos = IN.positionOS + normal * _OutlineWidth;
            OUT.positionHCS = TransformObjectToHClip(offsetPos);
            return OUT;
        }

        half4 frag(Varyings IN) : SV_Target
        {
            return _OutlineColor;
        }
        ENDHLSL
    }

    // === BASE URP LIT PASS ===
    Pass
    {
        Name "ForwardLit"
        Tags { "LightMode" = "UniversalForward" }
        Cull Back
        ZWrite On

        HLSLPROGRAM
        #pragma vertex LitPassVertex
        #pragma fragment LitPassFragment

        #pragma shader_feature_local _NORMALMAP
        #pragma shader_feature_local _EMISSION
        #pragma shader_feature_local _METALLICSPECGLOSSMAP
        #pragma shader_feature_local_fragment _SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A
        #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
        #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
        #pragma multi_compile_fragment _ _ADDITIONAL_LIGHT_SHADOWS
        #pragma multi_compile_fragment _ _SHADOWS_SOFT
        #pragma multi_compile_fragment _ _SCREEN_SPACE_OCCLUSION
        #pragma multi_compile_fragment _ _LIGHT_COOKIES
        #pragma multi_compile_fragment _ _FORWARD_PLUS
        #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
        #pragma multi_compile _ SHADOWS_SHADOWMASK
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DYNAMICLIGHTMAP_ON

        #pragma multi_compile_instancing
        #pragma instancing_options renderinglayer
        #include_with_pragmas "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DOTS.hlsl"

        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Shaders/LitForwardPass.hlsl"
        ENDHLSL
    }
    Pass
{
    Name "ShadowCaster"
    Tags{"LightMode" = "ShadowCaster"}

    ZWrite On
    ZTest LEqual
    ColorMask 0
    Cull Back

    HLSLPROGRAM
    #pragma vertex ShadowPassVertex
    #pragma fragment ShadowPassFragment

    #pragma multi_compile_shadowcaster
    #pragma multi_compile_instancing

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/ShadowCasterPass.hlsl"
    ENDHLSL
}

Pass
{
    Name "DepthNormals"
    Tags { "LightMode" = "DepthNormals" }

    Cull Back

    HLSLPROGRAM
    #pragma vertex DepthNormalsVertex
    #pragma fragment DepthNormalsFragment

    #pragma multi_compile_fragment _ _NORMALMAP
    #pragma multi_compile_instancing

    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
    #include "Packages/com.unity.render-pipelines.universal/Shaders/DepthNormalsPass.hlsl"
    ENDHLSL
}


}

FallBack "Hidden/Universal Render Pipeline/FallbackError"
CustomEditor "Custom.OutlineLitShaderGUI"
}
