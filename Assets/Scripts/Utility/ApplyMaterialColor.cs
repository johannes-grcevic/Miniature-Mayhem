using ConditionalField;
using UnityEngine;

public class ApplyMaterialColor : MonoBehaviour
{
    [SerializeField]
    private string targetProperty = "_EmissionColor";

    [SerializeField]
    [ConditionalField(nameof(randomColor), Conditional.Options.Invert)]
    private Color color = Color.white;

    [SerializeField]
    private bool randomColor = false;

    [SerializeField]
    [ConditionalField(nameof(randomColor))]
    private float hueMin = 0f;

    [SerializeField]
    [ConditionalField(nameof(randomColor))]
    private float hueMax = 1f;

    [SerializeField]
    [ConditionalField(nameof(randomColor))]
    private float saturationMin = 1f;

    [SerializeField]
    [ConditionalField(nameof(randomColor))]
    private float saturationMax = 1f;

    [SerializeField]
    [ConditionalField(nameof(randomColor))]
    private float valueMin = 0.5f;

    [SerializeField]
    [ConditionalField(nameof(randomColor))]
    private float valueMax = 1f;

    private void Awake()
    {
        if (!TryGetComponent(out Renderer renderer)) return;

        ApplyColor(renderer, targetProperty, randomColor ? Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax) : color);
    }

    public void ApplyColor(Renderer renderer, string name, Color value)
    {
        foreach (Material mat in renderer.materials)
        {
            // if the material shader supports the color and its not already set to that color
            if (mat.HasColor(name) && mat.color != value)
            {
                mat.SetColor(name, value);
            }
        }
    }
}
