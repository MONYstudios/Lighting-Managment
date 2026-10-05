using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    
    
    [Header("Time Definition")]
    public int morning = 9;
    public int noon = 12;
    public int afternoon = 17;
    public int evening = 19;
    public int sunset = 20;
    [SerializeField] float currentHour;

    [Header("Settings - Preferences")]
    public static TimeController instance;
    [SerializeField] TextMeshProUGUI timeText;
    public bool isTimePaused = false;
    
    void Awake()
    {
        instance = this;
    }

    void Update()
    {
        if (isTimePaused) return;

        currentHour += Time.deltaTime / 60f;

        int hours = Mathf.FloorToInt(currentHour);
        int minutes = Mathf.FloorToInt((currentHour - hours) * 60f);

        timeText.text = $"{hours:00}:{minutes:00}";
    }
    
    public void setCurrentHour(int value)
    {
        currentHour = value;
    }
    public void setTimePaused(bool value)
    {
        isTimePaused = value;
    }
    public float getHour()
    {
        return currentHour;
    }
}
