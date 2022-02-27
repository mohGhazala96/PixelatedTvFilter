Shader "InvertColor"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Threshold ("Threshold", float) = 0
        _Color("Color", Color) = (1,1,1,1)

    }
    SubShader
    {
        Tags {"Queue"="Transparent" "RenderType"="Transparent"}
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
 
        Pass
        {
            CGPROGRAM
            #pragma vertex vert_img
            #pragma fragment frag
           
            #include "UnityCG.cginc"
 
            sampler2D _MainTex;
            float _Threshold;
            float4 _Color;

 
            fixed4 frag (v2f_img i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                col.rgb = abs(_Threshold - _Color.rgb)*2;
                return col;
            }
            ENDCG
        }
    }
}
