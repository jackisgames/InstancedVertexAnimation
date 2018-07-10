Shader "Vertex Animator"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_AnimationData("AnimationData", 2D) = "black" {}
		_Color("Color", Color) = (1, 1, 1, 1)
		_VertexScaleAdjust("Vertex Scale", Float) = 1
		_VertexBound("Vertex Bound", Float) = 1
		_VertexCount("Vertex Count", Int) = 1
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
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				uint vid : SV_VertexID; // vertex ID
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				//UNITY_VERTEX_INPUT_INSTANCE_ID // necessary only if you want to access instanced properties in fragment Shader.
			};

			int _VertexCount;
			float _VertexScaleAdjust;
			float _VertexBound;
			sampler2D _MainTex; float4 _MainTex_ST;
			uniform sampler2D _AnimationData; uniform float4 _AnimationData_ST;

			/*UNITY_INSTANCING_BUFFER_START(Props)
			UNITY_DEFINE_INSTANCED_PROP(float4, _Color)
			UNITY_INSTANCING_BUFFER_END(Props)*/

			v2f vert(appdata v)
			{
				v2f o;

				UNITY_SETUP_INSTANCE_ID(v);
				//UNITY_TRANSFER_INSTANCE_ID(v, o); // necessary only if you want to access instanced properties in the fragment Shader.
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//animation
				float4 animationSample = float4((float)v.vid / _VertexCount, _Time.g,0,0);
				float3 vertexOffset = tex2Dlod(_AnimationData, animationSample);
				v.vertex.xyz += (vertexOffset.rgb-.5)*_VertexBound;
				o.vertex = UnityObjectToClipPos(v.vertex*_VertexScaleAdjust);
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//UNITY_SETUP_INSTANCE_ID(i); // necessary only if any instanced properties are going to be accessed in the fragment Shader.
				return tex2D(_MainTex, i.uv);// UNITY_ACCESS_INSTANCED_PROP(Props, _Color);
			}
			ENDCG
		}
	}
}