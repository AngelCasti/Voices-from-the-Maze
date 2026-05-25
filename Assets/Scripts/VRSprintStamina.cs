using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRSprintStamina : MonoBehaviour
{
    [Header("Movimiento")]
    public ContinuousMoveProviderBase moveProvider;
    private float defaultSpeed;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip tiredSound;
    private bool hasPlayedTiredSound = false;

    [Header("Velocidades")]
    public float normalSpeed = 1.5f;
    public float sprintSpeed = 3.0f;

    [Header("Estamina")]
    public float maxStamina = 5f;
    public float staminaDrainRate = 2f;
    public float staminaRecoveryRate = 0.75f;
    public float recoveryDelay = 1.5f;

    [Header("UI")]
    public Image staminaBar;

    private InputAction sprintAction;
    private float currentStamina;
    private float lastSprintTime;
    private bool isSprinting;
    private bool isExhausted = false;

    private void Awake()
    {
        currentStamina = maxStamina;
        defaultSpeed = moveProvider.moveSpeed;

        sprintAction = new InputAction("Sprint", InputActionType.Button);
        sprintAction.AddBinding("<OculusTouchController>{RightHand}/thumbstickClicked");
        sprintAction.AddBinding("<OculusTouchController>{LeftHand}/thumbstickClicked");
        sprintAction.AddBinding("<Keyboard>/leftShift");
    }

    private void OnEnable() { sprintAction.Enable(); }
    private void OnDisable() { sprintAction.Disable(); }

    private void Update()
    {
        if (moveProvider == null) return;

        bool isMoving = moveProvider.gameObject.GetComponent<CharacterController>().velocity.magnitude > 0.1f;
        bool sprintButtonPressed = sprintAction.IsPressed();

        // 1. Lógica de Sprint (Drenaje)
        if (sprintButtonPressed && isMoving && currentStamina > 0f && !isExhausted)
        {
            isSprinting = true;
            moveProvider.moveSpeed = sprintSpeed;
            
            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            lastSprintTime = Time.time; // Reiniciamos el tiempo de espera para recuperar

            if (currentStamina <= 0f)
            {
                isExhausted = true;
                StopSprintLogic();
                PlayTiredSound();
            }
        }
        else
        {
            // 2. Lógica de Recuperación
            StopSprintLogic();
            
            if (Time.time >= lastSprintTime + recoveryDelay)
            {
                currentStamina += staminaRecoveryRate * Time.deltaTime;
                currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);

                if (currentStamina >= maxStamina)
                {
                    isExhausted = false;
                    hasPlayedTiredSound = false;
                }
            }
        }

        UpdateStaminaBar();
        HandleBlinkingEffect();
    }

    private void StopSprintLogic()
    {
        if (isSprinting)
        {
            isSprinting = false;
            moveProvider.moveSpeed = defaultSpeed;
            
            // Esto fuerza al CharacterController a recalcular su posición 
            // respecto a los muros cercanos al detener el sprint
            var cc = moveProvider.gameObject.GetComponent<CharacterController>();
            if(cc != null) cc.Move(Vector3.zero); 
        }
    }

    private void UpdateStaminaBar()
    {
        if (staminaBar != null)
            staminaBar.fillAmount = currentStamina / maxStamina;
    }

    private void HandleBlinkingEffect()
    {
        if (staminaBar != null)
        {
            Color c = staminaBar.color;
            c.a = isExhausted ? Mathf.PingPong(Time.time * 5f, 1f) : 1f;
            staminaBar.color = c;
        }
    }

    private void PlayTiredSound()
    {
        if (tiredSound != null && audioSource != null && !hasPlayedTiredSound)
        {
            audioSource.PlayOneShot(tiredSound);
            hasPlayedTiredSound = true;
        }
    }
}