using UnityEngine;

public class BossHealthBar : MonoBehaviour
{
    public RectTransform fillRect;
    private float maxWidth;

    // Añadir estas variables
    public float maxHealth = 100f;
    private float currentHealth;

    void Awake()
    {
        maxWidth = fillRect.sizeDelta.x;
        currentHealth = maxHealth;

        // Llama a tu propio método, no te auto-referencies
        SetHealth(currentHealth, maxHealth);
    }

    public void SetHealth(float current, float max)
    {
        float ratio = Mathf.Clamp01(current / max);
        fillRect.sizeDelta = new Vector2(maxWidth * ratio, fillRect.sizeDelta.y);
    }

    void Update()
    {
        if (Camera.main != null)
        {
            transform.LookAt(transform.position + Camera.main.transform.rotation * Vector3.forward,
                             Camera.main.transform.rotation * Vector3.up);
        }
    }
}
