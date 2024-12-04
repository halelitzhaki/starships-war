using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class ShieldThePlayer : MonoBehaviour {
    [Tooltip("The number of seconds that the shield remains active")] [SerializeField] float duration;
    [Tooltip("The radius of the shield circle")] [SerializeField] float shieldRadius = 1.5f;
    [Tooltip("The width of the shield circle line")] [SerializeField] float lineWidth = 0.1f;
    [Tooltip("The color of the shield circle")] [SerializeField] Color shieldColor = Color.red;

    private LineRenderer shieldCircle;

    private void OnTriggerEnter2D(Collider2D other) {
        if (other.tag == "Player") {
            Debug.Log("Shield triggered by player");
            var destroyComponent = other.GetComponent<DestroyOnTrigger2D>();
            if (destroyComponent) {
                // Create the shield circle
                CreateShieldCircle(other);
                ShieldTemporarily(destroyComponent, other);
                Destroy(this.gameObject);  // Destroy the shield itself - to prevent double-use
            }
        } else {
            Debug.Log("Shield triggered by " + other.name);
        }
    }

    private async void ShieldTemporarily(DestroyOnTrigger2D destroyComponent, Collider2D other) {
        destroyComponent.enabled = false;
        for (float t = duration; t > 0; t--) {
            Debug.Log("Shield: " + t + " seconds remaining!");
            UpdateShieldOpacity(t / duration);
            await Awaitable.WaitForSecondsAsync(1);
        }
        Debug.Log("Shield gone!");
        DestroyShieldCircle();
        destroyComponent.enabled = true;
    }

    private void CreateShieldCircle(Collider2D other) {
        GameObject shieldObject = new GameObject("ShieldCircle");
        shieldObject.transform.SetParent(other.transform);
        shieldObject.transform.localPosition = Vector3.zero;

        shieldCircle = shieldObject.AddComponent<LineRenderer>();
        shieldCircle.startWidth = lineWidth;
        shieldCircle.endWidth = lineWidth;
        shieldCircle.useWorldSpace = false;
        shieldCircle.loop = true;

        // Assign a material with transparency support
        Material material = new Material(Shader.Find("Sprites/Default"));
        material.color = shieldColor;
        shieldCircle.material = material;

        shieldCircle.startColor = shieldColor;
        shieldCircle.endColor = shieldColor;
        shieldCircle.positionCount = 100;

        float angleStep = 360f / (shieldCircle.positionCount - 1);
        for (int i = 0; i < shieldCircle.positionCount; i++) {
            float angle = Mathf.Deg2Rad * i * angleStep;
            float x = Mathf.Cos(angle) * shieldRadius;
            float y = Mathf.Sin(angle) * shieldRadius;
            shieldCircle.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    private void UpdateShieldOpacity(float alpha) {
        if (shieldCircle != null) {
            Color startColor = shieldCircle.startColor;
            startColor.a = alpha;
            shieldCircle.startColor = startColor;

            Color endColor = shieldCircle.endColor;
            endColor.a = alpha;
            shieldCircle.endColor = endColor;
        }
    }

    private void DestroyShieldCircle() {
        if (shieldCircle != null) {
            Destroy(shieldCircle.gameObject);
        }
    }
}
