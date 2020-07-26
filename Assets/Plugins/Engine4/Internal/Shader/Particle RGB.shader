
Shader "Particles/RGB Cube" { 
	Properties {
		alpha ("Alpha", Range(0,1)) = 0.1
		mult ("Brightness", Range(-1,1)) = 0
		cons ("Contrast", Range(-1,1)) = 0
	}
   SubShader { 
   Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
   Blend SrcAlpha One//MinusDstAlpha
//	AlphaTest Greater .01
//	ColorMask RGB
	Cull Off Lighting Off ZWrite Off
	
Pass { 
      CGPROGRAM 
 
      #pragma vertex vert
      #pragma fragment frag
      half alpha;
      uniform half cons;
      uniform half mult;
        
        struct vertexOutput {
            float4 pos : SV_POSITION;
            fixed4 col : TEXCOORD0;
         };
 
         vertexOutput vert(float4 vertexPos : POSITION) 
         {
            vertexOutput output; 
 
 			half a = 1 + cons;
			half b = mult - cons * 0.5f;  
 
            output.pos =  UnityObjectToClipPos(vertexPos);
            output.col = half4((normalize(vertexPos.xyz 
            		+ half3(0.5, 0.5, 0.5))) *a+b,alpha); 


            return output;
         }
 
         fixed4 frag(vertexOutput input) : COLOR // fragment shader
         {
              return input.col; 
         }
 
         ENDCG  
      }
   }
}