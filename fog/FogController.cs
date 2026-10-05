using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("Settings - Fog")]
    [SerializeField] FogMode fogMode = FogMode.ExponentialSquared;
    [SerializeField] Color fogColor = Color.gray;
    [SerializeField] Color fogForestColor = Color.gray;
    [SerializeField] float baseDensity = 1.5f;         
    [SerializeField] float transitionSpeed = 0.01f;   
    [SerializeField] GodRayController godRayController;

    [Header("Settings")]
    [SerializeField] bool useFog = true;
    [SerializeField] float nightMultiplier = 2f;
    [SerializeField] float fogSpeed = 0.2f;

    [Header("Fog - Settings")]
    public bool isFoggyDay = false;
    public bool isInForest = false;
    [SerializeField] float scatteringSpeed = 0.02f;
    int morning, noon, afternoon, evening, sunset;
    [Header("Settings - Godray")]
    Color rayColor;
    float rayDensity;
    float rayIntensity; 
    public void EnableFog()
    {
        useFog = true;
        RenderSettings.fog = true;
    }

    public void DisableFog()
    {
        useFog = false;
        RenderSettings.fog = false;
    }

    void GetTime()
    {
        var t = TimeController.instance;
        morning = t.morning;
        noon = t.noon;
        afternoon = t.afternoon;
        evening = t.evening;
        sunset = t.sunset;
    }

    float GetMultiplier()
    {
        float hour = TimeController.instance.getHour();
        float multiplier;

        if (hour >= sunset) multiplier = 2f;
        else if (hour >= evening) multiplier = 1.8f;
        else if (hour >= afternoon) multiplier = 1.3f;
        else if (hour >= noon) multiplier = 1.5f;
        else if (hour >= morning) multiplier = 2f;
        else multiplier = nightMultiplier;

        return multiplier;
    }
    void Start()
    {
        GetTime();

        RenderSettings.fog = useFog;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = 0.001f;
    }

    void Update()
    {
        if (!useFog)
            return;

        UpdateFog(GetMultiplier());
    }

    void UpdateFog(float multiplier)
    {
        // Falls es ein Nebeliger Tag ist <- Anpassung des Multiplikators!
        if (isFoggyDay)
            multiplier *= 5f;
        
        // Hier wird dividiert weil wir davor zu große Angaben hatten
        float target = baseDensity * multiplier / 1000f;

        // Falls Spieler im Wald ist <- Anpassung der Atmosphäre!
        if (isInForest)
        {
            target *= 20f;
        }

        // Setzt die Farbe es Nebels 
        SetFogColor();

        // Setzt die Werte des God-Rays
        SetGodRay();

        RenderSettings.fogDensity = Mathf.MoveTowards(RenderSettings.fogDensity, target, transitionSpeed * Time.deltaTime);
    }

    void SetFogColor()
    {
        // Ahand der Position verändert der Fog sich 
        // Ternary Statment
        Color targetColor = isInForest ? fogForestColor : fogColor;
        RenderSettings.fogColor = Color.Lerp(RenderSettings.fogColor, targetColor, fogSpeed * Time.deltaTime);
    }

    void SetGodRay()
    {
        rayDensity = isInForest ? 0.3f : 0.2f;
        if (godRayController.GetDensityBooster() != rayDensity)
        {
            godRayController.SetDensityBooster(rayDensity);
        }

        rayIntensity = isInForest ? 0.8f : 0.5f;
        float forwardScattering = isInForest ? 0f : 0.5f;
        if (godRayController.scatteringBooster != forwardScattering)
        {
            godRayController.scatteringBooster = Mathf.MoveTowards(godRayController.scatteringBooster, forwardScattering, scatteringSpeed * Time.deltaTime);
        }
        if (godRayController.GetIntensityBooster() != rayIntensity)
        {
            godRayController.SetIntensityBooster(rayIntensity);
        }
    }
}
