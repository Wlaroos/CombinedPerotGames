Shader "UI/StencilCutout"
{
    Properties { _MainTex("Texture", 2D) = "white" {} }
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Cull Off ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
        Stencil {
            Ref 1
            Comp Always
            Pass Replace
        }
        Pass {
            ColorMask 0
            ZTest Always
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            struct v2f { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; float4 color : COLOR; };
            sampler2D _MainTex; float4 _MainTex_ST;
            v2f vert(appdata v) { v2f o; o.pos = UnityObjectToClipPos(v.vertex); o.uv = TRANSFORM_TEX(v.uv, _MainTex); o.color = v.color; return o; }
            fixed4 frag(v2f i) : SV_Target { fixed4 tex = tex2D(_MainTex, i.uv); return tex * i.color; }
            ENDCG
        }
    }
}