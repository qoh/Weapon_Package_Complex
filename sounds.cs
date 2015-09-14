datablock AudioDescription(Audio3dUniform48)
{
	type = 2;
	is3D = true;
	referenceDistance = 48;
	maxDistance = 96;
	// referenceDistance = 0;
	// maxDistance = 999999;
};

datablock AudioDescription(Audio3dUniform64)
{
	type = 2;
	is3D = true;
	referenceDistance = 64;
	maxDistance = 128;
	// referenceDistance = 1;
	// maxDistance = 1000;
};

new ScriptObject(ComplexRicSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/rics/ric3.wav";
	file[main, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/rics/ric4.wav";
	file[main, 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/rics/ric5.wav";
};

new ScriptObject(ComplexNearMissSFX)
{
	class = "SFXEffect";

	file[ltor, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/nearmiss/bulletltor01.wav";
	file[ltor, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/nearmiss/bulletltor06.wav";
	file[ltor, 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/nearmiss/bulletltor07.wav";
	file[ltor, 4] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/nearmiss/bulletltor08.wav";
	file[ltor, 5] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/nearmiss/bulletltor09.wav";
	file[ltor, 6] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/nearmiss/bulletltor10.wav";
	file[ltor, 7] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/nearmiss/bulletltor11.wav";
	file[ltor, 8] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/nearmiss/bulletltor13.wav";
	file[ltor, 9] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/nearmiss/bulletltor14.wav";
};

new ScriptObject(ComplexHeadshotSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/headshot1.wav";
	file[main, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/headshot2.wav";
	use2D[main] = "source";
};

new ScriptObject(ComplexDefaultImpactBulletSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/impact/default_impact_bullet1.wav";
	file[main, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/impact/default_impact_bullet2.wav";
	file[main, 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/impact/default_impact_bullet3.wav";
	file[main, 4] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/impact/default_impact_bullet4.wav";
};

new ScriptObject(ComplexFleshImpactBulletSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/impact/flesh_impact_bullet1.wav";
	file[main, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/impact/flesh_impact_bullet2.wav";
	file[main, 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/impact/flesh_impact_bullet3.wav";
	file[main, 4] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/impact/flesh_impact_bullet4.wav";
	file[main, 5] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/impact/flesh_impact_bullet5.wav";
};

new ScriptObject(GenericShellSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/shellCasing1.wav";
	file[main, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/shellCasing2.wav";
	file[main, 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/shellCasing3.wav";
};

new ScriptObject(BuckshotShellSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/shellDrop1.wav";
	file[main, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/shellDrop2.wav";
	file[main, 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/shellDrop3.wav";
};

new ScriptObject(WeaponSoftImpactSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/physics/weapon_impact_soft1.wav";
	file[main, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/physics/weapon_impact_soft2.wav";
	file[main, 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/physics/weapon_impact_soft3.wav";
};

new ScriptObject(WeaponHardImpactSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/physics/weapon_impact_hard1.wav";
	file[main, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/physics/weapon_impact_hard2.wav";
	file[main, 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/physics/weapon_impact_hard3.wav";
};

new ScriptObject(GrenadePinSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/ring.wav";
};

datablock AudioProfile(ComplexBoltSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/bolt.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ComplexClipInSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/clipin.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ComplexClipOutSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/clipout.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ComplexClipEmptyPistolSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/clipempty_pistol.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ComplexClipEmptyRifleSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/clipempty_rifle.wav";
	description = AudioClose3d;
	preload = true;
};