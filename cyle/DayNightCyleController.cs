
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
    [SerializeField] AmbiantManager ambiantController;
    bool isDay = false;


    void Start()
    {
        ambiantController = GetComponent<AmbiantManager>();
        if (sun == null)
        {
            Debug.LogError("SunController: Es wurde keine Sonne gefunden!");
            return;
        }

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
        TimeController.instance.setCurrentHour(9);
    }
    void moonCyle()
    {
        if (isDay || isMoonFinished) return;

        
        float rotation = (TimeController.instance.getHour() - 20f) * 15f;
        currentRotation = rotation;

        if (currentRotation >= moonMaxRotation)
        {
            Debug.Log("Moon is finished!");
            isMoonFinished = true;
            TimeController.instance.setTimePaused(true);
        }

        sun.transform.localRotation = Quaternion.Euler(rotation, 90f, 0f);

        
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
