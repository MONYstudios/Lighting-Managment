
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
    [SerializeField] public bool isMoonFinished = false;
    [SerializeField] float resetSunValue = 0f;
    [SerializeField] int dayLenghtMinutes = 24;
    [SerializeField] AmbiantManager ambiantController;
    [SerializeField] DayNightCyleColorController DayNightCyleColorController;
    bool isDay = false;


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
        isDay = ambiantController.isDay;
        sunCyle();
        moonCyle();
    }
    void resetCyle()
    {
        isMoonFinished = false;

        dayNightCyle = true;
        TimeController.instance.setCurrentHour(8);
    }
    void moonCyle()
    {
        if (isDay) return;

        // Neues Feld oben: [SerializeField] float sunsetHour = 20f;
        float rotation = (TimeController.instance.getHour() - 20f) * 15f;
        currentRotation = rotation;

        sun.transform.localRotation = Quaternion.Euler(rotation, 90f, 0f);

        if (currentRotation >= sunMaxRotation && !isMoonFinished)
        {
            Debug.Log("Moon is finished!");
            isMoonFinished = true;
        }
    }

    void sunCyle()
    {
        if (!dayNightCyle || !isDay)
            return;

        float rotation = (TimeController.instance.getHour() - 8f) * 15f;

        sun.transform.localRotation = Quaternion.Euler(rotation, 90f, 0f);

        if (TimeController.instance.getHour() >= 20)
        {
            resetSun();
            ambiantController.isDay = false;
            Debug.Log($"DayNightCyleController: isDay-State: {isDay}");
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
