Shader "Custom/URP/AnimatedLaserShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {} // ������ �ؽ�ó
        _LaserColor ("Laser Color", Color) = (1, 0, 0, 1) // ������ ����
        _GlowIntensity ("Glow Intensity", Range(0, 5)) = 1.0 // �� ���� ����
        _ScrollSpeed ("Scroll Speed", Range(0, 5)) = 1.0 // ��ũ�� �ӵ�
        _WaveFrequency ("Wave Frequency", Range(0, 20)) = 10.0 // ���� ��
        _WaveAmplitude ("Wave Amplitude", Range(0, 1)) = 0.05 // ���� ����
        _Transparency ("Transparency", Range(0, 1)) = 1.0 // ����
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

                // UV ��ǥ ����
                float wave = sin(_Time.y * _ScrollSpeed + input.uv.x * _WaveFrequency) * _WaveAmplitude;
                output.uv = input.uv + float2(0, wave); // ���� ȿ�� �߰�
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                // �ؽ�ó ���ø�
                float2 uv = input.uv;
                uv.x += _Time.y * _ScrollSpeed; // �ؽ�ó �̵�
                half4 texColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv);

                // ������ ���� �� ���� ����
                half4 laser = texColor * _LaserColor;
                laser.a *= _Transparency;

                // �۷ο� ȿ�� �߰�
                float glow = exp(-_GlowIntensity * abs(uv.y - 0.5));
                laser.rgb += glow * laser.a;

                return laser;
            }
            ENDHLSL
        }
    }
}
