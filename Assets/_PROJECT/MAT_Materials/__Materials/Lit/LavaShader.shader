Shader "Custom/LavaShaderURP"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _BaseColor ("Base Color", Color) = (1, 0.3, 0, 1)
        _EmissionColor ("Emission Color", Color) = (1, 0.5, 0, 1)
        _EmissionStrength ("Emission Strength", Float) = 1.0
        _Frequency ("Wave Frequency", Float) = 3.0
        _Amplitude ("Wave Amplitude", Float) = 0.1
        _Speed ("Wave Speed", Float) = 1.0
        _FlowSpeed ("Flow UV Speed", Float) = 0.1
        _NoiseScale ("Noise Scale", Float) = 10.0
        _NoiseStrength ("Noise Strength", Float) = 0.05
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            Name "ForwardLit"
            ZWrite On

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            TEXTURE2D(_BaseMap);
            SAMPLER(sampler_BaseMap);

            float4 _BaseColor;
            float4 _EmissionColor;
            float _EmissionStrength;
            float _Frequency;
            float _Amplitude;
            float _Speed;
            float _FlowSpeed;
            float _NoiseScale;
            float _NoiseStrength;

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1, 311.7))) * 43758.5453123);
            }

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1.0, 0.0));
                float c = hash(i + float2(0.0, 1.0));
                float d = hash(i + float2(1.0, 1.0));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) +
                       (c - a) * u.y * (1.0 - u.x) +
                       (d - b) * u.x * u.y;
            }

            Varyings vert (Attributes IN)
            {
                Varyings OUT;

                float3 worldPos = TransformObjectToWorld(IN.positionOS.xyz);
                worldPos.y += sin(worldPos.x * _Frequency + _Time.y * _Speed) * _Amplitude;
                OUT.worldPos = worldPos;

                OUT.positionHCS = TransformWorldToHClip(worldPos);

                // Scrolluj¹cy UV + noise
                float2 uv = IN.uv;
                uv += float2(_Time.y * _FlowSpeed, 0); // przesuwanie UV
                float n = noise(uv * _NoiseScale) * _NoiseStrength;
                uv += n;

                OUT.uv = uv;

                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.uv);

                // Pulsuj¹cy base color (sinus do efektu ¿arzenia)
                float pulse = sin(_Time.y * 2.0) * 0.5 + 0.5;
                float3 base = texColor.rgb * lerp(_BaseColor.rgb * 0.8, _BaseColor.rgb * 1.2, pulse);

                // Emisja równie¿ pulsuje
                float3 emission = _EmissionColor.rgb * (_EmissionStrength + pulse * 2.0);

                return float4(base + emission, 1.0);
            }
            ENDHLSL
        }
    }
}
