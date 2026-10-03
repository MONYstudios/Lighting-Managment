
using UnityEngine;

public class DayNightCyleController : MonoBehaviour
{
    [SerializeField] public Light sun;
    [SerializeField] float sunMaxRotation = 180f;
    [SerializeField] float moonMaxRotation = 90f;
    [SerializeField] float sunSpeed = 1f;
    [SerializeField] float moonSpeed = 1f;
    public float currentRotation;
    [SerializeField] public bool dayNightCyle = false;
    [SerializeField] public bool nightFogAppeared = false;
    [SerializeField] public bool isMoonFinished = false;
    [SerializeField] float resetSunValue = 0f;
    [SerializeField] int dayLenghtMinutes = 24;
    [SerializeField] AmbiantManager ambiantController;
    [SerializeField] DayNightCyleColorController DayNightCyleColorController;
    [SerializeField] bool isMoonEnabled = false;


    void Start()
    {
        ambiantController = GetComponent<AmbiantManager>();
        if (sun == null)
        {
            Debug.LogError("SunController: Es wurde keine Sonne gefunden!");
            return;
        }

        float distance = Mathf.Abs(sunMaxRotation - resetSunValue);
        sunSpeed = distance / (dayLenghtMinutes * 60f);

        resetSun();
        resetCyle();
    }

    void Update()
    {
        sunCyle();
        moonCyle();
    }
    void resetCyle()
    {
        nightFogAppeared = false;
        isMoonEnabled = false;
        isMoonFinished = false;

        dayNightCyle = true;
        TimeController.instance.currentHour = 8f;
    }
    void moonCyle()
    {
        if (!isMoonEnabled) return;

        currentRotation = Mathf.MoveTowards(
            currentRotation,
            moonMaxRotation,
            sunSpeed * Time.deltaTime
        );

        sun.transform.localRotation = Quaternion.Euler(currentRotation, 0f, 0f);

        if (currentRotation >= moonMaxRotation && !isMoonFinished)
        {
            Debug.Log("Moon is finished!");
            isMoonFinished = true;
        }
    }

    void sunCyle()
    {
        if (!dayNightCyle || isMoonEnabled)
            return;

        float rotation = (TimeController.instance.currentHour - 8f) * 22.5f;

        sun.transform.localRotation = Quaternion.Euler(
            rotation,
            0f,
            0f
        );
    
        if (currentRotation >= sunMaxRotation && !nightFogAppeared)
        {
            nightFogAppeared = true;
            ambiantController.fogController.NightTimeFog();
            
            resetSun();
            ambiantController.isDay = false;
            Debug.Log($"DayNightCyleController: isDay: {ambiantController.isDay}");
            DayNightCyleColorController.setMoon();
            isMoonEnabled = true;
        }
    }
    void resetSun()
    {
        currentRotation = resetSunValue;

        Vector3 raw = sun.transform.localEulerAngles;
        raw.x = currentRotation;
        sun.transform.localEulerAngles = raw;
    }

    public void toggleDayNightCycle()
    {
        dayNightCyle = !dayNightCyle;

        Debug.Log($"SunController: DayNightCycle = {dayNightCyle}");
    }
}
