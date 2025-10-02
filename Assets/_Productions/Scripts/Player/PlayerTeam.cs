using UnityEngine;
using Lean.Pool;

public class PlayerTeam : MonoBehaviour
{
    public CharacterData[] troopsData;
    public Troops troopsPrefab;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SpawnTroops()
    {
        for(int i = 0; i < troopsData.Length; i++)
        {
            Troops troops = LeanPool.Spawn(troopsPrefab, this.transform.position, Quaternion.identity);
            troops.SetupTroops(this.transform,troopsData[i]);
        }
    }
}
