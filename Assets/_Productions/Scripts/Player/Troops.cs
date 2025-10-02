using UnityEngine;

public class Troops : MonoBehaviour
{
    public TroopsMovement troopsMovement;
    public CharacterData troopsData;

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetupTroops(Transform playerTransform, CharacterData characterData)
    {
        troopsMovement.player = playerTransform;
        troopsData = characterData;
    }
}
