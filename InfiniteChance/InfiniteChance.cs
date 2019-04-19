using System;
using BepInEx;
using RoR2;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;

namespace TwylaInfiniteChance
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Twyla.InfiniteChance", "InfiniteChance", "1.2.1")]
    public class NoMoreLimits : BaseUnityPlugin
    {
        public float CostMulti(string configline)
        {
            if (float.TryParse(configline, out float x))
            {
                return x;
            }
            return 0f;
        }

        float CostMult => CostMulti(Config.Wrap("Money", "CostMultiplier", "By how much the cost will be multiplied after each purchase (if this doesn't work try using a comma instead of a dot)", "1.1").Value);

        private static ConfigWrapper<int> maxPurchase { get; set; }
        public static int maxPurchases { get { return NoMoreLimits.maxPurchase.Value; } protected set { NoMoreLimits.maxPurchase.Value = value; } }


        public void Awake()
        {
            NoMoreLimits.maxPurchase = base.Config.Wrap<int>("Gamble", "Max Uses", "How many items you'll get before you start regretting your gambling addiction.", 9000);
            Chat.AddMessage("Infinite Chance loaded. Good luck you sick bastards!");
            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                orig(self);
                self.maxPurchaseCount = maxPurchases;
                Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, CostMult);
                Chat.AddMessage(self.maxPurchaseCount.ToString());
            };
        }
    }
}
