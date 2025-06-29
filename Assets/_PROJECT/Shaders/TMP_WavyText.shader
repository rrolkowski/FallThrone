Shader "Custom/TMP_WavyMobileSDF"
{
    Properties
    {
        _MainTex ("Font Atlas", 2D) = "white" {}
        _FaceColor ("Face Color", Color) = (1,1,1,1)
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.1)) = 0.0

        _WaveAmplitude ("Wave Amplitude", Float) = 0.05
        _WaveFrequency ("Wave Frequency", Float) = 2
        _WaveSpeed ("Wave Speed", Float) = 2
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        ZWrite Off
        Cull Off
        Lighting Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _FaceColor;
            float4 _OutlineColor;
            float _OutlineWidth;

            float _WaveAmplitude;
            float _WaveFrequency;
            float _WaveSpeed;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;   // Dodajemy vertex color
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            v2f vert (appdata v)
            {
                v2f o;
                
                float wave = sin(v.vertex.x * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
                v.vertex.y += wave;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float distance = tex2D(_MainTex, i.uv).a;

                float smoothing = fwidth(distance) * 0.5;
                float alpha = smoothstep(0.5 - smoothing, 0.5 + smoothing, distance);

                float outlineAlpha = smoothstep(0.5 - _OutlineWidth - smoothing, 0.5 - _OutlineWidth + smoothing, distance);

                fixed4 col = lerp(_OutlineColor, _FaceColor, alpha);

                col *= i.color; // Uwzglêdnienie vertex color

                col.a *= max(alpha, outlineAlpha) * _FaceColor.a;

                return col;
            }
            ENDCG
        }
    }
}
