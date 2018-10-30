﻿Shader "Custom/AdjustColorShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_v1 ("V1", Vector) = (1,0,0,0)
		_v2 ("V2", Vector) = (0,1,0,0)
		_v3 ("V3", Vector) = (0,0,1,0)
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Blend SrcAlpha OneMinusSrcAlpha
		//ZWrite On Lighting Off Cull Off Fog { Mode Off } Blend SrcAlpha OneMinusSrcAlpha
		Tags { "Queue"="Transparent" "RenderType"="Transparent" "IgnoreProjector"="True" }

		//LOD 100

		Pass {
		    CGPROGRAM

		    #pragma vertex vert             
		    #pragma fragment frag
		    #include "UnityCG.cginc"

		    sampler2D _MainTex;
		    float4 _v1;
		    float4 _v2;
		    float4 _v3;
		    float4 _MainTex_TexelSize;

		    struct vertInput {
		        float4 pos : POSITION;
		        float2 uv : TEXCOORD0;
		    };

		    struct vertOutput {
		        float4 pos : SV_POSITION;
		        float2 uv : TEXCOORD0;
		    };

		    float4 _MainTex_ST;

		    vertOutput vert(vertInput input) {
		        vertOutput o;
		        o.pos = UnityObjectToClipPos(input.pos);
		        o.uv = TRANSFORM_TEX(input.uv,_MainTex);

		        return o;
		    }

		    half4 frag(vertOutput output) : COLOR {
		    	float4 col = tex2D(_MainTex, output.uv);
		    	float x = dot(col, _v1);//col*_v1;//col.x+col.y+col.z;
		    	float y = dot(col, _v2);
		    	float z = dot(col, _v3);
		    	return float4(x,y,z,1);
		    	//return col*_Color;//2*col*_Color;
		    }
		    ENDCG
		}
	}
}
