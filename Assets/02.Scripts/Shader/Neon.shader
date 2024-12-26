Shader "Custom/NeonEffectUI"
{
    Properties
    {
        _BaseColor ("Base Color", Color) = (1,1,1,1)
        _EmissionColor ("Emission Color", Color) = (1,0,0,1)
        _EmissionIntensity ("Emission Intensity", Range(0, 10)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Overlay" "IgnoreProjector"="True" "Canvas"="True" }
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties from the Unity Editor
            float4 _BaseColor;
            float4 _EmissionColor;
            float _EmissionIntensity;

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

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = UnityObjectToClipPos(IN.positionOS);
                OUT.uv = IN.uv;
                return OUT;
            }

            fixed4 frag (Varyings IN) : SV_Target
            {
                // Base color and emission effect
                float4 baseColor = _BaseColor;
                float4 emission = _EmissionColor * _EmissionIntensity;
                float4 finalColor = baseColor + emission;

                // Ensure proper alpha blending for UI
                finalColor.a = baseColor.a;
                return finalColor;
            }
            ENDHLSL
        }
    }
    FallBack "UI/Default"
}
