// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "paolo/HeightColorShader" {

	Properties{
		
		
		_CentrePoint("Centre", Vector) = (0, 0, 0, 0)
		_planetRadius("planetRadius", Float) = 0
		_SunPosition("_SunPosition", Vector) = (1000, 0, 0, 0)
		
		_Blue("blue", Color) = (0,0.5,1,1)
		_Cyan("cyan", Color) = (0.5,0.8,1,1)
		_Yellow("yellow", Color) = (0.9,0.9,0,1)
		_Green("green", Color) = (0,1,0,1)
		_White("white", Color) = (1,1,1,1)
		_Black("black", Color) = (0,0,0,1)

		
	}


		SubShader{

			Tags{ "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			#pragma surface surf Lambert vertex:vert 
			#include "UnityCG.cginc" 

			
			fixed4 _Blue;
			fixed4 _Cyan;
			fixed4 _Green;
			fixed4 _Yellow;
			fixed4 _White;
			fixed4 _Black;

	
			float _planetRadius;
			float4 _CentrePoint;
			float4 _SunPosition;
			float3 worldPos;

			struct Input
			{
				float2 uv_MainTex;
				float4 screenPos;
				float3 worldPos;
				float3 objPos;

				
				
			};


			

			void vert(inout appdata_full v, out Input o) {
				UNITY_INITIALIZE_OUTPUT(Input, o);
				o.objPos = v.vertex;

				float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
				
			}

			void surf(Input IN, inout SurfaceOutput o) {

				
				
				float originDistance = distance(_CentrePoint, IN.objPos.xyz);

				float pointDistance = distance(_SunPosition, worldPos);

				
				fixed4 c;

				originDistance = originDistance - _planetRadius; // somewhat from -1 to +1



				if (originDistance <-0.1) {
					c = _Yellow;
				}
				else if (-0.1 <= originDistance && originDistance < 0.1) {
					c = lerp(_Yellow, _Green, (originDistance+0.1)*5);
				}
				else if (0.8 <= originDistance && originDistance < 0.9) {
					c = lerp(_Green, _White, (originDistance - 0.8) * 10 );
				}
				else if (originDistance <0.9) {
					c = _Green;
				}
				else {
					c = _White;
				}


				c = lerp(_Black, c, (originDistance+1)/2);
			



				float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
				float4 lightPositionWorld = (1000.0, 0.0, 0.0, 1.0);

				float fragDistance = distance(IN.worldPos.xyz, float4(1000.0, 0.0, -1000.0, 1.0));

				float centreDistance = distance(objectOrigin, float4(1000.0, 0.0, -1000.0, 1.0));


				float diff = centreDistance - fragDistance;

				if(diff<-10)
					o.Emission = c.rgb;
				else if (diff>10)
					o.Emission = c.rgb / 6;

				else
					o.Emission = lerp(c.rgb, c.rgb / 6, (diff +10)/20);


				o.Alpha = 1;
			

			}


			ENDCG
		}

			Fallback "Diffuse"
			

	
}