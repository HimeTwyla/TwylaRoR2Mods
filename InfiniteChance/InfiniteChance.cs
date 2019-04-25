using System;
using System.Globalization;
using BepInEx;
using RoR2;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using Mono.Cecil.Cil;
using MonoMod.Cil;

namespace TwylaInfiniteChance
{

    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Twyla.InfiniteChance", "InfiniteChance", "1.3.1")]


    public class NoMoreLimits : BaseUnityPlugin
    {

        public float CostMulti(string configline)
        {
            try
            {
                float x = float.Parse(configline, CultureInfo.InvariantCulture);
                return x;
            }
            catch (FormatException)
            {
                return 0.0f;
            }
        }

        private static ConfigWrapper<int> maxPurchase { get; set; }
        public static int maxPurchases { get { return NoMoreLimits.maxPurchase.Value; } protected set { NoMoreLimits.maxPurchase.Value = value; } }



        public void Awake()
        {
            
            float CostMult = CostMulti(Config.Wrap("Money", "CostMultiplier", "By how much the cost will be multiplied after each purchase", "1.1").Value);
            NoMoreLimits.maxPurchase = base.Config.Wrap<int>("Gamble", "Max Uses", "How many items you'll get before you start regretting your gambling addiction.", 9000);                     
            Chat.AddMessage("Infinite Chance loaded. Good luck you sick bastards!");
            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                orig(self);
                self.maxPurchaseCount = maxPurchases;
                Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, CostMult);
            };

            IL.RoR2.ShrineChanceBehavior.AddShrineStack += il =>
            {
                var c = new ILCursor(il).Goto(144);
                c.Remove();
                c.Emit(OpCodes.Ldc_R4, 0f);
            };
        }
    }
}
