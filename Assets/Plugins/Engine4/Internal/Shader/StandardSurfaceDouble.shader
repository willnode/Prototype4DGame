// Based on:
// https://forum.unity3d.com/threads/standard-shader-modified-to-be-double-sided-is-very-shiny-on-the-underside.393068/

Shader "Custom/Standard Surface (Double Sided)" {
    Properties {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGBA)", 2D) = "white" {}
        [Space]
        [NoScaleOffset] _BumpMap("Normal Map", 2D) = "bump" {}
        _BumpScale("Bump Scale", Float) = 1.0
        [Space]
        [Toggle] _UseMetallicMap ("Using Metallic Map", Float) = 0.0
        [NoScaleOffset] _MetallicGlossMap("Metallic (RA)", 2D) = "black" {}
        [Space]
        [Gamma] _Metallic ("Metallic", Range(0,1)) = 0.0
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Cutoff("Alpha Cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags { "Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout" }
        Blend SrcAlpha OneMinusSrcAlpha
        LOD 200
        ZWrite Off
        Cull Off
 
        Pass {
            ColorMask 0
            ZWrite On
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
         
            #include "UnityCG.cginc"
 
            struct v2f {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
            };
 
            sampler2D _MainTex;
           	float4 _MainTex_ST;
            fixed _Cutoff;
 
            v2f vert (appdata_img v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
                if (_Cutoff > 0.01)
	                clip(col.a - _Cutoff);
                return 0;
            }
            ENDCG
        }
 
        Pass
        {
            Tags {"LightMode"="ShadowCaster"}
            ZWrite On
            Cull Off
 
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_shadowcaster
            #include "UnityCG.cginc"

 
            struct v2f {
                V2F_SHADOW_CASTER;
                float2 texcoord : TEXCOORD1;
            };
 
            sampler2D _MainTex;
           	float4 _MainTex_ST;
            fixed _Cutoff;

            v2f vert(appdata_base v)
            {
                v2f o;
                TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }
         
            float4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.texcoord);
            		if (_Cutoff > 0.01)
	                 clip(col.a - _Cutoff);
                SHADOW_CASTER_FRAGMENT(i)
            }
            ENDCG
        }
     
        CGPROGRAM
        #pragma surface surf Standard nolightmap // alpha:fade fullforwardshadows 
        #pragma shader_feature _USEMETALLICMAP_ON
        #pragma target 3.0
 
        sampler2D _MainTex;
        sampler2D _MetallicGlossMap;
        sampler2D _BumpMap;
 
        struct Input {
            float2 uv_MainTex;
            fixed facing : VFACE;
        };
 
        half _Glossiness;
        half _Metallic;
        fixed4 _Color;
        half _BumpScale;
        fixed _Cutoff;
 
        void surf (Input IN, inout SurfaceOutputStandard o) {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;

            #ifdef _USEMETALLICMAP_ON
            fixed4 mg = tex2D(_MetallicGlossMap, IN.uv_MainTex);
            o.Metallic = mg.r * _Metallic;
            o.Smoothness = mg.a * _Glossiness;
            #else
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            #endif

 				o.Normal = UnpackScaleNormal(tex2D(_BumpMap, IN.uv_MainTex), _BumpScale);
				

            // Rescales the alpha on the blended pass
            if (_Cutoff > 0.01)
	         	o.Alpha = saturate(c.a / _Cutoff);
 				else
 					o.Alpha = 1;

            if (IN.facing < 0.5)
                o.Normal *= -1.0;
        }
        ENDCG
    }
    FallBack "Diffuse"
}