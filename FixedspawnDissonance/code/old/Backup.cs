
/*
public static void SoulWispGreater(On.RoR2.GlobalEventManager.orig_OnCharacterDeath orig, global::RoR2.GlobalEventManager self, global::RoR2.DamageReport damageReport)
{
    if (NetworkServer.active && damageReport.victimBody && damageReport.victimMaster)
    {
        SoulInventoryToCopy = damageReport.victimMaster.inventory;

        if (damageReport.victimMaster.masterIndex == SoulGreaterWispIndex || damageReport.victimMaster.masterIndex == SoulArchWispIndex || damageReport.victimMaster.masterIndex == IndexAffixHealingCore)
        {
            SoulGreaterDecider = 5;
            SoulMoney = damageReport.victimMaster.money / 3;
        }
        else if (damageReport.victimBody.isChampion)
        {
            SoulGreaterDecider = 2;
            SoulMoney = damageReport.victimMaster.money;
        }
        else if (damageReport.victimBody.baseMaxHealth > 520)
        {
            SoulGreaterDecider = 1;
            SoulMoney = damageReport.victimMaster.money;
        }
    }
    orig(self, damageReport);
}


public static RoR2.CharacterMaster SoulSpawnGreaterUniversalOld(On.RoR2.MasterSummon.orig_Perform orig, global::RoR2.MasterSummon self)
{
    //if (self.masterPrefab == GlobalEventManager.CommonAssets.wispSoulMasterPrefabMasterComponent.gameObject)
    //Idk how this comparison works so I have no idea how optimal this is
    if (self.masterPrefab == SoulLesserWispMaster)
    {
        Debug.Log(self.summonerBodyObject);



        if (SoulGreaterDecider == 1)
        {
            self.masterPrefab = SoulGreaterWispMaster;
            SoulGreaterDecider = 0;
        }
        else if (SoulGreaterDecider == 2)
        {
            self.masterPrefab = SoulArchWispMaster;
            SoulGreaterDecider = 0;
        }
        else if (SoulGreaterDecider == 5)
        {
            SoulGreaterDecider = 0;
            return null;
        }
        self.inventoryToCopy = SoulInventoryToCopy;
        CharacterMaster tempmaster = orig(self);
        tempmaster.GiveMoney(SoulMoney * 50);
        Debug.Log(tempmaster.money);
        tempmaster.money = SoulMoney * 100;
        Debug.Log(tempmaster.money);
        return tempmaster;
    }
    return orig(self);
}
*/
/*
EnigmaUpgrade = ConfigFile.Bind(
    "1g - Enigma",
    "Upgrade Enigma Artifact",
    true,
    "Make Artifact of Enigma work more like how it did in RoR1. You will get a Enigma Box equipment that activates a random equipment and all equipment will be replaced by Equipment Cooldown Reduction"
);


EnigmaChatMessage = ConfigFile.Bind(
             "1g - Enigma",
                           "Enable Enigma Chat Message",
                           true,
                           "Your Enigma invokes EquipmentName"
                       );
EnigmaCooldown = ConfigFile.Bind(
               "1g - Enigma",
               "Enigma Equipment Cooldown",
               60,
               "Vanilla is 60"
           );
*/

/*private void Run_Start(On.RoR2.Run.orig_Start orig, Run self)
{
    orig(self);
    if (runstartonetimeonly == false)
    {
        runstartonetimeonly = true;
        //Used for Kin Enemies Boss Items Overloading Lemurians Charged Perfoerator
        KinBossDropsForEnemies.ol = Language.GetString("ELITE_MODIFIER_LIGHTNING");
        KinBossDropsForEnemies.ol = KinBossDropsForEnemies.ol.Replace("{0}", "");
    }
}*/
/*
if (SoulImmortal.Value == true)
{

   SoulWispBody.baseMoveSpeed *= 1.75f;
   SoulWispBody.baseAcceleration *= 1.75f;
   SoulWispBody.baseMaxHealth = 40;
   SoulWispBody.levelMaxHealth = 12;
   SoulWispBody.baseRegen = -2.75f;
   SoulWispBody.levelRegen = -0.55f;
   SoulWispBody.baseDamage *= 0.6f;
   SoulWispBody.levelDamage *= 0.7f;
   SoulWispBody.baseAttackSpeed *= 1.25f;

   GreaterSoulWispBody.baseMoveSpeed *= 1.75f;
   GreaterSoulWispBody.baseAcceleration *= 1.75f;
   GreaterSoulWispBody.baseRegen = -27.5f;
   GreaterSoulWispBody.levelRegen = -5.5f;
   GreaterSoulWispBody.baseDamage *= 0.5f;
   GreaterSoulWispBody.levelDamage *= 0.6f;
   GreaterSoulWispBody.baseAttackSpeed *= 1.33f;

   SoulLesserWispBody.transform.GetChild(0).GetChild(0).GetChild(1).GetChild(0).GetChild(1).gameObject.SetActive(false);
   SoulGreaterWispBody.transform.GetChild(0).GetChild(0).GetChild(0).GetChild(2).gameObject.SetActive(false);

   GivePickupsOnStart.ItemInfo AlienHead = new GivePickupsOnStart.ItemInfo { itemString = ("AlienHead"), count = 1, };
   GivePickupsOnStart.ItemInfo DeathMark = new GivePickupsOnStart.ItemInfo { itemString = ("DeathMark"), count = 1, };
   GivePickupsOnStart.ItemInfo StunGrenade = new GivePickupsOnStart.ItemInfo { itemString = ("StunChanceOnHit"), count = 1, };
   GivePickupsOnStart.ItemInfo[] AlienHeadInfos = new GivePickupsOnStart.ItemInfo[0];

   AlienHeadInfos = AlienHeadInfos.Add(AlienHead, DeathMark, StunGrenade);


   SoulLesserWispMaster.AddComponent<GivePickupsOnStart>().itemInfos = AlienHeadInfos;
   SoulGreaterWispMaster.AddComponent<GivePickupsOnStart>().itemInfos = AlienHeadInfos;


//SoulLesserWispGiver = SoulLesserWispMaster.GetComponent<GivePickupsOnStart>();
//SoulGreaterWispGiver = SoulGreaterWispMaster.GetComponent<GivePickupsOnStart>();
}
*/
/*
public static void EnemyItemChanges()
{

    
On.RoR2.HealthComponent.Heal += (orig, self, amount, procChainMask, nonRegen) =>
{
if (self.body && self.body.inventory && nonRegen && self.body.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) > 0)
{
amount /= 2;
}
return orig(self, amount, procChainMask, nonRegen);
};

    


    On.RoR2.DotController.AddDot += (orig, self, attackerObject, duration, dotIndex, damageMultiplier) =>
    {
        //Debug.LogWarning(damageMultiplier);


        if (dotIndex == DotController.DotIndex.Helfire && attackerObject.GetComponent<CharacterBody>().teamComponent.teamIndex == TeamIndex.Monster && self.victimObject.GetComponent<CharacterBody>().teamComponent.teamIndex == TeamIndex.Player)
        {
            orig(self, attackerObject, duration, dotIndex, damageMultiplier / 72);
            return;
        }
        orig(self, attackerObject, duration, dotIndex, damageMultiplier);

    };


    On.RoR2.DotController.OnDotStackAddedServer += (orig, self, dotstack) =>
    {
        //Debug.LogWarning("OnDotStackAddedServer");


        if (dotstack.GetFieldValue<DotController.DotIndex>("dotIndex") == DotController.DotIndex.Helfire)
        {
            if (dotstack.GetFieldValue<TeamIndex>("attackerTeam") == TeamIndex.Monster)
            {
                dotstack.SetFieldValue<DamageType>("damageType", DamageType.NonLethal | DamageType.Silent);
            };
        }
        orig(self, dotstack);
    };

    On.RoR2.Orbs.DevilOrb.OnArrival += (orig, self) =>
    {
        if (self.teamIndex == TeamIndex.Monster)
        {
            self.procCoefficient = 0;

            if (self.effectType == RoR2.Orbs.DevilOrb.EffectType.Skull && self.attacker)
            {
                var tempbod = self.attacker.GetComponent<CharacterBody>();
                if (tempbod)
                {
                    float basehpforcalc = tempbod.baseMaxHealth;
                    float level = tempbod.level;
                    int boosthp = tempbod.inventory.GetItemCount(RoR2Content.Items.BoostHp);
                    if (basehpforcalc > 3200) { basehpforcalc = 3200; };
                    if (boosthp > 170) { boosthp = 170; };
                    //basehpforcalc = (basehpforcalc - basehpforcalc*(basehpforcalc/3800/2))*0.25f + boosthp;

                    basehpforcalc = basehpforcalc = (basehpforcalc - basehpforcalc * (basehpforcalc / 3000 / 2)) * 0.25f + boosthp;

                    if (level < 100)
                    {
                        basehpforcalc *= (0.5f + (level + 1) / 200);
                    }


                    //Debug.LogWarning(RoR2.Util.GetBestBodyName(self.attacker) + " : NkuhanaOpinion prev Damage " + self.damageValue + "   NkuhanaOpinion recalculated " + basehpforcalc);

                    self.damageValue = basehpforcalc;
                    //self.damageValue = basehpforcalc = (basehpforcalc - basehpforcalc * (basehpforcalc / 3800 / 2)) * 0.25f + boosthp;


                }
            }

            //self.attacker
            //Square root of the damage?
        }
        orig(self);
    };

    On.RoR2.Orbs.LightningOrb.OnArrival += (orig, self) =>
    {
        if (self.teamIndex == TeamIndex.Monster)
        {
            if (self.lightningType == RoR2.Orbs.LightningOrb.LightningType.Tesla && self.attacker)
            {


                var tempbod = self.attacker.GetComponent<CharacterBody>();
                if (tempbod)
                {

                    if (tempbod.inventory.GetItemCount(RoR2Content.Items.InvadingDoppelganger) == 0)
                    {
                        float damageforcalc = tempbod.damage;
                        //float currentlevel = tempbod.level;
                        float boostdamage = tempbod.inventory.GetItemCount(RoR2Content.Items.BoostDamage) / 10 + 1;
                        if (tempbod.baseDamage > 20)
                        {
                            //damageforcalc /= (damageforcalc / 20);
                            damageforcalc = damageforcalc / tempbod.baseDamage * 20;
                        }
                        else if (tempbod.baseDamage > 4)
                        {
                            self.procCoefficient = 0.6f;
                            damageforcalc /= 10;
                        }
                        if (boostdamage > 1)
                        {
                            damageforcalc = (damageforcalc / boostdamage) * (1 + (boostdamage - 1) / 4);
                        }



                        //Debug.LogWarning(RoR2.Util.GetBestBodyName(self.attacker) + " : Tesla prev Damage " + self.damageValue*2 + "   Tesla recalculated " + damageforcalc);

                        self.damageValue = damageforcalc;
                    }
                }
            }

            //self.attacker
            //Square root of the damage?
        }
        orig(self);
    };
    
}
*/

/*
//Don't really remember what this was used for
internal static void AffixesIn()
{
    RoR2.LegacyResourcesAPI.Load<EquipmentDef>("equipmentdefs/AffixLunar").isLunar = false;
    List<EquipmentIndex> FullEquipmentList = EquipmentCatalog.equipmentList;
    int[] invoutput = new int[EquipmentCatalog.equipmentCount];

    for (var i = 0; i < invoutput.Length; i++)
    {

        string tempname = EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i).name;


        if (tempname.Contains("Affix") || tempname.Contains("Void"))
        {
            if (tempname.Contains("Gold") || tempname.Contains("SecretSpeed") || tempname.Contains("Echo") || tempname.Contains("Yellow"))
            {
            }
            else
            {
                EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i).canDrop = true;
                EquipmentCatalog.GetEquipmentDef((EquipmentIndex)i).isBoss = true;
            }
        }
    }
}
*/


/*
DirectorCard DSBeetleQueen = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBeetleQueen"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSVagrant = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVagrant"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSImpBoss = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscImpBoss"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSRoboBallBoss = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscRoboBallBoss"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSGrovetender = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscGravekeeper"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSMagmaWorm = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMagmaWorm"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Far
};
DirectorCard DSElectricWorm = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscElectricWorm"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Far
};


DirectorCard DSGolem = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscGolem"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSGreaterWisp = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscGreaterWisp"),

    preventOverhead = true,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSBell = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBell"),

    preventOverhead = true,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSElderLemurian = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLemurianBruiser"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSBison = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBison"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSClayTemp = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscClayBruiser"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSVoidReaver = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscNullifier"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSParent = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscParent"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};


DirectorCard DSLemurian = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLemurian"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSWisp = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLesserWisp"),

    preventOverhead = true,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSBeetle = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBeetle"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSJellyfish = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscJellyfish"),

    preventOverhead = true,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSImp = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscImp"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSVulture = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscVulture"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSRoboBallMini = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscRoboBallMini"),

    preventOverhead = true,
    selectionWeight = 1,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSMushroom = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscMiniMushroom"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSBeetleGuard = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscBeetleGuard"),

    preventOverhead = false,
    selectionWeight = 3,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};

DirectorCard DSClayBoss = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscClayBoss"),
    selectionWeight = 3,

    preventOverhead = false,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSGrandparent = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/titan/cscGrandparent"),
    selectionWeight = 3,

    preventOverhead = false,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSLunarExploder = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLunarExploder"),
    selectionWeight = 3,

    preventOverhead = false,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSLunarGolem = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLunarGolem"),
    selectionWeight = 3,

    preventOverhead = false,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
DirectorCard DSLunarWisp = new DirectorCard
{
    spawnCard = RoR2.LegacyResourcesAPI.Load<CharacterSpawnCard>("SpawnCards/CharacterSpawnCards/cscLunarWisp"),
    selectionWeight = 3,

    preventOverhead = true,
    minimumStageCompletions = 0,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};


DirectorCard DSVoidInfestor = new DirectorCard
{
    spawnCard = Addressables.LoadAssetAsync<CharacterSpawnCard>(key: "RoR2/DLC1/EliteVoid/cscVoidInfestor.asset").WaitForCompletion(),
    selectionWeight = 1,
    preventOverhead = true,
    minimumStageCompletions = 5,
    spawnDistance = DirectorCore.MonsterSpawnDistance.Standard
};
*/

/*
          RoR2.CharacterAI.AISkillDriver DiabloStrikePrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver DiabloStrikeUtility = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver ActivateBeaconsSpecial = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver HealingBeaconPrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver HealingBeaconSecondary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver ShockBeaconPrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver ShockBeaconSecondary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver ResupplyBeaconPrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver ResupplyBeaconSecondary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver HackingBeaconPrimary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();
          RoR2.CharacterAI.AISkillDriver HackingBeaconSecondary = CaptainMaster.AddComponent<RoR2.CharacterAI.AISkillDriver>();

          DiabloStrikePrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallAirstrikeAlt");
          DiabloStrikeUtility.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/PrepAirstrikeAlt");
          ActivateBeaconsSpecial.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/PrepSupplyDrop");
          HealingBeaconPrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallSupplyDropHealing");
          HealingBeaconSecondary.requiredSkill = HealingBeaconPrimary.requiredSkill;
          ShockBeaconPrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallSupplyDropShocking");
          ShockBeaconSecondary.requiredSkill = ShockBeaconPrimary.requiredSkill;
          ResupplyBeaconPrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallSupplyDropEquipmentRestock");
          ResupplyBeaconSecondary.requiredSkill = ResupplyBeaconPrimary.requiredSkill;
          HackingBeaconPrimary.requiredSkill = RoR2.LegacyResourcesAPI.Load<RoR2.Skills.SkillDef>("skilldefs/captainbody/CallSupplyDropHacking");
          HackingBeaconSecondary.requiredSkill = HackingBeaconPrimary.requiredSkill;

          DiabloStrikePrimary.skillSlot = SkillSlot.Primary;
          DiabloStrikeUtility.skillSlot = SkillSlot.Utility;
          ActivateBeaconsSpecial.skillSlot = SkillSlot.Special;
          HealingBeaconPrimary.skillSlot = SkillSlot.Primary;
          HealingBeaconSecondary.skillSlot = SkillSlot.Secondary;
          ShockBeaconPrimary.skillSlot = SkillSlot.Primary;
          ShockBeaconSecondary.skillSlot = SkillSlot.Secondary;
          ResupplyBeaconPrimary.skillSlot = SkillSlot.Primary;
          ResupplyBeaconSecondary.skillSlot = SkillSlot.Secondary;
          HackingBeaconPrimary.skillSlot = SkillSlot.Primary;
          HackingBeaconSecondary.skillSlot = SkillSlot.Secondary;

          DiabloStrikePrimary.customName = "PrepDiablo";
          DiabloStrikeUtility.customName = "MarkDiablo";
          ActivateBeaconsSpecial.customName = "PrepSupplyDrop";
          HealingBeaconPrimary.customName = "BeaconHealingPrimary";
          HealingBeaconSecondary.customName = "BeaconHealingSecondary";
          ShockBeaconPrimary.customName = "BeaconShockPrimary";
          ShockBeaconSecondary.customName = "BeaconShockSecondary";
          ResupplyBeaconPrimary.customName = "BeaconResupplyPrimary";
          ResupplyBeaconSecondary.customName = "BeaconResupplySecondary";
          HackingBeaconPrimary.customName = "BeaconHackingPrimary";
          HackingBeaconSecondary.customName = "BeaconHackingSecondary";

          //ActivateBeaconsSpecial.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;
          HealingBeaconPrimary.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;
          HealingBeaconSecondary.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;
          ResupplyBeaconPrimary.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;
          ResupplyBeaconSecondary.moveTargetType = RoR2.CharacterAI.AISkillDriver.TargetType.NearestFriendlyInSkillRange;


          HealingBeaconPrimary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
          HealingBeaconSecondary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
          ShockBeaconPrimary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
          ShockBeaconSecondary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
          ResupplyBeaconPrimary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
          ResupplyBeaconSecondary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
          HackingBeaconPrimary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;
          HackingBeaconSecondary.buttonPressType = RoR2.CharacterAI.AISkillDriver.ButtonPressType.TapContinuous;

          DiabloStrikePrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
          HealingBeaconPrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
          HealingBeaconSecondary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
          ShockBeaconPrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
          ShockBeaconSecondary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
          ResupplyBeaconPrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
          ResupplyBeaconSecondary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
          HackingBeaconPrimary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
          HackingBeaconSecondary.aimType = RoR2.CharacterAI.AISkillDriver.AimType.AtCurrentEnemy;
          */
/*
DiabloStrikePrimary.activationRequiresTargetLoS  = true;
HealingBeaconPrimary.activationRequiresTargetLoS = true;
HealingBeaconSecondary.activationRequiresTargetLoS = true;
ShockBeaconPrimary.activationRequiresTargetLoS = true;
ShockBeaconSecondary.activationRequiresTargetLoS = true;
ResupplyBeaconPrimary.activationRequiresTargetLoS = true;
ResupplyBeaconSecondary.activationRequiresTargetLoS = true;
HackingBeaconPrimary.activationRequiresTargetLoS = true;
HackingBeaconSecondary.activationRequiresTargetLoS = true;
*/
/*
DiabloStrikePrimary.driverUpdateTimerOverride = 0.2f;
HealingBeaconPrimary.driverUpdateTimerOverride = 0.2f;
HealingBeaconSecondary.driverUpdateTimerOverride = 0.2f;
ShockBeaconPrimary.driverUpdateTimerOverride = 0.2f;
ShockBeaconSecondary.driverUpdateTimerOverride = 0.2f;
ResupplyBeaconPrimary.driverUpdateTimerOverride = 0.2f;
ResupplyBeaconSecondary.driverUpdateTimerOverride = 0.2f;
HackingBeaconPrimary.driverUpdateTimerOverride = 0.2f;
HackingBeaconSecondary.driverUpdateTimerOverride = 0.2f;
*/
//DiabloStrikeUtility.requireSkillReady = true;
/*
ActivateBeaconsSpecial.requireSkillReady = true;
DiabloStrikePrimary.requireSkillReady = true;
HealingBeaconPrimary.requireSkillReady = true;
HealingBeaconSecondary.requireSkillReady = true;
ShockBeaconPrimary.requireSkillReady = true;
ShockBeaconSecondary.requireSkillReady = true;
ResupplyBeaconPrimary.requireSkillReady = true;
ResupplyBeaconSecondary.requireSkillReady = true;
HackingBeaconPrimary.requireSkillReady = true;
HackingBeaconSecondary.requireSkillReady = true;
*/



/*
//Idk what this is for I guess something like no enemies getting FreeChest item and that being overwritten here//
private PickupIndex FreeChestDropTable_GenerateDropPreReplacement(On.RoR2.FreeChestDropTable.orig_GenerateDropPreReplacement orig, FreeChestDropTable self, Xoroshiro128Plus rng)
{
    int num = RoR2.Util.GetItemCountForTeam(TeamIndex.Player, DLC1Content.Items.FreeChest.itemIndex, false, true);
    self.selector.Clear();
    self.Add(Run.instance.availableTier1DropList, self.tier1Weight);
    self.Add(Run.instance.availableTier2DropList, self.tier2Weight * (float)num);
    self.Add(Run.instance.availableTier3DropList, self.tier3Weight * Mathf.Pow((float)num, 2f));
    return PickupDropTable.GenerateDropFromWeightedSelection(rng, self.selector);
}
*/