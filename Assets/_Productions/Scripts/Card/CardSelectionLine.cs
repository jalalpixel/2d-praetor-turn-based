using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class CardSelectionLine : MonoBehaviour
{
    [Header("References")]
    public WorldCardGrid worldCardGrid;

    [Header("Line Settings")]
    public Color lineColor = Color.green;
    public float lineWidth = 0.05f;
    [Range(2, 50)] public int segmentCount = 20;
    public float curveStrength = 1.5f;

    private LineRenderer lineRenderer;

    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.enabled = false;

        lineRenderer.startColor = lineColor;
        lineRenderer.endColor = lineColor;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.positionCount = segmentCount;
    }

    void Update()
    {
        if (worldCardGrid == null) return;

        if (worldCardGrid.selectedCharacter != null)
        {
            lineRenderer.enabled = true;

            Vector3 startPos = worldCardGrid.selectedCharacter.transform.position;
            Vector3 endPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            endPos.z = 0f;

            // Midpoint
            Vector3 midPoint = (startPos + endPos) / 2f;

            // Determine bend direction
            float dir = Mathf.Sign(endPos.x - startPos.x); // >0 right, <0 left
            Vector3 perpendicular = Vector3.up * dir; // Bend up for right, down for left

            // Offset midpoint to create curve
            midPoint += perpendicular * curveStrength;

            // Generate bezier curve points
            for (int i = 0; i < segmentCount; i++)
            {
                float t = i / (segmentCount - 1f);
                Vector3 a = Vector3.Lerp(startPos, midPoint, t);
                Vector3 b = Vector3.Lerp(midPoint, endPos, t);
                Vector3 curvePoint = Vector3.Lerp(a, b, t);
                lineRenderer.SetPosition(i, curvePoint);
            }
        }
        else
        {
            lineRenderer.enabled = false;
        }
    }
}
