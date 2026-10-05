
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GodRayController : MonoBehaviour
{
    [Header("Godray - Presets")]
    [SerializeField] GodRayPreset morningGodRay;
    [SerializeField] GodRayPreset noonGodRay;
    [SerializeField] GodRayPreset afternoonGodRay;
    [SerializeField] GodRayPreset eveningGodRay;
    [SerializeField] GodRayPreset sunsetGodRay;

    [Header("Godray - Presets State")]
    [SerializeField] GodRayPreset currentPreset;
    [SerializeField] GodRayPreset targetPreset;

    [Header("Settings - References")]
    DayNightCyleController sunController;
    [SerializeField] Material godRayMaterial;
    [SerializeField] UniversalRendererData rendererData;
    [SerializeField] float speed = 0.5f;
    float intensityBooster = 1f;
    float densityBooster = 1f;
    public float scatteringBooster = 0.5f;

    int morning;
    int noon;
    int afternoon;
    int evening;
    int sunset;


    void GetTime()
    {
        var t = TimeController.instance;
        morning = t.morning;
        noon = t.noon;
        afternoon = t.afternoon;
        evening = t.evening;
        sunset = t.sunset;
    }

    void Start()
    {
        GetTime();

        currentPreset = morningGodRay;
        targetPreset = morningGodRay;
        updateGodRays();

        sunController = GetComponent<DayNightCyleController>();

        if (sunController == null)
        {
            Debug.LogError("GodRayController: No SunController was found!");
            return;
        }
    }

    void Update()
    {
        float hour = TimeController.instance.getHour();

        if (hour >= sunset)
        {
            if (currentPreset != sunsetGodRay)
            {
                targetPreset = sunsetGodRay;
                currentPreset = targetPreset;
            }
        }
        else if (hour >= evening)
        {
            if (currentPreset != eveningGodRay)
            {
                targetPreset = eveningGodRay;
                currentPreset = targetPreset;
            }
        }
        else if (hour >= afternoon)
        {
            if (currentPreset != afternoonGodRay)
            {
                targetPreset = afternoonGodRay;
                currentPreset = targetPreset;
            }
        }
        else if (hour >= noon)
        {
            if (currentPreset != noonGodRay)
            {
                targetPreset = noonGodRay;
                currentPreset = targetPreset;
            }
        }
        else
        {
            if (currentPreset != morningGodRay)
            {
                targetPreset = morningGodRay;
                currentPreset = targetPreset;
            }
        }

        updateGodRays();
    }

    public float GetIntensityBooster()
    {
        return intensityBooster;
    }

    public float GetDensityBooster()
    {
        return intensityBooster;
    }

    public void SetIntensityBooster(float value)
    {
        intensityBooster = value;
    }

    public void SetDensityBooster(float value)
    {
        densityBooster = value;
    }

    void OnDisable()
    {
        godRayMaterial.SetFloat("_MaxDistance", 16);
        godRayMaterial.SetFloat("_Steps", 16);
        godRayMaterial.SetFloat("_Density", 0);
        godRayMaterial.SetFloat("_RayIntensity", 0);
        godRayMaterial.SetFloat("_ForwardScatter", 0);
        godRayMaterial.SetFloat("_SkyRayAmount", 0);
    }
    
    public void updateGodRays()
    {
        float density = Mathf.Lerp(godRayMaterial.GetFloat("_Density"), targetPreset._Density * densityBooster, speed * Time.deltaTime);
        float rayIntensity = Mathf.Lerp(godRayMaterial.GetFloat("_RayIntensity"), targetPreset._RayIntensity * intensityBooster, speed * Time.deltaTime);
        float forwardScattering = Mathf.Lerp(godRayMaterial.GetFloat("_ForwardScatter"), scatteringBooster, speed * Time.deltaTime);
        float rayOnSky = Mathf.Lerp(godRayMaterial.GetFloat("_SkyRayAmount"), targetPreset._RayOnSky, speed * Time.deltaTime);
        Color rayColor = Color.Lerp(godRayMaterial.GetColor("_RayColor"), targetPreset._RayColor, speed * Time.deltaTime);


        godRayMaterial.SetFloat("_MaxDistance", targetPreset._RayMaxDistance);
        godRayMaterial.SetFloat("_Steps", targetPreset._Steps);
        godRayMaterial.SetFloat("_Density", density);
        godRayMaterial.SetFloat("_RayIntensity", rayIntensity);
        godRayMaterial.SetFloat("_ForwardScatter", forwardScattering);
        godRayMaterial.SetFloat("_SkyRayAmount", rayOnSky);
        godRayMaterial.SetColor("_RayColor", rayColor);
    }
}
