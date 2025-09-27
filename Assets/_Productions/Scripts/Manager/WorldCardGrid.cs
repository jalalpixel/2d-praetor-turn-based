using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using Sirenix.OdinInspector;

public class WorldCardGrid : MonoBehaviour
{
    [Header("Card Setup")]
    public GameObject cardPrefab;
    public CardDataManager cardDataManager;
    public CardTurnHandler cardTurnHandler;

    [Header("Grid Settings")]
    public int cardsPerRow = 5;         // 5 cards horizontally
    public float cardSpacing = 2f;      // Space between cards

    [Header("Starting Positions")]
    public Vector2 playerGridStartPosition = new Vector2(-4, -3);
    public Vector2 enemyGridStartPosition = new Vector2(-4, 3);

    // Lists to store the spawned cards for players and enemies
    public List<GameObject> playerCards = new List<GameObject>();
    public List<GameObject> enemyCards = new List<GameObject>();

    public List<GameObject> GetEnemyCards()
    {
        return new List<GameObject>(enemyCards);
    }

    public List<GameObject> GetPlayerCards()
    {
        return new List<GameObject>(playerCards);
    }

    [Header("Fly-In Effect Settings")]
    [Tooltip("Offset from the target position where the card will spawn (e.g., (0,10) spawns 10 units above).")]
    public Vector2 flyFromOffset = new Vector2(0, 10f);
    [Tooltip("Initial speed of the card while flying in.")]
    public float flySpeed = 10f;
    [Tooltip("Steering acceleration for the homing effect.")]
    public float steeringAcceleration = 30f;
    [Tooltip("How quickly the card rotates toward its movement direction (degrees per second).")]
    public float rotateSpeed = 360f;
    [Tooltip("Distance threshold to consider the card has reached its target.")]
    public float targetThreshold = 0.1f;

    [Header("Spawn Delay Settings")]
    [Tooltip("Delay between each card spawn in seconds.")]
    public float spawnDelay = 0.2f;


    [Header("Selected Card")]
    public CharacterCard selectedCard;

    void Start()
    {
        cardTurnHandler = GetComponent<CardTurnHandler>();

        // Spawn player and enemy grids concurrently with delays.
        StartCoroutine(SpawnGrid(playerGridStartPosition, playerCards, true));
        StartCoroutine(SpawnGrid(enemyGridStartPosition, enemyCards, false));
    }

    /// <summary>
    /// Spawns a row of cards from the given target position, with a delay between each card.
    /// </summary>
    /// <param name="targetStartPos">The final grid start position.</param>
    /// <param name="spawnedCards">List to store the spawned card GameObjects.</param>
    private IEnumerator SpawnGrid(Vector2 targetStartPos, List<GameObject> spawnedCards, bool isPlayer)
    {
        for (int i = 0; i < cardsPerRow; i++)
        {
            // Calculate the final target position for this card.
            Vector3 targetPosition = new Vector3(targetStartPos.x + i * cardSpacing, targetStartPos.y, 0);
            // Calculate the initial spawn position using the fly offset.
            Vector3 spawnPosition = targetPosition + (Vector3)flyFromOffset;
            // Optionally, add a random initial tilt.
            Quaternion startRotation = Quaternion.Euler(0, 0, Random.Range(-30f, 30f));

            // Spawn the card using LeanPool.
            GameObject card = LeanPool.Spawn(cardPrefab, spawnPosition, startRotation);
            CharacterCard characterCard = card.GetComponent<CharacterCard>();
            characterCard.characterData = cardDataManager.characterDatas[Random.Range(0, cardDataManager.characterDatas.Length)];
            characterCard.SetupCard();
            characterCard.worldCardGrid = this;
            characterCard.gridIndex = i;

            if (isPlayer == false)
            {
                characterCard.isPlayer = false;
            }
            card.name = $"Card_{targetStartPos.y}_{i}";
            spawnedCards.Add(card);

            // Animate the card using homing-style movement.
            StartCoroutine(AnimateCardHoming(card, targetPosition));

            // Wait before spawning the next card.
            yield return new WaitForSeconds(spawnDelay);
        }

        // Setup turn
        if (isPlayer)
        {
            cardTurnHandler.SetupPlayerTurn();
        }
        else
        {
            cardTurnHandler.SetupEnemiesTurn();
        }
    }

    /// <summary>
    /// Destroys all spawned cards and clears the lists.
    /// </summary>
    [Button("Delete All Cards")]
    public void DeleteAllCards()
    {
        DeleteAllEnemiesCard();
        DeleteAllPlayerCards();
    }

    [Button("Delete All Player Cards")]
    public void DeleteAllPlayerCards()
    {
        foreach (var card in playerCards)
        {
            if (card != null)
            {
                CharacterCard m_card = card.GetComponent<CharacterCard>();
                m_card.characterCardEffect.PopAndDespawn();
            }
        }
        playerCards.Clear();
    }

    [Button("Delete All Enemies Cards")]
    public void DeleteAllEnemiesCard()
    {
        foreach (var card in enemyCards)
        {
            if (card != null)
            {
                CharacterCard m_card = card.GetComponent<CharacterCard>();
                m_card.characterCardEffect.PopAndDespawn();
            }
        }
        enemyCards.Clear();
    }

    /// <summary>
    /// Respawns new cards by deleting the old ones and creating new grids with delayed spawning.
    /// </summary>
    [Button("Spawn Cards")]
    public void RespawnAllCards()
    {
        DeleteAllCards();
        StartCoroutine(SpawnGrid(playerGridStartPosition, playerCards, true));
        StartCoroutine(SpawnGrid(enemyGridStartPosition, enemyCards, false));
    }

    [Button("Spawn Player Cards")]
    public void SpawnPlayerCard()
    {
        StartCoroutine(SpawnGrid(playerGridStartPosition, playerCards, true));
    }

    [Button("Spawn Enemies Cards")]
    public void SpawnEnemiesCard()
    {
        StartCoroutine(SpawnGrid(enemyGridStartPosition, enemyCards, false));
    }

    /// <summary>
    /// Animates a card from its spawn position to its target position using a homing-style movement.
    /// The card rotates to follow its movement direction while keeping its cardHead (if assigned) upright.
    /// </summary>
    /// <param name="card">The card GameObject to animate.</param>
    /// <param name="targetPosition">The destination position on the grid.</param>
    private IEnumerator AnimateCardHoming(GameObject card, Vector3 targetPosition)
    {
        // Initialize velocity toward the target.
        Vector3 velocity = (targetPosition - card.transform.position).normalized * flySpeed;

        while (Vector3.Distance(card.transform.position, targetPosition) > targetThreshold)
        {
            // Calculate the desired velocity.
            Vector3 desiredVelocity = (targetPosition - card.transform.position).normalized * flySpeed;
            // Compute steering to adjust current velocity.
            Vector3 steering = desiredVelocity - velocity;
            steering = Vector3.ClampMagnitude(steering, steeringAcceleration * Time.deltaTime);
            velocity += steering;
            card.transform.position += velocity * Time.deltaTime;

            // Rotate the card so its "top" points in the direction of movement.
            float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg - 90f;
            Quaternion targetRot = Quaternion.Euler(0, 0, angle);
            card.transform.rotation = Quaternion.RotateTowards(card.transform.rotation, targetRot, rotateSpeed * Time.deltaTime);

            // If the card has a CharacterCard component with a cardHead assigned, keep it upright.
            CharacterCard cardComp = card.GetComponent<CharacterCard>();
            if (cardComp != null && cardComp.cardHead != null)
            {
                cardComp.cardHead.rotation = Quaternion.identity;
            }

            yield return null;
        }

        // Snap the card exactly to its target position and rotation.
        card.transform.position = targetPosition;
        card.transform.rotation = Quaternion.identity;

        // Ensure the cardHead remains upright.
        CharacterCard finalCardComp = card.GetComponent<CharacterCard>();
        if (finalCardComp != null && finalCardComp.cardHead != null)
        {
            finalCardComp.cardHead.rotation = Quaternion.identity;
        }
    }

    public void SetupSelectedCard(CharacterCard tempCard)
    {
        if (selectedCard != null)
        {
            selectedCard.DeselectCard();
        }
        selectedCard = tempCard;
        selectedCard.isSelected = true;
    }
}
