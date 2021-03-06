//@@@DynamicShaderInfoStart
//<readonly> NGUI 定义一张黑白图片作为Mask <UV动画> (注意：Shader须配合UITextture使用)
//@@@DynamicShaderInfoEnd
Shader "AFW/NGUI/NGUI - ClipMaskUvMove"
{
	Properties
	{
		_MainTex("Base (RGB), Alpha (A)", 2D) = "black" {}
		_MaskTex("Mask Texture", 2D) = "white" {}
        _XOffset("XOffset", float) = 0
        _YOffset("YOffset", float) = 0
	}

	SubShader
	{
		LOD 200

		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Offset -1, -1
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			sampler2D _MainTex;
			float4 _MainTex_ST;
			sampler2D _MaskTex;

            float _XOffset;
            float _YOffset;

			struct appdata_t
			{
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : POSITION;
				half4 color : COLOR;
				float2 uv : TEXCOORD0;
				fixed gray : TEXCOORD2;
			};

			v2f vert(appdata_t v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.uv = v.texcoord;
				o.gray = step(dot(v.color, fixed4(1,1,1,0)),0);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				// Sample the texture
				fixed4 col = tex2D(_MainTex, float2(i.uv.x - _XOffset, i.uv.y - _YOffset));

				//mask
				fixed4 mask = tex2D(_MaskTex, i.uv);

				col.a *= mask.r;

				fixed4 gray = dot(col.rgb, float3(0.299, 0.587, 0.114));

				gray.a = col.a* i.color.a;
				col = lerp(col* i.color, gray, i.gray);

				return col;
			}
			ENDCG
		}//end pass
	}//end SubShader

	SubShader
	{
		LOD 100

		Tags { "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

		Pass
		{
			Cull Off
			Lighting Off
			ZWrite Off
			Fog { Mode Off }
			ColorMask RGB
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMaterial AmbientAndDiffuse

			SetTexture[_MainTex]
			{
				Combine Texture * Primary
			}
		}//end pass
	}//end SubShader
}//end Shader
