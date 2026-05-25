Shader "FX/Hologram Shader"
{
    Properties
    {
        [HDR] _Color("Color", Color) = (0, 1, 1, 1) // Flagged for HDR
        _Brightness("Overall Brightness", Range(1.0, 10.0)) = 2.0 // Global multiplier
        _MainTex("Base (RGB)", 2D) = "white" {}
        _AlphaTexture ("Alpha Mask (R)", 2D) = "white" {}
        _Scale ("Alpha Tiling", Float) = 3
        _ScrollSpeedV("Alpha scroll Speed", Range(0, 5.0)) = 1.0
        _GlowIntensity ("Glow Intensity", Range(0.01, 1.0)) = 0.5
        _GlitchSpeed ("Glitch Speed", Range(0, 50)) = 50.0
        _GlitchIntensity ("Glitch Intensity", Range(0.0, 0.1)) = 0
    }

    SubShader
    {
        Tags{ "Queue" = "Overlay" "IgnoreProjector" = "True" "RenderType" = "Transparent" }

        Pass
        {
            Lighting Off 
            ZWrite On
            Blend SrcAlpha One
            Cull Back

            CGPROGRAM
                #pragma vertex vertexFunc
                #pragma fragment fragmentFunc
                #include "UnityCG.cginc"

                struct appdata{
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    float3 normal : NORMAL;
                };

                struct v2f{
                    float4 position : SV_POSITION;
                    float2 uv : TEXCOORD0;
                    float3 grabPos : TEXCOORD1;
                    float3 viewDir : TEXCOORD2;
                    float3 worldNormal : NORMAL;
                };

                fixed4 _Color, _MainTex_ST;
                sampler2D _MainTex, _AlphaTexture;
                half _Scale, _ScrollSpeedV, _GlowIntensity, _GlitchSpeed, _GlitchIntensity, _Brightness;

                v2f vertexFunc(appdata IN){
                    v2f OUT;
                    OUT.position = UnityObjectToClipPos(IN.vertex);
                    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                    OUT.grabPos = UnityObjectToViewPos(IN.vertex);
                    OUT.grabPos.y += _Time * _ScrollSpeedV;
                    OUT.worldNormal = UnityObjectToWorldNormal(IN.normal);
                    OUT.viewDir = normalize(UnityWorldSpaceViewDir(OUT.grabPos.xyz));
                    return OUT;
                }

                fixed4 fragmentFunc(v2f IN) : SV_Target{
                    float glitchOffset = sin(_Time.y * _GlitchSpeed + IN.uv.y * 20.0) * _GlitchIntensity;
                    
                    float2 glitchedUV = IN.uv;
                    glitchedUV.x += glitchOffset;

                    fixed4 alphaColor = tex2D(_AlphaTexture, IN.grabPos.xy * _Scale);
                    fixed4 pixelColor = tex2D (_MainTex, glitchedUV);
                    pixelColor.a = alphaColor.a; 

                    half rim = 1.0 - saturate(dot(IN.viewDir, IN.worldNormal));

                    // Uniform scaling applied here
                    return pixelColor * _Color * (rim + _GlowIntensity) * _Brightness;
                }
            ENDCG
        }
    }
}