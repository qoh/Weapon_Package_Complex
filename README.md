If you want you can skip to the comprehensive feature list and other juicy stuff scroll down a bit.

---------------------------
Have you enjoyed games like Receiver or Fistful of Frags and their gunplay?
Have you yearned for some of those mechanics in Blockland?
Have you hated the simple R-to-Reload systems almost every gun in every videogame in existance has and instead have reloading be a part of the gun's micromanaging process? (Wow.)

Well this add-on is for you!

Weapon_Package_Complex is a weapon pack designed for those servers that need a little more "realistic" edge in their gunplay mechanics. Weapons use Port's revolutionary Timed/Segmented Raycasts system - basically not-instant raycast "projectiles" that have things like gravity, etc. All guns have also been tweaked to emulate their real-life counterparts in terms of performance while also keeping stern balancing mechanics in place - every weapon in the pack can counter another in skilled hands. Damage-wise all weapons are balanced and every weapon with the exception of the shotgun have headshots - usually *3 multiplier on base damage (exception is the M24 Sniper with *4 mult and automatic weapons like Thompson and UZI having different multipliers)

---------------------------
FEATURE LIST (OH BOY!)
* Headshots thanks to Port's Region Damage Support - If I wanted to I could've added armshots, legshots, etc. but I chose not to!
* Timed/Delayed Raycasts - Thanks to this, all weapons have incredibly fast "bullets" based on their real life stats, not to mention gravity! (Gravity only noticeable over long distances)
* Port's ItemProps system - Forget about old as hell ammo system Tier+Tactical and other weapons like to use! All weapon-specific variables are tracked down in their every possible state, this means that your pistol could be fully loaded without a chambered bullet or completely empty with a single bullet in chamber and you could drop your pistol no problem!
* Super cool SFX system (by Port :P) - Supports SHELL COLLISION SOUNDS! WEAPON COLLISION SOUNDS! (relive your CS:GO's) GRENADE COLLISION SOUNDS! DIFFERENT DISTANT GUNSHOT SOUND EFFECTS! (Some weapons unsupported as of yet (the revolvers forexample))
* Even the freakin GRENADES require you pulling the pin out first before throwing them! (Warning: throwing the grenade without pulling the pin out first will make you look like a complete doofus)
* The main selling point of this pack: SUPER COMPLEX HYPER AMAZING RELOADING SYSTEMS THAT ARE ACTUALLY NOT THAT HARD TO FIGURE OUT!
* In-game tutorial system! Type /gunhelp with your gun out and you will be guided through the reloading process! The centerprint will also highlight the steps you need to perform to reload the gun!

These weapons are mostly WWII-styled with an exception of Micro-UZI. There are a total of 9 weapons in the pack including the grenade.
Current weapons:
* Colt 1911 Pistol (.45 bullet type, magazine UI name "A: M1911")
* Colt Walker Revolver (.357 bullet type, no magazine)
* M1 Garand Rifle (.30-06 bullet type, magazine UI name "A: Garand")
* M24 Sniper Rifle (.30-06 bullet type, magazine UI name "A: M24")
* Micro UZI (.45 bullet type, magazine UI name(s) "A: Uzi" and "A: Uzi Extended")
* Remmington 870 Shotgun (buckshot bullet type, no magazine)
* Smith&Wesson Revolver (.357 bullet type, no magazine)
* Thompson Machinegun (.45 bullet type, magazine UI name "A: Thompson")
* High-Explosive Grenade (Right click to pull the pin out, left click to throw, Ctrl+W drop can be used for a "soft throw")

Regarding the magazines, you CANNOT insert magazines into incompatible guns (can't insert pistol magazine into an UZI despite same bullet type, etc.)
Ammo stats:
UIname - Amount
A: M1911 (pistol) - x12 .45 bullets
A: M24 (sniper) - x5 .30-06 bullets
A: Thompson - x50 .45 bullets
A: Garand - x8 .30-06 bullets
A: Uzi - x20 .45 bullets
A: Uzi Extended - x32 .45 bullets
  (Unspent) bullets that will fill emptiest magazines on pickup:
Shell: .45
Shell: .30-06
Bullets that will fill your "ammo pool" for use with magless guns:
Shell: .357
Shell: Buckshot
  Ammo packs to decrease the need of spammed single-bullet items:
AmmoPack: .357 - x12 bullets
AmmoPack: Buckshot - x12 bullets

Note that weapon damage is ACCOUNTED FOR CROUNCHING PLAYERS, this means that the damage will count as headshots for crouched players but the default crouch damage multiplier won't come into play.
Weapon Damage:
* Colt 1911 Pistol - 16HP (36HP Headshot)
* Colt Walker Revolver - 75HP(225HP Headshot)
* M1 Garand Rifle - 30HP (90HP Headshot)
* M24 Sniper Rifle - 90HP (360HP Headshot)
* Micro UZI - 10HP(15HP Headshot)
* Remmington 870 Shotgun - 9HP per pellet
* Smith&Wesson Revolver - 25HP (75HP Headshot)
* Thompson Machinegun - 7.5HP (11HP Headshot)
* High-Explosive Grenade:
	impulseRadius = 16;
	impulseForce = 3000;
	damageRadius = 14;
	radiusDamage = 150;

Credits:
Receiver by Wolfire Games - Initial source of Inspiration
Jack Noir (AKA Crystalwarrior) - Original S&W Revolver, coding, modelling
Brian SMith - Original version of magazine-based weapons starting with m1911 revolver
Port - COMPLETE rewrite of the entire weapon pack, item props system and various other support scripts that made this weapon pack possible. Also a bunch of new gun scripts woo