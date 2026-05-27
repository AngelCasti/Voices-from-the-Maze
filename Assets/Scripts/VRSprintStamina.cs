using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class VRSprintStamina : MonoBehaviour
{
    [Header("Movimiento")]
    public ContinuousMoveProviderBase moveProvider;
    private float defaultSpeed;

    [Header("Audio cansancio")]
    public AudioSource audioSource;
    public AudioClip tiredSound;
    private bool hasPlayedTiredSound = false;

    [Header("Audio pasos")]
    public AudioSource footstepAudioSource;
    public AudioClip walkStepSound;
    public float walkStepInterval = 0.55f;
    private float stepTimer = 999f;

    [Header("Velocidades")]
    public float normalSpeed = 2.0f;
    public float sprintSpeed = 4.0f;

    [Header("Estamina")]
    public float maxStamina = 5f;
    public float staminaDrainRate = 1.0f;
    public float staminaRecoveryRate = 0.75f;
    public float recoveryDelay = 1.5f;

    [Header("UI")]
    public Image staminaBar;

    private InputAction sprintAction;
    private float currentStamina;
    private float lastSprintTime;
    private bool isSprinting;
    private bool isExhausted = false;
    private CharacterController characterController;

    private void Awake()
    {
        currentStamina = maxStamina;

        if (moveProvider != null)
            defaultSpeed = moveProvider.moveSpeed;

        characterController = moveProvider.gameObject.GetComponent<CharacterController>();

        sprintAction = new InputAction("Sprint", InputActionType.Button);
        sprintAction.AddBinding("<OculusTouchController>{RightHand}/thumbstickClicked");
        sprintAction.AddBinding("<OculusTouchController>{LeftHand}/thumbstickClicked");
        sprintAction.AddBinding("<Keyboard>/leftShift");
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
        if (moveProvider == null || characterController == null) return;

        bool isMoving = characterController.velocity.magnitude > 0.1f;
        bool sprintButtonPressed = sprintAction.IsPressed();

        if (sprintButtonPressed && isMoving && currentStamina > 0f && !isExhausted)
        {
            isSprinting = true;
            moveProvider.moveSpeed = sprintSpeed;

            currentStamina -= staminaDrainRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            lastSprintTime = Time.time;

            if (currentStamina <= 0f)
            {
                isExhausted = true;
                StopSprintLogic();
                PlayTiredSound();
            }
        }
        else
        {
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

        HandleFootsteps(isMoving, isSprinting);
        UpdateStaminaBar();
        HandleBlinkingEffect();
    }

    private void StopSprintLogic()
    {
        if (isSprinting)
        {
            isSprinting = false;
            moveProvider.moveSpeed = defaultSpeed;

            if (characterController != null)
                characterController.Move(Vector3.zero);
        }
    }

   private void HandleFootsteps(bool isMoving, bool sprinting)
{
    if (footstepAudioSource == null || walkStepSound == null)
        return;

    footstepAudioSource.clip = walkStepSound;
    footstepAudioSource.loop = true;

    if (isMoving)
    {
        footstepAudioSource.pitch = sprinting ? 1.6f : 1.2f;

        if (!footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Play();
        }
    }
    else
    {
        if (footstepAudioSource.isPlaying)
        {
            footstepAudioSource.Stop();
        }

        footstepAudioSource.pitch = 1f;
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