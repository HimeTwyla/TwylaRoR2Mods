using System;
using BepInEx;
using RoR2;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;

namespace TwylaInfiniteChance
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Twyla.InfiniteChance", "InfiniteChance", "1.0.0")]
    public class NoMoreLimits : BaseUnityPlugin
    {
        private static ConfigWrapper<int> maxPurchase { get; set; }
        public static int maxPurchases { get { return NoMoreLimits.maxPurchase.Value; } protected set { NoMoreLimits.maxPurchase.Value = value; } }
        public void Awake()
        {
            NoMoreLimits.maxPurchase = base.Config.Wrap<int>("Gamble", "Max Uses", "How many times you can use the shrine of chance.", 9000);
            Chat.AddMessage("Infinite Chance loaded. Good luck gamers!");
            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                orig(self);
                self.maxPurchaseCount = maxPurchases;
                Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, 1.1f);
                Chat.AddMessage(self.maxPurchaseCount.ToString());
            };
        }
    }
}
