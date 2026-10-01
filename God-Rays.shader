Shader "MONYstudio/GodRaysTest"
{
    Properties
    {
        _MaxDistance("Max Distance", Range(10,200)) = 100
        _Steps("Raymarch Schritte", Range(16,128)) = 64
        _Density("Dichte", Range(0,0.1)) = 0.02
        _RayIntensity("Strahl Intensität", Range(0, 2)) = 1
        _ForwardScatter("Vorwärts Streuung (g)", Range(0, 0.99)) = 0.75
        _RayColor("Ray Color", Color) = (1.0, 0.92, 0.75, 1)
        _SkyRayAmount("Rays auf Himmel", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags { "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            ZTest Always
            ZWrite Off
            Cull Off
            Blend Off

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment FogFragment

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            float _MaxDistance;
            float _Steps;
            float _Density;
            float _RayIntensity;
            float _ForwardScatter;
            float4 _RayColor;
            float _SkyRayAmount;
            
            // -- Short Description --
            // Scattering meaning = More Sunlight means more Raystrenght

            float Scattering(float angleCos, float g)
            {
                float ghoch2 = g*g;
                float bottom = 1 + ghoch2 - 2 * g * angleCos;
                float top = 1 - ghoch2;

                // Hier wird die Finale Variable erstellt damit wir mehr Übersicht haben
                // max = schaut das der Wert niemals unter den 2ten Parameter gehen kann!
                // pow = nimmt den ersten parameter und nimmt ihn mal hoch 1.5
                float final = 12.57 * pow(max(bottom, 0.0001), 1.5);
                
                // Rechnet den Bruch 
                return top / final;
            }

            half4 FogFragment(Varyings input) : SV_TARGET
            {
                // Holt sich die UV-Koordinaten des Pixels
                half2 uv = input.texcoord;

                // Es wird eine Texture erstellt, folgender Ablauf:
                // _BlitTexture = gerendertes Bild der Scene
                // sampler_LinearClamp = normaler übergang 
                // UV bestimmt die Position?
                half4 screen_color = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, uv);

                float brightness = (screen_color.r + screen_color.g + screen_color.b) / 3;
                float factor = 1 - brightness;
                // Holt sich die Tiefeninfomationen des angebenen Parameters
                float depth = SampleSceneDepth(uv);

                // Holt sich die Position in der Welt anhand:
                // UV-Koordinate, Tiefeninfomation, UNITY_MATRIX_I_VP 
                
                // Erklärung zu UNITY_MATRIX_I_VP:
                // VP = View x Projection
                // I = Inverse
                // Quasi Inverse-View-Projection-Matrix
                float3 worldPosition = ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);
                
                // Unitys-Intigierte Variable (_WorldSpaceCameraPos)
                float3 cameraPosition = _WorldSpaceCameraPos;

                // Erstellt eine Variable um die Position des Pixels zu ermitteln
                float3 cameraToPixel = worldPosition - cameraPosition;
                
                // Erstellt einen Float der die Länge beinhaltet
                float rayLength = length(cameraToPixel);

                // Erstellt einen Float3 der die Richtung des Ray hält
                // Verhindert eine Division durch 0, indem der Nenner mindestens 0.001 beträgt
                float3 rayDirection = cameraToPixel / max(rayLength, 0.001);

                // Berechnet die Tatsächliche länge des God-Rays
                // Wir nutzen min um den kleineren Wert der Parameter zu nutzen
                float marchDistance = min(rayLength, _MaxDistance);
                
                // Macht aus einem Float einen int
                int stepCount = (int) _Steps;

                // Erstellt einen leeren Float
                float lightAmount = 0.0;
                
                // Erstellt stepSize ahander der Disivision von marchDistance / stepCount
                float stepSize = marchDistance / stepCount;
                
                // Sagt / Deklariert den dem Compilor das es ein Loop ist
                [loop]
                for (int i = 0; i < stepCount; i++)
                {
                    // Erstellt eine Variable wo ein neuer Punkt für den Ray berechnet wird
                    // cameraPosition = Startpunkt
                    // rayDirection = Richtung des Rays
                    // i * stepSize = bisher zurück gelegte Strecke
                    float3 samplePos = cameraPosition + rayDirection * (i * stepSize);
                    
                    // Hier wird die shadowCoordinate kreiert 
                    float4 shadowCoord = TransformWorldToShadowCoord(samplePos);
                    
                    // Nutzt von Unity vorgegebene Funktion um die Sonne zu deklarieren
                    Light sun = GetMainLight(shadowCoord);
                    
                    // Fügt der Licht Variable folgendes hinzu:
                    // sun.shadowAttenuation (0 = Schatten, 1 = Licht)
                    // _Density
                    // stepSize
                    lightAmount += sun.shadowAttenuation * _Density * stepSize;
                }
                
                // Holt sich das main Light
                Light mainLight = GetMainLight();

                // Berechnet den Wert von dem Winkel zur Sonne anhand des Rays und der Direction des MainLights
                float angleToSun = dot(rayDirection, mainLight.direction);
                
                // Erstellt Scatter Variable durch Hilfs-Funktion
                float scatter = Scattering(angleToSun, _ForwardScatter);
                
                // Berechnet die finale Farbe des God-Rays
                // Lichtfarbe * Sonnenfarbe * Intensität * Streuuung * Lichtmenge
                float3 rays = mainLight.color.rgb * _RayColor.rgb * _RayIntensity * scatter * lightAmount;
                rays *= factor;

                // Himmel erkennen (keine Geometrie, Tiefe = Far Plane)
#if UNITY_REVERSED_Z
    bool isSky = depth <= 0.0001;
#else
    bool isSky = depth >= 0.9999;
#endif

rays *= isSky ? _SkyRayAmount : 1.0;

                // Hier wird die normale Farbe es Screens mit den God-Rays gemischt
                // Jedoch funktioniert es nur wenn man es als half4 deklariert und einen Alpha Wert hinzu gibt!
                half4 result = half4(screen_color.rgb + rays,1);
                
                // Gibt die Resultate zurück!
                return result;
            }
            ENDHLSL
        }
    }  
}
