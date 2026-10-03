
using UnityEngine;

public class FogController : MonoBehaviour
{
    [SerializeField] FogMode fogMode = FogMode.ExponentialSquared;
    [SerializeField] float fogDensity = 0.01f;
    [SerializeField] bool useFog = true;
    [SerializeField] float fogDensityNight = 0.02f;
    [SerializeField] Color fogColor = Color.gray;

    [SerializeField] bool isDebuggingMode = false;

    void Start()
    {
        fogDensity = 0f;
        ApplyFog();
    }

    public void EnableFog()
    {
        useFog = true;
        ApplyFog();
    }

    public void DisableFog()
    {
        useFog = false;
        RenderSettings.fog = false;
    }

    void Update()
    {
        NightTimeFog();
    }
    public void ApplyFog()
    {
        RenderSettings.fog = useFog;

        if (!useFog)
            return;

        RenderSettings.fogColor = fogColor;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogDensity = fogDensity;

        if (isDebuggingMode)
            Debug.Log("AmbientController: Fog angewendet!");
    }
    
    public void NightTimeFog()
    {
        if (TimeController.instance.currentHour >= 9f)
        {
            RenderSettings.fogDensity = Mathf.Lerp(RenderSettings.fogDensity,fogDensityNight, Time.deltaTime);   
        }   
    }
}
