using UnityEngine;
using TMPro;
using Lean.Pool;

public class DamagedPopUpText : MonoBehaviour
{
    [Header("Text Reference")]
    public TextMeshProUGUI damageText;

    [Header("Physics Settings")]
    public float initialUpwardForce = 5f;   // Adjust for how high you want the text to jump
    public float horizontalForce = 2f;      // Maximum horizontal impulse (left/right)
    public float lifetime = 2f;             // Time before the pop-up is destroyed

    private Rigidbody2D rb;

    void OnEnable()
    {
        // Cache the Rigidbody2D component
        rb = GetComponent<Rigidbody2D>();
    }

    /// <summary>
    /// Sets up the damage text and applies an initial force.
    /// </summary>
    public void SetupText(string text)
    {
        // Set the damage text
        damageText.SetText(text);

        // Calculate a random horizontal force
        float randomHorizontal = Random.Range(-horizontalForce, horizontalForce);
        Vector2 force = new Vector2(randomHorizontal, initialUpwardForce);

        // Apply the force as an impulse
        rb.AddForce(force, ForceMode2D.Impulse);

        // Automatically destroy the pop-up after its lifetime
        LeanPool.Despawn(gameObject, lifetime);
    }
}
