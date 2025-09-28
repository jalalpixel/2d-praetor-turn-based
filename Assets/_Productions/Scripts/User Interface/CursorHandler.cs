using UnityEngine;
using Lean.Pool;

public class CursorHandler : MonoBehaviour
{
    [Header("Cursor Settings")]
    [SerializeField] private GameObject cursorPrefab;

    private Camera mainCamera;
    private Transform cursorTransform;

    void Start()
    {
        mainCamera = Camera.main;

        // Hide system cursor
        Cursor.visible = false;

        // Spawn custom cursor prefab
        GameObject cursorObj = Instantiate(cursorPrefab);
        cursorTransform = cursorObj.transform;
    }

    void Update()
    {
        if (cursorTransform == null) return;

        // Convert mouse position to world position
        Vector3 mousePos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        cursorTransform.position = mousePos;
    }

    void OnDestroy()
    {
        // Show system cursor again when leaving play mode or destroying object
        Cursor.visible = true;
    }
}
