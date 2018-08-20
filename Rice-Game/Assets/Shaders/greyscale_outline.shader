Shader "Custom/grayscale_outline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
           _Color("Outline Color", Color) = (1.0,0.0,0.0,1.0)
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
        Fog { Mode Off }
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                half2 texcoord  : TEXCOORD0;
            };

            fixed4 _Color;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = mul(UNITY_MATRIX_MVP, IN.vertex);
                OUT.texcoord = IN.texcoord;
                OUT.color = IN.color ;

                return OUT;
            }

            sampler2D _MainTex;

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed threshold = 10.75;
                fixed radius = 0.001;
                fixed4 normal = tex2D(_MainTex, IN.texcoord);
                fixed4 accum = fixed4(0.0, 0.0, 0.0, 0.0);
                //fixed4 accum.r = 0.0;
                //fixed4 accum.g = 0.0;
                //fixed4 accum.b = 0.0;
                //fixed4 accum.a = 0.0;
                
                accum += tex2D(_MainTex, fixed2(IN.texcoord.x - radius, IN.texcoord.y - radius));
                accum += tex2D(_MainTex, fixed2(IN.texcoord.x + radius, IN.texcoord.y - radius));
                accum += tex2D(_MainTex, fixed2(IN.texcoord.x + radius, IN.texcoord.y + radius));
                accum += tex2D(_MainTex, fixed2(IN.texcoord.x - radius, IN.texcoord.y + radius));
                
             
                accum *= threshold;
                accum.rgb = _Color.rgb * accum.a;
                normal = ( accum * (1.0 - normal.a)) + (normal * normal.a);
                //fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                //c.rgb *= c.a;                
                //return c;
                fixed4 c = IN.color * normal;
                
			     normal.rgb = dot(normal.rgb, float3(0.3, 0.59, 0.11));
                return normal;

                
             
            }
        ENDCG
        }
    }
}