using UnityEngine;
using Lean.Pool;
using System.Collections;
using UnityEngine.EventSystems;

public class CharacterCard : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    private CardStatsHUDManager cardStatsHUDManager;
    public WorldCardGrid worldCardGrid;
    public int gridIndex;

    [Header("Character Data")]
    public CharacterData characterData;
    public SpriteRenderer frameSpriteRenderer;
    public Animator anim;

    [Header("Damaged Variable")]
    public DamagedPopUpText damagedPopTextPrefab;
    public CharacterCardEffect characterCardEffect;
    public Material initMaterial;
    public Material damagedMaterial;
    public GameObject bloodSplatterPrefab;

    [HideInInspector]
    public int currentHealth;

    // Local copies of stats for easy reference
    private int armor;
    private float agility;
    private int damage;

    public Transform cardHead;
    public bool isSelected;
    public bool isPlayer = true;
    private bool isHovered;
    public GameObject hoveredIndicator;

    public void SetupCard()
    {
        cardStatsHUDManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<CardStatsHUDManager>();
        characterCardEffect = GetComponent<CharacterCardEffect>();
        initMaterial = frameSpriteRenderer.material;
        characterCardEffect.CardSelectedIndicator(false);

        if (characterData != null)
        {
            currentHealth = characterData.maxHealth;
            armor = characterData.armor;
            agility = characterData.agility;
            damage = characterData.damage;
            anim.runtimeAnimatorController = characterData.characterAnimator;
        }
        else
        {
            Debug.LogError($"{name} has no CharacterData assigned!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && isPlayer)
        {
            if(isSelected == false)
            {
                worldCardGrid.SetupSelectedCharacter(this);
                characterCardEffect.CardSelectedIndicator(true);
                SetCardStatsHUD();
                SelectingCard(true);
            }
            else
            {
                DeselectCard();
            }
            Debug.Log("Object selected!");
        }

        if(eventData.button == PointerEventData.InputButton.Left && isPlayer == false)
        {
            if (worldCardGrid.selectedCharacter == null) return;
            SelectingCard(false);
            TakeDamage(worldCardGrid.selectedCharacter.characterData.damage);

            worldCardGrid.selectedCharacter.DeselectCard();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && !isHovered)
        {
            isHovered = true;
            SelectingCard(true);
            characterCardEffect.CardSelectedIndicator(true);
            Debug.Log("Is Hovered!");
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected && isHovered)
        {
            isHovered = false;
            SelectingCard(false);
            Debug.Log("Is Not Hovered!");
        }

        if (isSelected == false)
        {
            characterCardEffect.CardSelectedIndicator(false);
        }
    }

    public void SelectingCard(bool isSelecting)
    {
        if (isSelecting)
        {
            SetCardStatsHUD();
        }
        else if (!isSelected) // Only reset when not selected
        {
            if (isPlayer)
            {
                cardStatsHUDManager.PlayerCardUnhovered();
            }
            else
            {
                cardStatsHUDManager.EnemiesCardUnhovered();
            }
        }
    }

    private void SetCardStatsHUD()
    {
        if (isPlayer)
        {
            cardStatsHUDManager.SetupPlayerStats(characterData);
        }
        else
        {
            cardStatsHUDManager.SetupEnemiesStats(characterData);
        }
    }

    public void DeselectCard()
    {
        isSelected = false;
        characterCardEffect.borderCardFrameImage.SetActive(false);
        characterCardEffect.CardSelectedIndicator(false);
    }

    public void Attack(CharacterCard target)
    {
        target.TakeDamage(damage);
    }

    public void TakeDamage(int incomingDamage)
    {
        if (isPlayer == false)
        {
            worldCardGrid.cardTurnHandler.playerTurnAmount -= 1;
            worldCardGrid.cardTurnHandler.SetupEnergyTurn(true, (int)worldCardGrid.cardTurnHandler.playerTurnAmount);
            if ((int)worldCardGrid.cardTurnHandler.playerTurnAmount < 1)
            {
                worldCardGrid.cardTurnHandler.StartEnemyTurn();
            }
        }

        var damagePopUp = LeanPool.Spawn(damagedPopTextPrefab, transform.position, Quaternion.identity);
        if (Random.Range(0f, 1f) < agility)
        {
            damagePopUp.SetupText("Dodge!");
            return;
        }

        // Damaged Effect
        StartCoroutine(DamagedFlash());
        LeanPool.Spawn(characterCardEffect.explosionEffectPrefab, transform.position, Quaternion.identity);


        int effectiveDamage = Mathf.Max(incomingDamage - armor, 1);
        currentHealth -= effectiveDamage;
        damagePopUp.SetupText(effectiveDamage.ToString());

        for (int i = 0; i < effectiveDamage; i++)
        {
            Vector2 tempPos = new Vector2(transform.position.x + Random.Range(-.3f, .3f), transform.position.y + Random.Range(-.6f, .6f));
            LeanPool.Spawn(bloodSplatterPrefab, tempPos, Quaternion.identity);
        }

        if (currentHealth <= 0)
        {
            if (isPlayer)
            {
                worldCardGrid.playerCharacters[gridIndex] = null;
            }
            else
            {
                worldCardGrid.enemyCharacters[gridIndex] = null;
            }

            characterCardEffect.PopAndDespawn();
        }
    }

    IEnumerator DamagedFlash()
    {
        frameSpriteRenderer.material = damagedMaterial;
        yield return new WaitForSeconds(.05f);
        frameSpriteRenderer.material = initMaterial;
    }
}
