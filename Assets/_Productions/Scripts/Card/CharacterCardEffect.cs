using UnityEngine;
using Lean.Pool;
using System.Collections;
using UnityEngine.EventSystems;

public class CharacterCardEffect : MonoBehaviour
{
    [Header("Placement Settings")]
    public bool isPlaced = false;
    public Rigidbody2D cardRigidbody2D;

    [Header("Wiggle Settings")]
    [Tooltip("Maximum rotation angle in degrees.")]
    public float wiggleAmplitude = 4f;
    [Tooltip("Current wiggle speed (frequency).")]
    public float wiggleFrequency = 2f;
    [Tooltip("Minimum wiggle speed (frequency).")]
    public float wiggleMinFrequency = 1f;
    [Tooltip("Maximum wiggle speed (frequency).")]
    public float wiggleMaxFrequency = 3f;
    [Tooltip("Time interval (in seconds) to randomize the wiggle speed.")]
    public float wiggleRandomizeInterval = 2f;

    [Header("Hover Settings")]
    [Tooltip("Factor by which the X-scale increases on hover.")]
    public SpriteRenderer cardFrameSpriteRenderer;
    public GameObject borderCardFrameImage;
    public Vector2 initialcardFramePosition;
    public float hoverScaleFactor = 1.2f;
    private Vector3 initialScale;

    [Header("Pop-Off (Despawn) Settings")]
    public float popUpForce = 5f;
    public float popHorizontalForce = 2f;
    public float despawnDelay = 3f;
    public float gravityAfterPop = 1f;

    private bool isPopping = false;
    private Coroutine wiggleRoutine;

    [Header("Taking Damage Effect")]
    public GameObject explosionEffectPrefab;

    void OnEnable()
    {
        borderCardFrameImage.SetActive(false);
        initialcardFramePosition = cardFrameSpriteRenderer.transform.localPosition;
        initialScale = cardFrameSpriteRenderer.transform.localScale;

        if (cardRigidbody2D != null)
        {
            cardRigidbody2D.gravityScale = 0f;
            cardRigidbody2D.linearVelocity = Vector2.zero;
        }

        // Optionally randomize the initial wiggle amplitude.
        wiggleAmplitude = Random.Range(2f, 6f);

        // Start the coroutine that will randomize the wiggle frequency.
        wiggleRoutine = StartCoroutine(RandomizeWiggleSpeed());
    }

    void Update()
    {
        if (isPlaced && !isPopping)
        {
            // Calculate rotation angle using a sine wave.
            float angle = Mathf.Sin(Time.time * wiggleFrequency * Mathf.PI * 2) * wiggleAmplitude;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    private IEnumerator RandomizeWiggleSpeed()
    {
        while (!isPopping)
        {
            float targetFrequency = Random.Range(wiggleMinFrequency, wiggleMaxFrequency);
            float startFrequency = wiggleFrequency;
            float elapsed = 0f;

            while (elapsed < wiggleRandomizeInterval)
            {
                wiggleFrequency = Mathf.Lerp(startFrequency, targetFrequency, elapsed / wiggleRandomizeInterval);
                elapsed += Time.deltaTime;
                yield return null;
            }
            wiggleFrequency = targetFrequency;
        }
    }
    
    public void SetBiggerCardSize()
    {
        cardFrameSpriteRenderer.transform.localScale = new Vector3(initialScale.x * hoverScaleFactor, initialScale.y * hoverScaleFactor, initialScale.z);
        cardFrameSpriteRenderer.transform.localPosition = new Vector2(cardFrameSpriteRenderer.transform.localPosition.x, cardFrameSpriteRenderer.transform.localPosition.y + 0.15f);
    }


    public void ResetSizeCard()
    {
        cardFrameSpriteRenderer.transform.localScale = initialScale;
        cardFrameSpriteRenderer.transform.localPosition = initialcardFramePosition;
    }


    public void PopAndDespawn()
    {
        if (isPopping)
            return;

        isPopping = true;

        if (wiggleRoutine != null)
            StopCoroutine(wiggleRoutine);

        if (cardRigidbody2D != null)
        {
            cardRigidbody2D.gravityScale = gravityAfterPop;
            float horizontalForce = Random.Range(-popHorizontalForce, popHorizontalForce);
            Vector2 force = new Vector2(horizontalForce, popUpForce);
            cardRigidbody2D.AddForce(force, ForceMode2D.Impulse);

            var objectRotating = GetComponent<ObjectRotateOverTime>();
            if (objectRotating != null)
                objectRotating.enabled = true;
        }

        Invoke(nameof(Despawn), despawnDelay);
    }

    private void OnDisable()
    {
        var objectRotating = GetComponent<ObjectRotateOverTime>();
        if (objectRotating != null)
            objectRotating.enabled = false;
    }

    private void Despawn()
    {
        LeanPool.Despawn(gameObject);
    }
}
