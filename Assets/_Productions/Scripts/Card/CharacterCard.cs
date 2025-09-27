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
    public ParticleSystem hoveredParticlesSystem;
    public bool isSelected;
    public bool isPlayer = true;
    private bool isHovered;

    public void SetupCard()
    {
        cardStatsHUDManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<CardStatsHUDManager>();
        characterCardEffect = GetComponent<CharacterCardEffect>();
        initMaterial = frameSpriteRenderer.material;
        StartCoroutine(SetupCardIsPlaced());
        TurnOffParticleSystem(true);

        if (characterData != null)
        {
            currentHealth = characterData.maxHealth;
            armor = characterData.armor;
            agility = characterData.agility;
            damage = characterData.damage;
            frameSpriteRenderer.sprite = characterData.characterSprite;
        }
        else
        {
            Debug.LogError($"{name} has no CharacterData assigned!");
        }
    }

    IEnumerator SetupCardIsPlaced()
    {
        yield return new WaitForSeconds(.4f);
        characterCardEffect.isPlaced = true;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left && isPlayer)
        {
            worldCardGrid.SetupSelectedCard(this);
            SetCardStatsHUD();
            Debug.Log("Object selected!");
        }

        if(eventData.button == PointerEventData.InputButton.Left && isPlayer == false)
        {
            if (worldCardGrid.selectedCard == null) return;
            TakeDamage(worldCardGrid.selectedCard.characterData.damage);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isSelected && !isHovered)
        {
            isHovered = true;
            SelectingCard(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isSelected && isHovered)
        {
            isHovered = false;
            SelectingCard(false);
        }
    }

    public void SelectingCard(bool isSelecting)
    {
        if (!characterCardEffect.isPlaced) return;

        if (isSelecting)
        {
            characterCardEffect.SetBiggerCardSize();
            characterCardEffect.borderCardFrameImage.SetActive(true);
            TurnOffParticleSystem(false);
            SetCardStatsHUD();
        }
        else if (!isSelected) // Only reset when not selected
        {
            characterCardEffect.ResetSizeCard();
            characterCardEffect.borderCardFrameImage.SetActive(false);
            TurnOffParticleSystem(true);

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
        characterCardEffect.ResetSizeCard();
        characterCardEffect.borderCardFrameImage.SetActive(false);
        TurnOffParticleSystem(true);
    }

    private void TurnOffParticleSystem(bool turnOff)
    {
        var emission = hoveredParticlesSystem.emission;
        emission.rateOverTime = turnOff ? 0f : 30f;
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
                worldCardGrid.playerCards[gridIndex] = null;
            }
            else
            {
                worldCardGrid.enemyCards[gridIndex] = null;
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
