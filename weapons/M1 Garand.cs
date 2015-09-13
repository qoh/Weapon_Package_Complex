addDamageType("M1Garand",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_m1garand> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_m1garand> %1',
	0.2, 1);
addDamageType("M1GarandHeadshot",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_m1garand><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_m1garand><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	0.2, 1);

datablock AudioProfile(M1GarandFireSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/m1garand/fire.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(M1GarandFireLastSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/m1garand/fire_last.wav";
	description = AudioClose3d;
	preload = true;
};

datablock ItemData(M1GarandItem)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/m1garand.dts";

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

	uiName = "M1 Garand";
	iconName = "Add-Ons/Weapon_Package_Complex/assets/icons/m1garand";
	image = M1GarandImage;
	canDrop = true;

	itemPropsClass = "SimpleMagWeaponProps";
};

datablock ShapeBaseImageData(M1GarandImage)
{
	className = "TimeSliceRayWeapon";
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/m1garand.dts";

	fireMuzzleVelocity = cf_muzzlevelocity_ms(853);
	fireVelInheritFactor = 0.5;
	fireGravity = cf_bulletdrop_grams(10);
	fireSpread = 0.02;
	fireHitExplosion = GunProjectile;
	fireHitOtherSFX = ComplexDefaultImpactBulletSFX;
	fireRicSFX = ComplexRicSFX;
	fireNearMissSFX = ComplexNearMissSFX;

	item = M1GarandItem;
	armReady = true;
	speedScale = 0.7;

	stateName[0] = "Activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 0.2;
	stateAllowImageChange[0] = false;
	stateTransitionOnTimeout[0] = "Empty";

	stateName[1] = "Empty";
	stateSequence[1] = "root";
	stateTransitionOnTriggerDown[1] = "EmptyFire";
	stateTransitionOnAmmo[1] = "Reload";
	stateTransitionOnLoaded[1] = "Ready";

	stateName[2] = "EmptyFire";
	stateScript[2] = "onEmptyFire";
	stateTimeoutValue[2] = 0.13;
	stateWaitForTimeout[2] = true;
	stateAllowImageChange[2] = false;
	stateTransitionOnTriggerUp[2] = "Empty";

	stateName[3] = "Ready";
	stateSequence[3] = "root";
	stateTransitionOnTriggerDown[3] = "Fire";
	stateTransitionOnAmmo[3] = "Reload";
	stateTransitionOnNotLoaded[3] = "Empty";

	stateName[4] = "Fire";
	stateScript[4] = "onFire";
	stateSequence[4] = "Fire";
	stateEmitter[4] = advBigBulletFireEmitter;
	stateEmitterTime[4] = 0.05;
	stateEmitterNode[4] = "muzzleNode";
	stateTimeoutValue[4] = 0.2;
	stateFire[4] = true;
	stateAllowImageChange[4] = false;
	stateTransitionOnTimeout[4] = "Smoke";

	stateName[5] = "Smoke";
	stateEmitter[5] = advBigBulletSmokeEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
	stateTimeoutValue[5] = 0.3;
	stateAllowImageChange[5] = false;
	stateWaitForTimeout[5] = true;
	stateTransitionOnTimeout[5] = "CheckTrigger"; //Alright, delay passed, let's check if the dude released the trigger yet

	stateName[6] = "Reload";
	stateSound[6] = ComplexBoltSound;
	stateSequence[6] = "eject";
	stateScript[6] = "onReload";
	stateTimeoutValue[6] = 0.2;
	stateAllowImageChange[6] = false;
	stateTransitionOnTimeout[6] = "Empty";

	stateName[20] = "CheckTrigger";
	stateTransitionOnTriggerUp[20] = "Empty";
};

function M1GarandImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber $= "")
		%props.chamber = 0;

	%obj.setImageLoaded(%slot, %props.chamber == 1);
}

function M1GarandImage::onEmptyFire(%this, %obj, %slot)
{
	serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function M1GarandImage::onFire(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%props.chamber = 2;

	Parent::onFire(%this, %obj, %slot);

	%obj.playThread(2, "shiftAway");
	%obj.schedule(100, playThread, 3, "plant");

	if (%props.magazine.count >= 1)
		serverPlay3D(M1GarandFireSound, %obj.getMuzzlePoint(%slot));
	else
		serverPlay3D(M1GarandFireLastSound, %obj.getMuzzlePoint(%slot));

	%this.pullSlide(%obj, %slot);

	%obj.applyComplexKnockback(3.4);
	%obj.applyComplexScreenshake(1);
}

function M1GarandImage::onReload(%this, %obj, %slot)
{
	%this.pullSlide(%obj, %slot);
	%obj.setImageAmmo(%slot, false);
}

function M1GarandImage::pullSlide(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber != 0)
	{
		%obj.ejectShell(Bullet30Item, 1, %props.chamber == 2);
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

function M1GarandImage::onLight(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (isObject(%props.magazine))
	{
		%obj.giveMagazineProps(%props.magazine);
		%props.magazine = "";

		%obj.playThread(2, "shiftRight");
		serverPlay3D(ComplexClipOutSound, %obj.getMuzzlePoint(%slot));
	}
	else
	{
		%props.magazine = %obj.takeMagazineProps(%this.item);

		if (isObject(%props.magazine))
		{
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

function M1GarandImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	%props = %obj.getItemProps();

	if (%trigger == 4 && %state && %obj.getImageState(%slot) !$= "Reload")
	{
		%obj.setImageAmmo(%slot, true);
		return 1;
	}

	return 0;
}

function M1GarandImage::damage(%this, %obj, %col, %position, %normal)
{
	if (%col.getRegion(%position, true) $= "head")
	{
		ComplexHeadshotSFX.playFrom(%position, %col);

		%damage = 90;
		%damageType = $DamageType::M1GarandHeadshot;
	}
	else
	{
		ComplexFleshImpactBulletSFX.playFrom(%position, %col);

		%damage = 30;
		%damageType = $DamageType::M1Garand;
	}

	if (!$NoCrouchDamageBonus && %col.isCrouched())
		%damage /= 2;

	%col.damage(%obj, %position, %damage, %damageType);
}

function M1GarandImage::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber == 1)
		return "Your gun is ready to fire. Left click to shoot. Its semi-auto action will automatically try to load the next bullet.";

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

function M1GarandImage::getDetailedGunHelp(%this, %obj, %slot)
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
