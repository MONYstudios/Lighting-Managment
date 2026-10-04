using UnityEngine;

public class FogController : MonoBehaviour
{
    [Header("Settings - Fog")]
    [SerializeField] FogMode fogMode = FogMode.ExponentialSquared;
    [SerializeField] Color fogColor = Color.gray;
    [SerializeField] float baseDensity = 1f;         
    [SerializeField] float transitionSpeed = 0.01f;   

    [Header("Settings")]
    [SerializeField] bool useFog = true;
    [SerializeField] float nightMultiplier = 2f;

    [Header("Fog - Settings")]
    public bool isFoggyDay = false;

    int morning, noon, afternoon, evening, sunset;

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

    void Start()
    {
        GetTime();

        RenderSettings.fog = useFog;
        RenderSettings.fogMode = fogMode;
        RenderSettings.fogColor = fogColor;
        RenderSettings.fogDensity = 0.001f; // sicherer Startwert
    }

    void Update()
    {
        if (!useFog)
            return;

        float hour = TimeController.instance.getHour();
        float multiplier;

        // Von SPÄT nach FRÜH prüfen
        if (hour >= sunset) multiplier = 2f;
        else if (hour >= evening) multiplier = 2f;
        else if (hour >= afternoon) multiplier = 1.3f;
        else if (hour >= noon) multiplier = 1.5f;
        else if (hour >= morning) multiplier = 5000f;   // Test zweck :|
        else multiplier = nightMultiplier; 

        UpdateFog(multiplier);
    }

    void UpdateFog(float multiplier)
    {
        if (isFoggyDay)
            multiplier *= 5f;
        
        // Hier wird dividiert weil wir davor zu große Angaben hatten
        float target = baseDensity * multiplier / 1000f;

        RenderSettings.fogDensity = Mathf.MoveTowards(RenderSettings.fogDensity, target, transitionSpeed * Time.deltaTime);
    }
}
