Shader "Custom/VertexAnimation"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0
        
        _Speed ("Speed", Float) = 1.0
        [HideInInspector]_MaxDist ("Max Dist", Float) = 0.0
        _PosTex ("Position Texture", 2D) = "black" {}
        _NormTex ("Normal Texture", 2D) = "bump" {}
        
        [HideInInspector]_SpeedController("C", Float) = 0
        [HideInInspector]_StartOffset("Start Offset", Range(0,1)) = 0.0
        [HideInInspector]_ClipSize ("Clip Size", Range(0,1)) = 1.0
        }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows vertex:vert addshadow
        
        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _PosTex, _NormTex, _TanTex;

        struct Input
        {
             float2 uv_MainTex : TEXCOORD0;
             float2 uv2 : TEXCOORD1;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        float _Speed;
        float _MaxDist;
        float _SpeedController;
        fixed _StartOffset;
        fixed _ClipSize;
        
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void vert (inout appdata_full v, out Input o)
        {
            UNITY_INITIALIZE_OUTPUT(Input, o);
            
            o.uv2 = v.texcoord2;
            o.uv2.y += _Time.y * _Speed * _SpeedController;
            o.uv2.y = (o.uv2.y % _ClipSize) + _StartOffset;
            
            float3 animTex = tex2Dlod(_PosTex, float4(o.uv2 , 0, 0)).rgb;
            float3 normTex = tex2Dlod(_NormTex, float4(o.uv2 , 0, 0)).rgb;
            
            //unpack
            animTex = animTex * 2.0 - 1.0;
            normTex = normTex * 2.0 - 1.0;
            
            //uncompress
            animTex *= _MaxDist;
            //animTex *= 1.667;
            
            v.vertex.xyz = animTex;
            v.normal = normTex;
        }
        
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
