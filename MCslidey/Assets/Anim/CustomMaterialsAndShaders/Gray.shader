Shader "Custom/Gray"
{
	Properties
	{
		_MainTex("Main Texture", 2D) = "white" {}
		//置灰开关
		[Toggle(OpenGray)] _OpenGray("OpenGray", Int) = 0
	}
 
	SubShader
	{
		Tags
		{
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
		}
 
		Pass
		{
			Tags{"LightMode" = "ForwardBase"}
            ZWrite Off
//            Blend SrcAlpha OneMinusSrcAlpha
            Blend SrcAlpha OneMinusSrcAlpha
     
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #pragma shader_feature OpenGray
            #include "UnityCG.cginc"
     
            sampler2D _MainTex;
            fixed4 _MainTex_ST;
     
            struct appdata
            {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
     
            struct v2f
            {
                float4 pos : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };
     
            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }
     
            fixed4 frag(v2f i) : SV_TARGET
            {
                fixed4 albedo = tex2D(_MainTex, i.uv) * i.color;
//置灰颜色
#if OpenGray
                float grey = dot(albedo.rgb, float3(0.299, 0.587, 0.114));
                albedo.rgb = float3(grey, grey, grey);
#endif
                return albedo;
            }
     
            ENDCG
        }
    }
    Fallback "Diffuse"
}