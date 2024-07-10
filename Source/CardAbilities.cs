using HarmonyLib;
using HyperCard;
using LOR_XML;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace LOR_Mawan_Mod
{

    public class LOR_Mawan_Mod : ModInitializer
    {
        override public void OnInitializeMod()
        {
            base.OnInitializeMod();
            Harmony harmony = new Harmony("LoR.khatharsis.silverWind");
            harmony.PatchAll();
            Harmony.DEBUG = true;
        }

        /** Returns a random values from this list. */
        public static T getRandomValues<T>(List<T> list)
        {
            var random = new System.Random();
            var index = random.Next(list.Count);
            return list[index];
        }
    }

    public class DiceCardSelfAbility_energy1ally : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]: Restore 1 Light to a random ally.";
        public new string[] Keywords = { "Energy_Keyword" };
        public override void OnUseCard()
        {
            var random = new UnityEngine.Random();
            List<BattleUnitModel> aliveUnits = BattleObjectManager.instance.GetAliveList(this.owner.faction);
            aliveUnits.Remove(this.owner);
            if (aliveUnits.Count > 1)
                LOR_Mawan_Mod.getRandomValues(aliveUnits).cardSlotDetail.RecoverPlayPointByCard(1);
            else
                owner.cardSlotDetail.RecoverPlayPointByCard(1);
        }
    }

    public class DiceCardAbility_energy2atk : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Restore 2 Light.";
        public new string[] Keywords = { "Energy_Keyword" };
        public override void OnSucceedAttack()
        {
            FileLog.Log("Using ability: energy2atk");
            this.owner.cardSlotDetail.RecoverPlayPointByCard(2);
        }
    }
    public class DiceCardSelfAbility_energy1ally2 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]: Restore 2 Light to 2 random allies.";
        public new string[] Keywords = { "Energy_Keyword" };
        public override void OnUseCard()
        {
            List<BattleUnitModel> aliveUnits = BattleObjectManager.instance.GetAliveList(this.owner.faction);
            aliveUnits.Remove(this.owner);
            if (aliveUnits.Count >= 2)
            {
                var firstWinner = LOR_Mawan_Mod.getRandomValues(aliveUnits);
                aliveUnits.Remove(firstWinner);
                var secondWinner = LOR_Mawan_Mod.getRandomValues(aliveUnits);
                firstWinner.cardSlotDetail.RecoverPlayPointByCard(2);
                secondWinner.cardSlotDetail.RecoverPlayPointByCard(2);
                return;
            }
            if (aliveUnits.Count == 1)
            {
                aliveUnits.First().cardSlotDetail.RecoverPlayPointByCard(2);
                owner.cardSlotDetail.RecoverPlayPointByCard(2);
                return;
            }
            owner.cardSlotDetail.RecoverPlayPointByCard(4);
        }
    }
    public class DiceCardAbility_clashdraw1_max1 : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Win] Draw 1 Page (1 time).";
        public new string[] Keywords = { "DrawCard_Keyword" };

        bool hasAlreadyRolled = false;
        public override void OnWinParryingDefense()
        {
            this.owner.allyCardDetail.DrawCards(1);
            behavior.abilityList.Remove(this);
        }
    }
    public class DiceCardSelfAbility_energy1ally2_draw1ally : DiceCardSelfAbility_energy1ally2
    {
        public static string Desc = "[On Use]: Restore 2 Light to 2 random allies. A random ally draws a page.";
        public new string[] Keywords = { "DrawCard_Keyword", "Energy_Keyword" };
        public override void OnUseCard()
        {
            base.OnUseCard();
            List<BattleUnitModel> aliveUnits = BattleObjectManager.instance.GetAliveList(this.owner.faction);
            aliveUnits.Remove(this.owner);
            if (aliveUnits.Count >= 1)
                LOR_Mawan_Mod.getRandomValues(aliveUnits).allyCardDetail.DrawCards(1);
            else
                owner.allyCardDetail.DrawCards(1);
        }
    }

    public class BattleDiceCardBuf_retention : BattleUnitBuf
    {
        private BattleDiceCardModel cardToDraw = null;
        public override void OnUseCard(BattlePlayingCardDataInUnitModel card)
        {
            base.OnUseCard(card);
            if (cardToDraw != null || card.card.XmlData.IsEgo() || card.card.XmlData.IsPersonal() || card.card.XmlData.IsExhaustOnUse())
                return;
            FileLog.Log("Retention activating on " + card.card.GetName() + "/" + card.card.GetTextId() + "/" + card.card.GetHashCode());
            cardToDraw = card.card;
        }
        public override void OnRoundEnd()
        {
            base.OnRoundEnd();
            FileLog.Log("Retention: Returning card to hand: " + cardToDraw.GetName() + "/" + cardToDraw.GetTextId() + "/" + cardToDraw.GetHashCode());
            var cardToMove = _owner.allyCardDetail._cardInUse.Find(match => match == cardToDraw);
            if (cardToMove != null)
            {
                FileLog.Log("Retention: Found by reference in _cardInUse");
            }
            else cardToMove = _owner.allyCardDetail._cardInUse.Find(match => match.GetID() == cardToDraw.GetID());
            _owner.allyCardDetail._cardInUse.Remove(cardToMove);
            FileLog.Log("Removed card from _cardInUse.");
            _owner.allyCardDetail._cardInHand.Add(cardToMove);
            _owner.bufListDetail.RemoveBuf(this);
        }
    }

    public class DiceCardSelfAbility_retention : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]: Return the first combat page played this scene to hand at the end of the scene.\r\n" +
            "(Excluding On-play, Single-Use, EGO and character-specific pages).";
        public new string[] Keywords = { "Instant_Keyword" };
        public override void OnUseInstance(BattleUnitModel unit, BattleDiceCardModel self, BattleUnitModel targetUnit)
        {
            base.OnUseInstance(unit, self, targetUnit);
            var buf = new BattleDiceCardBuf_retention();
            unit.bufListDetail.AddBuf(buf);
        }
    }
}
