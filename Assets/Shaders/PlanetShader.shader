// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "paolo/PlanetShader"
{
	
	Properties
	{
		// we have removed support for texture tiling/offset,
		// so make them not be displayed in material inspector
		[NoScaleOffset] _MainTex("Texture", 2D) = "white" {}
	}
		SubShader
	{
		Pass
	{
		CGPROGRAM
		// use "vert" function as the vertex shader
		#pragma vertex vert
		// use "frag" function as the pixel (fragment) shader
		#pragma fragment frag

		float4 _Fade;
	float _Shift;

		// vertex shader inputs
	struct appdata
	{
		float4 vertex : POSITION; // vertex position
		float2 uv : TEXCOORD0; // texture coordinate
	};

	// vertex shader outputs ("vertex to fragment")
	struct v2f
	{
		float2 uv : TEXCOORD0; // texture coordinate
		float4 vertex : SV_POSITION; // clip space position

		float4 fragPos : TEXCOORD2;
		float4 fade : TEXCOORD3;
	};

	// vertex shader
	v2f vert(appdata v)
	{
		v2f o;
		// transform position to clip space
		// (multiply with model*view*projection matrix)
		o.vertex = UnityObjectToClipPos(v.vertex);
		// just pass the texture coordinate
		o.uv = v.uv;
		return o;
	}

	// texture we will sample
	sampler2D _MainTex;

	// pixel shader; returns low precision ("fixed4" type)
	// color ("SV_Target" semantic)
	fixed4 frag(v2f i) : SV_Target
	{
		// sample texture and return it
		fixed4 col = tex2D(_MainTex, i.uv);

		float dist = distance(i.fade, i.fragPos);
		float4 objectOrigin = mul(unity_ObjectToWorld, float4(0.0, 0.0, 0.0, 1.0));
		float dist2 = distance(objectOrigin, _WorldSpaceCameraPos);

		if (dist2<1000)
		{
			col = col/2;
		}

		return col;
	}
		ENDCG
	}
	}
}