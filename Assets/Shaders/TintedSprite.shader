Shader "Metro/TintedSprite"
{

	Properties
	{
		[PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
		_Color ("Main Tint", Color) = (1,1,1,1)

		[Toggle(DO_TINT)] _TintToggle ("Tint enabled", Float) = 1
		_BackColor ("Background Tint", Color) = (1,1,1,1)
		_UnfocusedColor ("Unfocused Color",  Color) = (1,1,1,1)

		[MaterialToggle] _IsFocused ("Is Focused", Float) = 1
		_FocusArea ("Focus Area", Vector) = (-50,-50,50,50)
	}

	SubShader
	{
		Tags
		{ 
			"Queue"="Transparent" 
			"IgnoreProjector"="True" 
			"RenderType"="Transparent" 
			"PreviewType"="Plane"
			"CanUseSpriteAtlas"="True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma shader_feature DO_TINT
			#include "UnityCG.cginc"
			
			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color    : COLOR;
				float2 uv  : TEXCOORD0;
				float2 worldPos : TEXCOORD1;
			};
			
			fixed4 _Color;
			fixed4 _BackColor;
			fixed4 _UnfocusedColor;

			bool _IsFocused;
			float4 _FocusArea;

			v2f vert(appdata_t IN)
			{
				v2f OUT;
				OUT.vertex = UnityObjectToClipPos(IN.vertex);
				OUT.worldPos = mul (unity_ObjectToWorld, IN.vertex);
				OUT.uv = IN.uv;
				OUT.color = IN.color * _Color;

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = tex2D (_MainTex, IN.uv);
#ifdef DO_TINT	
				float value = (c.r - 0.5f) / 0.5f;
				c.rgb = lerp(_BackColor.rgb, c.rgb * IN.color.rgb, value);
                /*if (c.r > 0.6){
                    c *= IN.color;
                }else{
                    c.rgb = _BackColor.rgb;
                }*/
#endif
				if (!_IsFocused)
				{
					c.rgb = (_UnfocusedColor.rgb * _UnfocusedColor.a + c.rgb * c.a * (1 -_UnfocusedColor.a));
					//c.a = a;
					//fixed lum = Luminance(c.rgb);
					//c.rgb = lerp(c.rgb, lum.xxx, _UnfocusedSaturation);
					
				}else
				{
					if (!(IN.worldPos.x < _FocusArea.x && _FocusArea.z < IN.worldPos.x &&
					      IN.worldPos.y < _FocusArea.y && _FocusArea.w < IN.worldPos.y))
						{
							c.rgb = (_UnfocusedColor.rgb * _UnfocusedColor.a + c.rgb * c.a * (1 -_UnfocusedColor.a));
							//c.a = a;
							//fixed lum = Luminance(c.rgb);
							//c.rgb = lerp(c.rgb, lum.xxx, _UnfocusedSaturation);
							//c.a *= _UnfocusedTransparency;
						}
				}
				c.rgb *= c.a;

				return c;
			}
		ENDCG
		}
	}
}
