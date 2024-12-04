using UnityEngine;

public class BlackHoleMovement : MonoBehaviour
{
    [Tooltip("The speed at which the black hole moves along the edges")]
    public float speed = 5f;

    [Tooltip("Offset from the edges to prevent the black hole from being cut off")]
    public float edgeOffset = 0.5f;
    
    [Tooltip("Every object tagged with this tag will trigger the destruction of this object")]
    [SerializeField] string triggeringTag;

    private Vector2[] screenEdges; // Store the adjusted screen corners
    private int targetEdgeIndex = 0; // Current target edge
    private Mover mover; // Reference to the Mover component

    private void Start()
    {
        // Get the Mover component attached to this GameObject
        mover = GetComponent<Mover>();
        if (mover == null)
        {
            Debug.LogError("No Mover component found on the Black Hole object!");
            return;
        }

        // Calculate screen edges based on camera bounds
        screenEdges = new Vector2[4];
        Camera cam = Camera.main;

        Vector2 screenMin = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)); // Bottom-left of screen
        Vector2 screenMax = cam.ViewportToWorldPoint(new Vector3(1, 1, 0)); // Top-right of screen

        // Apply offsets to the screen edges
        screenEdges[0] = new Vector2(screenMin.x + edgeOffset, screenMax.y - edgeOffset); // Top-left
        screenEdges[1] = new Vector2(screenMax.x - edgeOffset, screenMax.y - edgeOffset); // Top-right
        screenEdges[2] = new Vector2(screenMax.x - edgeOffset, screenMin.y + edgeOffset); // Bottom-right
        screenEdges[3] = new Vector2(screenMin.x + edgeOffset, screenMin.y + edgeOffset); // Bottom-left

        // Start at the first edge
        transform.position = screenEdges[0];
        UpdateVelocity();
    }

    private void Update()
    {
        if (mover == null) return;

        // Check if the black hole has reached the target edge
        Vector2 targetPosition = screenEdges[targetEdgeIndex];
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            // Move to the next edge (looping back to the start)
            targetEdgeIndex = (targetEdgeIndex + 1) % screenEdges.Length;
            UpdateVelocity();
        }
    }

    private void UpdateVelocity()
    {
        // Calculate the direction to the next edge
        Vector3 targetPosition = screenEdges[targetEdgeIndex];
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Set the velocity on the Mover component
        mover.SetVelocity(direction * speed);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == triggeringTag && enabled) {
            Destroy(other.gameObject);
        }
    }
}
