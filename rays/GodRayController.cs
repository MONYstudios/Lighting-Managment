
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class GodRayController : MonoBehaviour
{
    [SerializeField] GodRayPreset morningGodRay;
    [SerializeField] GodRayPreset noonGodRay;
    [SerializeField] GodRayPreset afternoonGodRay;
    [SerializeField] GodRayPreset eveningGodRay;
    [SerializeField] GodRayPreset sunsetGodRay;
    [SerializeField] GodRayPreset currentPreset;
    [SerializeField] GodRayPreset targetPreset;
    DayNightCyleController sunController;
    [SerializeField] Material godRayMaterial;
    [SerializeField] UniversalRendererData rendererData;
    [SerializeField] int morning = 9;
    [SerializeField] int noon = 11;
    public TimeController timeController;
    [SerializeField] float speed = 0.5f;
    

    void Start()
    {
        currentPreset = morningGodRay;
        targetPreset = morningGodRay;
        updateFog();

        sunController = GetComponent<DayNightCyleController>();

        if (sunController == null)
        {
            Debug.LogError("GodRayController: No SunController was found!");
            return;
        }
    }

    void Update()
    {
        if (timeController.currentHour <= morning)
        {
            if (currentPreset != morningGodRay)
            {
                targetPreset = morningGodRay;
            }
        }
        else if (timeController.currentHour <= noon)
        {
            if (currentPreset != noonGodRay)
            {
                targetPreset = noonGodRay;
            }
        }
        updateFog();
    }

    public void updateFog()
    {
        float density = Mathf.Lerp(godRayMaterial.GetFloat("_Density"), targetPreset._Density, speed * Time.deltaTime);
        float rayIntensity = Mathf.Lerp(godRayMaterial.GetFloat("_RayIntensity"), targetPreset._RayIntensity, speed * Time.deltaTime);
        float forwardScattering = Mathf.Lerp(godRayMaterial.GetFloat("_ForwardScatter"), targetPreset._ForwardScattering, speed * Time.deltaTime);
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
