Shader "Custom/URP/AnimatedLaserShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // 레이저 텍스처
        _LaserColor ("Laser Color", Color) = (1, 0, 0, 1) // 레이저 색상
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.0 // 빛 번짐 강도
        _ScrollSpeed ("Scroll Speed", Range(0, 5)) = 1.0 // 스크롤 속도
        _WaveFrequency ("Wave Frequency", Range(0, 20)) = 10.0 // 파형 빈도
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.05 // 파형 진폭
        _Transparency ("Transparency", Range(0, 1)) = 1.0 // 투명도
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
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
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            float4 _LaserColor;
            float _GlowIntensity;
            float _ScrollSpeed;
            float _WaveFrequency;
            float _WaveAmplitude;
            float _Transparency;

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);

                // UV 좌표 수정
                float wave = sin(_Time.y * _ScrollSpeed + input.uv.x * _WaveFrequency) * _WaveAmplitude;
                output.uv = input.uv + float2(0, wave); // 파형 효과 추가
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // 텍스처 샘플링
                float2 uv = input.uv;
                uv.x += _Time.y * _ScrollSpeed; // 텍스처 이동
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                // 레이저 색상 및 투명도 조합
                half4 laser = texColor * _LaserColor;
                laser.a *= _Transparency;

                // 글로우 효과 추가
                float glow = exp(-_GlowIntensity * abs(uv.y - 0.5));
                laser.rgb += glow * laser.a;

                return laser;
            }
            ENDHLSL
        }
    }
}
