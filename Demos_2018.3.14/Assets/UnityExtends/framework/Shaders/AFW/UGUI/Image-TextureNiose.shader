//@@@DynamicShaderInfoStart
//<readonly> UGUI 根据另外一张贴图的RGB扭曲最终效果
//@@@DynamicShaderInfoEnd
Shader "AFW/UGUI/Image - TextureNoise" 
{

	Properties 
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

		_noiseTex("Noise(RGB)",2D)="white"{}
		_size("noise Size",range(0,1))=0.1
		_speed("water speed",range(-1,1))=1
		_Scale("wave Scale",range(0.01,2))=1
		_alpha("wave  alpha", float)=1

		[HideInInspector]_StencilComp ("Stencil Comparison", Float) = 8
		[HideInInspector]_Stencil ("Stencil ID", Float) = 0
		[HideInInspector]_StencilOp ("Stencil Operation", Float) = 0
		[HideInInspector]_StencilWriteMask ("Stencil Write Mask", Float) = 255
		[HideInInspector]_StencilReadMask ("Stencil Read Mask", Float) = 255
		[HideInInspector]_ColorMask ("Color Mask", Float) = 15

		[Enum(UnityEngine.Rendering.BlendMode)] _srcBlend("Src Blend Mode", Float) = 5
		[Enum(UnityEngine.Rendering.BlendMode)] _dstBlend("Dst Blend Mode", Float) = 10
		[Enum(UnityEngine.Rendering.BlendMode)] _srcAlphaBlend("Src Alpha Blend Mode", Float) = 1
		[Enum(UnityEngine.Rendering.BlendMode)] _dstAlphaBlend("Dst Alpha Blend Mode", Float) = 10
	}

	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" }
	
		Stencil
		{
			Ref [_Stencil]
			Comp [_StencilComp]
			Pass [_StencilOp] 
			ReadMask [_StencilReadMask]
			WriteMask [_StencilWriteMask]
		}
	
		Pass
		{

    		Lighting Off
    		Fog { Mode Off }
			Cull Off
			ZWrite Off
			ZTest [unity_GUIZTestMode]
			Blend[_srcBlend][_dstBlend],[_srcAlphaBlend][_dstAlphaBlend]

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile DUMMY PIXELSNAP_ON

			sampler2D _MainTex;
			sampler2D _noiseTex;
			float  _size; 
			float  _speed; 
			float  _Scale;              
			float  _alpha; 

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				half2 texcoord  : TEXCOORD0;
			};

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color ;
				#ifdef PIXELSNAP_ON
					OUT.vertex = UnityPixelSnap (OUT.vertex);
				#endif

				return OUT;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				float2 noiseUV= IN.texcoord;
				noiseUV+=_Time*_speed;
				//只要RG
				fixed2 noiseCol = tex2D(_noiseTex,noiseUV);
				fixed2 mainUV=IN.texcoord;

				mainUV+=noiseCol*0.2*_size*_Scale;

				fixed4 mainCol = tex2D(_MainTex,mainUV);		

				return mainCol*IN.color*fixed4(1,1,1,_alpha);
			}
			ENDCG
		}//end pass
	}//end SubShader
}//end Shader