Shader "Unlit/RetroShader"
{
Properties
    {
        _LineTop("Line Top Color", Color) = (0, 1, 1, 1)
        _LineBottom("Line Bottom Color", Color) = (0, 0.5, 1, 1)
        _BackgroundTop("Background Top", Color) = (0, 0, 0.2, 1)
        _BackgroundBottom("Background Bottom", Color) = (0, 0, 0, 1)
        _GridSpacing("Grid Spacing", Float) = 0.1
        _LineThickness("Line Thickness", Range(0.001, 0.2)) = 0.01
        _ScrollSpeed("Scroll Speed", Float) = 0.2
        _UVScale("UV Scale", Float) = 10.0
        _Tilt("Tilt Amount", Range(-1.0, 1.0)) = 0.5
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

            fixed4 _LineTop;
            fixed4 _LineBottom;
            fixed4 _BackgroundTop;
            fixed4 _BackgroundBottom;
            float _GridSpacing;
            float _LineThickness;
            float _ScrollSpeed;
            float _UVScale;
            float _Tilt;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv * _UVScale;
                return o;
            }

            float lineMask(float coord, float spacing, float thickness)
            {
                float f = abs(frac(coord / spacing) - 0.5) * spacing;
                float halfT = thickness * 0.5;
                return saturate(1.0 - smoothstep(halfT, halfT + 0.005, f));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;

                // Transformación para inclinar/perspectiva
                uv.y += _Time.y * _ScrollSpeed;
                uv.y += (uv.x - 0.5) * _Tilt * _UVScale; // efecto de inclinación

                // Grid
                float lx = lineMask(uv.x, _GridSpacing, _LineThickness);
                float ly = lineMask(uv.y, _GridSpacing, _LineThickness);
                float grid = max(lx, ly);

                // Gradiente de fondo
                float t = saturate(i.uv.y); // UV original 0..1
                fixed4 bg = lerp(_BackgroundBottom, _BackgroundTop, t);

                // Gradiente de línea
                fixed4 lineColor = lerp(_LineBottom, _LineTop, t);

                // Mezclar línea sobre fondo
                return lerp(bg, lineColor, grid);
            }
            ENDCG
        }
    }

    FallBack "Unlit/Color"
}
