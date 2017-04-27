Shader "Noise/Test/FBM"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Intencity("Intencity", Range(0,1)) = 0.01
		_TouchSize("TouchSize", float) = 1
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100
		Cull off
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			#include "Shader/ClassicNoise3D.cginc"
			#include "Shader/Color.cginc"
			#include "Shader/Noise.cginc"
			#include "Shader/FBM.cginc"
			#include "UnityCG.cginc"

			struct touchdata
			{
				float2 tpPoint;
				float lifeTime;
			};

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float4 screenPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			float _Intencity;
			float _TouchSize;

			StructuredBuffer<touchdata> touchDataList;
			int touchCount;

			float pattern(float3 p)
			{

				float l = 0.9;
				float g = 0.4;
				int octaves = 2;
				float3 q = float3(0, 0, 0);
				float3 r = float3(0, 0, 0);
				float3 x = float3(0, 0, 0);
/*
				q.x = fbm(p + float3(time, time * 1, time));
				q.y = fbm(p + float3(0.2 * time, 0.3 * time, time));*/

				q.x = fbm(p + float3(2.2, 10.2, 0.2), octaves);
				q.y = fbm(p + float3(2.3, 2.8, 0.2), octaves);

				r.x = fbm(p + 2.0 * q + float3(0.7, 9.2, 0.2), octaves);
				r.y = fbm(p + 2.0 * q + float3(2.3, 2.8, 0.2), octaves);

				x.x = fbm(p + 4.0 * r + float3(0.7, 9.2, 0.2), octaves);
				x.y = fbm(p + 4.0 * r + float3(2.3, 2.8, 0.2), octaves);

				return fbm(p + 5.0 * x, octaves);
			}

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.screenPos = ComputeScreenPos(o.vertex);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				float2 screenUVPos = i.screenPos / i.screenPos.w;

				float brush = 0;
				
				for (int j = 0; j < touchCount; j++)
				{
					touchdata tc = touchDataList[j];
					
					float dist = distance(float2(tc.tpPoint.x, tc.tpPoint.y), screenUVPos * _ScreenParams.xy);

					if (dist < _TouchSize * 100 && tc.lifeTime > 0)
					{
						brush += pow(lerp(0.3, 0, dist / (_TouchSize * 100)),3) * 2.5 * tc.lifeTime;// lerp(1, 0, dist / 100) * tc.time;
					}
				}

				float o = 0;
				float3 coord = float3(i.uv * 3, _Time.y * 0.05);

				o += pattern(coord) * 0.1 * _Intencity;
				o *= saturate(brush);
				return tex2D(_MainTex, i.uv.xy + o);

		/*		float3 col = hsb2rgb(float3(i.uv.x, 0.5, 1));
				return brush * float4(col,1);*/
			}
			ENDCG
		}
	}
}
