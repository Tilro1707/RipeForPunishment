using TMPro;
using UnityEngine;

public class DayCycle : MonoBehaviour
{
    [SerializeField] private Spawner spawner;
    [SerializeField] private TextMeshProUGUI dayCounterTXT;
    [SerializeField] private GameManager gameManager;

    [Header("Phasen")]
    [SerializeField] private float dayDuration = 30f;
    [SerializeField] private float nightDuration = 15f;

    [Header("Wurfabstaende an Tag 1")]
    [SerializeField] private float firstDayStartInterval = 1f;
    [SerializeField] private float firstDayEndInterval = 0.8f;

    [Header("Steigerung und Untergrenzen")]
    [SerializeField] private float intervalReductionPerDay = 0.1f;
    [SerializeField] private float minimumStartInterval = 0.6f;
    [SerializeField] private float minimumEndInterval = 0.4f;

    [Header("DayWheel")]
    [SerializeField] private Transform dayWheel;

    [Header("Shop")]
    [SerializeField] private GameObject shopCanvas;

    public bool IsNight => isNight;

    private float phaseTimer;
    private int currentDay = 1;
    private bool isNight;

    // Diese Werte werden nur zu Beginn eines Tages neu berechnet.
    private float dayStartInterval;
    private float dayEndInterval;

    private void Start()
    {
        BeginDay();
    }

    private void Update()
    {
        if (gameManager.IsGameOver || gameManager.IsEnding)
        {
            return;
        }

            if (Time.deltaTime <= 0f)
            return;

        phaseTimer += Time.deltaTime;
        UpdateWheelRotation();
        if (!isNight)
        {
            float dayProgress = phaseTimer / dayDuration;
            spawner.SpawnInterval = Mathf.Lerp(
                dayStartInterval,
                dayEndInterval,
                dayProgress
            );

            if (phaseTimer >= dayDuration)
            {
                if (currentDay >= 5)
                {
                    gameManager.WinGame();
                    return;
                }

                isNight = true;
                phaseTimer = 0f;
                spawner.enabled = false;

                shopCanvas.SetActive(true);
            }
        }
        else if (phaseTimer >= nightDuration)
        {
            
            currentDay++;
            BeginDay();
        }
    }

    private void BeginDay()
    {
        shopCanvas.SetActive(false);
        isNight = false;
        phaseTimer = 0f;

        float reduction = (currentDay - 1) * intervalReductionPerDay;
        dayStartInterval = Mathf.Max(
            minimumStartInterval,
            firstDayStartInterval - reduction
        );
        dayEndInterval = Mathf.Max(
            minimumEndInterval,
            firstDayEndInterval - reduction
        );

        spawner.SpawnInterval = dayStartInterval;
        spawner.enabled = true;

        UpdateUI();
        Debug.Log("Es beginnt Tag " + currentDay);
    }

    private void UpdateWheelRotation()
    {
        float phaseDuration = dayDuration;

        if (isNight)
        {
            phaseDuration = nightDuration;
        }

        float rotation = -180 / phaseDuration * Time.deltaTime;
        dayWheel.Rotate(0, 0, rotation);
    }

    private void UpdateUI()
    {
        dayCounterTXT.SetText("Day " + currentDay);
    }
}
