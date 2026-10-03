using UnityEngine;

[ExecuteInEditMode]
public class DayNightCyleColorController : MonoBehaviour
{
    [Header("Settings - References")]
    [SerializeField] AmbiantManager ambiantController;
    [SerializeField] Material skyboxRawMaterial;

    [Header("Settings - SunPresets")]
    [SerializeField] SunPreset sunMorningPreset;
    [SerializeField] SunPreset sunNoonPreset;
    [SerializeField] SunPreset sunAfternoonPreset;
    [SerializeField] SunPreset sunEveningPreset;
    [SerializeField] SunPreset sunsetPreset;


    [Header("Settings - MoonPresets")]
    [SerializeField] SunPreset MoonPreset;

    [Header("Settings - SkyPresets")]
    [SerializeField] SkyPreset skyMorningPreset;
    [SerializeField] SkyPreset skyNoonPreset;
    [SerializeField] SkyPreset skyAfternoonPreset;
    [SerializeField] SkyPreset skyEveningPreset;
    [SerializeField] SkyPreset skySunsetPreset;
    [SerializeField] SkyPreset skyNightPreset;

    [Header("Presets - Current Values")]
    [SerializeField] SunPreset currentSunPreset;
    [SerializeField] SunPreset lastSunPreset;
    [SerializeField] SkyPreset currentSkyPreset;
    [SerializeField] SkyPreset lastSkyPreset;

    [Header("Settings - Other")]
    [SerializeField] float replaceRegion = 150f;
    [SerializeField] getMainLightDirection getMainLightDirection;
    
    
    void Start()
    {
        RenderSettings.skybox = skyboxRawMaterial;
        getMainLightDirection.skyboxMaterial = skyboxRawMaterial;
        
        currentSunPreset = sunMorningPreset;
        currentSkyPreset = skyMorningPreset;

        lastSkyPreset = null;
        lastSunPreset = null;
        setSun();
    }


    void Update()
    {
        if (ambiantController.isDay)
        {
            setAmbientDayTime();
            updateSunBasedOnTime();
            return;
        }
        setAmbientNightTime();
    }

    public void setMoon()
    {
        skyboxRawMaterial.SetFloat("_SunSize", currentSunPreset.sunSize);
        skyboxRawMaterial.SetColor("_suncolor", currentSunPreset.sunColor);

        //ambiantController.sunController.currentRotation = 0f;

        // Accutall Light
        ambiantController.sunController.sun.intensity = currentSunPreset.sunLightIntensity;

    }

    void setSun()
    {
        // Helps to block wrong rotations
        if (ambiantController.sunController.currentRotation >= replaceRegion) return;

        skyboxRawMaterial.SetFloat("_SunSize", currentSunPreset.sunSize);
        skyboxRawMaterial.SetColor("_suncolor", currentSunPreset.sunColor);

        //ambiantController.sunController.currentRotation = 0f;
        ambiantController.sunController.sun.intensity = currentSunPreset.sunLightIntensity;
    }

    void updateSunBasedOnTime()
    {
        float rotation = ambiantController.sunController.currentRotation;

        Color sunColor;
        float sunLightIntensity;

        lastSunPreset = null;
        currentSunPreset = sunMorningPreset;

        if (rotation < 50f)
        {
            currentSunPreset = sunNoonPreset;
            lastSunPreset = sunMorningPreset;

            sunColor = LerpColor(lastSunPreset.sunColor, currentSunPreset.sunColor, rotation, 0f, 50f);
            sunLightIntensity = currentSunPreset.sunLightIntensity;
        }

        else if (rotation < 90f)
        {
            currentSunPreset = sunAfternoonPreset;
            lastSunPreset = sunNoonPreset;

            sunColor = LerpColor(lastSunPreset.sunColor, currentSunPreset.sunColor, rotation, 0f, 50f);
            sunLightIntensity = currentSunPreset.sunLightIntensity;
        }

        else if (rotation < 150f)
        {
            currentSunPreset = sunEveningPreset;
            lastSunPreset = sunAfternoonPreset;

            sunColor = LerpColor(lastSunPreset.sunColor, currentSunPreset.sunColor, rotation, 0f, 50f);
            sunLightIntensity = currentSunPreset.sunLightIntensity;
        }

        else
        {
            currentSunPreset = sunsetPreset;
            lastSunPreset = sunEveningPreset;

            sunLightIntensity = currentSunPreset.sunLightIntensity;
            sunColor = LerpColor(lastSunPreset.sunColor, currentSunPreset.sunColor, rotation, 0f, 50f);
        }

        skyboxRawMaterial.SetColor("_suncolor", sunColor);
        ambiantController.sunController.sun.intensity = sunLightIntensity;
    }

    void setAmbientDayTime()
    {
        float rotation = ambiantController.sunController.currentRotation;

        Color skyColor;
        Color horizontalColor;
        float starPower;
        float starIntensity;

        lastSkyPreset = null;
        currentSkyPreset = skyMorningPreset;

        if (rotation < 50f)
        {
            lastSkyPreset = skyMorningPreset;
            currentSkyPreset = skyNoonPreset;

            skyColor = LerpColor(lastSkyPreset.SkyColor, currentSkyPreset.SkyColor, rotation, 0f, 50f);
            horizontalColor = LerpColor(lastSkyPreset.HorizonColor, currentSkyPreset.HorizonColor, rotation, 0f, 50f);
            starIntensity = currentSkyPreset.StarIntensity;
            starPower = currentSkyPreset.StarPower;
        }

        else if (rotation < 90f)
        {
            lastSkyPreset = skyNoonPreset;
            currentSkyPreset = skyAfternoonPreset;

            skyColor = LerpColor(lastSkyPreset.SkyColor, currentSkyPreset.SkyColor, rotation, 50f, 90f);
            horizontalColor = LerpColor(lastSkyPreset.HorizonColor, currentSkyPreset.HorizonColor, rotation, 50f, 90f);

            starIntensity = currentSkyPreset.StarIntensity;
            starPower = currentSkyPreset.StarPower;
        }

        else if (rotation < 150f)
        {
            lastSkyPreset = skyAfternoonPreset;
            currentSkyPreset = skyEveningPreset;

            skyColor = LerpColor(lastSkyPreset.SkyColor, currentSkyPreset.SkyColor, rotation, 90f, 150f);
            horizontalColor = LerpColor(lastSkyPreset.HorizonColor, currentSkyPreset.HorizonColor, rotation, 90f, 150f);
            starIntensity = currentSkyPreset.StarIntensity;
            starPower = currentSkyPreset.StarPower;
        }

        else
        {
            lastSkyPreset = skyEveningPreset;
            currentSkyPreset = skySunsetPreset;

            skyColor = LerpColor(lastSkyPreset.SkyColor, currentSkyPreset.SkyColor, rotation, 150f, 200f);
            horizontalColor = LerpColor(lastSkyPreset.HorizonColor, currentSkyPreset.HorizonColor, rotation, 150f, 200f);
            starIntensity = currentSkyPreset.StarIntensity;
            starPower = currentSkyPreset.StarPower;
        }

        skyboxRawMaterial.SetColor("_Skycolor", skyColor);
        skyboxRawMaterial.SetColor("_Horizon_color", horizontalColor);

        skyboxRawMaterial.SetFloat("_StarPower", starPower);
        skyboxRawMaterial.SetFloat("_StarIntensity", starIntensity);
    }

    void setAmbientNightTime()
    {
        float rotation = ambiantController.sunController.currentRotation;

        Color skyColor;
        Color horizontalColor;
        
        lastSkyPreset = skySunsetPreset;
        currentSkyPreset = skyNightPreset;

        skyColor = LerpColor(lastSkyPreset.SkyColor, currentSkyPreset.SkyColor, rotation, 0f, 20f);
        horizontalColor = LerpColor(lastSkyPreset.HorizonColor, currentSkyPreset.HorizonColor, rotation, 0f, 20f);

        skyboxRawMaterial.SetColor("_Skycolor", skyColor);
        skyboxRawMaterial.SetColor("_Horizon_color", horizontalColor);

        currentSunPreset = MoonPreset;
        lastSunPreset = sunsetPreset;
        
        skyboxRawMaterial.SetFloat("_StarPower", currentSkyPreset.StarPower);
        skyboxRawMaterial.SetFloat("_StarIntensity", currentSkyPreset.StarIntensity);
        skyboxRawMaterial.SetFloat("_Starheight", currentSkyPreset.StarDistance);
    }
    
    Color LerpColor(Color from, Color to, float rotation, float startRotation, float endRotation)
    {
        float t = Mathf.InverseLerp(startRotation, endRotation, rotation);
        t = Mathf.SmoothStep(0f, 1f, t);

        return Color.Lerp(from, to, t);
    }

    public void updateSun()
    {
        skyboxRawMaterial.SetFloat("_SunSize", currentSunPreset.sunSize);
        skyboxRawMaterial.SetColor("_suncolor", currentSunPreset.sunColor);

        ambiantController.sunController.sun.transform.localRotation = Quaternion.Euler(ambiantController.sunController.currentRotation, 0f, 0f); ;
        ambiantController.sunController.sun.intensity = currentSunPreset.sunLightIntensity;
        setAmbientDayTime();
    }

    public void updateMoon()
    {
        currentSunPreset = MoonPreset;
        skyboxRawMaterial.SetFloat("_StarPower", currentSkyPreset.StarPower);
        skyboxRawMaterial.SetFloat("_StarIntensity", currentSkyPreset.StarIntensity);
        skyboxRawMaterial.SetFloat("_Starheight", currentSkyPreset.StarDistance);


        skyboxRawMaterial.SetFloat("_SunSize", currentSunPreset.sunSize);
        skyboxRawMaterial.SetColor("_suncolor", currentSunPreset.sunColor);

        ambiantController.sunController.sun.transform.localRotation = Quaternion.Euler(ambiantController.sunController.currentRotation, 0f, 0f); ;
        ambiantController.sunController.sun.intensity = currentSunPreset.sunLightIntensity;
        setAmbientNightTime();
    }
}
