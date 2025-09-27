using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardStatsHUDManager : MonoBehaviour
{
    private WorldCardGrid worldCardGrid;

    [Header("Player Stats")]
    public CanvasGroup canvasGroupPlayer;
    public TextMeshProUGUI characterNameTextPlayer;
    public TextMeshProUGUI healthTextPlayer;
    public TextMeshProUGUI damageTextPlayer;
    public TextMeshProUGUI armorTextPlayer;
    public TextMeshProUGUI agilityTextPlayer;
    public float cooldownHoverStatsPlayer;
    public bool notHoverPlayer;

    [Header("Enemies Stats")]
    public CanvasGroup canvasGroupEnemies;
    public TextMeshProUGUI characterNameTextEnemies;
    public TextMeshProUGUI healthTextEnemies;
    public TextMeshProUGUI damageTextEnemies;
    public TextMeshProUGUI armorTextEnemies;
    public TextMeshProUGUI agilityTextEnemies;
    public float cooldownHoverStatsEnemies;
    public bool notHoverEnemies;

    private void OnEnable()
    {
        worldCardGrid = GetComponent<WorldCardGrid>();
    }

    private void Update()
    {
        if (notHoverPlayer && cooldownHoverStatsPlayer >= 0)
        {
            cooldownHoverStatsPlayer -= Time.deltaTime;
            if(cooldownHoverStatsPlayer < 0)
            {
                if(worldCardGrid.selectedCard == null)
                {
                    canvasGroupPlayer.alpha = 0f;
                    characterNameTextPlayer.SetText(" ");
                }
                else
                {
                    SetupPlayerStats(worldCardGrid.selectedCard.characterData);
                }
            }
        }
        
        if (notHoverEnemies && cooldownHoverStatsEnemies >= 0)
        {
            cooldownHoverStatsEnemies -= Time.deltaTime;
            if(cooldownHoverStatsEnemies < 0)
            {
                if(worldCardGrid.selectedCard == null)
                {
                    canvasGroupEnemies.alpha = 0f;
                    characterNameTextEnemies.SetText(" ");
                }
            }
        }
    }


    public void SetupPlayerStats(CharacterData data)
    {
        canvasGroupPlayer.alpha = 1f;
        notHoverPlayer = false;
        characterNameTextPlayer.SetText(data.CharacterName);
        healthTextPlayer.SetText(data.maxHealth.ToString());
        damageTextPlayer.SetText(data.damage.ToString());
        armorTextPlayer.SetText(data.armor.ToString());
        agilityTextPlayer.SetText(data.agility.ToString());

    }

    public void PlayerCardUnhovered()
    {
        notHoverPlayer = true;
        cooldownHoverStatsPlayer = 2f;
    }

    public void SetupEnemiesStats(CharacterData data)
    {
        canvasGroupEnemies.alpha = 1f;
        notHoverEnemies = false;
        characterNameTextEnemies.SetText(data.CharacterName);
        healthTextEnemies.SetText(data.maxHealth.ToString());
        damageTextEnemies.SetText(data.damage.ToString());
        armorTextEnemies.SetText(data.armor.ToString());
        agilityTextEnemies.SetText(data.agility.ToString());

    }

    public void EnemiesCardUnhovered()
    {
        notHoverEnemies = true;
        cooldownHoverStatsEnemies = 2f;
    }
}
