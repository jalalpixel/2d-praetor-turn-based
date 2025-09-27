using UnityEngine;

public class CameraFollows : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform playerTransform;
    [SerializeField] private float followSpeed = 5f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 5f;
    private Camera mainCamera;
    private Coroutine zoomCoroutine;

    [Header("Toggle Settings")]
    [SerializeField] private bool followPlayer = true; // default ON

    void Start()
    {
        mainCamera = Camera.main;
    }

    void LateUpdate()
    {
        if (followPlayer && playerTransform != null)
        {
            // Follow player
            Vector3 targetPos = new Vector3(playerTransform.position.x, playerTransform.position.y, -10f);
            transform.position = Vector3.Lerp(transform.position, targetPos, followSpeed * Time.deltaTime);
        }
        else
        {
            // Stay at world origin (0,0,0)
            transform.position = new Vector3(0f, 0f, -10f);
        }
    }

    /// <summary>
    /// Enable or disable following the player.
    /// </summary>
    public void SetFollowPlayer(bool value)
    {
        followPlayer = value;
    }

    /// <summary>
    /// Toggle follow on/off.
    /// </summary>
    public void ToggleFollowPlayer()
    {
        followPlayer = !followPlayer;
    }

    public void ZoomTo(float targetZoom)
    {
        if (zoomCoroutine != null)
            StopCoroutine(zoomCoroutine);

        zoomCoroutine = StartCoroutine(ZoomRoutine(targetZoom));
    }

    private System.Collections.IEnumerator ZoomRoutine(float targetZoom)
    {
        float startSize = mainCamera.orthographicSize;
        float t = 0f;

        while (Mathf.Abs(mainCamera.orthographicSize - targetZoom) > 0.01f)
        {
            t += Time.deltaTime * zoomSpeed;
            mainCamera.orthographicSize = Mathf.Lerp(startSize, targetZoom, t);
            yield return null;
        }

        mainCamera.orthographicSize = targetZoom;
    }
}
