using UnityEngine;
using UnityEngine.UI;

public class FlashlightToggle : MonoBehaviour
{
    [Header("Light")]
    [SerializeField] private Light flashlightLight;

    [Header("Battery UI")]
    [SerializeField] private Image[] batteryBlocks;

    [Header("Battery Settings")]
    [SerializeField] private float batteryDuration = 300f; // 5 minutos
    [SerializeField] private bool startOn = true;

    [Header("Blink Warning")]
    [SerializeField] private float blinkWarningTime = 10f;
    [SerializeField] private float blinkSpeed = 5f;

    private float currentBattery;
    private bool isOn;

    private void Awake()
    {
        if (flashlightLight == null)
        {
            flashlightLight = GetComponentInChildren<Light>(true);
        }

        currentBattery = batteryDuration;

        SetLight(startOn);
        UpdateBatteryUI();
    }

    private void Update()
    {
        if (!isOn)
        {
            UpdateBatteryUI();
            return;
        }

        if (currentBattery > 0f)
        {
            currentBattery -= Time.deltaTime;
            currentBattery = Mathf.Clamp(currentBattery, 0f, batteryDuration);

            UpdateBatteryUI();

            if (currentBattery <= 0f)
            {
                SetLight(false);
            }
        }
    }

    public void ToggleLight()
    {
        if (currentBattery <= 0f)
        {
            SetLight(false);
            return;
        }

        SetLight(!isOn);
    }

    public void SetLight(bool value)
    {
        isOn = value && currentBattery > 0f;

        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;
        }
    }

    private void UpdateBatteryUI()
    {
        if (batteryBlocks == null || batteryBlocks.Length == 0)
        {
            return;
        }

        int totalBlocks = batteryBlocks.Length;
        float blockDuration = batteryDuration / totalBlocks;

        float percentage = currentBattery / batteryDuration;
        int activeBlocks = Mathf.CeilToInt(percentage * totalBlocks);

        activeBlocks = Mathf.Clamp(activeBlocks, 0, totalBlocks);

        if (currentBattery <= 0f)
        {
            activeBlocks = 0;
        }

        int blinkingBlockIndex = -1;

        if (activeBlocks > 0)
        {
            float nextThreshold = (activeBlocks - 1) * blockDuration;
            float timeUntilBlockTurnsOff = currentBattery - nextThreshold;

            if (timeUntilBlockTurnsOff <= blinkWarningTime)
            {
                blinkingBlockIndex = activeBlocks - 1;
            }
        }

        for (int i = 0; i < totalBlocks; i++)
        {
            if (i < activeBlocks)
            {
                batteryBlocks[i].enabled = true;

                if (i == blinkingBlockIndex)
                {
                    bool blinkVisible = Mathf.PingPong(Time.time * blinkSpeed, 1f) > 0.5f;
                    batteryBlocks[i].enabled = blinkVisible;
                }
            }
            else
            {
                batteryBlocks[i].enabled = false;
            }
        }
    }
}