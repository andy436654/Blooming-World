Shader "UI/StrongBlur"
{
    Properties 
    {
        _Color ("Tint", Color) = (1,1,1,0.2)
        _BlurSize ("Blur Size", Range(0, 0.15)) = 0.15
        _Iterations ("Iterations", Range(1, 4)) = 2
    }
    
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        
        GrabPass { "_BlurTexture" }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_horizontal
            #include "UnityCG.cginc"
            
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };
            
            v2f vert(appdata_base v) 
            {
                v2f o; 
                o.pos = UnityObjectToClipPos(v.vertex); 
                o.uv = ComputeGrabScreenPos(o.pos); 
                return o; 
            }
            
            sampler2D _BlurTexture;
            float _BlurSize;
            int _Iterations;
            
            fixed4 frag_horizontal(v2f i) : SV_Target 
            {
                float4 sum = 0;
                float weights[5] = {0.0545, 0.2442, 0.4026, 0.2442, 0.0545};
                float offsets[5] = {-2.0, -1.0, 0.0, 1.0, 2.0};
                
                for (int iter = 0; iter < _Iterations; iter++)
                {
                    float iterFactor = (iter + 1) * 0.5;
                    for (int j = 0; j < 5; j++)
                    {
                        sum += tex2Dproj(_BlurTexture, i.uv + float4(_BlurSize * offsets[j] * iterFactor, 0, 0, 0)) * weights[j];
                    }
                }
                
                return sum / _Iterations;
            }
            ENDCG
        }
        
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag_vertical
            #include "UnityCG.cginc"
            
            struct v2f 
            {
                float4 pos : SV_POSITION;
                float4 uv : TEXCOORD0;
            };
            
            v2f vert(appdata_base v) 
            {
                v2f o; 
                o.pos = UnityObjectToClipPos(v.vertex); 
                o.uv = ComputeGrabScreenPos(o.pos); 
                return o; 
            }
            
            sampler2D _BlurTexture;
            float _BlurSize;
            fixed4 _Color;
            int _Iterations;
            
            fixed4 frag_vertical(v2f i) : SV_Target 
            {
                float4 sum = 0;
                float weights[5] = {0.0545, 0.2442, 0.4026, 0.2442, 0.0545};
                float offsets[5] = {-2.0, -1.0, 0.0, 1.0, 2.0};
                
                for (int iter = 0; iter < _Iterations; iter++)
                {
                    float iterFactor = (iter + 1) * 0.5;
                    for (int j = 0; j < 5; j++)
                    {
                        sum += tex2Dproj(_BlurTexture, i.uv + float4(0, _BlurSize * offsets[j] * iterFactor, 0, 0)) * weights[j];
                    }
                }
                return sum / _Iterations * _Color;
            }
            ENDCG
        }
    }
}