addDamageType("Remington870",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_remmington> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_remmington> %1',
	0.2, 1);

new ScriptObject(Remington870FireSFX)
{
	class = "SFXEffect";

	file["close", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/remington870/fire.wav";
	filterDistanceMax["close"] = 48;
	use2D["close"] = "source";

	file["far", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/far/fire_shotgun.0.wav";
	file["far", 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/far/fire_shotgun.1.wav";
	file["far", 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/far/fire_shotgun.2.wav";
	file["far", 4] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/far/fire_shotgun.3.wav";
	filterDistanceMin["far"] = 48;
	filterDistanceMax["far"] = 128;
	use2D["far"] = "always";

	file["very_far", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/very_far/fire_shotgun.0.wav";
	file["very_far", 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/very_far/fire_shotgun.1.wav";
	file["very_far", 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/general/distant/very_far/fire_shotgun.2.wav";
	filterDistanceMin["very_far"] = 128;
	filterDistanceMax["very_far"] = 1024;
	use2D["very_far"] = "always";
};

datablock AudioProfile(Remington870FireSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/remington870/fire.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Remington870PumpSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/remington870/pump.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Remington870InsertSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/remington870/reload.wav";
	description = AudioClose3d;
	preload = true;
};

datablock ItemData(Remington870Item)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington870.dts";

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

	uiName = "Remington 870";
	iconName = "Add-Ons/Weapon_Package_Complex/assets/icons/remmington";
	image = Remington870Image;
	canDrop = true;

	bulletType = "Buckshot";

	shellCollisionThreshold = 2;
	shellCollisionSFX = WeaponHardImpactSFX;
};

datablock ShapeBaseImageData(Remington870Image)
{
	className = "TimeSliceRayWeapon";
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington870.dts";

	fireMuzzleVelocity = cf_muzzlevelocity_ms(472.44);
	fireVelInheritFactor = 0.75;
	fireGravity = cf_bulletdrop_grams(25);
	fireHitExplosion = GunProjectile;
	fireCount = 8;
	// fireSpread = 11;
	fireSpread = 7;
	fireHitOtherSFX = ComplexDefaultImpactBulletSFX;
	fireRicSFX = ComplexRicSFX;
	fireNearMissSFX = ComplexNearMissSFX;

	item = Remington870Item;
	armReady = true;
	speedScale = 0.795;

	stateName[0] = "Activate";
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 0.15;
	stateTransitionOnTimeout[0] = "Empty";

	stateName[1] = "Empty";
	stateSequence[1] = "root";
	stateTransitionOnTriggerDown[1] = "EmptyFire";
	stateTransitionOnLoaded[1] = "Ready";
	stateTransitionOnAmmo[1] = "Pump";

	stateName[2] = "EmptyFire";
	stateScript[2] = "onEmptyFire";
	stateTimeoutValue[2] = 0.13;
	stateWaitForTimeout[2] = true;
	stateTransitionOnTriggerUp[2] = "Empty";

	stateName[3] = "Ready";
	stateSequence[1] = "root";
	stateTransitionOnTriggerDown[3] = "Fire";
	stateTransitionOnNotLoaded[3] = "Empty";
	stateTransitionOnAmmo[3] = "Pump";

	stateName[4] = "Fire";
	stateFire[4] = true;
	// stateSound[4] = Remington870FireSound;
	stateSound[4] = "";
	stateScript[4] = "onFire";
	stateSequence[4] = "Fire";
	stateEmitter[4] = advSmallBulletFireEmitter;
	stateEmitterTime[4] = 0.05;
	stateEmitterNode[4] = "muzzleNode";
	stateTimeoutValue[4] = 0.15;
	stateTransitionOnTimeout[4] = "Smoke";

	stateName[5] = "Smoke";
	stateEmitter[5] = advSmallBulletSmokeEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
	stateTimeoutValue[5] = 0.15;
	stateTransitionOnTriggerUp[5] = "Empty";

	stateName[6] = "Pump";
	stateSound[6] = Remington870PumpSound;
	stateScript[6] = "onPump";
	stateSequence[6] = "Pump";
	stateTimeoutValue[6] = 0.35; // 0.2
	stateWaitForTimeout[6] = true;
	stateTransitionOnNoAmmo[6] = "Empty";
};

function Remington870Image::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber $= "")
		%props.chamber = 0;

	if (%obj.bulletCount[%this.item.bulletType] $= "")
		%obj.bulletCount[%this.item.bulletType] = 0;

	%obj.setImageLoaded(%slot, %props.chamber == 1);
}

function Remington870Image::onEmptyFire(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	// if (%props.count >= 1)
	// {
	//     %obj.setImageAmmo(%slot, true);
	//     return;
	// }

	serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function Remington870Image::onFire(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if(!%obj.suiciding) //If we're not firing through suicide
		Parent::onFire(%this, %obj, %slot);
	Remington870FireSFX.playFrom(%obj.getMuzzlePoint(%slot), %obj);

	%obj.playThread(2, "shiftLeft");
	%obj.playThread(3, "shiftRight");

	%props.chamber = 2;
	%obj.setImageLoaded(%slot, false);

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();

	%obj.applyComplexKnockback(3);
	%obj.applyComplexScreenshake(0.6);
}

function Remington870Image::onSuicide(%this, %obj, %slot)
{
	%image = %obj.getMountedImage(%slot);
	%state = %obj.getImageState(%slot);
	if(%state !$= "Ready" && %state !$= "Empty")
	{
		return 1;
	}
	%props = %obj.getItemProps();
	if(%props.chamber != 1) //Empty
	{
		%obj.setImageTrigger(%slot, 1);
		%obj.playThread(2, plant);
	}
	else
	{
		//What you're about to see below is probably the ugliest thing I ever coded. ~Jack Noir
		%obj.playThread(2, shiftRight);
		%obj.playThread(3, shiftLeft);
		%obj.applyComplexKnockback(5);
		serverPlay3D(M1GarandFireLastSound, %obj.getMuzzlePoint(%slot));
		%obj.suiciding = 1;
		%props.chamber = 2;
		%obj.setImageTrigger(%slot, 1);
		%proj = new ScriptObject()
		{	
			class = "ProjectileRayCast";
			superClass = "TimedRayCast";

			position = %obj.getEyePoint();
			velocity = "0 0 0";

			lifetime = %this.fireLifetime;
			gravity = %this.fireGravity;

			mask = %this.fireMask $= "" ? $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType : %this.fireMask;
			exempt = "";
			sourceObject = %obj;
			sourceClient = %obj.client;
			damage = %this.directDamage;
			damageType = %this.directDamageType;
			damageRef = %this;
			hitExplosion = %this.projectile;
		};
		MissionCleanup.add(%proj);
		%obj.setDamageLevel(%obj.getDataBlock().maxDamage - 1); //Set their HP to 1 so headshot will be a guaranteed instakill
		%proj.fire();
	}
	return 1;
}

function Remington870Image::onPump(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber != 0)
	{
		%obj.ejectShell(BulletBuckshotItem, 1.35, %props.chamber == 2);
		%props.chamber = 0;
	}

	if (%props.count >= 1)
	{
		%props.count--;
		%props.chamber = 1;
		// insert shell
	}

	%obj.setImageLoaded(%slot, %props.chamber == 1);
	%obj.setImageAmmo(%slot, false);

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function Remington870Image::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	if (%trigger == 4 && %state && %obj.getImageState(%slot) !$= "Pump")
	{
		%obj.setImageAmmo(%slot, true);
		return 1;
	}

	return 0;
}

function Remington870Image::onLight(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%state = %obj.getImageState(%slot);

	if (%props.count < 6 && (%state $= "Ready" || %state $= "Empty") && (%obj.bulletCount[%this.item.bulletType] >= 1 || %obj.bulletCount[%this.item.bulletType] == -1))
	{
		if ($Sim::Time - %obj.lastRemingtonInsert <= 0.2)
			return 1;

		%obj.lastRemingtonInsert = $Sim::Time;
		if (%obj.bulletCount[%this.item.bulletType] != -1)
			%obj.bulletCount[%this.item.bulletType]--;
		%props.count++;
		%obj.playThread(2, "plant");
		serverPlay3D(Remington870InsertSound, %obj.getMuzzlePoint(%slot));

		if (isObject(%obj.client))
			%obj.client.updateDetailedGunHelp();

		return 1;
	}

	return 0;
}

function Remington870Image::damage(%this, %obj, %col, %position, %normal)
{
	%damage = 10;
	%damageType = $DamageType::Remington870;

	if (!$NoCrouchDamageBonus && %col.isCrouched())
		%damage /= 2;

	ComplexFleshImpactBulletSFX.playFrom(%position, %col);
	%col.damage(%obj, %position, %damage, %damageType);
}

// function Remington870Image::getDebugText(%this, %obj, %slot)
// {
//     %props = %obj.getItemProps();
//     %text = "\c6" @ (%props.chamber == 2 ? "spent" : (%props.chamber == 1 ? "loaded" : "empty")) @ " \c3";
//
//     for (%i = 0; %i < %props.count; %i++) %text = %text @ "o";
//     %text = %text @ "\c7";
//     for (0; %i < 6; %i++) %text = %text @ "o";
//     %text = %text @ "\n";
//
//     return %text;
// }

function Remington870Image::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber == 1)
		return "Your gun is ready to fire. Left click to shoot. You will need to pump the action afterwards to chamber the next bullet.";

	if (%props.count < 1)
		return "Your gun has no more bullets stored. Press the light key to insert one.";

	return "Your gun has bullets stored, but no bullet is chambered. Right click to pump the action and chamber the next bullet.";
}

function Remington870Image::getDetailedGunHelp(%this, %obj, %slot, %hidden)
{
	%props = %obj.getItemProps();

	if (%obj.bulletCount[%this.item.bulletType] != -1)
		%text = "<just:center><font:Arial:20>\c6Buckshot\c3:" SPC %obj.bulletCount[%this.item.bulletType];

	%kt_lmb = "Primary";
	%kt_rmb = "Jet    ";
	%kt_r   = "Light  ";

	%at_fire     = "Dryfire     ";
	%at_action   = "Pump Action ";
	%at_magazine = "Insert Round";

	%ac_fire     = "\c7";
	%ac_action   = "\c7";
	%ac_magazine = "\c7";

	if (%props.chamber == 1)
		%at_fire = "Fire        ";

	if (%props.chamber == 1)
		%ac_fire = "\c6";
	else
	{
		if (%props.count >= 1)
			%ac_action = "\c6";
		else
			%ac_magazine = "\c6";
	}

	if (%props.chamber == 2)
		%ac_action = "\c6";

	if (%props.count < 6)
		%ac_magazine = "\c6";

	if (%obj.bulletCount[%this.item.bulletType] < 1 && %obj.bulletCount[%this.item.bulletType] != -1)
		%ac_magazine = "\c7";

	if (!%hidden)
	{
		%text = %text @ "<just:right><font:consolas:16>";
		%text = %text @ %ac_fire     @ %at_fire     @ "   " @ %kt_lmb @ " \n";
		%text = %text @ %ac_action   @ %at_action   @ "   " @ %kt_rmb @ " \n";
		%text = %text @ %ac_magazine @ %at_magazine @ "   " @ %kt_r   @ " \n";
	}
	return %text;
}
