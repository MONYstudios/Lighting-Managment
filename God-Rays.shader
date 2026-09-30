Shader "Tutorial/Selbst"
{
    // Inspector 
    // (Die Namen hier bleiben absichtlich gleich wie vorher,
    //  sonst verliert dein Material seine eingestellten Werte)
    Properties
    {
        [Header(Allgemein)]
        _MaxDistance("Max Distance", Float) = 150
        _Steps("Raymarch Schritte", Range(16,128)) = 64
        _JitterStrenght("Jitter", Range(0,1)) = 1


        [Header(Nebel Dichte)]
        _Density("Dichte", Range(0,1)) = 0.008
        _HeightBase("Nebel Höhe", Float) = 0
        _HeightFalloff("Höhen Abfall", Range(0,1)) = 0.011
        _AmbientColor("Nebelfarbe", Color) = (0.55, 0.65, 0.8, 1)
        _AmbientAmount("Nebelstärke", Range(0,2)) = 0.786

        [Header(Noise (optional))]
        _FogNoise("3D Noise", 3D) = "white" {}
        _NoiseTiling("Noise tiling", Float) = 1
        _NoiseStrength("Noise Stärke", Range(0, 1)) = 0.6
        _NoiseThreshold("Noise Threshold", Range(0, 0.95)) = 0.25
        _FogSpeed("Fog Speed", Vector) = (0.03, 0.01, 0.02, 0)

        [Header(God Rays)]
        // HDR um Farbpicker zu erstellen 
        [HDR] _SunTint("Sonnen Tint", Color) = (1.0, 0.92, 0.75, 1)
        _RayIntensity("Strahl Intensität", Range(0, 50)) = 8
        _ForwardScatter("Vorwärts Streuung (g)", Range(0, 0.99)) = 0.75
        _BackScatter("Rückwärts Streuung (g)", Range(-0.99, 0)) = -0.25
        _BackWeight("Rückwärts Anteil", Range(0, 1)) = 0.2
        _ShadowSharpness("Schatten Härte", Range(0.5, 8)) = 2
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass 
        {
            ZTest Always
            ZWrite Off // Keine Tiefe zeichnen
            Cull Off 
            Blend Off

            HLSLPROGRAM

            // "Vert" kommt fertig von Unity (Blit.hlsl), die Fragment-Funktion ist unsere eigene
            #pragma vertex Vert
            #pragma fragment FogFragment

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"


            // ------------------------------------------------
            // Variablen aus dem Inspector
            // ------------------------------------------------
            float _MaxDistance;
            float _Steps;
            float _JitterStrenght;

            float _Density;
            float _HeightBase;
            float _HeightFalloff;
            float4 _AmbientColor;
            float _AmbientAmount;

            TEXTURE3D(_FogNoise);
            float _NoiseTiling;
            float _NoiseStrength;
            float _NoiseThreshold;
            float4 _FogSpeed;

            float4 _SunTint;
            float _RayIntensity;
            float _ForwardScatter;
            float _BackScatter;
            float _BackWeight;
            float _ShadowSharpness;


            // Funktion um Zahlen von 0 - 1 zu limitieren
            // saturate
            float LimitToZeroOneRange(float value)
            {
                if (value < 0.0)
                {
                    return 0.0;
                }
                if (value > 1.0)
                {
                    return 1.0;
                }
                return value;
            }

            // Erstellt mittelwert
            float BlendBetween(float valueA, float valueB, float amount)
            {
                return valueA + (valueB - valueA) * amount;
            }


            float CalculateLightScattering(float angleCosine, float scatterDirection)
            {
                // Bruch mit bottomPart (unten) & topPart (oben)

                // hoch 2 rechnung (x * x)
                float scatterSquared = scatterDirection * scatterDirection;

                // Untererteil des Bruchs
                // Klein wenn man zur Sonne schaut
                // Groß wenn man zur Sonne wegschaut
                float bottomPart = 1.0 + scatterSquared - 2.0 * scatterDirection * angleCosine;

                // Vermeidet 0, weil man damit nicht multiplizieren kann!
                if (bottomPart < 0.0001)
                {
                    bottomPart = 0.0001;
                }
                
                // Oberer Teil des Bruchs
                float topPart = 1.0 - scatterSquared;
                
                // pow(bottomPart, 1.5) = bottomPart Hoch 1.5
                // 4.0 * PI = ca 12
                // topPart geteilt durch den ganzen nenner gerechnet
                float scattering = topPart / (12.57 * pow(bottomPart, 1.5));
                return scattering;
            }


            // Sonnenstrahlen Stärke abhängig von Blickrichtung
            float GetSunScatteringAmount(float angleCosine)
            {
                // Holt sich Blickrichtungen
                float forwardScattering = CalculateLightScattering(angleCosine, _ForwardScatter);
                float backwardScattering = CalculateLightScattering(angleCosine, _BackScatter);
                
                // Mach einen Mittelwert daraus
                float mixedScattering = BlendBetween(forwardScattering, backwardScattering, _BackWeight);
                return mixedScattering;
            }


            
            float GetFogDensityAtPosition(float3 worldPosition)
            {
                // wiederholt sich alle 1.0 = 1 Meter
                //  wiederholt sich alle 0.01 = 100 Meter
                // Rechnet um auf kleinere Zahlen
                float3 noisePosition = worldPosition * 0.01 * _NoiseTiling;
                
                // _Time.y = Zeit seit Start
                // FogSpeed = Verschiebung pro Sekunde
                noisePosition = noisePosition + _FogSpeed.xyz * _Time.y;
                
                // Wert aus Texture lesen
                // sampler_TrilinearRepeat = sanft gemischt
                // noisePosition = Stelle lesen
                // 0 = Detailierungs-Stufe (0 = Volle Auflösung)
                // .r = roter Kanal (0 Dukel | 1 Hell) <- Wird in NoiseValue gesafed
                float noiseValue = SAMPLE_TEXTURE3D_LOD(_FogNoise, sampler_TrilinearRepeat, noisePosition, 0).r;
                
                // Wie weit der Wert unter der Schwelle liegt
                float noiseAboveThreshold = noiseValue - _NoiseThreshold;
                
                // Wie viel Plat ist über der Schwelle bis Maximum 1?
                float noiseRange = 1.0 - _NoiseThreshold;

                // Limitiert von 0 - 1
                float shapedNoise = LimitToZeroOneRange(noiseAboveThreshold / noiseRange);
                
                // Wird genutzt um die Stärke zu bestimmen
                float noiseFactor = BlendBetween(1.0, shapedNoise, _NoiseStrength);
                
                // Dichte * Stärke
                float fogDensity = _Density * noiseFactor;
                // Gibt fertig berechnete Dichte zurück
                return fogDensity;
            }


            // half4 bedeutet:
            // R,G,B,A
            half4 FogFragment(Varyings input) : SV_Target
            {
                // Holt sich die Screenposition
                // Textcoord = 0 bis 1
                float2 screenPosition = input.texcoord;
                
                // Holt sich die originale Farbe
                // _BlitTexture = Fertige Bild ohne Shader
                // sampler_LinearClamp = ?
                // screenPosition wird genutzt um zu ermitteln was gemeint ist
                float4 originalSceneColor = SAMPLE_TEXTURE2D(_BlitTexture, sampler_LinearClamp, screenPosition);

                // Wie weit das Objekt wirklich entfernt ist
                float sceneDepth = SampleSceneDepth(screenPosition);

                // Holt sich die position des Pixels
                // screenPosition = stelle auf dem Monitor
                // sceneDepth = tiefe auf dieser Stelle
                // UNITY_MATRIX_I_VP = lol kanns nicht erklären
                float3 pixelWorldPosition = ComputeWorldSpacePosition(screenPosition, sceneDepth, UNITY_MATRIX_I_VP);

                // Kamera Position = Richtige Position
                float3 cameraPosition = _WorldSpaceCameraPos;
                
                // Holt sich die Position des Ziel Pixels
                float3 cameraToPixel = pixelWorldPosition - cameraPosition;

          
                float rayLength = sqrt(cameraToPixel.x * cameraToPixel.x + cameraToPixel.y * cameraToPixel.y + cameraToPixel.z * cameraToPixel.z);

                // Teilen durch 0 verhindern
                float safeRayLength = rayLength;
                if (safeRayLength < 0.00001)
                {
                    safeRayLength = 0.00001;
                }

                float3 rayDirection = cameraToPixel / safeRayLength;

                float marchDistance = rayLength;
                if (marchDistance > _MaxDistance)
                {
                    marchDistance = _MaxDistance;
                }

               
                int stepCount = (int)_Steps;
                float stepSize = marchDistance / stepCount;

         
                float2 screenPixelSize = _BlitTexture_TexelSize.zw;
                float2 pixelCoordinates = screenPosition * screenPixelSize;
                float jitter = InterleavedGradientNoise(pixelCoordinates, 0) * _JitterStrenght;

                Light sun = GetMainLight();

             
                float angleToSun = dot(rayDirection, sun.direction);
                float sunScattering = GetSunScatteringAmount(angleToSun);

                float3 sunColor = sun.color.rgb * _SunTint.rgb * _RayIntensity;

                float transmittance = 1.0;                 // 1 = Bild voll sichtbar, 0 = komplett Nebel
                float3 collectedFogLight = float3(0, 0, 0); // gesammeltes Licht im Nebel

                [loop]
                for (int stepIndex = 0; stepIndex < stepCount; stepIndex++)
                {
                    float distanceAlongRay = (stepIndex + jitter) * stepSize;

                    float3 samplePosition = cameraPosition + rayDirection * distanceAlongRay;

                    float fogDensity = GetFogDensityAtPosition(samplePosition);

                    if (fogDensity > 0.0001)
                    {
                        float4 shadowCoordinate = TransformWorldToShadowCoord(samplePosition);
                        Light sunWithShadow = GetMainLight(shadowCoordinate);

                        float shadowAmount = LimitToZeroOneRange(sunWithShadow.shadowAttenuation);
                        shadowAmount = pow(shadowAmount, _ShadowSharpness);

                        float3 directSunLight = sunColor * sunScattering * shadowAmount;
                        float3 ambientLight = _AmbientColor.rgb * _AmbientAmount;
                        float3 totalLight = directSunLight + ambientLight;

                        collectedFogLight = collectedFogLight + transmittance * totalLight * fogDensity * stepSize;

                        transmittance = transmittance * exp(-fogDensity * stepSize);

                        if (transmittance < 0.01)
                        {
                            break;
                        }
                    }
                }

                float3 finalColor = originalSceneColor.rgb * transmittance + collectedFogLight;

                return half4(finalColor, originalSceneColor.a);
            }

            ENDHLSL
        }
    }
}
