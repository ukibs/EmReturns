Shader "Unlit/RetroShader"
{
    Properties
    {
        _LineColor("Line Color", Color) = (0, 1, 1, 1)
        _BackgroundColor("Background Color", Color) = (0, 0, 0, 1)
        _GridSpacing("Grid Spacing", Float) = 0.1
        _LineThickness("Line Thickness", Range(0.001, 0.2)) = 0.01
        _ScrollSpeed("Scroll Speed", Float) = 0.2
        _UVScale("UV Scale", Float) = 10.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Background" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv     : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 pos : SV_POSITION;
            };

            fixed4 _LineColor;
            fixed4 _BackgroundColor;
            float _GridSpacing;
            float _LineThickness;
            float _ScrollSpeed;
            float _UVScale;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _UVScale;
                return o;
            }

            // produce a 0..1 mask for a line at multiples of 'spacing'
            float lineMask(float coord, float spacing, float thickness)
            {
                float f = abs(frac(coord / spacing) - 0.5) * spacing;
                float halfT = thickness * 0.5;
                // use smoothstep to avoid hard aliasing; small epsilon for crispness
                return saturate(1.0 - smoothstep(halfT, halfT + 0.005, f));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                // _Time.y = seconds (Unity built-in)
                uv.y += _Time.y * _ScrollSpeed;

                float lx = lineMask(uv.x, _GridSpacing, _LineThickness);
                float ly = lineMask(uv.y, _GridSpacing, _LineThickness);
                float grid = max(lx, ly);

                return lerp(_BackgroundColor, _LineColor, grid);
            }
            ENDCG
        }
    }

    FallBack "Unlit/Color"
}
