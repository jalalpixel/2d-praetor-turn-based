using UnityEngine;
using Lean.Pool;
using System.Collections;
using UnityEngine.EventSystems;

public class CharacterCardEffect : MonoBehaviour
{
    private CharacterCard characterCard;

    [Header("Placement Settings")]
    public Rigidbody2D cardRigidbody2D;

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
        characterCard = GetComponent<CharacterCard>();

        borderCardFrameImage.SetActive(false);
        initialcardFramePosition = cardFrameSpriteRenderer.transform.localPosition;
        initialScale = cardFrameSpriteRenderer.transform.localScale;

        if (cardRigidbody2D != null)
        {
            cardRigidbody2D.gravityScale = 0f;
            cardRigidbody2D.linearVelocity = Vector2.zero;
        }

    }

    void Update()
    {

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

    public void CardSelectedIndicator(bool isSelected)
    {
        characterCard.hoveredIndicator.SetActive(isSelected);
        Debug.Log("Called");
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
