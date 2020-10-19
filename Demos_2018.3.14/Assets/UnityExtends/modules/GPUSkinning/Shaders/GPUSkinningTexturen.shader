// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/GPUSkinningTexturen"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_AnimMap("AnimMap", 2D) = "white" {}
		[Toggle(_GPUAnimation)] _GPUAnimation("Enable GPUAnimation", Float) = 1
	}
	SubShader
	{
		Tags{ "RenderType" = "Opaque" }
		LOD 100			

			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_fog
				#pragma multi_compile_instancing
				#pragma multi_compile _GPUAnimation
				#include "UnityCG.cginc"
				#pragma target 3.5

				struct appdata
				{
					float4 vertex : POSITION;
					float2 uv : TEXCOORD0;
					float4 tangent : TANGENT;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};
				struct v2f
				{
					float2 uv : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
					UNITY_VERTEX_INPUT_INSTANCE_ID
				};

				sampler2D _MainTex;
				float4 _MainTex_ST;

				sampler2D _AnimMap;
				float4 _AnimMap_TexelSize;
				UNITY_INSTANCING_BUFFER_START(MyProperties)
				UNITY_DEFINE_INSTANCED_PROP(float, _Frame)
					#define _Frame_arr MyProperties
				UNITY_INSTANCING_BUFFER_END(MyProperties)


				v2f vert(appdata v)
				{
					UNITY_SETUP_INSTANCE_ID(v);
					v2f o;

					//----------anima(v.tangent, v.vertex);
					float4	tangent = v.tangent;
					float4	vertex = v.vertex;
					float animMap_x = UNITY_ACCESS_INSTANCED_PROP(_Frame_arr, _Frame)* _AnimMap_TexelSize.x;
					float ii = tangent.x * 4 + 0.5;
					float animMap_y = ii * _AnimMap_TexelSize.y;//=1/height
					float4 mx0 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
					animMap_y += _AnimMap_TexelSize.y;
					float4 mx1 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
					animMap_y += _AnimMap_TexelSize.y;
					float4 mx2 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
					animMap_y += _AnimMap_TexelSize.y;
					float4 mx3 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));

					float4x4 mx4_0 = float4x4(mx0.x, mx0.y, mx0.z, mx0.w,
						mx1.x, mx1.y, mx1.z, mx1.w,
						mx2.x, mx2.y, mx2.z, mx2.w,
						mx3.x, mx3.y, mx3.z, mx3.w);



					ii = tangent.z * 4 + 0.5;
					animMap_y = ii * _AnimMap_TexelSize.y;
					mx0 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
					animMap_y += _AnimMap_TexelSize.y;
					mx1 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
					animMap_y += _AnimMap_TexelSize.y;
					mx2 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
					animMap_y += _AnimMap_TexelSize.y;
					mx3 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));

					float4x4 mx4_1 = float4x4(mx0.x, mx0.y, mx0.z, mx0.w,
						mx1.x, mx1.y, mx1.z, mx1.w,
						mx2.x, mx2.y, mx2.z, mx2.w,
						mx3.x, mx3.y, mx3.z, mx3.w);


					float4 pos = mul(mx4_0, vertex) *tangent.y + mul(mx4_1, vertex) * tangent.w;

					o.vertex = UnityObjectToClipPos(pos);
					o.uv = TRANSFORM_TEX(v.uv, _MainTex);
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					fixed4 col = tex2D(_MainTex, i.uv);
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG
		}

		//------------------------------------- shadow
		//Pass
		//{
		//	Blend SrcAlpha  OneMinusSrcAlpha
		//	//ZWrite Off
		//	Cull Back
		//	ColorMask RGB

		//	CGPROGRAM
		//	#pragma vertex vert
		//	#pragma fragment frag
		//	#pragma multi_compile_fog
		//	#pragma multi_compile_instancing
		//	#pragma multi_compile _GPUAnimation
		//	#include "UnityCG.cginc"
		//	#pragma target 3.5

		//	struct appdata
		//	{
		//		float4 vertex : POSITION;
		//		float2 uv : TEXCOORD0;
		//		float4 tangent : TANGENT;
		//		UNITY_VERTEX_INPUT_INSTANCE_ID
		//	};
		//	struct v2f
		//	{
		//		float2 uv : TEXCOORD0;
		//		UNITY_FOG_COORDS(1)
		//		float4 vertex : SV_POSITION;
		//		UNITY_VERTEX_INPUT_INSTANCE_ID
		//	};

		//	sampler2D _MainTex;
		//	float4 _MainTex_ST;

		//	sampler2D _AnimMap;
		//	float4 _AnimMap_TexelSize;
		//	UNITY_INSTANCING_BUFFER_START(MyProperties)
		//	UNITY_DEFINE_INSTANCED_PROP(float, _Frame)
		//	#define _Frame_arr MyProperties
		//	UNITY_INSTANCING_BUFFER_END(MyProperties)


		//	v2f vert(appdata v)
		//	{
		//		UNITY_SETUP_INSTANCE_ID(v);
		//		v2f o;

		//		//----------anima(v.tangent, v.vertex);
		//		float4	tangent = v.tangent;
		//		float4	vertex = v.vertex;
		//		float animMap_x = UNITY_ACCESS_INSTANCED_PROP(_Frame_arr, _Frame)* _AnimMap_TexelSize.x;
		//		float ii = tangent.x * 4 + 0.5;
		//		float animMap_y = ii * _AnimMap_TexelSize.y;//=1/height
		//		float4 mx0 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
		//		animMap_y += _AnimMap_TexelSize.y;
		//		float4 mx1 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
		//		animMap_y += _AnimMap_TexelSize.y;
		//		float4 mx2 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
		//		animMap_y += _AnimMap_TexelSize.y;
		//		float4 mx3 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));

		//		float4x4 mx4_0 = float4x4(mx0.x, mx0.y, mx0.z, mx0.w,
		//			mx1.x, mx1.y, mx1.z, mx1.w,
		//			mx2.x, mx2.y, mx2.z, mx2.w,
		//			mx3.x, mx3.y, mx3.z, mx3.w);



		//		ii = tangent.z * 4 + 0.5;
		//		animMap_y = ii * _AnimMap_TexelSize.y;
		//		mx0 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
		//		animMap_y += _AnimMap_TexelSize.y;
		//		mx1 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
		//		animMap_y += _AnimMap_TexelSize.y;
		//		mx2 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));
		//		animMap_y += _AnimMap_TexelSize.y;
		//		mx3 = tex2Dlod(_AnimMap, float4(animMap_x, animMap_y, 0, 0));

		//		float4x4 mx4_1 = float4x4(mx0.x, mx0.y, mx0.z, mx0.w,
		//			mx1.x, mx1.y, mx1.z, mx1.w,
		//			mx2.x, mx2.y, mx2.z, mx2.w,
		//			mx3.x, mx3.y, mx3.z, mx3.w);


		//		float4 pos = mul(mx4_0, vertex) *tangent.y + mul(mx4_1, vertex) * tangent.w;

		//		pos.y = 0;
		//		o.vertex = UnityObjectToClipPos(pos);
		//		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		//		UNITY_TRANSFER_FOG(o,o.vertex);
		//		return o;
		//	}

		//	float4 frag(v2f i) : SV_Target
		//	{
		//		float4 color;
		//		color.xyzw = float4(0.0, 0.0, 0.0,0.8f);
		//		return color;
		//	}

		//	ENDCG
		//}
	}
}
