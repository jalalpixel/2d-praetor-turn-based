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
        if (worldCardGrid.cardTurnHandler.playerTurnAmount < 1) return;

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
            worldCardGrid.selectedCharacter.Attack(this);

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
        worldCardGrid.selectedCharacter = null;
        characterCardEffect.borderCardFrameImage.SetActive(false);
        characterCardEffect.CardSelectedIndicator(false);
    }

    public void Attack(CharacterCard target)
    {
        StartCoroutine(AttackRoutine(target));
        Debug.Log(target);
    }

    private IEnumerator AttackRoutine(CharacterCard target)
    {
        if (target == null) yield break;

        // Save original position
        Vector3 startPos = transform.position;

        // Step 1: Move toward target
        Vector3 targetPos = target.transform.position + (transform.position - target.transform.position).normalized * 0.1f; // stop near target
        anim.SetFloat("MoveSpeed", 1.1f); // running animation
        while (Vector3.Distance(transform.position, targetPos) > 0.0015f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, 2f * Time.deltaTime);
            if(target.transform.position.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = Vector3.one;
            }
            yield return null;
        }
        anim.SetFloat("MoveSpeed", 0f);

        // Step 2: Randomize attack type
        int attackType = Random.Range(1, 3); // 1 or 2

        // Step 3: Play attack animation
        anim.Play("Attack" + attackType);

        // Wait a bit before slash
        yield return new WaitForSeconds(0.5f);

        // Step 4: Play slash animation
        anim.Play("Attack" + attackType + "_Slash");

        // Deal damage
        target.TakeDamage(damage);

        // Wait until slash animation ends
        yield return new WaitForSeconds(0.3f);

        // Step 5: Move back to original position
        anim.SetFloat("MoveSpeed", 1.1f);
        while (Vector3.Distance(transform.position, startPos) > 0.05f)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, 5f * Time.deltaTime); 
            if (startPos.x < transform.position.x)
            {
                transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                transform.localScale = Vector3.one;
            }
            yield return null;
        }
        anim.SetFloat("MoveSpeed", 0f);
        anim.SetTrigger("Idle");
        transform.localScale = Vector3.one;
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
