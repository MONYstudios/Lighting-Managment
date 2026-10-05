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
    public bool isInForest = false;

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
        RenderSettings.fogDensity = 0.001f; 
    }

    void Update()
    {
        if (!useFog)
            return;

        float hour = TimeController.instance.getHour();
        float multiplier;

        
        if (hour >= sunset) multiplier = 2f;
        else if (hour >= evening) multiplier = 2f;
        else if (hour >= afternoon) multiplier = 1.3f;
        else if (hour >= noon) multiplier = 1.5f;
        else if (hour >= morning) multiplier = 10f;  
        else multiplier = nightMultiplier; 

        UpdateFog(multiplier);
    }

    void UpdateFog(float multiplier)
    {
        if (isFoggyDay)
            multiplier *= 5f;
        
        // Hier wird dividiert weil wir davor zu große Angaben hatten
        float target = baseDensity * multiplier / 1000f;
        if (isInForest)
            target *= 2.5f;
        RenderSettings.fogDensity = Mathf.MoveTowards(RenderSettings.fogDensity, target, transitionSpeed * Time.deltaTime);
    }
}
