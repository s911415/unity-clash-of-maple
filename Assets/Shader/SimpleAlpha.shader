Shader "Custom/SimpleAlpha" {
    Properties{
        _MainTex("Base", 2D) = "white" {}
        _Color("Color", Color) = (1, 1, 1, 1)
        _BaseTransVal("Base Transparency", Range(0, 1)) = 1
        _TransVal("Transparency Value", Range(0, 1)) = 0.5
    }
    SubShader{
        Tags{ "RenderType" = "Opaque" "Queue" = "Transparent" }
        LOD 200

        CGPROGRAM
#pragma surface surf Lambert alpha

        sampler2D _MainTex;
        fixed4 _Color;
        float _TransVal;
        float _BaseTransVal;

        struct Input {
            float2 uv_MainTex;
        };

        void surf(Input IN, inout SurfaceOutput o)
        {
            half3 c = (tex2D(_MainTex, IN.uv_MainTex).rgb) * (_BaseTransVal) + (_Color.rgb) * (1 - _BaseTransVal);
            o.Albedo = c.rgb;
            o.Alpha = _TransVal;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
