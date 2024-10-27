using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static Timer Instance;
    [SerializeField] private bool startTimer = false;
    [SerializeField] private TMP_Text timer;
    [SerializeField] private float time = 20;
    private float currentTime;
    public float TimeRemaining { get { return currentTime; } }
    private void Awake() {
        currentTime = time;
        if(Instance is null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }else{
            Destroy(gameObject);
        }
    }
    
    private void Start() {
        timer.text = $"{currentTime}:00";
    }
    private void Update() {
        if (startTimer)
        {
            if(currentTime <= 0)
            {
                StopTimer();
            }
            currentTime -= Time.deltaTime;
            
            timer.text = currentTime.ToString("F2");
        }
    }
    public void StartTimer()
    {
        timer.color = Color.green;
        startTimer = true;
    }
    public void StopTimer()
    {
        timer.color = Color.red;
        startTimer = false;
    }
    public void RestartTime()
    {
        Debug.Log("Timer restarted");
        currentTime = time;
        timer.text = $"{currentTime}:00";
        timer.color = Color.cyan;
    }
}
