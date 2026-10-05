using UnityEngine;

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
    float hour;
    int morning;
    int noon;
    int afternoon;
    int evening;
    int sunset;

    void Start()
    {
        RenderSettings.skybox = skyboxRawMaterial;
        getMainLightDirection.skyboxMaterial = skyboxRawMaterial;

        currentSunPreset = sunMorningPreset;
        currentSkyPreset = skyMorningPreset;

        lastSkyPreset = null;
        lastSunPreset = null;
    }

    void Update()
    {
        // Variablen updaten
        hour = TimeController.instance.getHour();

        morning = TimeController.instance.morning;
        noon = TimeController.instance.noon;
        afternoon = TimeController.instance.afternoon;
        evening = TimeController.instance.evening;
        sunset = TimeController.instance.sunset;

        // Envirorment setzen
        updateSun();
        updateSky();
    }


    void updateSun()
    {
        Color sunColor;
        float sunLightIntensity;

        updateSunPreset(null, sunMorningPreset);

        if (hour < noon)
        {
            updateSunPreset(sunMorningPreset, sunNoonPreset);
            sunColor = LerpColor(lastSunPreset.sunColor, currentSunPreset.sunColor, hour, morning, noon);
            sunLightIntensity = currentSunPreset.sunLightIntensity;
        }
        else if (hour < afternoon)
        {
            updateSunPreset(sunNoonPreset, sunAfternoonPreset);
            sunColor = LerpColor(lastSunPreset.sunColor, currentSunPreset.sunColor, hour, noon, afternoon);
            sunLightIntensity = currentSunPreset.sunLightIntensity;
        }
        else if (hour < evening)
        {
            updateSunPreset(sunAfternoonPreset, sunEveningPreset);
            sunColor = LerpColor(lastSunPreset.sunColor, currentSunPreset.sunColor, hour, afternoon, evening);
            sunLightIntensity = currentSunPreset.sunLightIntensity;
        }
        else if (hour < sunset)
        {
            updateSunPreset(sunEveningPreset, sunsetPreset);
            sunLightIntensity = currentSunPreset.sunLightIntensity;
            sunColor = LerpColor(lastSunPreset.sunColor, currentSunPreset.sunColor, hour, evening, sunset);
        }
        else
        {
            // MOND
            updateSunPreset(sunsetPreset, MoonPreset);
            sunLightIntensity = currentSunPreset.sunLightIntensity;
            sunColor = LerpColor(lastSunPreset.sunColor, currentSunPreset.sunColor, hour, sunset, sunset + 2f);
        }

        skyboxRawMaterial.SetColor("_suncolor", sunColor);
        ambiantController.sunController.sun.intensity = sunLightIntensity;
    }


    void updateSky()
    {
        // standart-Wert
        updateSkyPreset(null, skyMorningPreset);

        float start, end;

        if (hour < noon)
        {
            updateSkyPreset(skyMorningPreset, skyNoonPreset);
            start = morning; end = noon;
        }
        else if (hour < afternoon)
        {
            updateSkyPreset(skyNoonPreset, skyAfternoonPreset);
            start = noon; end = afternoon;
        }
        else if (hour < evening)
        {
            updateSkyPreset(skyAfternoonPreset, skyEveningPreset);
            start = afternoon; end = evening;
        }
        else if (hour < sunset)
        {
            updateSkyPreset(skyEveningPreset, skySunsetPreset);
            start = evening; end = sunset;
        }
        else
        {
            // NACHT
            updateSkyPreset(skySunsetPreset, skyNightPreset);
            start = sunset; end = sunset + 2f;
        }

        applyPresets(hour, start, end);
    }


    void updateSunPreset(SunPreset lastPreset, SunPreset currentPreset)
    {
        Debug.Log($"Old Presets: {lastSunPreset} | {currentSunPreset}");
        lastSunPreset = lastPreset;
        currentSunPreset = currentPreset;
        Debug.Log($"Updated Presets: {lastSunPreset} | {currentSkyPreset}");
    }
    
    void updateSkyPreset(SkyPreset lastPreset, SkyPreset currentPreset)
    {
        Debug.Log($"Old Presets: {lastSkyPreset} | {currentSkyPreset}");
        lastSkyPreset = lastPreset;
        currentSkyPreset = currentPreset;
        Debug.Log($"Updated Presets: {lastSkyPreset} | {currentSkyPreset}");
    }

    void applyPresets(float rotation, float startRotation, float endrotation)
    {
        Color _Skycolor;
        Color _HorizontalColor;

        float _StarIntesity;
        float _Starpower;

        _Skycolor = LerpColor(lastSkyPreset.SkyColor, currentSkyPreset.SkyColor, rotation, startRotation, endrotation);
        _HorizontalColor = LerpColor(lastSkyPreset.HorizonColor, currentSkyPreset.HorizonColor, rotation, startRotation, endrotation);

        _StarIntesity = Mathf.Lerp(lastSkyPreset.StarIntensity, currentSkyPreset.StarIntensity, 5f * Time.deltaTime);
        _Starpower = Mathf.Lerp(lastSkyPreset.StarPower, currentSkyPreset.StarPower, 5f * Time.deltaTime);

        skyboxRawMaterial.SetColor("_Skycolor", _Skycolor);
        skyboxRawMaterial.SetColor("_Horizon_color", _HorizontalColor);

        skyboxRawMaterial.SetFloat("_StarPower", _Starpower);
        skyboxRawMaterial.SetFloat("_StarIntensity", _StarIntesity);
    }

    Color LerpColor(Color from, Color to, float rotation, float startRotation, float endRotation)
    {
        float t = Mathf.InverseLerp(startRotation, endRotation, rotation);
        t = Mathf.SmoothStep(0f, 1f, t);

        return Color.Lerp(from, to, t);
    }
}
