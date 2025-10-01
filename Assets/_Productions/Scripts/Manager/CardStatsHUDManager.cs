using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardStatsHUDManager : MonoBehaviour
{
    private WorldCardGrid worldCardGrid;

    [Header("Player Stats")]
    public CanvasGroup canvasGroupStats;
    public TextMeshProUGUI characterNameText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI damageText;
    public TextMeshProUGUI armorText;
    public TextMeshProUGUI agilityText;
    public float cooldownHoverStats;
    public bool notHovering;

    private void OnEnable()
    {
        worldCardGrid = GetComponent<WorldCardGrid>();
    }

    private void Update()
    {
        if (notHovering && cooldownHoverStats >= 0)
        {
            cooldownHoverStats -= Time.deltaTime;
            if(cooldownHoverStats < 0)
            {
                if(worldCardGrid.selectedCharacter == null)
                {
                    canvasGroupStats.alpha = 0f;
                    characterNameText.SetText(" ");
                }
                else
                {
                    SetupPlayerStats(worldCardGrid.selectedCharacter, worldCardGrid.selectedCharacter.transform);
                }
            }
        }
    }

    public void SetupPlayerStats(CharacterCard card, Transform transform)
    {
        canvasGroupStats.transform.position = new Vector3(transform.position.x + 0.55f, transform.position.y, 0f);
        canvasGroupStats.alpha = 1f;
        notHovering = false;
        characterNameText.SetText(card.characterData.characterName);
        healthText.SetText(card.currentHealth.ToString());
        damageText.SetText(card.damage.ToString());
        armorText.SetText(card.armor.ToString());
        agilityText.SetText(card.agility.ToString());
    }

    public void PlayerCardUnhovered()
    {
        notHovering = true;
        cooldownHoverStats = 2f;
    }
}
