using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lean.Pool;
using Sirenix.OdinInspector;

public class WorldAutoBattlerGrid : MonoBehaviour
{
    [Header("References")]
    public GameObject autoBattlerTroopPrefab;
    public CharacterData[] characterDatas;
    public AutoBattlerTroops selectedAutoBattlerTroops;

    [Header("Areas")]
    public Vector3 playerAreaCenter = new Vector3(-4, -3, 0);
    public Vector3 playerAreaSize = new Vector3(5, 2, 0);

    public Vector3 enemyAreaCenter = new Vector3(-4, 3, 0);
    public Vector3 enemyAreaSize = new Vector3(5, 2, 0);

    [Header("Spawn Settings")]
    public int troopsPerRow = 3;
    public float spawnDelay = 0.2f;

    [Header("Lists")]
    public List<GameObject> playerTroops = new List<GameObject>();
    public List<GameObject> enemyTroops = new List<GameObject>();

    private void Start()
    {
        StartCoroutine(SpawnArea(playerAreaCenter, playerAreaSize, playerTroops, true));
        StartCoroutine(SpawnArea(enemyAreaCenter, enemyAreaSize, enemyTroops, false));
    }

    private IEnumerator SpawnArea(Vector3 areaCenter, Vector3 areaSize, List<GameObject> spawnedList, bool isPlayer)
    {
        Vector2 min = areaCenter - areaSize / 2f;
        Vector2 max = areaCenter + areaSize / 2f;

        for (int i = 0; i < troopsPerRow; i++)
        {
            // Position spread horizontally, random vertically
            float x = Mathf.Lerp(min.x, max.x, (i + 1f) / (troopsPerRow + 1f));
            float y = Random.Range(min.y, max.y);
            Vector3 spawnPos = new Vector3(x, y, 0f);

            // Spawn troop
            GameObject troop = LeanPool.Spawn(autoBattlerTroopPrefab, spawnPos, Quaternion.identity);
            AutoBattlerTroops troopComp = troop.GetComponent<AutoBattlerTroops>();

            troopComp.SetupTroopsData(characterDatas[Random.Range(0, characterDatas.Length)]);

            // Assign the correct layer
            troop.layer = LayerMask.NameToLayer(isPlayer ? "Player" : "Enemy");

            // For their detection logic (detect opponents)
            if (isPlayer)
                troopComp.opponentLayer = LayerMask.GetMask("Enemy");
            else
                troopComp.opponentLayer = LayerMask.GetMask("Player");

            spawnedList.Add(troop);

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    [Button("Delete All Troops")]
    public void DeleteAllTroops()
    {
        foreach (var p in playerTroops)
            if (p != null) LeanPool.Despawn(p);
        foreach (var e in enemyTroops)
            if (e != null) LeanPool.Despawn(e);

        playerTroops.Clear();
        enemyTroops.Clear();
    }

    // Optional: helper methods for other systems
    public List<AutoBattlerTroops> GetPlayerTroops()
    {
        List<AutoBattlerTroops> list = new List<AutoBattlerTroops>();
        foreach (var go in playerTroops)
        {
            if (go != null)
            {
                var t = go.GetComponent<AutoBattlerTroops>();
                if (t != null) list.Add(t);
            }
        }
        return list;
    }

    public List<AutoBattlerTroops> GetEnemyTroops()
    {
        List<AutoBattlerTroops> list = new List<AutoBattlerTroops>();
        foreach (var go in enemyTroops)
        {
            if (go != null)
            {
                var t = go.GetComponent<AutoBattlerTroops>();
                if (t != null) list.Add(t);
            }
        }
        return list;
    }

    // 🔹 Draw Gizmos for Player & Enemy spawn areas
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0f, 1f, 0f, 0.25f);
        Gizmos.DrawCube(playerAreaCenter, playerAreaSize);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(playerAreaCenter, playerAreaSize);

        Gizmos.color = new Color(1f, 0f, 0f, 0.25f);
        Gizmos.DrawCube(enemyAreaCenter, enemyAreaSize);
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(enemyAreaCenter, enemyAreaSize);
    }
}
