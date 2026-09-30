using Lepsima.Shaders.AutoExposure;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class AmbiantController : MonoBehaviour
{
    [SerializeField] public FogController fogController;
    [SerializeField] public SunController sunController;
    [SerializeField] Volume globalVolume;
    [SerializeField] bool useFog = true;
    public bool isDay = true;
    [SerializeField] UniversalRendererData rendererData;


    void Start()
    {
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
