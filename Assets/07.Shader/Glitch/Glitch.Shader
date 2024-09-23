

Shader "Hidden/GlitchColorEffect"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Intensity ("Intensity", Range(0, 1)) = 0.5
        _ColorDrift ("Color Drift", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Intensity;
            float _ColorDrift;

            float rand(float2 co) {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }
            half4 frag(v2f_img i) : SV_Target
            {
                float2 uv = i.uv;
                
                // Horizontal shake (Only affect x coordinate)
                float horizontalShake = (rand(float2(_Time.y, uv.y)) - 0.5) * _Intensity * 0.1;
                uv.x += horizontalShake;

                // Color channel shifting for color drift effect (Only affect x coordinate)
                float colorDrift = sin(_Time.y * 10.0) * _ColorDrift;

                // Sample texture with RGB shifts
                half4 color;
                color.r = tex2D(_MainTex, uv + float2(colorDrift * 0.03, 0)).r * 0.7;
                color.g = tex2D(_MainTex, uv).g * 0.7;
                color.b = tex2D(_MainTex, uv - float2(colorDrift * 0.04, 0)).b * 0.8;

                return color;
            }
            ENDCG
        }
    }
}




