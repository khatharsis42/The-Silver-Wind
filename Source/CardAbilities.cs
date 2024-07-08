using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

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
    }

    public class DiceCardSelfAbility_energy1ally : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]: Restore 1 Light to a random ally.";
        public override void OnUseCard()
        {
            var random = new Random();
            List<BattleUnitModel> aliveUnits = BattleObjectManager.instance.GetAliveList(this.owner.faction);
            int index = 0;
            if (aliveUnits.Count > 1) do
                {
                    index = random.Next(aliveUnits.Count);
                } while (aliveUnits[index] == this.owner);
            aliveUnits[index].cardSlotDetail.RecoverPlayPoint(1);
            FileLog.Log("Using ability: energy1ally on " + index);
        }
    }

    public class DiceCardAbility_energy2atk : DiceCardAbilityBase
    {
        public static string Desc = "[On Hit] Restore 2 Light.";
        public override void OnSucceedAttack()
        {
            FileLog.Log("Using ability: energy2atk");
            this.owner.cardSlotDetail.RecoverPlayPoint(2);
        }
    }
    public class DiceCardSelfAbility_energy1ally2 : DiceCardSelfAbilityBase
    {
        public static string Desc = "[On Use]: Restore 2 Light to 2 random allies.";
        public override void OnUseCard()
        {
            var random = new Random();
            List<BattleUnitModel> aliveUnits = BattleObjectManager.instance.GetAliveList(this.owner.faction);
            int index = 0;
            if (aliveUnits.Count > 1) do
                {
                    index = random.Next(aliveUnits.Count);
                } while (aliveUnits[index] == this.owner);
            aliveUnits[index].cardSlotDetail.RecoverPlayPoint(1);
            aliveUnits.Remove(aliveUnits[index]);
            FileLog.Log("Using ability: energy1ally2 on " + index);
            index = 0;
            if (aliveUnits.Count > 1) do
                {
                    index = random.Next(aliveUnits.Count);
                } while (aliveUnits[index] == this.owner);
            aliveUnits[index].cardSlotDetail.RecoverPlayPoint(1);
            FileLog.Log("Using ability: energy1ally2 on " + index);
        }
    }
    public class DiceCardAbility_clashdraw1_max1 : DiceCardAbilityBase
    {
        public static string Desc = "[On Clash Win] Draw 1 Page (1 time).";
        bool hasAlreadyRolled = false;
        public override void OnWinParryingDefense()
        {

            if (!hasAlreadyRolled)
            {
                FileLog.Log("Using ability: clashdraw1_max1");
                this.owner.allyCardDetail.DrawCards(1);
            } else {
            FileLog.Log("Can't use ability: clashdraw1_max1");
                hasAlreadyRolled = true;
            }
        }
    }
}
