using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class FlashlightGlowWhenNotHeld : MonoBehaviour
{
    [Header("Renderers that will glow")]
    [SerializeField] private Renderer[] renderersToGlow;

    [Header("Glow Settings")]
    [SerializeField] private Color glowColor = Color.white;
    [SerializeField] private float glowIntensity = 2f;

    private XRGrabInteractable grabInteractable;

    private static readonly int EmissionColorID = Shader.PropertyToID("_EmissionColor");

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (renderersToGlow == null || renderersToGlow.Length == 0)
        {
            renderersToGlow = GetComponentsInChildren<Renderer>(true);
        }
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrabbed);
            grabInteractable.selectExited.AddListener(OnReleased);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
            grabInteractable.selectExited.RemoveListener(OnReleased);
        }
    }

    private void Start()
    {
        SetGlow(true);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        SetGlow(false);
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        SetGlow(true);
    }

    private void SetGlow(bool active)
    {
        Color finalEmissionColor = active ? glowColor * glowIntensity : Color.black;

        foreach (Renderer rend in renderersToGlow)
        {
            Material[] materials = rend.materials;

            foreach (Material mat in materials)
            {
                if (mat.HasProperty(EmissionColorID))
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor(EmissionColorID, finalEmissionColor);
                }
            }
        }
    }
}