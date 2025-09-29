using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;

public class CardTurnHandler : MonoBehaviour
{
    public WorldCardGrid worldCardGrid;
    public float enemiesAttackDelay = 1f; // Delay between each enemy attack

    [Header("Turn Variables")]
    public float playerTurnAmount;
    public float enemiesTurnAmount;
    public GameObject turnIconPrefab;
    public GameObject[] turnIconPrefabPlayerSpawned;
    public GameObject[] turnIconPrefabEnemiesSpawned;
    public RectTransform gridPanelTransformPlayer;
    public RectTransform gridPanelTransformEnemies;

    public void SetupEnergyTurn(bool isPlayer, int index) // Only called when decreasing energy turn
    {
        if (isPlayer)
        {
            Destroy(turnIconPrefabPlayerSpawned[index]);
            turnIconPrefabPlayerSpawned[index] = null;
        }
        else
        {
            Destroy(turnIconPrefabEnemiesSpawned[index]);
            turnIconPrefabEnemiesSpawned[index] = null;
        }
    }

    public void SetupPlayerTurn()
    {
        foreach (var card in worldCardGrid.playerCharacters)
        {
            CharacterCard characterCard = card?.GetComponent<CharacterCard>();
            if (characterCard == null) continue; // Skip if null

            if (characterCard.characterData.classType == CharacterData.ClassType.agility)
            {
                playerTurnAmount += 0.85f;
            }
            else
            {
                playerTurnAmount += .55f;
            }
        }

        for (int i = 0; i < (int)playerTurnAmount; i++)
        {
            turnIconPrefabPlayerSpawned[i] = LeanPool.Spawn(turnIconPrefab, gridPanelTransformPlayer);
        }
    }

    public void SetupEnemiesTurn()
    {
        foreach (var card in worldCardGrid.enemyCharacters)
        {
            CharacterCard characterCard = card?.GetComponent<CharacterCard>();
            if (characterCard == null) continue; // Skip if null

            if (characterCard.characterData.classType == CharacterData.ClassType.agility)
            {
                enemiesTurnAmount += 0.85f;
            }
            else
            {
                enemiesTurnAmount += .55f;
            }
        }

        for (int i = 0; i < (int)enemiesTurnAmount; i++)
        {
            turnIconPrefabEnemiesSpawned[i] = LeanPool.Spawn(turnIconPrefab, gridPanelTransformEnemies);
        }
    }


    public void StartEnemyTurn()
    {
        StartCoroutine(EnemyAttackRoutine(2f));
    }

    private IEnumerator EnemyAttackRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);

        List<GameObject> enemies = worldCardGrid.GetEnemyCards();
        List<GameObject> players = worldCardGrid.GetPlayerCards();

        if (enemies == null || players == null || enemies.Count == 0 || players.Count == 0 || (int)enemiesTurnAmount < 1)
        {
            Debug.Log("Enemies Won or No Enemies Left!");
            yield break; // Stop if no players or enemies exist
        }

        foreach (GameObject enemyCardObj in enemies)
        {
            if (players.Count == 0 || (int)enemiesTurnAmount < 1) break; // Stop if all players are defeated

            CharacterCard enemyCard = enemyCardObj?.GetComponent<CharacterCard>();
            if (enemyCard == null) continue; // Skip if enemyCard is null

            GameObject targetPlayerObj = null;
            CharacterCard targetPlayerCard = null;

            // Try finding a valid target
            for (int i = 0; i < players.Count; i++)
            {
                targetPlayerObj = players[Random.Range(0, players.Count)];
                targetPlayerCard = targetPlayerObj?.GetComponent<CharacterCard>();
                if (targetPlayerCard != null) break; // Found a valid target
            }

            if (targetPlayerCard != null)
            {
                enemyCard.Attack(targetPlayerCard);
                yield return new WaitForSeconds(enemiesAttackDelay);
            }

            enemiesTurnAmount -= 1;
            SetupEnergyTurn(false, (int)enemiesTurnAmount);
        }

        if ((int)enemiesTurnAmount < 1)
        {
            Debug.Log("Reset Round!");
            SetupPlayerTurn();
            SetupEnemiesTurn();
        }
    }
}
