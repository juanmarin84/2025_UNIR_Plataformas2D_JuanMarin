using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Slider healthSlider;
    [SerializeField] EntityController target;
    [SerializeField] CanvasGroup canvas;

    [SerializeField] float visibleTime = 1.5f;
    [SerializeField] float fadeSpeed = 2f;

    float timer;

    void Start()
    {
        healthSlider.maxValue = target.MaxHealth;
        healthSlider.value = target.CurrentHealth;
        canvas.alpha = 0;
    }

    void Update()
    {
        healthSlider.value = target.CurrentHealth;

        if (timer > 0)
        {
            timer -= Time.deltaTime;
            canvas.alpha = 1;
        }
        else
        {
            canvas.alpha = Mathf.MoveTowards(canvas.alpha, 0, fadeSpeed * Time.deltaTime);
        }
    }

    public void Show()
    {
        timer = visibleTime;
        canvas.alpha = 1;
    }
}
