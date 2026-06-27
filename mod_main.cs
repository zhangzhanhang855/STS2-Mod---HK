using System;
using System.Collections.Generic;
using UnityEngine;

public enum HKCharmType { WaywardCompass, GatheringSwarm, StalwartShell, SoulCatcher, ShamanStone, Grimmchild, Kingsoul }
public enum HKCardTag { NailArt, SoulSpell, Infected, VesselBasic }

public class HollowKnightMegaIntegration : MonoBehaviour
{
    [Header("=== Hollow Knight Core Data ===")]
    public string ModName = "Hollow Knight: Void & Soul Integration";
    public int MaxSoul = 99;
    public int CurrentSoul = 0;
    public int MaxCharmNotches = 4;
    public int CurrentUsedNotches = 0;
    public bool IsOvercharmed = false;

    [Header("=== Penalty State Monitor ===")]
    public int InfectionLevel = 0;
    public bool IsFocusing = false;
    private int focusTurnCounter = 0;
    private int silksongDelayTurns = 0;

    public List<HKCharmType> EquippedCharms = new List<HKCharmType>();
    
    private PlayerController player;
    private BattleManager combatManager;

    private void Start()
    {
        player = PlayerController.Instance;
        combatManager = BattleManager.Instance;

        GameEvents.OnBattleStart += OnHKBattleStart;
        GameEvents.OnBattleVictory += OnHKVictory;
        GameEvents.OnTurnStart += OnHKTurnStart;
        GameEvents.OnTurnEnd += OnHKTurnEnd;
        GameEvents.OnCardPlayed += OnHKCardPlayed;
        GameEvents.OnCardDraw += OnHKCardDraw;
        GameEvents.OnPlayerTakeDamage += OnHKPlayerDamage;
        GameEvents.OnPlayerHeal += OnHKPlayerHeal;
        
        EquipCharm(HKCharmType.SoulCatcher);
        EquipCharm(HKCharmType.ShamanStone);
        EquipCharm(HKCharmType.Grimmchild);
    }

    private void OnDestroy()
    {
        GameEvents.OnBattleStart -= OnHKBattleStart;
        GameEvents.OnBattleVictory -= OnHKVictory;
        GameEvents.OnTurnStart -= OnHKTurnStart;
        GameEvents.OnTurnEnd -= OnHKTurnEnd;
        GameEvents.OnCardPlayed -= OnHKCardPlayed;
        GameEvents.OnCardDraw -= OnHKCardDraw;
        GameEvents.OnPlayerTakeDamage -= OnHKPlayerDamage;
        GameEvents.OnPlayerHeal -= OnHKPlayerHeal;
    }

    public void EquipCharm(HKCharmType charm)
    {
        int cost = GetCharmCost(charm);
        EquippedCharms.Add(charm);
        CurrentUsedNotches += cost;

        if (CurrentUsedNotches > MaxCharmNotches)
        {
            IsOvercharmed = true;
            Debug.LogWarning("[⚠️ OVERCHARMED] You have equipped more charms than your notch limit! You are now OVERCHARMED and will take DOUBLE damage!");
        }
    }

    private int GetCharmCost(HKCharmType charm)
    {
        switch (charm)
        {
            case HKCharmType.WaywardCompass: return 1;
            case HKCharmType.GatheringSwarm: return 1;
            case HKCharmType.StalwartShell: return 2;
            case HKCharmType.SoulCatcher: return 2;
            case HKCharmType.ShamanStone: return 3;
            case HKCharmType.Grimmchild: return 2;
            case HKCharmType.Kingsoul: return 5;
            default: return 1;
        }
    }

    private void OnHKCardDraw(CardInstance drawnCard)
    {
        if (UnityEngine.Random.Range(0, 100) < 20)
        {
            drawnCard.HKTags.Add(HKCardTag.Infected);
            drawnCard.CardName = "[Infected] " + drawnCard.CardName;
            InfectionLevel++;
            Debug.Log($"[☣️ INFECTION] Your card has been infected by the plague. Current Infection Level: {InfectionLevel}");
        }
    }

    private void OnHKCardPlayed(CardInstance card)
    {
        if ((card.HKTags.Contains(HKCardTag.NailArt) || card.HKTags.Contains(HKCardTag.SoulSpell)) && UnityEngine.Random.Range(0, 100) < 10)
        {
            silksongDelayTurns += 1;
            Debug.LogError("[🤡 SILKSONG DELAY] Team Cherry went for coffee. Your powerful combo is 'still in development'. Your turn ends immediately!");
            combatManager.EndCurrentTurnImmediately();
            return;
        }

        if (card.BaseType == CardType.Attack)
        {
            int soulGain = 11;
            if (EquippedCharms.Contains(HKCharmType.SoulCatcher)) soulGain += 4;
            
            CurrentSoul = Mathf.Min(MaxSoul, CurrentSoul + soulGain);
            Debug.Log($"[⚪ SOUL GAINED] Nail struck the enemy. Current Soul: {CurrentSoul}/{MaxSoul}");
        }

        if (card.HKTags.Contains(HKCardTag.SoulSpell))
        {
            if (CurrentSoul >= 33)
            {
                CurrentSoul -= 33;
                int extraDamage = 15;
                if (EquippedCharms.Contains(HKCharmType.ShamanStone)) extraDamage += 10;
                
                combatManager.ApplyAoEDamageToEnemies(extraDamage);
                Debug.Log($"[🔮 HOWLING WRAITHS] Consumed 33 Soul. Cast Howling Wraiths! Dealt {extraDamage} AoE damage.");
            }
            else
            {
                player.CurrentHealth -= 10;
                Debug.LogWarning("[❌ SOUL DEPLETED] Cast spell without enough Soul. Suffered void backlash! Lost 10 HP.");
            }
        }

        if (card.HKTags.Contains(HKCardTag.Infected))
        {
            InfectionLevel = Mathf.Max(0, InfectionLevel - 1);
        }
    }

    private void OnHKTurnStart()
    {
        if (silksongDelayTurns > 0)
        {
            silksongDelayTurns--;
            player.CurrentActionPoints = 0;
            Debug.LogWarning($"[🤡 DELAY PENALTY] The game is not out yet. You get 0 AP this turn! Delay turns remaining: {silksongDelayTurns}");
        }

        if (EquippedCharms.Contains(HKCharmType.Kingsoul))
        {
            CurrentSoul = Mathf.Min(MaxSoul, CurrentSoul + 4);
        }

        if (IsFocusing)
        {
            focusTurnCounter++;
            if (focusTurnCounter >= 1)
            {
                if (CurrentSoul >= 33)
                {
                    CurrentSoul -= 33;
                    player.CurrentHealth = Mathf.Min(player.MaxHealth, player.CurrentHealth + 20);
                    Debug.Log("[❤️ FOCUS SUCCESS] You maintained focus. Consumed 33 Soul and restored 20 HP.");
                }
                IsFocusing = false;
                focusTurnCounter = 0;
            }
        }

        if (EquippedCharms.Contains(HKCharmType.Grimmchild))
        {
            if (UnityEngine.Random.Range(0, 100) < 30)
            {
                player.CurrentHealth -= 5;
                Debug.Log("[🔥 GRIMMCHILD] Grimmchild missed! You took 5 friendly fire damage.");
            }
            else
            {
                combatManager.ApplyDamageToRandomEnemy(8);
                Debug.Log("[🔥 GRIMMCHILD] Grimmchild dealt 8 fire damage to a random enemy.");
            }
        }
    }

    private void OnHKTurnEnd()
    {
        int remainingInfection = 0;
        foreach (var card in player.Hand)
        {
            if (card.HKTags.Contains(HKCardTag.Infected))
            {
                remainingInfection++;
            }
        }

        if (remainingInfection > 0)
        {
            int plagueDamage = remainingInfection * 8;
            player.CurrentHealth -= plagueDamage;
            player.Deck.Add(CardFactory.CreateCard("InfectedSap"));
            Debug.LogError($"[💥 INFECTION BURST] Turn ended with {remainingInfection} infected cards in hand! They exploded, dealing {plagueDamage} unblockable damage.");
        }
    }

    private void OnHKPlayerDamage(ref int incomingDamage)
    {
        if (incomingDamage <= 0) return;

        if (IsFocusing)
        {
            IsFocusing = false;
            focusTurnCounter = 0;
            CurrentSoul = Mathf.Max(0, CurrentSoul - 33);
            incomingDamage = Mathf.RoundToInt(incomingDamage * 1.5f);
            Debug.LogError("[💥 FOCUS BROKEN] You were hit while Focusing! Heal failed, 33 Soul lost, and you take 1.5x damage!");
        }

        if (IsOvercharmed)
        {
            incomingDamage *= 2;
            Debug.LogWarning($"[⚠️ OVERCHARMED DAMAGE] Due to Overcharm, incoming damage is doubled to {incomingDamage}!");
        }

        if (EquippedCharms.Contains(HKCharmType.StalwartShell))
        {
            incomingDamage = Mathf.Max(1, incomingDamage - 3);
            Debug.Log("[🛡️ STALWART SHELL] Charm activated. Reduced damage by 3.");
        }
    }

    private void OnHKPlayerHeal(ref int healAmount)
    {
        if (!IsFocusing)
        {
            healAmount = Mathf.RoundToInt(healAmount * 0.2f);
            Debug.LogWarning("[❌ HALLOWNEST CURSE] Normal healing is 80% less effective. Rely on Focus!");
        }
    }

    private void OnHKVictory()
    {
        if (EquippedCharms.Contains(HKCharmType.WaywardCompass))
        {
            combatManager.ClearCardRewards(); 
            Debug.LogError("[🧭 LOST COMPASS] Wayward Compass equipped. You got lost in the rewards screen and missed all card choices!");
        }

        if (EquippedCharms.Contains(HKCharmType.GatheringSwarm))
        {
            int goldFound = combatManager.GetRewardGold();
            player.LoseGold(goldFound);
            Debug.Log($"[🪙 GATHERING SWARM] Converted {goldFound} Gold into Geo! Unfortunately, standard merchants don't accept bugs' currency.");
        }
    }

    private void OnHKBattleStart()
    {
    }

    public void CommandFocus()
    {
        if (CurrentSoul < 33)
        {
            Debug.LogWarning("Not enough Soul to Focus! Requires at least 33.");
            return;
        }
        IsFocusing = true;
        focusTurnCounter = 0;
        Debug.Log("[🧘 FOCUSING...] You started Focusing. If you take damage this turn, the consequences will be dire.");
    }
}

public class CardInstance
{
    public string CardName;
    public CardType BaseType;
    public List<HKCardTag> HKTags = new List<HKCardTag>();
}

public class PlayerController
{
    public static PlayerController Instance;
    public int CurrentHealth;
    public int MaxHealth;
    public int Gold;
    public int CurrentActionPoints;
    public List<CardInstance> Hand = new List<CardInstance>();
    public List<CardInstance> Deck = new List<CardInstance>();
    public void LoseGold(int amount) => Gold -= amount;
}

public class BattleManager
{
    public static BattleManager Instance;
    public bool IsInCombat;
    public void EndCurrentTurnImmediately() { }
    public void ApplyAoEDamageToEnemies(int dmg) { }
    public void ApplyDamageToRandomEnemy(int dmg) { }
    public void ClearCardRewards() { }
    public int GetRewardGold() => 100;
}

public static class GameEvents
{
    public static Action OnBattleStart;
    public static Action OnBattleVictory;
    public static Action OnTurnStart;
    public static Action OnTurnEnd;
    public static Action<CardInstance> OnCardPlayed;
    public static Action<CardInstance> OnCardDraw;
    
    public delegate void DamageDelegate(ref int damage);
    public static DamageDelegate OnPlayerTakeDamage;

    public delegate void HealDelegate(ref int heal);
    public static HealDelegate OnPlayerHeal;
}

public class CardFactory
{
    public static CardInstance CreateCard(string id) => new CardInstance { CardName = "Infected Sap", BaseType = CardType.Status, HKTags = new List<HKCardTag> { HKCardTag.Infected } };
}
