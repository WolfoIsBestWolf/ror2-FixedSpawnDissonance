## Changelog:
```
v3.4.3
Dissonance:
-Now affects Halcyon Shrines:
-->Just 1 random mob, limited cost to spawn at least 5 monsters per shrine.
-Void Locust now has a chance to spawn Voidtouched creatures.
 
Fixed Dissonance Beetle Queen & Beetle Guard not spawning. (Mod issue)

Devotion:
-Added AlliesAvoidVoidImplosions as dependency so they can avoid Void Implosions.
-Devoted Lemurians now expell Void Infestors at low health.
--Leaves them as a big threat
--They may juggle between lemurians
--But no longer an instant kill to something that is sometimes impossible to prevent.
 
 

v3.4.2
Fixed Worm Elite config being applied inverted.

v3.4.1
Fixed latest patch breaking multiplayer with non mod owners due to Command choice of Elite Aspect.

v3.4.0 - Fixed for DLC2.4
__Soul__
Removed Arch Soul.
Replaced with Lunar Soul wisp that only uses secondary.
-Spawns from very healthy enemies instead of just bosses.
Soul Wisp HP now scales based on victims health.
- Results in less hp especially in early game.
Void team also spawn souls now.
Changed way I block Soul Wisps to so it cant fail by mod breaking from updates.
________
Elite Equipment turn into a different Enigma Fragment.
Added Enigma Fragments and Soul Wisps to log by default. (If content is enabled)
Spite bombs will now hurt void team too.
Spite bombs will spawn from void team too.
Moved various fixes to fix mod.
Added config for always random Umbra


v3.3.0
Mod now auto-chooses to add content based on if you have any other content mod.
Fixed vanilla issue where Devotion Inventories sometimes wouldn't get destroyed.
Fixed BoostDamage & BoostHealth being visibile in Devoted Lemurian inventories
Fixed disconnected players being shown when Devotion Inventory is active.

v3.2.7
Made IL hooks more robust to avoid issues with other mods that affect Evolution/Swarms/Overlays

v3.2.6
Fixed mod breaking Vengence.
Vengence now scales with Player Level instead of Monster level.


v3.2.5
Enabling content that would desync with players now properly tags lobby as modded.
Removed Devotion adding DLC2 Elites as that was changed & fixed
Fixed vanilla issue where Evolution could sometiles fail if a Lemurian died weirdly.
Fixed vanilla issue with Devotion where sometimes eggs wouldnt work properly if multiple runs were played.


v3.2.4
Fixed Artifact of Honor not disabling properly.
Made Devoted Lemurians immune to fall damage, lava damage and fog damage.

v3.2.3
Fixed for False Son patch and Removed some redundant fixes as they were fixed officially.

v3.2.2
Fixed a bug with Sacrifice and Forgive me Please. (Missing body check)

v3.2.1
Removed Gilded from Honor spawns Minions as elites.
Removed stat boosts from Honor Minions.
If you have Moffein-Evolution-Config, "Evolution more items" from this mod no longer loads as it conflicts.
(Why would you use both at the same time is beyond me, since they do the same thing)


v3.2.0
Devotion : 
-You can now give Void Items to Lemurians
-UnBlacklists Breaching Fin and Luminious Shot, and makes them function.
(They are blacklisted because enemies cant proc them normally, so I made it so that Lemurian/Elders can)
-Devotion inventory should be more consistent. (Fix for base game)
-(Lemurians always had 1 less stacks of some items after evolving)
-Devoted Lemurians teleport far more often to get stuck less.
-Fixed Devoted Inventory not showing up if your oldest minion wasn't a lemurian.
-Fixes Twisteds not being able to tether to players. (Might be a much deeper issue)
-Fixed some items like Opal randomly breaking on Devoted lemurians.


Honor Changes:
-Tier2 also have half the stats and cost, not just Tier 1.
-Works better with modded elites.
(Halves HP and Damage of all elite types, so "Honor" elite versions are no longer needed)

Rebirth now accepts Void and Lunar items.
Rebirth will store a random item from your previous run with Rebirth, if no item is specifically stored. 
(Or infinite rebirth mods are installed)

Added Risk of Options config.

Removed items from Soul Wisps generally lowered health.
AIBlacklisted Luminious Shot as it does not function for enemies.
Artifact of Glass makes you look glass
Fixed Evolution + Eulogy not using blacklist.
Fixed Vengence + Swarms not working. (Vanilla bug ever since SotV)
Fixed Artifact of Soul changes being disabled by default.
Fixed Command Void Particles not working on Clients.



v3.1.1
Fixed an issue with removing Fire from Elite Worms, leading to REX root not disappearing
Bundled few assets mod does have.
Mod now actually tagged as "NotRequiredByAll" to support Content disabling config

v3.1.0
Added Ambient Spite as a dependency. Just makes more sense for Spite to work this way.
Ported Nuxlars Devotion Lemurian Inventory mod. (with Permission)
False Son will be Gilded when Honor is enabled.

Fixed Devotion eggs not working properly if Devotion is diabled (Vanilla bug)
Fixed Lemurians not properly evolving after Devotion gets disabled (Vanilla bug)
(Still broken if it never got enabled in the first place)
Fixed Family Lines being broadcast even with Dissonance on. (Vanilla, SotS bug)

v3.0.3 - Fixed an issue preventing Dissonance from working.
v3.0.2
Added the 2 new elite types to the Devotion elite pool.

v3.0.1
Added the 3 new enemies to Dissonance. (Gearbox missed them)
-Fixed various parts of the mod breaking if a character didn't have a proper vengance enemy.
--(Such as Greater Souls infinitely spawning, Honor minions breaking entirely)
-Removed Fire Trail from Blazing Elite Worms as it telefrags you.
-Added new useless for enemy items to AIBlacklist.

v3.0.0 - Simple Sots Fix.
Removed Lunar Equipment from Enigma

v2.5.4 - Fix for Devotion Update.
Now the mod loads again.

v2.5.3 - Fixed storepage image being gone due to Discord.
-Modded Umbras can now use Equipments

v2.5.2
-Fixed an error with honor if command module is disabled
-Fixed an error if elite aspects couldn't drop.
-Nerfed late game Spite damage

v2.5.1  
-Fixed various configs from not working.  
-Set default for "Honor : Start with Elite Equip" to false.  
  
v2.5.0 - Cleanup & Renaming  
-Enigma :  
--Fragments now drop driectly instead of needing to turn Equipment into Fragments  
--Fragments cooldown reduction increased from 8% to 12%
-Sacricfice : less greens from normal but only green+ from bosses  
-Evolution : Void Team gets Void items  
-Vengance : General Balance Check  
--Half Healing  
--Removed Bears  
--Removed Potions  
--Less Health but Adaptive Armor  
---They should always be a little tanky but so many items made them stupid tanky  
-Soul :  
--Soul Wisps slow  
--Soul Wisps drop money  
--Soul Arch Wisp from bosses  
-Spite :  
--General numbers cleanup  
--Bombs deal team damage  
-Removed Frailty Changes 



OLD Changelog:
v2.0.2 - Fix for version 1.2.3.0
v2.0.1
* Enemies can get Shipping Request again but rarity only increases based on amount of items the players have.
* Command won't make a cube if there's only 1 option.
* Bug fixes.

v2.0.0
* Updated for Survivors of the Void
* Sacrifice drops Void Items from Void enemies
* Void Team now works properly with Evolution

v1.4.5
* Moved Minions inherit elite equipment to LittleGameplayTweaks

v1.4.4
* Reverted some Soul and Enigma changes
* Soul Wisps can be hurt and killed again as opposed to simply needing to wait
* Enigma Box was removed, Enigma will just be random equipment again. 
* Moved Scavs as Bosses to LittleGameplayTweaks

v1.4.3
* Solved an issue with Umbra leveling being too high.

v1.4.2 
* Some mild limiters on Umbra item inheritence (such as only 1 Teddy Bear,Little Disciple).
* Umbras will be able to drop items from Artifact of Evolution again.
* Radnom Umbras for Vengence + Metamorphosis 

v1.4.1 Blacklist adjustments + Fixes\
v1.4.0
* Artifact of Frailty tweaks
* Artifact of Enigma upgrade
* Artifact of Soul tweaks
* Technical stuff

v1.3.4
* Umbras now only drop Green/Red/Yellow items
* Umbras try to drop an item from your inventory instead of their own

v1.3.3 - PreSet Item Blacklist for Umbras & Equipment Blacklist for Scavs\
v1.3.2 
* Adds Clay Men, Arch Wisp, Ancient Wisps to the Dissonance spawn pool when respective mod is present.

v1.3.1 - Config mistake fixed\
v1.3.0
* Umbras will always sprint when chasing their target.


v1.2.9 - Config to make Hermit Crabs&Solus Probes rarer for Disso\
v1.2.8 - Defaulted Yellow Scav to 1 Item\
v1.2.7 - Reorganized Config
* Command allows choice of Aspect when Aspects drop

v1.2.6
* Config to make Umbras use Equipment

v1.2.5
* Hopefully Fixed bug making only Umbras of 1 player scale

v1.2.4
* Config to make Umbras scale with ambient level.

v1.2.3
* Yellow drops for Scavenger, Overloading Elder/Lemurian
* Config to have Scavenger TP Bosses spawn with 2 Yellow items

v1.2.2
* Config to only apply More Evolution Items after Looping

v1.2.1
* Config for Turrets not inherting Tonic Affliction

v1.2.0
* Config for Artifact of Kin to try to not have 2 of the same enemy in a row.

v1.1.9
* Config to start Artifact of Honor runs with a Tier 1 Elite Equipment
* Config to make Minions & Friendly Aurelionite a random Elite when using Artifact of Honor
* Config to make Minions inherit Elite Equipment

v1.1.8
* Config for Honor to make Mithrix & Twisted Scavengers always Perfect instead of random Tier 1 Elites.

v1.1.7
* Config for Artifact of Evolution item amounts (per tier), disabled by default.

v1.1.6
* Option to allow all enemies to be Perfected elites on Commencement for Dissonance

v1.1.5 - Minor Fix for HonorOnly Worms\
v1.1.4 - Minor Fix for Debug\
v1.1.3 - Minor Change for Enemy Item Blacklist\
v1.1.2 - Minor Fix + Removed Cleanse Pool stuff (Moved to SlightlyFasterInteractables)\
v1.1.1 - Minor Fix

v1.1.0
* For Technical reason the mod now deletes and then re-adds the list entirely.
* Changed vanilla category weight that made Minibosses spawn too often.
* Shifted around enemies in categories to make them all equally likely to be chosen as possible enemies for the current stage.
* Scavengers no longer appear on every stage with Dissonance.
* Added a Titan look randomizer on start up.
* Worm Bosses can now be Elites with Artifact of Honor.
* Most enemies can drop fitting Boss Items if they are encountered as a Horde of Many.
* Config files for previous 2 modules


v1.0.4
* Hopefully fixes a bug where Cleansing Pool drop tables could desync leading to stages loading in empty.
* Moved Lunar Wisp to Champions Category due to their high spawn cost.

v1.0.3 - Minor Fix

v1.0.2
* Hermit Crabs now spawn far away like how they do in the regular spawns.
* Sprint items are now AIBlacklisted as they should be.

v1.0.1
* Attempts to fix a bug where Cleansing Pools occasionally would give only Irradiant Pearls.
* Cleansing Pools display their cost (1 Lunar) again.

v1.0.0 - Release.