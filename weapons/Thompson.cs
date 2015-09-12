addDamageType("Thompson",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_thompson> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_thompson> %1',
	0.2, 1);
addDamageType("ThompsonHeadshot",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_thompson><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_thompson><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	0.2, 1);

datablock AudioProfile(ThompsonFireSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/thompson/fire.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ThompsonFireLastSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/thompson/fire_last.wav";
	description = AudioClose3d;
	preload = true;
};

datablock ItemData(ThompsonItem)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/thompson.dts";
	iconName = "Add-Ons/Weapon_Package_Complex/assets/icons/thompson";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	canDrop = 1;
	uiName = "Thompson";
	image = ThompsonImage;

	itemPropsClass = "SimpleMagWeaponProps";
};

function ThompsonItem::onAdd(%this, %obj)
{
	Parent::onAdd(%this, %obj);

	if (!isObject(%obj.itemProps.magazine))
		%obj.hideNode("clip");
}

datablock ShapeBaseImageData(ThompsonImage)
{
	className = "TimeSliceRayWeapon";
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/thompson.dts";

	item = ThompsonItem;
	armReady = 1;
	speedScale = 0.8;

	fireMuzzleVelocity = cf_muzzlevelocity_ms(285);
	fireVelInheritFactor = 0.6;
	fireGravity = cf_bulletdrop_grams(15);
	fireHitExplosion = GunProjectile;
	fireSpread = 2.5;
	fireHitOtherSFX = ComplexDefaultImpactBulletSFX;
	fireRicSFX = ComplexRicSFX;
	fireNearMissSFX = ComplexNearMissSFX;

	stateName[0] = "Activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 0.3;
	stateTransitionOnTimeout[0] = "CheckChamber";

	stateName[1] = "CheckChamber";
	stateTransitionOnLoaded[1] = "Ready";
	stateTransitionOnNotLoaded[1] = "Empty";

	stateName[2] = "Empty";
	stateSequence[2] = "noammo";
	stateTransitionOnLoaded[2] = "Ready";
	stateTransitionOnTriggerDown[2] = "EmptyFire";
	stateTransitionOnAmmo[2] = "Reload";

	stateName[3] = "EmptyFire";
	stateScript[3] = "onEmptyFire";
	stateSequence[3] = "emptyFire";
	stateTimeoutValue[3] = 0.13;
	stateWaitForTimeout[3] = true;
	stateAllowImageChange[3] = false;
	stateTransitionOnTriggerUp[3] = "Empty";

	stateName[4] = "Ready";
	stateSequence[4] = "root";
	stateTransitionOnAmmo[4] = "Reload";
	stateTransitionOnTriggerDown[4] = "Fire";

	stateName[5] = "Fire";
	stateFire[5] = true;
	stateScript[5] = "onFire";
	stateSequence[5] = "Fire";
	stateTimeoutValue[5] = 0.03;
	stateEmitter[5] = advBigBulletFireEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
	stateAllowImageChange[5] = false;
	stateTransitionOnTimeout[5] = "Smoke";

	stateName[6] = "Smoke";
	stateEmitter[6] = advBigBulletSmokeEmitter;
	stateEmitterTime[6] = 0.05;
	stateEmitterNode[6] = "muzzleNode";
	stateTimeoutValue[6] = 0.025;
	stateAllowImageChange[6] = false;
	stateTransitionOnTimeout[6] = "CheckChamber";

	stateName[7] = "Reload";
	stateScript[7] = "onReload";
	stateSound[7] = ComplexBoltSound;
	stateSequence[7] = "Fire";
	stateTimeoutValue[7] = 0.2;
	stateTransitionOnTimeout[7] = "CheckTrigger";

	stateName[8] = "CheckTrigger";
	stateTransitionOnTriggerUp[8] = "CheckChamber";
};

function ThompsonImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber $= "")
		%props.chamber = 0;
	if (!isObject(%props.magazine))
		%obj.mountImage(ThompsonEmptyImage, %slot);
	%obj.setImageLoaded(%slot, %props.chamber == 1);
}

function ThompsonImage::onEmptyFire(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	// if (%props.magazine.count >= 1)
	// {
	// 	%obj.setImageAmmo(%slot, true);
	// 	return;
	// }

	serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function ThompsonImage::onFire(%this, %obj, %slot)
{
	if (%obj.getState() $= "Dead")
		return;

	%obj.playThread(2, "plant");

	%props = %obj.getItemProps();
	%props.chamber = 2;

	Parent::onFire(%this, %obj, %slot);

	if (%props.magazine.count >= 1)
	{
		%obj.stopAudio(3);
		%obj.playAudio(3, ThompsonFireSound);
	}
	else
	{
		%obj.stopAudio(3);
		%obj.playAudio(3, ThompsonFireLastSound);
	}

	%this.pullSlide(%obj, %slot);

	%obj.applyComplexKnockback(0.5);
	%obj.applyComplexScreenshake(0.5);
}

function ThompsonImage::onReload(%this, %obj, %slot)
{
	%this.pullSlide(%obj, %slot);
	%obj.setImageAmmo(%slot, false);
}

function ThompsonImage::pullSlide(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber != 0)
	{
		%obj.ejectShell(Bullet45Item, 1, %props.chamber == 2);
		%props.chamber = 0;
	}

	if (%props.magazine.count >= 1)
	{
		%props.magazine.count--;
		%props.chamber = 1;
	}

	%obj.setImageLoaded(%slot, %props.chamber == 1);

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function ThompsonImage::onLight(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (isObject(%props.magazine))
	{
		%obj.giveMagazineProps(%props.magazine);
		%props.magazine = "";
		%obj.mountImage(ThompsonEmptyImage, %slot);
		%obj.playThread(2, "shiftRight");
		serverPlay3D(ComplexClipOutSound, %obj.getMuzzlePoint(%slot));
	}
	else
	{
		%props.magazine = %obj.takeMagazineProps(%this.item);

		if (isObject(%props.magazine))
		{
			%obj.mountImage(ThompsonImage, %slot);
			serverPlay3D(ComplexClipInSound, %obj.getMuzzlePoint(%slot));
			%obj.playThread(2, "shiftLeft");
		}
		else if (isObject(%obj.client))
			messageClient(%obj.client, '', '\c6You don\'t have any magazines for this weapon.');
	}

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();

	return 1;
}

function ThompsonImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	%props = %obj.getItemProps();

	if (%trigger == 4 && %state && %obj.getImageState(%slot) !$= "Reload")
	{
		%obj.setImageAmmo(%slot, true);
		return 1;
	}

	return 0;
}

function ThompsonImage::damage(%this, %obj, %col, %position, %normal)
{
	if (%col.getRegion(%position, true) $= "head")
	{
		ComplexHeadshotSFX.playFrom(%position, %col);

		%damage = 11;
		%damageType = $DamageType::ThompsonHeadshot;
	}
	else
	{
		ComplexFleshImpactBulletSFX.playFrom(%position, %col);

		%damage = 7.5;
		%damageType = $DamageType::Thompson;
	}

	if (!$NoCrouchDamageBonus && %col.isCrouched())
		%damage /= 2;

	%col.damage(%obj, %position, %damage, %damageType);
}

// function ThompsonImage::getDebugText(%this, %obj, %slot)
// {
//     %props = %obj.getItemProps();
//
//     %text = "\c6" @ (%props.loaded ? "loaded" : "empty");
//     %text = %text SPC (isObject(%props.magazine) ? "mag=" @ %props.magazine.count : "no-mag");
//     %text = %text SPC "state=" @ %obj.getImageState(%slot);
//
//     return %text;
// }

function ThompsonImage::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber == 1)
		return "Your gun is ready to fire. Left click to shoot. Its full-auto action will automatically try to load the next bullet.";

	if (isObject(%props.magazine))
	{
		if (%props.magazine.count < 1)
			return "The magazine in your gun is empty. Press the light key to eject it.";

		return "Your gun has a magazine inserted, but no bullet is chambered. Right click to cycle the action and chamber the next bullet.";
	}

	%index = %obj.findMagazine(%this.item);

	if (%index == -1)
		return "You have no magazines for your gun. You need to get a magazine first.";

	if (%index == -2)
		return "All your magazines are empty. You need to get a non-empty magazine first.";

	return "Your gun has no magazine inserted. Press the light key to insert whichever magazine you have with the most bullets.";
}

function ThompsonImage::getDetailedGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	%kt_lmb = "Primary";
	%kt_rmb = "Jet    ";
	%kt_r   = "Light  ";

	%at_fire     = "Dryfire        ";
	%at_action   = "Pull Slide     ";
	%at_magazine = "Insert Magazine";

	%ac_fire     = "\c7";
	%ac_action   = "\c7";
	%ac_magazine = "\c7";

	if (%props.chamber == 1)
		%at_fire = "Fire           ";

	if (isObject(%props.magazine))
		%at_magazine = "Eject Magazine ";

	if (%props.chamber == 1)
	{
		%ac_fire = "\c6";
	}
	else if (isObject(%props.magazine))
	{
		if (%props.magazine.count >= 1)
			%ac_action = "\c6";
		else
			%ac_magazine = "\c6";
	}

	if (!isObject(%props.magazine))
		%ac_magazine = "\c6";

	%text = "<just:right><font:consolas:16>";
	%text = %text @ %ac_fire     @ %at_fire     @ "   " @ %kt_lmb @ " \n";
	%text = %text @ %ac_action   @ %at_action   @ "   " @ %kt_rmb @ " \n";
	%text = %text @ %ac_magazine @ %at_magazine @ "   " @ %kt_r   @ " \n";
	return %text;
}

//No clip version of Thompson

datablock ShapeBaseImageData(ThompsonEmptyImage : ThompsonImage)
{
	stateSequence[0] = "noclip";
	stateSequence[2] = "noclip";
	stateSequence[3] = "noclip";
	stateSequence[4] = "noclip";
	stateSequence[5] = "noclip_fire";
	stateSequence[7] = "noclip_fire";
};

function ThompsonEmptyImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (%props.chamber $= "")
		%props.chamber = 0;
	%obj.setImageLoaded(%slot, %props.chamber == 1);
}

//Datablock-inherited functions below. Why is this not done by default I will never know.
function ThompsonEmptyImage::onEmptyFire(%this, %obj, %slot)
{
	ThompsonImage::onEmptyFire(%this,%obj,%slot);
}
function ThompsonEmptyImage::onFire(%this, %obj, %slot)
{
	ThompsonImage::onFire(%this,%obj,%slot);
}
function ThompsonEmptyImage::onReload(%this, %obj, %slot)
{
	ThompsonImage::onReload(%this,%obj,%slot);
}
function ThompsonEmptyImage::pullSlide(%this, %obj, %slot)
{
	ThompsonImage::pullSlide(%this,%obj,%slot);
}
function ThompsonEmptyImage::onLight(%this, %obj, %slot)
{
	ThompsonImage::onLight(%this,%obj,%slot);
}
function ThompsonEmptyImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	ThompsonImage::onTrigger(%this, %obj, %slot, %trigger, %state);
}
function ThompsonEmptyImage::damage(%this, %obj, %col, %position, %normal)
{
	ThompsonImage::damage(%this, %obj, %col, %position, %normal);
}
// function ThompsonEmptyImage::getDebugText(%this, %obj, %slot)
// {
// 	ThompsonImage::getDebugText(%this,%obj,%slot);
// }
function ThompsonEmptyImage::getGunHelp(%this, %obj, %slot)
{
	ThompsonImage::getGunHelp(%this,%obj,%slot);
}
function ThompsonEmptyImage::getDetailedGunHelp(%this, %obj, %slot)
{
	ThompsonImage::getDetailedGunHelp(%this, %obj, %slot);
}
function ThompsonEmptyImage::onDrop(%this, %obj, %slot)
{
	%obj.unMountImage(%slot); //Unmount the empty image
	return 0; //Don't interfere with the default serverCmdDropTool function
}
