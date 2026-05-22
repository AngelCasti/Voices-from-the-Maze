using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRSprintStamina : MonoBehaviour
{
    [Header("Movimiento")]
    public ContinuousMoveProviderBase moveProvider;

    [Header("Velocidades")]
    public float normalSpeed = 1.5f;
    public float sprintSpeed = 3.0f;

    [Header("Estamina")]
    public float maxStamina = 5f;
    public float staminaDrainRate = 1f;
    public float staminaRecoveryRate = 0.75f;
    public float recoveryDelay = 1.5f;

    [Header("UI")]
    public Image staminaBar;

    private InputAction sprintAction;
    private float currentStamina;
    private float lastSprintTime;
    private bool isSprinting;

    private void Awake()
    {
        currentStamina = maxStamina;

        sprintAction = new InputAction("Sprint", InputActionType.Button);

        // sprintAction.AddBinding("<XRController>{RightHand}/primary2DAxisClick");
        sprintAction.AddBinding("<OculusTouchController>{RightHand}/thumbstickClicked");
    }

    private void OnEnable()
    {
        sprintAction.Enable();
    }

    private void OnDisable()
    {
        sprintAction.Disable();
    }

    private void Update()
    {
        if (moveProvider == null)
            return;

        bool sprintButtonPressed = sprintAction.IsPressed();

        if (sprintButtonPressed && currentStamina > 0f)
        {
            StartSprint();
        }
        else
        {
            StopSprint();
        }

        if (isSprinting)
        {
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            lastSprintTime = Time.time;

            if (currentStamina <= 0f)
            {
                StopSprint();
            }
        }
        else
        {
            if (Time.time >= lastSprintTime + recoveryDelay)
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            }
        }

        UpdateStaminaBar();
    }

    private void StartSprint()
    {
        isSprinting = true;
        moveProvider.moveSpeed = sprintSpeed;
    }

    private void StopSprint()
    {
        isSprinting = false;
        moveProvider.moveSpeed = normalSpeed;
    }

    private void UpdateStaminaBar()
    {
        if (staminaBar != null)
        {
            staminaBar.fillAmount = currentStamina / maxStamina;
        }
    }
}