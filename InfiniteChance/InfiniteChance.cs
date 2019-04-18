using System;
using BepInEx;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace TwylaInfiniteChance
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Twyla.InfiniteChance", "InfiniteChance", "1.0.0")]
    public class NoMoreLimits : BaseUnityPlugin
    {
        public void Awake()
        {
            Chat.AddMessage("Infinite Chance loaded. Good luck gamers!");
            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                orig(self);
                self.maxPurchaseCount = 9000
                Harmony.AccessTools.Field(Harmony.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, 1.1f);
            };
        }
    }
}
