using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

[ExecuteInEditMode]
public class AmbiantManager : MonoBehaviour
{
    [SerializeField] public FogController fogController;
    [SerializeField] public DayNightCyleController sunController;
    [SerializeField] bool useFog = true;
    public bool isDay = true;
    [SerializeField] UniversalRendererData rendererData;
    [SerializeField] Material fogMaterial;
    [SerializeField] FogPreset fogPreset;
    
    void Start()
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == "FullScreenPassRendererFeature")
            {
                feature.SetActive(true);
            }
        }
        
        isDay = true;
        Debug.Log($"AmbientController: Wichtig isDay {isDay}");
    }

    public void toggleGodRays()
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == "FullScreenPassRendererFeature")
            {
                useFog = !useFog;
                Debug.Log($"AmbiantController: Fog-Status {useFog}");

                feature.SetActive(useFog);                
            }
        }       
    }
}
