using UnityEngine;

public class BowlingManager : MonoBehaviour
{
    [Header("References")]
    public BallSwing ball;
    public SwingMeter meter;
    public BounceMarker bounceMarker;

    [Header("Swing Direction")]
    [Tooltip("+1 = away swing (right), -1 = in swing (left)")]
    public int swingDirection = 1;

    private enum BowlState { Idle, MeterRunning }
    private BowlState state = BowlState.Idle;

    private void Start()
    {
        // Subscribe to meter stop event
        meter.OnMeterStopped += OnMeterStopped;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            OnBowlButtonPressed();

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMode();
        }
    }

    public void OnBowlButtonPressed()
    {
        switch (state)
        {
            case BowlState.Idle:
                // First press: reset ball to start, start meter
                ball.ResetBall();
                meter.StartMeter();
                state = BowlState.MeterRunning;
                break;

            case BowlState.MeterRunning:
                // Second press: lock meter -> this fires OnMeterStopped
                meter.StopMeter();
                break;
        }
    }

    private void OnMeterStopped(float swingPercent)
    {
        ball.swingStrength = swingPercent;
        ball.swingDirection = swingDirection;

        Vector3 bouncePos = bounceMarker.GetBouncePosition();

        ball.SetTarget(bouncePos);

        ball.Bowl();

        state = BowlState.Idle;
    }

    public void ToggleSwingSide()
    {
        swingDirection *= -1;
        Debug.Log($"[Bowling] Swing direction: {(swingDirection > 0 ? "Away (Right)" : "In (Left)")}");
    }

    public void ToggleMode()
    {
        if (ball.currentMode == BallSwing.BowlType.Swing)
            ball.currentMode = BallSwing.BowlType.Spin;
        else
            ball.currentMode = BallSwing.BowlType.Swing;

        Debug.Log("Mode: " + ball.currentMode);
    }
}
