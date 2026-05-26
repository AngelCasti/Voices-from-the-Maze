using UnityEngine;

public class FlashlightToggle : MonoBehaviour
{
    [SerializeField] private Light flashlightLight;
    [SerializeField] private bool startOn = true;

    private bool isOn;

    private void Awake()
    {
        if (flashlightLight == null)
        {
            flashlightLight = GetComponentInChildren<Light>(true);
        }

        SetLight(startOn);
    }

    public void ToggleLight()
    {
        SetLight(!isOn);
    }

    public void SetLight(bool value)
    {
        isOn = value;

        if (flashlightLight != null)
        {
            flashlightLight.enabled = isOn;
        }
    }
}
