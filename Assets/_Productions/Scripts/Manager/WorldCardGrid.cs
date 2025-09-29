using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using Sirenix.OdinInspector;

public class WorldCardGrid : MonoBehaviour
{
    private GameManager gameManager;

    [Header("Character Prefab")]
    public GameObject characterPrefab;
    public CardDataManager cardDataManager;
    public CardTurnHandler cardTurnHandler;

    [Header("Areas")]
    public Vector3 playerAreaCenter = new Vector3(-4, -3, 0);
    public Vector3 playerAreaSize = new Vector3(5, 2, 0); // width x height

    public Vector3 enemyAreaCenter = new Vector3(-4, 3, 0);
    public Vector3 enemyAreaSize = new Vector3(5, 2, 0);

    [Header("Spacing Settings")]
    public int charactersPerRow = 3;
    public float spawnDelay = 0.2f;

    public List<GameObject> playerCharacters = new List<GameObject>();
    public List<GameObject> enemyCharacters = new List<GameObject>();

    [Header("Selected Character")]
    public CharacterCard selectedCharacter;

    void Start()
    {
        gameManager = GetComponent<GameManager>();
        cardTurnHandler = GetComponent<CardTurnHandler>();

        StartCoroutine(SpawnArea(playerAreaCenter, playerAreaSize, playerCharacters, true));
        StartCoroutine(SpawnArea(enemyAreaCenter, enemyAreaSize, enemyCharacters, false));
    }

    private IEnumerator SpawnArea(Vector3 areaCenter, Vector3 areaSize, List<GameObject> spawnedList, bool isPlayer)
    {
        // Calculate min/max bounds of the box
        Vector2 min = areaCenter - areaSize / 2f;
        Vector2 max = areaCenter + areaSize / 2f;

        for (int i = 0; i < charactersPerRow; i++)
        {
            // Spread characters inside the area horizontally
            float x = Mathf.Lerp(min.x, max.x, (i + 1f) / (charactersPerRow + 1f));
            float y = Random.Range(min.y, max.y); // random Y
            Vector3 spawnPos = new Vector3(x, y, 0f);

            // Spawn character
            GameObject character = LeanPool.Spawn(characterPrefab, spawnPos, Quaternion.identity);
            CharacterCard cardComp = character.GetComponent<CharacterCard>();
            cardComp.characterData = cardDataManager.characterDatas[Random.Range(0, cardDataManager.characterDatas.Length)];
            cardComp.SetupCard();
            cardComp.worldCardGrid = this;
            cardComp.isPlayer = isPlayer;

            spawnedList.Add(character);

            yield return new WaitForSeconds(spawnDelay);
        }

        // Setup turn after spawn
        if (isPlayer)
            cardTurnHandler.SetupPlayerTurn();
        else
            cardTurnHandler.SetupEnemiesTurn();
    }

    [Button("Delete All Characters")]
    public void DeleteAllCharacters()
    {
        foreach (var c in playerCharacters) if (c != null) LeanPool.Despawn(c);
        foreach (var e in enemyCharacters) if (e != null) LeanPool.Despawn(e);
        playerCharacters.Clear();
        enemyCharacters.Clear();
    }

    public void SetupSelectedCharacter(CharacterCard tempCard)
    {
        if (selectedCharacter != null)
        {
            selectedCharacter.DeselectCard();
        }
        selectedCharacter = tempCard;
        selectedCharacter.isSelected = true;
    }

    #region Getters
    public List<GameObject> GetEnemyCards()
    {
        if (enemyCharacters == null) return new List<GameObject>();
        List<GameObject> result = new List<GameObject>(enemyCharacters.Count);
        foreach (var go in enemyCharacters)
            if (go != null)
                result.Add(go);
        return result;
    }

    public List<GameObject> GetPlayerCards()
    {
        if (playerCharacters == null) return new List<GameObject>();
        List<GameObject> result = new List<GameObject>(playerCharacters.Count);
        foreach (var go in playerCharacters)
            if (go != null)
                result.Add(go);
        return result;
    }

    public List<CharacterCard> GetEnemyCharacterCards()
    {
        List<CharacterCard> result = new List<CharacterCard>();
        foreach (var go in GetEnemyCards())
        {
            var cc = go.GetComponent<CharacterCard>();
            if (cc != null) result.Add(cc);
        }
        return result;
    }

    public List<CharacterCard> GetPlayerCharacterCards()
    {
        List<CharacterCard> result = new List<CharacterCard>();
        foreach (var go in GetPlayerCards())
        {
            var cc = go.GetComponent<CharacterCard>();
            if (cc != null) result.Add(cc);
        }
        return result;
    }
    #endregion

    // 🔹 Draw Gizmos for player and enemy spawn areas
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f); // green transparent
        Gizmos.DrawCube(playerAreaCenter, playerAreaSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(playerAreaCenter, playerAreaSize);

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f); // red transparent
        Gizmos.DrawCube(enemyAreaCenter, enemyAreaSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(enemyAreaCenter, enemyAreaSize);
    }
}
