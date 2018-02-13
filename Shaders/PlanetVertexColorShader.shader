Shader "paolo/PlanetVertexColorShader" {

	Properties{
		//_MainTex("Base (RGB)", 2D) = "white" {}
		_CentrePoint("Centre", Vector) = (0, 0, 0, 0)

		_Color("Top Color", Color) = (1,0.8,0.3,1)
		_Color2("Bottom Color", Color) = (0,1,0,1)
		//_Color("Top Color", Color) = (255,255,255,1)
		//_Color2("Bottom Color", Color) = (0,0,0,1)

		_Blue("blue", Color) = (0,0,1,1)
		_Cyan("cyan", Color) = (0.3,0.3,1,1)
		_Yellow("yellow", Color) = (1,0.8,0.3,1)
		_Green("green", Color) = (0,1,0,1)
		_White("white", Color) = (1,1,1,1)
	}


		SubShader{
		Lighting Off
		Tags{ "RenderType" = "Opaque" }
		LOD 200

		CGPROGRAM
#pragma surface surf Lambert vertex:vert 
#include "UnityCG.cginc" 
		//sampler2D _MainTex;
		fixed4 _Color;
	fixed4 _Color2;
	fixed4 _Blue;
	fixed4 _Cyan;
	fixed4 _Green;
	fixed4 _Yellow;
	fixed4 _White;
	float4 _CentrePoint;


	struct Input
	{
		//float2 uv_MainTex;
		float4 screenPos;
		float3 worldPos;
		float3 objPos;
	};

	void vert(inout appdata_full v, out Input o) {
		UNITY_INITIALIZE_OUTPUT(Input, o);
		o.objPos = v.vertex;
	}

	void surf(Input IN, inout SurfaceOutput o) {

		//if(IN.screenPos.w!=0)
		//float2 screenUV = IN.screenPos.xy / IN.screenPos.w;


		//float curDistance = distance(_CentrePoint.xyz, IN.screenPos.w);
		//float lenght = length(ObjSpaceViewDir(v.vertex));

		//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * lerp(_Color2, _Color, screenUV.y);

		//fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * lerp(_Color2, _Color, screenUV.y);
		//fixed4 c = lerp(_Color, _Color2, IN.worldPos.x);

		//float4 objectOrigin = mul(_Object2World, float4(0.0, 0.0, 0.0, 1.0));

		//float distToOrigin = distance(objectOrigin, screenUV);

		//float curDistance = distance(objectOrigin, IN.screenPos.xy);

		//float pixDistance = distance(IN.objPos, screenUV);

		float originDistance = distance(float4(0.0, 0.0, 0.0, 1.0), IN.objPos.xyz);

		//fixed4 c = lerp(_Color, _Color2, IN.objPos.x+0.5);
		//fixed4 c = lerp(_Color, _Color2, abs(IN.objPos.x/0.5));
		//fixed4 c = lerp(_Color, _Color2,  -0.8+(distance2)  );

		fixed4 c;

		if (originDistance < 1.25) {

			c = _Blue;
		}
		else if (originDistance < 1.3005) {
			c = _Cyan;
		}
		else if (originDistance < 1.32) {
			c = _Yellow;
		}
		else if (originDistance < 1.38) {
			c = _Green;
		}
		else if (originDistance < 1.5) {
			c = _White;
		}

		//fixed4 c = lerp(_Color, _Color2,  1.3/(originDistance)  -0.5  );


		//o.Emission = c.rgb;
		o.Albedo = c.rgb;
		//o.Alpha = c.a;

		//o.Albedo = IN.objPos;
		o.Alpha = 1;

	}



	fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
	{
		fixed4 c;
		c.rgb = s.Albedo;
		c.a = s.Alpha;
		return c;
	}

	ENDCG
	}

		//Fallback "Diffuse"
		Fallback "VertexLit"


}