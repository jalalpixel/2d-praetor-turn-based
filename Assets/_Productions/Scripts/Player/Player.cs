using UnityEngine;

public class Player : MonoBehaviour
{
    public GameManager gameManager;
    public PlayerMovement playerMovement;
    public PlayerTeam playerTeam;
    public CharacterData characterData;


    void Awake()
    {
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();        
    }

    void Update()
    {
        
    }
}
