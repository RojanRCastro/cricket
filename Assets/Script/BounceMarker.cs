using UnityEngine;

public class BounceMarker : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 5f;

    [Header("Pitch Bounds (local to pitch centre)")]
    public float minZ = 2f;   // closest to batsman end
    public float maxZ = 10f;  // furthest (full pitch length)
    public float minX = -1.5f;
    public float maxX = 1.5f;

    [Header("Ground Snap")]
    [Tooltip("Raycast from above to stick marker to uneven pitch surface.")]
    public LayerMask groundLayer;
    public float raycastHeight = 2f;

    private Vector3 currentPos;

    private void Start()
    {
        currentPos = transform.position;
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal"); // A/D
        float v = Input.GetAxis("Vertical");   // W/S

        currentPos.x += h * moveSpeed * Time.deltaTime;
        currentPos.z += v * moveSpeed * Time.deltaTime;

        // Clamp to pitch bounds
        currentPos.x = Mathf.Clamp(currentPos.x, minX, maxX);
        currentPos.z = Mathf.Clamp(currentPos.z, minZ, maxZ);

        // Snap Y to ground surface
        SnapToGround();

        transform.position = currentPos;
    }

    private void SnapToGround()
    {
        Ray ray = new Ray(currentPos + Vector3.up * raycastHeight, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, raycastHeight + 1f, groundLayer))
        {
            currentPos.y = hit.point.y + 0.01f; // tiny offset to sit on surface
        }
    }

    public Vector3 GetBouncePosition() => currentPos;
}
