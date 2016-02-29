addDamageType("MicroUzi",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_uzi> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_uzi> %1',
	0.2, 1);
addDamageType("MicroUziHeadshot",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_uzi><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_uzi><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	0.2, 1);

datablock AudioProfile(MicroUziFireSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/mac10/fire_close_mono.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(MicroUziBoltPullSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/mac10/bolt_back.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(MicroUziBoltReleaseSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/mac10/bolt_forward.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(MicroUziClipInSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/mac10/clip_in.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(MicroUziClipOutSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/mac10/clip_out.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(MicroUziDrawSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/mac10/draw.wav";
	description = AudioClose3d;
	preload = true;
};

// new ScriptObject(MicroUziFireSFX)
// {
// 	class = "SFXEffect";

// 	// file["source", 1] ="Add-Ons/Weapon_Package_Complex/assets/sounds/mac10/fire_close_stereo.wav" ;
// 	// use2D["source"] = "always";
// 	// filterListener["source"] = "source";
// 	// filterDistanceMax["source"] = 64;

// 	file["close", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/mac10/fire_close_mono.wav";
// 	description["close", 1] = Audio3dUniform64;
// 	// use2D["close"] = "source";
// 	// filterListener["close"] = "other";
// 	filterDistanceMax["close"] = 64;

// 	file["distant", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/mac10/fire_distant_mono_need_stereo.wav";
// 	use2D["distant"] = "always";
// 	filterDistanceMin["distant"] = 64;
// 	filterDistanceMax["distant"] = 384;
// };

datablock ItemData(MicroUziItem)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/micro_uzi.dts";
	iconName = "Add-Ons/Weapon_Package_Complex/assets/icons/uzi";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	canDrop = 1;
	uiName = "Micro Uzi";
	image = MicroUziImage;

	shellCollisionThreshold = 2;
	shellCollisionSFX = WeaponSoftImpactSFX;

	itemPropsClass = "SimpleMagWeaponProps";
};

function MicroUziItem::onAdd(%this, %obj)
{
	Parent::onAdd(%this, %obj);

	if (!isObject(%obj.itemProps.magazine))
		%obj.hideNode("mag");
}

datablock ShapeBaseImageData(MicroUziImage)
{
	className = "TimeSliceRayWeapon";
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/micro_uzi.dts";

	item = MicroUziItem;
	armReady = 1;

	fireMuzzleVelocity = cf_muzzlevelocity_ms(400);
	fireVelInheritFactor = 0.7;
	fireGravity = cf_bulletdrop_grams(8);
	fireHitExplosion = QuietGunProjectile;
	fireSpread = 3.25;
	fireHitOtherSFX = ComplexDefaultImpactBulletSFX;
	fireRicSFX = ComplexRicSFX;
	fireNearMissSFX = ComplexNearMissSFX;

	stateName[0] = "Activate";
	stateSound[0] = "MicroUziDrawSound";
	stateTimeoutValue[0] = 0.1;
	stateTransitionOnTimeout[0] = "ActivateFromSwitch";
	stateTransitionOnNotLoaded[0] = "ActivateFromSwitch";

	stateName[20] = "ActivateFromSwitch";
	stateScript[20] = "onSwitched";
	stateTransitionOnTimeout[20] = "Empty";

	stateName[1] = "Empty";
	stateTransitionOnTriggerDown[1] = "EmptyFire";
	stateTransitionOnAmmo[1] = "Reload";
	stateTransitionOnLoaded[1] = "Ready";

	stateName[2] = "EmptyFire";
	stateScript[2] = "onEmptyFire";
	stateSequence[2] = "emptyfire";
	stateTimeoutValue[2] = 0.13;
	stateWaitForTimeout[2] = true;
	stateAllowImageChange[2] = false;
	stateTransitionOnTriggerUp[2] = "Empty";

	stateName[3] = "Ready";
	stateTransitionOnTriggerDown[3] = "Fire";
	stateTransitionOnAmmo[3] = "Reload";
	stateTransitionOnNotLoaded[3] = "Empty";

	stateName[4] = "Fire";
	stateFire[4] = true;
	stateScript[4] = "onFire";
	stateSequence[4] = "Fire";
	stateTimeoutValue[4] = 0.02;
	stateEmitter[4] = advSmallBulletFireEmitter;
	stateEmitterTime[4] = 0.05;
	stateEmitterNode[4] = "muzzleNode";
	stateAllowImageChange[4] = false;
	stateTransitionOnTimeout[4] = "Smoke";

	stateName[5] = "Smoke";
	stateEmitter[5] = advSmallBulletSmokeEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
	stateTimeoutValue[5] = 0.01;
	stateAllowImageChange[5] = false;
	stateTransitionOnTimeout[5] = "Empty";

	stateName[6] = "Reload";
	stateScript[6] = "onReload";
	stateSequence[6] = "hammerpull";
	stateTimeoutValue[6] = 0.25;
	stateTransitionOnTimeout[6] = "Empty";
};

function MicroUziImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (!isObject(%props.magazine))
		%obj.mountImage(MicroUziEmptyImage, %slot);
}

function MicroUZIImage::onSwitched(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber $= "")
		%props.chamber = 0;
	%obj.setImageLoaded(%slot, %props.chamber == 1);
}

function MicroUziImage::onEmptyFire(%this, %obj, %slot)
{
	serverPlay3D(ComplexClipEmptyPistolSound, %obj.getMuzzlePoint(%slot));
}

function MicroUziImage::onFire(%this, %obj, %slot)
{
	if (%obj.getState() $= "Dead")
		return;

	%obj.playThread(2, "plant");

	%props = %obj.getItemProps();
	%props.chamber = 2;

	Parent::onFire(%this, %obj, %slot);
	// MicroUziFireSFX.playFrom(%obj.getMuzzlePoint(%slot), %obj);

	// if (%props.magazine.count >= 1)
	%obj.stopAudio(3);
	%obj.playAudio(3, MicroUziFireSound);
	// else
	//     serverPlay3D(ThompsonFireLastSound, %obj.getMuzzlePoint(%slot));

	if (%props.magazine.count < 1)
		%obj.schedule(50, playAudio, 2, ComplexClipEmptyPistolSound);

	%this.action(%obj, %slot);

	%obj.applyComplexKnockback(0.5);
	%obj.applyComplexScreenshake(0.5);
}

function MicroUziImage::onReload(%this, %obj, %slot)
{
	%obj.schedule(0, playAudio, 2, MicroUziBoltPullSound);
	%obj.schedule(175, playAudio, 2, MicroUziBoltReleaseSound);

	%this.action(%obj, %slot);
	%obj.setImageAmmo(%slot, false);
}

function MicroUziImage::action(%this, %obj, %slot)
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

function MicroUziImage::onLight(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%state = %obj.getImageState(%slot);

	if (%state !$= "Ready" && %state !$= "Empty")
	{
		return 1;
	}

	if (isObject(%props.magazine))
	{
		%obj.giveMagazineProps(%props.magazine);
		%props.magazine = "";
		%obj.mountImage(MicroUziEmptyImage, %slot);
		%obj.setImageLoaded(%slot, 0);
		%obj.playThread(2, "shiftRight");
		serverPlay3D(MicroUziClipOutSound, %obj.getMuzzlePoint(%slot));
	}
	else
	{
		%props.magazine = %obj.takeMagazineProps(%this.item);

		if (isObject(%props.magazine))
		{
			%obj.mountImage(MicroUziImage, %slot);
			%obj.setImageLoaded(%slot, 0);
			serverPlay3D(MicroUziClipInSound, %obj.getMuzzlePoint(%slot));
			%obj.playThread(2, "shiftLeft");
		}
		else if (isObject(%obj.client))
			messageClient(%obj.client, '', '\c6You don\'t have any magazines for this weapon.');
	}

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();

	return 1;
}

function MicroUziImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	%props = %obj.getItemProps();

	if (%trigger == 4 && %state && %obj.getImageState(%slot) !$= "Reload")
	{
		%obj.setImageAmmo(%slot, true);
		return 1;
	}

	return 0;
}

function MicroUziImage::damage(%this, %obj, %col, %position, %normal)
{
	if (%col.getRegion(%position, true) $= "head")
	{
		ComplexHeadshotSFX.playFrom(%position, %col);

		%damage = 10*1.5;
		%damageType = $DamageType::MicroUziHeadshot;
	}
	else
	{
		ComplexFleshImpactBulletSFX.playFrom(%position, %col);

		%damage = 10;
		%damageType = $DamageType::MicroUzi;
	}

	%col.damage(%obj, %position, %damage, %damageType);
}

// function MicroUziImage::getDebugText(%this, %obj, %slot)
// {
//     %props = %obj.getItemProps();

//     %text = "\c6" @ (%props.loaded ? "loaded" : "empty");
//     %text = %text SPC (isObject(%props.magazine) ? "mag=" @ %props.magazine.count : "no-mag");
//     %text = %text SPC "state=" @ %obj.getImageState(%slot);

//     return %text;
// }

function MicroUziImage::getGunHelp(%this, %obj, %slot)
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

function MicroUziImage::getDetailedGunHelp(%this, %obj, %slot)
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

//No clip version of MicroUzi

datablock ShapeBaseImageData(MicroUziEmptyImage : MicroUziImage)
{
	stateSequence[0] = "noclip";
	stateSequence[20] = "noclip";
	stateSequence[2] = "noclip_emptyfire";
	stateSequence[3] = "noclip";
	stateSequence[4] = "noclip_Fire";
	stateSequence[6] = "noclip_hammerpull";
};

function MicroUziEmptyImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (%props.chamber $= "")
		%props.chamber = 0;
	// %obj.setImageLoaded(%slot, %props.chamber == 1);
}
//Datablock-inherited functions below. Why is this not done by default I will never know.

function MicroUziEmptyImage::onSwitched(%this, %obj, %slot)
{
	MicroUZIImage::onSwitched(%this, %obj, %slot);
}

function MicroUziEmptyImage::onEmptyFire(%this, %obj, %slot)
{
	MicroUziImage::onEmptyFire(%this,%obj,%slot);
}
function MicroUziEmptyImage::onFire(%this, %obj, %slot)
{
	MicroUziImage::onFire(%this,%obj,%slot);
}
function MicroUziEmptyImage::onReload(%this, %obj, %slot)
{
	MicroUziImage::onReload(%this,%obj,%slot);
}
function MicroUziEmptyImage::action(%this, %obj, %slot)
{
	MicroUziImage::action(%this,%obj,%slot);
}
function MicroUziEmptyImage::onLight(%this, %obj, %slot)
{
	MicroUziImage::onLight(%this,%obj,%slot);
}
function MicroUziEmptyImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	MicroUziImage::onTrigger(%this, %obj, %slot, %trigger, %state);
}
function MicroUziEmptyImage::damage(%this, %obj, %col, %position, %normal)
{
	MicroUziImage::damage(%this, %obj, %col, %position, %normal);
}
function MicroUziEmptyImage::getDebugText(%this, %obj, %slot)
{
	MicroUziImage::getDebugText(%this,%obj,%slot);
}
function MicroUziEmptyImage::getGunHelp(%this, %obj, %slot)
{
	MicroUziImage::getGunHelp(%this,%obj,%slot);
}
function MicroUziEmptyImage::getDetailedGunHelp(%this, %obj, %slot)
{
	MicroUziImage::getDetailedGunHelp(%this,%obj,%slot);
}
function MicroUziEmptyImage::onDrop(%this, %obj, %slot)
{
	%obj.unMountImage(%slot); //Unmount the empty image
	return 0; //Don't interfere with the default serverCmdDropTool function
}
