using System;
using System.Globalization;
using BepInEx;
using RoR2;
using BepInEx.Configuration;
using UnityEngine;
using UnityEngine.Networking;
using MonoMod.Cil;
using Mono.Cecil.Cil;
using System.Collections;
using R2API.Utils;

namespace InfiniteChance2
{
    [BepInDependency("com.bepis.r2api")]
    [BepInPlugin("com.Twyla.InfiniteChance", "InfiniteChance", "1.4.0")]
    public class InfiniteChance : BaseUnityPlugin
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
        public static int maxPurchases { get { return InfiniteChance.maxPurchase.Value; } protected set { InfiniteChance.maxPurchase.Value = value; } }


        public void Awake ()
        {
            float CostMult = CostMulti(Config.Wrap("Money", "CostMultiplier", "By how much the cost will be multiplied after each purchase", "1.2").Value);
            InfiniteChance.maxPurchase = base.Config.Wrap<int>("Gamble", "Max Uses", "How many items you'll get before you start regretting your gambling addiction.", 9000);
            Chat.AddMessage("Infinite Chance loaded. Good luck you sick bastards!");

            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                orig(self);
                self.maxPurchaseCount = maxPurchases;
                HarmonyLib.AccessTools.Field(HarmonyLib.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, CostMult);
            };

            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, interactor) =>
            {
                orig(self, interactor);

                self.SetFieldValue("refreshTimer", 0f);
            };
        }
    }
}
