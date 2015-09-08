addDamageType("Revolver",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_snwrevolver> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_snwrevolver> %1',
	0.2, 1);
addDamageType("RevolverHeadshot",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_snwrevolver> <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_snwrevolver> <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_headshot> %1',
	0.2, 1);

datablock AudioProfile(RevolverFireSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/fire.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RevolverCycleSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/flick.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RevolverSpinSound1)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/spin1.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RevolverSpinSound2)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/spin2.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RevolverSpinSound3)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/spin3.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RevolverSpinSound4)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/spin4.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RevolverSpinSound5)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/spin5.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RevolverOpenSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/open.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RevolverCloseSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/close.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(RevolverClickSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver/click.wav";
	description = AudioClose3D;
	preload = true;
};

datablock ItemData(RevolverItem)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/S&W_Revolver.dts";
	iconName = "Add-Ons/Weapon_Package_Complex/assets/icons/snwrevolver";
	uiName = "S&W Revolver";

	canDrop = true;
	image = RevolverImage;

	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

	itemPropsClass = "RevolverProps";
};

datablock ShapeBaseImageData(RevolverImage)
{
	className = "TimeSliceRayWeapon";
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/S&W_Revolver.dts";

	fireMuzzleVelocity = cf_muzzlevelocity_ms(396.24);
	fireVelInheritFactor = 0.25;
	fireGravity = cf_bulletdrop_grams(10);
	fireHitExplosion = GunProjectile;

	item = RevolverItem;
	armReady = true;
	isRevolver = true;
	speedScale = 0.85;

	insertSound0 = AdvReloadInsert1Sound;
	insertSound1 = AdvReloadInsert2Sound;
	numInsertSound = 2;

	stateName[0] = "Activate";
	stateSequence[0] = "Activate";
	stateTimeoutValue[0] = 0.15;
	stateAllowImageChange[0] = false;
	stateTransitionOnTimeout[0] = "CheckChamber";

	stateName[2] = "Empty";
	stateSequence[2] = "noammo";
	stateTransitionOnLoaded[2] = "Ready";
	stateTransitionOnAmmo[2] = "Opening";
	stateTransitionOnTriggerDown[2] = "EmptyFire";

	stateName[3] = "EmptyFire";
	stateSound[3] = RevolverClickSound;
	stateScript[3] = "onEmptyFire";
	stateSequence[3] = "emptyFire";
	stateTimeoutValue[3] = 0.13;
	stateWaitForTimeout[3] = true;
	stateAllowImageChange[3] = false;
	stateTransitionOnTriggerUp[3] = "CheckChamber";

	stateName[4] = "Ready";
	stateSequence[4] = "root";
	stateTransitionOnNotLoaded[4] = "Empty";
	stateTransitionOnAmmo[4] = "Opening";
	stateTransitionOnTriggerDown[4] = "Fire";

	stateName[5] = "Fire";
	stateFire[5] = true;
	stateSound[5] = RevolverFireSound;
	stateScript[5] = "onFire";
	stateSequence[5] = "fire";
	stateEmitter[5] = advBigBulletFireEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
	stateTimeoutValue[5] = 0.1;
	stateWaitForTimeout[5] = true;
	stateAllowImageChange[5] = false;
	stateTransitionOnTimeout[5] = "Smoke";

	stateName[6] = "Smoke";
	stateEmitter[6] = advBigBulletSmokeEmitter;
	stateEmitterTime[6] = 0.05;
	stateEmitterNode[6] = "muzzleNode";
	stateTimeoutValue[6] = 0.11;
	stateWaitForTimeout[6] = true;
	stateAllowImageChange[6] = false;
	stateTransitionOnTimeout[6] = "CheckTrigger"; //Alright, delay passed, let's check if the dude released the trigger yet

	stateName[7] = "PullHammer";
	stateSound[7] = RevolverCycleSound;
	stateSequence[7] = "Clickdown";
	stateTimeoutValue[7] = 0.12;
	stateWaitForTimeout[7] = true;
	stateAllowImageChange[7] = false;
	stateTransitionOnTimeout[7] = "CheckChamber";

	stateName[8] = "Opening";
	stateSequence[8] = "OpenCylinder";
	stateLoadedFlag[8] = "NotLoaded"; // doesn't seem to work
	stateTimeoutValue[8] = 0.1;
	stateAllowImageChange[8] = false;
	stateTransitionOnTimeout[8] = "Opened";
	stateScript[8] = "onOpening";

	stateName[9] = "Opened";
	stateTransitionOnLoaded[9] = "ShellCheck";
	stateTransitionOnNoAmmo[9] = "Closing";

	stateName[10] = "Closing";
	stateSequence[10] = "CloseCylinder";
	stateTimeoutValue[10] = 0.1;
	stateAllowImageChange[10] = false;
	stateTransitionOnTimeout[10] = "CheckChamber";
	stateScript[10] = "onClosing";

	stateName[11] = "ShellCheck";
	stateTransitionOnNoAmmo[11] = "Closing";
	stateTransitionOnNotLoaded[11] = "Opened";
	stateTransitionOnLoaded[11] = "EjectShell";

	stateName[12] = "EjectShell";
	stateScript[12] = "onEjectShell";
	stateSequence[12] = "EjectShell";
	stateTimeoutValue[12] = 0.01;
	stateTransitionOnTimeout[12] = "ShellCheck";

	// doesn't use StopEject sequence right now!

	stateName[19] = "CheckTrigger";
	stateTransitionOnTriggerUp[19] = "PullHammer";

	stateName[20] = "CheckChamber";
	stateScript[20] = "onCheckChamber";
	stateTimeoutValue[20] = 0.01;
	stateTransitionOnTimeout[20] = "Empty";
};

function RevolverProps::onAdd(%this)
{
	%this.currSlot = 0;

	for (%i = 0; %i < 6; %i++)
		%this.slot[%i] = 0;
}

function RevolverImage::getDebugText(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%text = "<font:lucida console:20>";

	for (%i = 0; %i < %props.currSlot; %i++)
		%text = %text @ " ";

	%text = %text @ "\c3_\n";

	for (%i = 0; %i < 6; %i++)
	{
		switch (%props.slot[%i])
		{
			case 0: %text = %text @ "\c7o";
			case 1: %text = %text @ "\c6o";
			case 2: %text = %text @ "\c3o";
			default: %text = %text @ "\c0o";
		}
	}
}

function RevolverImage::onCheckChamber(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%obj.setImageLoaded(%slot, %props.slot[%props.currSlot] == 1);
}

function RevolverImage::onOpening(%this, %obj, %slot)
{
	%obj.playThread(2, "shiftLeft");
	serverPlay3D(RevolverOpenSound, %obj.getMuzzlePoint(%slot));
}

function RevolverImage::onClosing(%this, %obj, %slot)
{
	%obj.playThread(2, "shiftRight");
	serverPlay3D(RevolverCloseSound, %obj.getMuzzlePoint(%slot));
}

function RevolverImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	%props = %obj.getItemProps();

	if (!%state || %obj.getImageState(%slot) !$= "Opened")
		return 0;

	if (%trigger == 0)
	{
		if (%props.slot[%props.currSlot] == 0)
		{
			%sound = %this.insertSound[getRandom(%this.numInsertSound + 1)];
			serverPlay3D(%sound, %obj.getMuzzlePoint(%slot));
			%props.slot[%props.currSlot] = 1;
			%obj.playThread(2, "plant");
		}

		%props.currSlot = (%props.currSlot + 1) % 6;
	}
	else if (%trigger == 4)
	{
		%obj.setImageLoaded(%slot, true);
	}
}

function RevolverImage::onLight(%this, %obj, %slot)
{
	%state = %obj.getImageState(%slot);
	if (%state $= "Ready" || %state $= "Empty" || %state $= "opened")
	{
		%props = %obj.getItemProps();

		%obj.setImageAmmo(%slot, !%obj.getImageAmmo(%slot));
		%obj.setImageLoaded(%slot, false);

		return 1;
	}
	return 0;
}

function RevolverImage::onEmptyFire(%this, %obj, %slot)
{
	if (%obj.getState() $= "Dead")
		return;

	%props = %obj.getItemProps();
	%props.currSlot = (%props.currSlot + 1) % 6;
}

function RevolverImage::onFire(%this, %obj, %slot)
{
	if (%obj.getState() $= "Dead")
		return;

	Parent::onFire(%this, %obj, %slot);
	// %proj = new ScriptObject()
	// {
	// 	class = "ProjectileRayCast";
	// 	superClass = "TimedRayCast";
	// 	position = %obj.getMuzzlePoint(0);
	// 	velocity = vectorScale(%obj.getMuzzleVector(%slot), cf_muzzlevelocity_ms(251));
	// 	gravity = "0 0" SPC cf_bulletdrop_grams(15);
	// 	lifetime = 3;
	// 	mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;
	// 	exempt = %obj;
	// 	sourceObject = %obj;
	// 	sourceClient = %obj.client;
	// 	damage = 16;
	// 	damageType = $DamageType::Generic;
	// 	hitExplosion = GunProjectile;
	// };
	//
	// MissionCleanup.add(%proj);
	// %proj.fire();

	%props = %obj.getItemProps();
	%props.slot[%props.currSlot] = 2;
	%props.currSlot = (%props.currSlot + 1) % 6;

	%obj.playThread(2, "shiftLeft");
	%obj.playThread(3, "shiftRight");
}

function RevolverImage::onEjectShell(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.slot[%props.currSlot] != 0)
	{
		%obj.ejectShell(Bullet45Item, 0.5, %props.slot[%props.currSlot] == 2);
		%props.slot[%props.currSlot] = 0;
	}

	%props.currSlot = (%props.currSlot + 1) % 6;
	%obj.setImageLoaded(%slot, %props.slot[%props.currSlot] != 0);
}

function RevolverImage::damage(%this, %obj, %col, %position, %normal)
{
	if (%col.getRegion(%position, true) $= "head")
	{
		%damage = 75;
		%damageType = $DamageType::RevolverHeadshot;
	}
	else
	{
		%damage = 25;
		%damageType = $DamageType::Revolver;
	}

	if (!$NoCrouchDamageBonus && %col.isCrouched())
		%damage /= 2;

	%col.damage(%obj, %position, %damage, %damageType);
}

function RevolverImage::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	return "How the hell do you expect me to make gun help for a /revolver/?! Jesus Christ.";
}

function Player::revolverInput(%this, %x, %y, %z)
{
	%props = %this.getItemProps();

	if (%x == 0 && %y == -1 && %z == 0)
	{
		%props.currSlot = (%props.currSlot + 1) % 6;
		%playSound = true;
		%playThread = "rotCW";
	}
	else if (%x == 0 && %y == 1 && %z == 0)
	{
		%props.currSlot = (6 + (%props.currSlot - 1)) % 6;
		%playSound = true;
		%playThread = "rotCCW";
	}

	%state = %this.getImageState(0);

	if (%state $= "Ready" || %state $= "Empty")
		%this.setImageLoaded(0, %props.slot[%props.currSlot] == 1);

	if (%playSound && $Sim::Time - %this.lastRevolverSpinSound > 0.07)
	{
		serverPlay3D("RevolverSpinSound" @ getRandom(1, 5), %this.getMuzzlePoint(0));
		%this.lastRevolverSpinSound = $Sim::Time;
	}

	if (%playThread !$= "" && $Sim::Time - %this.lastRevolverSpinThread > 0.175)
	{
		%this.playThread(2, %playThread);
		%this.lastRevolverSpinThread = $Sim::Time;
	}
}

package RevolverInputPackage
{
	function serverCmdShiftBrick(%client, %x, %y, %z)
	{
		%player = %client.player;

		if (!isObject(%player))
			return Parent::serverCmdShiftBrick(%client, %x, %y, %z);

		if (!%player.getMountedImage(0).isRevolver)
			return Parent::serverCmdShiftBrick(%client, %x, %y, %z);

		%player.revolverInput(%x, %y, %z);
	}

	function serverCmdSuperShiftBrick(%client, %x, %y, %z)
	{
		%player = %client.player;

		if (!isObject(%player))
			return Parent::serverCmdSuperShiftBrick(%client, %x, %y, %z);

		if (!%player.getMountedImage(0).isRevolver)
			return Parent::serverCmdSuperShiftBrick(%client, %x, %y, %z);

		%player.revolverInput(%x, %y, %z);
	}
};

activatePackage("RevolverInputPackage");
