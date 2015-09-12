addDamageType("M24Rifle",
	'<bitmap:base/client/ui/ci/skull> %1',
	'%2 <bitmap:base/client/ui/ci/skull> %1',
	0.2, 1);
addDamageType("M24RifleHeadshot",
	'<bitmap:base/client/ui/ci/skull><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	'%2 <bitmap:base/client/ui/ci/skull><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	0.2, 1);

// datablock AudioProfile(SniperFire1Sound)
// {
// 	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/m24rifle/fire1.wav";
// 	description = AudioClose3D;
// 	preload = true;
// };
//
// datablock AudioProfile(SniperFire2Sound)
// {
// 	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/m24rifle/fire2.wav";
// 	description = AudioClose3D;
// 	preload = true;
// };
//
// datablock AudioProfile(SniperFire3Sound)
// {
// 	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/m24rifle/fire3.wav";
// 	description = AudioClose3D;
// 	preload = true;
// };
//
// $SniperFireSound0 = SniperFire1Sound;
// $SniperFireSound1 = SniperFire2Sound;
// $SniperFireSound2 = SniperFire3Sound;

new ScriptObject(M24RifleFireSFX)
{
	class = "SFXEffect";

    file["close", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/m24rifle/fire1.wav";
    file["close", 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/m24rifle/fire2.wav";
    file["close", 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/m24rifle/fire3.wav";
    filterDistanceMax["close"] = 48;
    use2D["close"] = "source";

    file["far", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/far/fire_sniper.0.wav";
    file["far", 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/far/fire_sniper.1.wav";
    file["far", 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/far/fire_sniper.2.wav";
    file["far", 4] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/far/fire_sniper.3.wav";
    filterDistanceMin["far"] = 48;
	filterDistanceMax["far"] = 128;
    use2D["far"] = "always";

	file["very_far", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/very_far/fire_sniper.0.wav";
    file["very_far", 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/very_far/fire_sniper.1.wav";
    file["very_far", 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/very_far/fire_sniper.2.wav";
    filterDistanceMin["very_far"] = 128;
	filterDistanceMax["very_far"] = 1024;
    use2D["very_far"] = "always";
};

datablock AudioProfile(SniperReloadSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/m24rifle/reload.wav";
	description = AudioClose3D;
	preload = true;
};

datablock ItemData(M24RifleItem)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington700_sniper.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	canDrop = 1;
	uiName = "M24 Rifle";
	image = M24RifleImage;

	itemPropsClass = "M24RifleProps";
};

function M24RifleProps::onAdd(%this)
{
	%this.chamber = 0;
}

datablock ShapeBaseImageData(M24RifleImage)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington700_sniper.dts";

	item = M24RifleItem;
	armReady = 1;

	speedScale = 0.6;

	stateName[0] = "Activate";
	stateTimeoutValue[0] = 0.3;
	stateTransitionOnTimeout[0] = "Ready";

	stateName[1] = "Ready";
	stateTransitionOnAmmo[1] = "Cycle";

	stateName[2] = "Cycle";
	stateSequence[2] = "Bolt";
	stateScript[2] = "onCycle";
	stateTimeoutValue[2] = 1;
	stateAllowImageChange[2] = false;
	stateWaitForTimeout[2] = true;
	stateTransitionOnNoAmmo[2] = "Ready";
};

// function M24RifleImage::getDebugText(%this, %obj, %slot)
// {
//     %props = %obj.getItemProps();
//     return "\c6chamber=" @ (%props.chamber == 1 ? "ready" : (%props.chamber == -1 ? "spent" : "empty")) @ " mag=" @ (isObject(%props.magazine) ? %props.magazine.count : "none") @ "\n";
// }

function M24RifleImage::onLight(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (isObject(%props.magazine))
	{
		%obj.giveMagazineProps(%props.magazine);
		%props.magazine = "";
	}
	else
	{
		%props.magazine = %obj.takeMagazineProps(%this.item);

		if (!isObject(%props.magazine) && isObject(%obj.client))
			messageClient(%obj.client, '', '\c6You don\'t have any magazines for this weapon.');
	}

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();

	return 1;
}

function M24RifleImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	if (%state && %obj.getImageState(%slot) $= "Ready")
	{
		if (%trigger == 0)
			%obj.mountImage(M24RifleScopeImage, %slot);
		else if (%trigger == 4)
			%obj.setImageAmmo(%slot, true);
	}

	return 0;
}

function M24RifleImage::onCycle(%this, %obj, %slot)
{
	serverPlay3D(SniperReloadSound, %obj.getMuzzlePoint(%slot));

	%props = %obj.getItemProps();

	if (%props.chamber != 0)
	{
		%obj.ejectShell(Bullet45Item, 1.95, %props.chamber == 2);
		%props.chamber = 0;
	}

	if (%props.magazine.count >= 1)
	{
		%props.chamber = 1;
		%props.magazine.count--;
	}
	else if (isObject(%obj.client))
		messageClient(%client, '', "\c6There are no more cartridges left to load. Insert a magazine.");

	%obj.setImageAmmo(%slot, false);

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

datablock ShapeBaseImageData(M24RifleScopeImage)
{
	className = "TimeSliceRayWeapon";
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington700_sniper_scoped_v3.dts";

	fireMuzzleVelocity = cf_muzzlevelocity_ms(790 * 1.5);
	fireVelInheritFactor = 0.75;
	fireGravity = cf_bulletdrop_grams(10);
	fireHitExplosion = GunProjectile;
	fireHitOtherSFX = ComplexDefaultImpactBulletSFX;
	fireRicSFX = ComplexRicSFX;
	fireNearMissSFX = ComplexNearMissSFX;

	item = M24RifleItem;
	armReady = 1;

	speedScale = 0.2;
	eyeOffset = "0 0.175 -0.8";

	stateName[0] = "Activate";
	stateAllowImageChange[0] = false;
	stateTimeoutValue[0] = 0.3;
	stateTransitionOnTimeout[0] = "CheckLoaded";

	stateName[1] = "Deactivate";
	stateScript[1] = "onDeactivateDone";
	// stateAllowImageChange[1] = false;
	// stateTimeoutValue[1] = 0.3;
	// stateTransitionOnTimeout[1] = "DeactivateDone";

	// stateName[2] = "DeactivateDone";
	// stateScript[2] = "onDeactivateDone";

	stateName[3] = "CheckLoaded";
	stateAllowImageChange[3] = false;
	stateTransitionOnLoaded[3] = "Ready";
	stateTransitionOnNotLoaded[3] = "Empty";

	stateName[4] = "Empty";
	stateTransitionOnAmmo[4] = "Deactivate";
	stateTransitionOnTriggerDown[4] = "EmptyFire";

	stateName[5] = "EmptyFire";
	stateScript[5] = "onEmptyFire";
	stateSequence[5] = "Fire";
	stateAllowImageChange[5] = false;
	stateTimeoutValue[5] = 0.25;
	stateWaitForTimeout[5] = true;
	stateTransitionOnTriggerUp[5] = "Empty";

	stateName[6] = "Ready";
	stateTransitionOnAmmo[6] = "Deactivate";
	stateTransitionOnTriggerDown[6] = "Fire";

	stateName[7] = "Fire";
	stateFire[7] = true;
	stateSequence[7] = "Fire";
	stateScript[7] = "onFire";
	stateAllowImageChange[7] = false;
	stateEmitter[7] = advBigBulletFireEmitter;
	stateEmitterTime[7] = 0.05;
	stateEmitterNode[7] = "muzzleNode";
	stateTimeoutValue[7] = 0.25;
	stateWaitForTimeout[7] = true;
	stateTransitionOnTriggerUp[7] = "Empty";
};

// function M24RifleScopeImage::getDebugText(%this, %obj, %slot)
// {
//     return M24RifleImage::getDebugText(%this, %obj, %slot);
// }

function M24RifleScopeImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%obj.setImageLoaded(%slot, %props.chamber == 1);
}

function M24RifleScopeImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	if (%trigger == 4 && %state)
		%obj.setImageAmmo(%slot, 1);

	return 0;
}

function M24RifleScopeImage::onDeactivateDone(%this, %obj, %slot)
{
	%obj.mountImage(M24RifleImage, %slot, false);
}

function M24RifleScopeImage::onEmptyFire(%this, %obj, %slot)
{
	serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function M24RifleScopeImage::onFire(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%props.chamber = 2;

	%obj.playThread(2, "shiftLeft");
	%obj.playThread(3, "shiftRight");

	Parent::onFire(%this, %obj, %slot);
	// serverPlay3D($SniperFireSound[getRandom(2)], %obj.getMuzzlePoint(%slot));
	M24RifleFireSFX.playFrom(%obj.getMuzzlePoint(%slot), %obj);

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();

	%obj.applyComplexKnockback(5);
	%obj.applyComplexScreenshake(2.5);
}

function M24RifleScopeImage::damage(%this, %obj, %col, %position, %normal)
{
	if (%col.getRegion(%position, true) $= "head")
	{
		ComplexHeadshotSFX.playFrom(%position, %col);

		%damage = 360;
		%damageType = $DamageType::M24RifleHeadshot;
	}
	else
	{
		ComplexFleshImpactBulletSFX.playFrom(%position, %col);

		%damage = 90;
		%damageType = $DamageType::M24Rifle;
	}

	if (!$NoCrouchDamageBonus && %col.isCrouched())
		%damage /= 2;

	%col.damage(%obj, %position, %damage, %damageType);
}

function M24RifleImage::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	%props = %obj.getItemProps();

	if (%props.chamber == 1)
		return "Your gun is ready to fire. Tap right click to use the scope and then left click to fire it.";

	if (isObject(%props.magazine))
	{
		if (%props.magazine.count < 1)
			return "The magazine in your gun is empty. Press the light key to eject it.";

		return "Your gun has a magazine inserted, but no bullet is chambered. Left click to operate the bolt-action and chamber the next bullet.";
	}

	%index = %obj.findMagazine(%this.item);

	if (%index == -1)
		return "You have no magazines for your gun. You need to get a magazine first.";

	if (%index == -2)
		return "All your magazines are empty. You need to get a non-empty magazine first.";

	return "Your gun has no magazine inserted. Press the light key to insert whichever magazine you have with the most bullets.";
}

function M24RifleScopeImage::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber == 1)
		return "Your gun is ready to fire. Left click to shoot. Right click to stop using the scope.";

	return "No bullet is chambered in your gun. Stop using the scope first by tapping right click.";
}

function M24RifleImage::getDetailedGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	%kt_lmb = "Primary";
	%kt_rmb = "Jet    ";
	%kt_r   = "Light  ";

	%at_fire     = "Scope          ";
	%at_action   = "Operate Bolt   ";
	%at_magazine = "Insert Magazine";

	%ac_fire     = "\c7";
	%ac_action   = "\c7";
	%ac_magazine = "\c7";

	if (isObject(%props.magazine))
		%at_magazine = "Eject Magazine ";

	if (%props.chamber == 1)
		%ac_fire = "\c6";

	if (%props.chamber == 2 || (%props.chamber == 0 && %props.magazine.count >= 1))
		%ac_action = "\c6";

	if (!isObject(%props.magazine) || %props.magazine.count < 1)
		%ac_magazine = "\c6";

	%text = "<just:right><font:consolas:16>";
	%text = %text @ %ac_fire     @ %at_fire     @ "   " @ %kt_lmb @ " \n";
	%text = %text @ %ac_action   @ %at_action   @ "   " @ %kt_rmb @ " \n";
	%text = %text @ %ac_magazine @ %at_magazine @ "   " @ %kt_r   @ " \n";
	return %text;
}

function M24RifleScopeImage::getDetailedGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	%kt_lmb = "Primary";
	%kt_rmb = "Jet    ";

	%at_fire     = "Dryfire        ";
	%at_action   = "Unscope        ";

	%ac_fire     = "\c7";
	%ac_action   = "\c6";

	if (%props.chamber == 1)
	{
		%at_fire = "Fire           ";
		%ac_fire = "\c6";
	}

	%text = "<just:right><font:consolas:16>";
	%text = %text @ %ac_fire     @ %at_fire     @ "   " @ %kt_lmb @ " \n";
	%text = %text @ %ac_action   @ %at_action   @ "   " @ %kt_rmb @ " \n";
	return %text;
}
