using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController instance;
    
    // Setzung des Wertes um Start-Zeit zu definieren
    public float currentHour = 8f;
    public bool isTimePaused = false;
    [SerializeField] TextMeshProUGUI timeText;
    
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
}
