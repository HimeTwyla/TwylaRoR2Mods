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
    [BepInPlugin("com.Twyla.InfiniteChance", "InfiniteChance", "2.0.0")]
    public class InfiniteChance : BaseUnityPlugin
    {
        public ConfigEntry<bool> DisableCooldown { get; set; }
        public ConfigEntry<bool> EnableIncreaseChance { get; private set; }
        public float FailWeight { get; set; }
        public float Failmult { get; set; }
        public float CostMult { get; set; }
        public int MaxPurchases { get; set; }
        public int GuaranteedGreen { get; set; }
        public int GuaranteedRed { get; set; }

        int success = 0;
        int fail = 0;
        float RNGWeight;

        public void Awake()
        {
            GuaranteedGreen = Config.Bind<int>("Settings", "Guaranteed Green", 5, "The number of fails before you get a guaranteed green or red item. This setting needs enableincreasechance set to true").Value;
            GuaranteedRed = Config.Bind<int>("Settings", "Guaranteed red", 10, "The number of fails before you get a guaranteed red item. This setting needs enableincreasechance set to true").Value;
            FailWeight = Config.Bind<float>("Settings", "Fail weight", 10.1f, "Vanilla is 10.1 (101 = 100% fail). This setting needs enableincreasechance set to true").Value;
            Failmult = Config.Bind<float>("Settings", "Fail multiplier", 0.95f, "Goes from 0 to 1 , the lower it goes the lower the chances to fail in a row. This setting needs enableincreasechance set to true").Value;
            CostMult = Config.Bind<float>("Settings", "Cost Multiplier", 1.2f, "The cost multiplier, vanilla is 1.2").Value;
            MaxPurchases = Config.Bind<int>("Settings", "Max Purchase count", 9000, "The number of item you'll get before the shrine turns off").Value;
            DisableCooldown = base.Config.Bind<bool>("Toggles", "Enable cooldown removal", true, "Set to True or False , if set to True , will disable the cooldown on shrines. Vanilla :false");
            EnableIncreaseChance = base.Config.Bind<bool>("Toggles", "Enable increase chance", true, "Set to True or False , if set to True shrines will get increasing chances on fail. Vanilla :false");
            RNGWeight = FailWeight;
            On.RoR2.ShrineChanceBehavior.Awake += (orig, self) =>
            {
                orig(self);
                self.maxPurchaseCount = MaxPurchases;
                HarmonyLib.AccessTools.Field(HarmonyLib.AccessTools.TypeByName("RoR2.ShrineChanceBehavior"), "costMultiplierPerPurchase").SetValue(self, CostMult);
            };


            On.RoR2.ShrineChanceBehavior.AddShrineStack += (orig, self, interactor) =>
            {
                bool chance = EnableIncreaseChance.Value;
                if (chance == true)
                {
                    var succ = self.GetFieldValue<int>("successfulPurchaseCount");
                    if (succ == success)
                    {
                        FailWeight = (FailWeight * Failmult);
                        ++fail;
                        sendChatMessage("Fail " + fail + " weight " + FailWeight);
                        if (fail >= GuaranteedGreen && fail < GuaranteedRed)
                        {

                            sendChatMessage(fail + " fails in a row , guaranteed green " + FailWeight);
                            self.equipmentWeight = 0f;
                            self.tier1Weight = 0f;
                            self.tier2Weight = 0.8f;
                            self.tier3Weight = 0.2f;
                        }
                        else if (fail >= GuaranteedRed)
                        {
                            sendChatMessage(fail + " fails in a row , guaranteed red " + FailWeight);
                            FailWeight = 1.01f;
                            self.equipmentWeight = 0f;
                            self.tier1Weight = 0f;
                            self.tier2Weight = 0f;
                            self.tier3Weight = 1f;
                        }
                    }
                    else if (succ != success)
                    {
                        ++success;
                        FailWeight = RNGWeight;
                        fail = 0;
                        sendChatMessage("success " + "counter = " + success + "number of successes" + succ + "resetting fail weight" + FailWeight);

                    }
                }
                bool CD = DisableCooldown.Value;
                if (CD == true)
                {
                    orig(self, interactor);
                    self.SetFieldValue("refreshTimer", 0f);
                }
                if (CD == false)
                {
                    orig(self, interactor);
                    self.SetFieldValue("refreshTimer", 2f);
                }
            };
        }
        private void sendChatMessage(string message)
        {
            Chat.SendBroadcastChat(new Chat.SimpleChatMessage
            {
                baseToken = message
            });
        }
    }
}
