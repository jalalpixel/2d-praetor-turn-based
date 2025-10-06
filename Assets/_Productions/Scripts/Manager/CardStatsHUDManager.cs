using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CardStatsHUDManager : MonoBehaviour
{
    private WorldAutoBattlerGrid worldAutoBattlerGrid;

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
        worldAutoBattlerGrid = GetComponent<WorldAutoBattlerGrid>();
    }

    private void Update()
    {
        if (notHovering && cooldownHoverStats >= 0)
        {
            cooldownHoverStats -= Time.deltaTime;
            if (cooldownHoverStats < 0)
            {
                if (worldAutoBattlerGrid.selectedAutoBattlerTroops == null)
                {
                    canvasGroupStats.alpha = 0f;
                    characterNameText.SetText(" ");
                }
                else
                {
                    SetupPlayerStats(worldAutoBattlerGrid.selectedAutoBattlerTroops, worldAutoBattlerGrid.selectedAutoBattlerTroops.transform);
                }
            }
        }
    }

    public void SetupPlayerStats(AutoBattlerTroops troops, Transform transform)
    {
        canvasGroupStats.transform.position = new Vector3(transform.position.x + 0.55f, transform.position.y, 0f);
        canvasGroupStats.alpha = 1f;
        notHovering = false;
        characterNameText.SetText(troops.characterName);
        healthText.SetText(troops.currentHealth.ToString());
        damageText.SetText(troops.damage.ToString());
        armorText.SetText(troops.armor.ToString());
        agilityText.SetText(troops.agility.ToString());
    }

    public void PlayerCardUnhovered()
    {
        notHovering = true;
        cooldownHoverStats = 2f;
    }
}
