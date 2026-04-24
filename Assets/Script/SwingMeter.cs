using UnityEngine;
using UnityEngine.UI;

public class SwingMeter : MonoBehaviour
{
    [Header("Meter UI")]
    [Tooltip("The RectTransform of the full meter bar.")]
    public RectTransform meterRect;

    [Tooltip("The indicator line that travels up and down.")]
    public RectTransform indicator;

    [Tooltip("The Image on the indicator — changes colour per zone.")]
    public Image indicatorImage;

    [Header("Meter Settings")]
    public float meterSpeed = 1.5f; // how fast indicator oscillates

    [Header("Zone Colours (bottom->top = red->yellow->green->blue->green->yellow->red)")]
    public Color colorRed = Color.red;
    public Color colorYellow = Color.yellow;
    public Color colorGreen = Color.green;
    public Color colorBlue = Color.blue;

    [Header("Output")]
    [Tooltip("Populated after player stops the meter. 0 = no effect, 1 = maximum.")]
    [Range(0f, 1f)]
    public float swingPercent = 0f;

    // -- Events ----------------------------------------------------------------
    public System.Action<float> OnMeterStopped; // fires with swingPercent value

    // -- Internal --------------------------------------------------------------
    private bool isRunning = false;
    private bool isLocked = false;
    private float t = 0f;   // 0..1, ping-pong
    private float direction = 1f;

    // -- Public API ------------------------------------------------------------

    public void StartMeter()
    {
        isRunning = true;
        isLocked = false;
        t = 0f;
        direction = 1f;
    }

    public void StopMeter()
    {
        if (!isRunning || isLocked) return;

        isLocked = true;
        isRunning = false;

        // t = 0 or 1 -> edges (red zone) = 0% power
        // t = 0.5    -> centre (blue zone) = 100% power
        // Map t to distance from centre: 0 at edges, 1 at centre
        swingPercent = 1f - (Mathf.Abs(t - 0.5f) * 2f);
        swingPercent = Mathf.Clamp01(swingPercent);

        OnMeterStopped?.Invoke(swingPercent);
        Debug.Log($"[Meter] Stopped at t={t:F2} -> Swing%={swingPercent * 100f:F0}%");
    }

    // -- Unity Lifecycle -------------------------------------------------------

    private void Update()
    {
        if (!isRunning) return;

        t += direction * meterSpeed * Time.deltaTime;

        if (t >= 1f) { t = 1f; direction = -1f; }
        if (t <= 0f) { t = 0f; direction = 1f; }

        UpdateIndicatorPosition();
        UpdateIndicatorColor();
    }

    private void UpdateIndicatorPosition()
    {
        float meterHeight = meterRect.rect.height;
        float yPos = Mathf.Lerp(-meterHeight / 2f, meterHeight / 2f, t);
        indicator.anchoredPosition = new Vector2(0f, yPos);
    }

    private void UpdateIndicatorColor()
    {
        // Distance from centre (0 = centre/blue, 1 = edges/red)
        float distFromCentre = Mathf.Abs(t - 0.5f) * 2f;

        Color zoneColor;
        if (distFromCentre < 0.15f) zoneColor = colorBlue;    // 100%
        else if (distFromCentre < 0.45f) zoneColor = colorGreen;   // ~70%
        else if (distFromCentre < 0.75f) zoneColor = colorYellow;  // ~40%
        else zoneColor = colorRed;     // ~0%

        if (indicatorImage != null)
            indicatorImage.color = zoneColor;
    }
}
