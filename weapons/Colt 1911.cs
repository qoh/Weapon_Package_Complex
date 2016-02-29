addDamageType("Colt1911",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_colt> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_colt> %1',
	0.2, 1);
addDamageType("Colt1911Headshot",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_colt><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_colt><bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	0.2, 1);

datablock AudioProfile(Colt1911FireSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/colt1911/fire.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Colt1911FireLastSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/colt1911/fire_last.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Colt1911SlidepullSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/colt1911/slide_pull.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Colt1911ClipInSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/colt1911/clip_in.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Colt1911ClipOutSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/colt1911/clip_out.wav";
	description = AudioClose3d;
	preload = true;
};

datablock ItemData(Colt1911Item)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/Colt_1911.dts";
	iconName = "Add-Ons/Weapon_Package_Complex/assets/icons/colt";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

	canDrop = 1;
	uiName = "Colt 1911";
	image = Colt1911Image;

	shellCollisionThreshold = 2;
	shellCollisionSFX = WeaponSoftImpactSFX;

	itemPropsClass = "SimpleMagWeaponProps";
};

datablock ShapeBaseImageData(Colt1911Image)
{
	className = "TimeSliceRayWeapon";
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/Colt_1911.dts";

	item = Colt1911Item;
	armReady = 1;
	speedScale = 0.9;

	fireMuzzleVelocity = cf_muzzlevelocity_ms(251);
	fireVelInheritFactor = 0.25;
	fireGravity = cf_bulletdrop_grams(15);
	fireSpread = 0.4;
	fireHitExplosion = QuietGunProjectile;
	fireHitOtherSFX = ComplexDefaultImpactBulletSFX;
	fireRicSFX = ComplexRicSFX;
	fireNearMissSFX = ComplexNearMissSFX;

	stateName[0] = "Activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 0.15;
	stateTransitionOnTimeout[0] = "Empty";

	stateName[1] = "Empty";
	stateSequence[1] = "noammo";
	stateTransitionOnTriggerDown[1] = "EmptyFire";
	stateTransitionOnAmmo[1] = "Reload";
	stateTransitionOnLoaded[1] = "Ready";

	stateName[2] = "EmptyFire";
	stateScript[2] = "onEmptyFire";
	stateSequence[2] = "emptyFire";
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
	stateFire[4] = true;
	stateScript[4] = "onFire";
	stateSequence[4] = "Fire";
	stateTimeoutValue[4] = 0.12;
	stateEmitter[4] = advSmallBulletFireEmitter;
	stateEmitterTime[4] = 0.05;
	stateEmitterNode[4] = "muzzleNode";
	stateAllowImageChange[4] = false;
	stateTransitionOnTimeout[4] = "Smoke";

	stateName[5] = "Smoke";
	stateEmitter[5] = advSmallBulletSmokeEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
	stateTimeoutValue[5] = 0.1;
	stateWaitForTimeout[5] = false;
	stateAllowImageChange[5] = false;
	stateTransitionOnTriggerUp[5] = "Empty"; //Thanks to this the smoke state can be entirely bypassed.

	stateName[6] = "Reload";
	stateSound[6] = Colt1911SlidepullSound;
	stateSequence[6] = "ejectShell";
	stateScript[6] = "onReload";
	stateTimeoutValue[6] = 0.2;
	stateAllowImageChange[6] = false;
	stateWaitForTimeout[6] = true;
	stateTransitionOnNoAmmo[6] = "Empty";
};

function Colt1911Image::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber $= "")
		%props.chamber = 0;

	%obj.setImageLoaded(%slot, %props.chamber == 1);
}

function Colt1911Image::onUnMount(%this, %obj, %slot)
{
}

function Colt1911Image::onEmptyFire(%this, %obj, %slot)
{
	serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function Colt1911Image::onFire(%this, %obj, %slot)
{
	if (%obj.getState() $= "Dead")
		return;

	%props = %obj.getItemProps();
	%props.chamber = 2;

	%obj.playThread(2, "shiftLeft");
	%obj.playThread(3, "shiftRight");

	Parent::onFire(%this, %obj, %slot);

	if (%props.magazine.count >= 1)
		serverPlay3D(Colt1911FireSound, %obj.getMuzzlePoint(%slot));
	else
		serverPlay3D(Colt1911FireLastSound, %obj.getMuzzlePoint(%slot));

	%this.pullSlide(%obj, %slot);

	%obj.applyComplexKnockback(0.5);
	%obj.applyComplexScreenshake(0.4);
}

function Colt1911Image::onReload(%this, %obj, %slot)
{
	%this.pullSlide(%obj, %slot);
	%obj.setImageAmmo(%slot, false);
}

function Colt1911Image::pullSlide(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber != 0)
	{
		%obj.ejectShell(Bullet45Item, 0.5, %props.chamber == 2);
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

function Colt1911Image::onLight(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (isObject(%props.magazine))
	{
		%obj.giveMagazineProps(%props.magazine);
		%props.magazine = "";

		%obj.playThread(2, "shiftRight");
		serverPlay3D(Colt1911ClipOutSound, %obj.getMuzzlePoint(%slot));
	}
	else
	{
		%props.magazine = %obj.takeMagazineProps(%this.item);

		if (isObject(%props.magazine))
		{
			serverPlay3D(Colt1911ClipInSound, %obj.getMuzzlePoint(%slot));
			%obj.playThread(2, "shiftLeft");
		}
		else if (isObject(%obj.client))
			messageClient(%obj.client, '', '\c6You don\'t have any magazines for this weapon.');
	}

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();

	return 1;
}

function Colt1911Image::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	%props = %obj.getItemProps();

	if (%trigger == 4 && %state && %obj.getImageState(%slot) !$= "Reload")
	{
		%obj.setImageAmmo(%slot, true);
		return 1;
	}

	return 0;
}

function Colt1911Image::damage(%this, %obj, %col, %position, %normal)
{
	if (%col.getRegion(%position, true) $= "head")
	{
		ComplexHeadshotSFX.playFrom(%position, %col);

		%damage = 32;
		%damageType = $DamageType::Colt1911Headshot;
	}
	else
	{
		ComplexFleshImpactBulletSFX.playFrom(%position, %col);

		%damage = 16;
		%damageType = $DamageType::Colt1911;
	}

	%col.damage(%obj, %position, %damage, %damageType);
}

function Colt1911Image::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber == 1)
		return "Your gun is ready to fire. Left click to shoot. Its semi-auto action will automatically try to load the next bullet.";

	if (isObject(%props.magazine))
	{
		if (%props.magazine.count < 1)
			return "The magazine in your gun is empty. Press the light key to eject it.";

		return "Your gun has a magazine inserted, but no bullet is chambered. Right click to pull the slide and chamber the next bullet.";
	}

	%index = %obj.findMagazine(%this.item);

	if (%index == -1)
		return "You have no magazines for your gun. You need to get a magazine first.";

	if (%index == -2)
		return "All your magazines are empty. You need to get a non-empty magazine first.";

	return "Your gun has no magazine inserted. Press the light key to insert whichever magazine you have with the most bullets.";
}

function Colt1911Image::getDetailedGunHelp(%this, %obj, %slot)
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
