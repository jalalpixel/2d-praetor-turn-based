using UnityEngine;
using UnityEngine.UI;

public class ObjectSpriteRandomizer : MonoBehaviour
{
    public Sprite[] sprites;

    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private bool isImage;
    [SerializeField] private Image image;
    private Color originalColor;
    private float initTransparency;

    void OnEnable()
    {
        if (isImage == false)
        {
            if(spriteRenderer == null)
            {
                spriteRenderer = GetComponent<SpriteRenderer>();
            }
            SetupSpriteRandomSpriteRenderer();
        }
        else
        {
            SetupSpriteRandomImage();
        }

    }

    private void SetupSpriteRandomImage()
    {
        int randValue = Random.Range(0, sprites.Length);
        image.sprite = sprites[randValue];
    }

    private void SetupSpriteRandomSpriteRenderer()
    {
        // Capture the original color and transparency
        if (originalColor == default) // Check if originalColor is uninitialized
        {
            originalColor = spriteRenderer.color;
            initTransparency = originalColor.a;
        }

        // Reset the sprite's color to the original color with the initial transparency
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, initTransparency);

        int randValue = Random.Range(0, sprites.Length);
        spriteRenderer.sprite = sprites[randValue];
    }
}
