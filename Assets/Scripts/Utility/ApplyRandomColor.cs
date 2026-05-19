using UnityEngine;

public class ApplyRandomColor : MonoBehaviour
{
    [SerializeField]
    private string targetProperty = "_Color";

    [SerializeField]
    private float hueMin = 0f;

    [SerializeField]
    private float hueMax = 1f;

    [SerializeField]
    private float saturationMin = 1f;

    [SerializeField]
    private float saturationMax = 1f;

    [SerializeField]
    private float valueMin = 0.5f;

    [SerializeField]
    private float valueMax = 1f;

    private void Awake()
    {
        if (TryGetComponent(out Renderer renderer))
        {
            Color randomColor = Random.ColorHSV(hueMin, hueMax, saturationMin, saturationMax, valueMin, valueMax);

            foreach (Material mat in renderer.materials)
            {
                if (!mat.HasColor(targetProperty))
                {
                    continue;
                }

                mat.SetColor(targetProperty, randomColor);
            }
        }
    }
}
