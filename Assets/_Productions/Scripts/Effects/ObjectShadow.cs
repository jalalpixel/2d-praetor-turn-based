using UnityEngine;

public class ObjectShadow : MonoBehaviour
{
    public Transform mainObject;  // Reference to the main object (e.g., gun)
    public bool isFollowingMainObject = true;
    [SerializeField] private SpriteRenderer mainSpriteRenderer;
    private SpriteRenderer shadowSpriteRenderer;
    public float positionY = 0.1f;
    public float positionX;

    public bool isFadingShadow = true;
    public float initShadowAlpha = 0.4f;

    public Vector3 initShadowPosition;

    public bool isFollowingScale;
    public Transform subjectTargetScale;

    void OnEnable()
    {
        initShadowPosition = transform.position;
        if(isFollowingMainObject)
        {
            // Get the SpriteRenderer components
            mainSpriteRenderer = mainObject.GetComponent<SpriteRenderer>();
            shadowSpriteRenderer = GetComponent<SpriteRenderer>();
            shadowSpriteRenderer.sprite = mainSpriteRenderer.sprite;
            shadowSpriteRenderer.color = new Color(0, 0, 0, initShadowAlpha);
        }
        else
        {
            shadowSpriteRenderer = GetComponent<SpriteRenderer>();
            shadowSpriteRenderer.color = new Color(0, 0, 0, initShadowAlpha);
        }
    }

    void LateUpdate()
    {
        transform.localPosition = new Vector3(positionX, positionY, mainObject.position.z);
        if (isFollowingMainObject)
        {
            shadowSpriteRenderer.sprite = mainSpriteRenderer.sprite;
        }
        transform.rotation = mainObject.transform.rotation; 

        if(shadowSpriteRenderer.color.a != 1 && isFadingShadow)
        {
            float tempAlpha = shadowSpriteRenderer.color.a;
            shadowSpriteRenderer.color = new Color(shadowSpriteRenderer.color.r, shadowSpriteRenderer.color.g, shadowSpriteRenderer.color.b, tempAlpha -= Time.deltaTime / 3f);
        }

        if (isFollowingScale)
        {
            gameObject.transform.localScale = subjectTargetScale.transform.localScale;
        }
    }
}
