Shader "Custom/Outline_v2"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
           _OutLineSpreadX ("Outline Spread", Range(0,0.03)) = 0.007
           _OutLineSpreadY ("Outline Spread", Range(0,0.03)) = 0.007
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
           float _OutLineSpreadX;
           float _OutLineSpreadY;

            fixed4 frag(v2f IN) : SV_Target
            {
                //fixed4 c = tex2D(_MainTex, IN.texcoord) * IN.color;
                //c.rgb *= c.a;                
                //return c;
                
               fixed4 TempColor = tex2D(_MainTex, IN.texcoord+float2(_OutLineSpreadX,0.0)) + tex2D(_MainTex, IN.texcoord-float2(_OutLineSpreadX,0.0));
               TempColor = TempColor + tex2D(_MainTex, IN.texcoord+float2(0.0,_OutLineSpreadY)) + tex2D(_MainTex, IN.texcoord-float2(0.0,_OutLineSpreadY));
               if(TempColor.a > 0.1){
                   TempColor.a = 1;
               }
               
               fixed4 AlphaColor = fixed4(TempColor.a, TempColor.a, TempColor.a, TempColor.a);
               fixed4 mainColor = AlphaColor * _Color.rgba;
               fixed4 addcolor = tex2D(_MainTex, IN.texcoord) * IN.color;
    
               if(addcolor.a > 0.95){
                   mainColor = addcolor;
               }
    
               //o.Albedo = mainColor.rgb;
               //o.Alpha = mainColor.a;
               
               //mainColor.rgb *= mainColor.a;
               return mainColor;
                
             
            }
        ENDCG
        }
    }
}