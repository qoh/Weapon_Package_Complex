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

new ScriptObject(RevolverInsertSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/bullet_insert.wav";
	file[main, 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/bullet_insert2.wav";
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

	shellCollisionThreshold = 2;
	shellCollisionSFX = WeaponSoftImpactSFX;

	bulletType = ".357";

	itemPropsClass = "RevolverProps";
};

datablock ShapeBaseImageData(RevolverImage)
{
	className = "TimeSliceRayWeapon";
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/S&W_Revolver.dts";

	fireMuzzleVelocity = cf_muzzlevelocity_ms(396.24);
	fireVelInheritFactor = 0.25;
	fireGravity = cf_bulletdrop_grams(10);
	fireSpread = 0.4;
	fireHitExplosion = GunProjectile;
	fireHitOtherSFX = ComplexDefaultImpactBulletSFX;
	fireRicSFX = ComplexRicSFX;
	fireNearMissSFX = ComplexNearMissSFX;

	item = RevolverItem;
	armReady = true;
	isRevolver = true;
	speedScale = 0.85;

	insertSFX = revolverInsertSFX;

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

// function RevolverImage::getDebugText(%this, %obj, %slot)
// {
// 	%props = %obj.getItemProps();
// 	%text = "<font:lucida console:20>";
// 	%state = %obj.getImageState(%slot);
// 	if (%state $= "Opened" || %state $= "EjectShell")
// 	{
// 		for (%i = 0; %i < %props.currSlot; %i++)
// 			%text = %text @ " ";

// 		%text = %text @ "\c3_\n";
// 		for (%i = 0; %i < 6; %i++)
// 		{
// 			switch (%props.slot[%i])
// 			{
// 				case 0: %text = %text @ "\c7o";
// 				case 1: %text = %text @ "\c6o";
// 				case 2: %text = %text @ "\c3o";
// 				default: %text = %text @ "\c0o";
// 			}
// 		}
// 	}
// 	%text = %text SPC "<font:Arial:20>\c6.357\c3:" SPC %obj.bulletCount[%this.item.bulletType];
// }

function RevolverImage::onMount(%this, %obj, %slot)
{
	if (%obj.bulletCount[%this.item.bulletType] $= "")
		%obj.bulletCount[%this.item.bulletType] = 0;
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
	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function RevolverImage::onClosing(%this, %obj, %slot)
{
	%obj.playThread(2, "shiftRight");
	serverPlay3D(RevolverCloseSound, %obj.getMuzzlePoint(%slot));
	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function RevolverImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	%props = %obj.getItemProps();

	if (%obj.getImageState(%slot) !$= "Opened")
		return 0;

	if (%trigger == 0)
	{
		if(%state)
			%obj.revolverAmmoLoop(%this);
		else
			cancel(%obj.revolverAmmoLoop);
	}
	else if (%trigger == 4 && %state)
	{
		%loaded = false;
		for (%i = 0; %i < 6; %i++)
		{
			if (%props.slot[%i] != 0)
			{
				%loaded = true;
				break;
			}
		}
		if (%loaded)
			%obj.setImageLoaded(%slot, true);
	}
}
function Player::revolverAmmoLoop(%this, %image) //This lets you hold left click to fill up the barrel.
{
	cancel(%this.revolverAmmoLoop);
	if(%this.getMountedImage(0) != %image)
		return;

	%state = %this.getImageState(0);
	if(%state !$= "Opened")
		return;

	if (%this.getMountedImage(0).InsertShell(%this))
		%this.playThread(2, "plant");
	%this.revolverAmmoLoop = %this.schedule(150, revolverAmmoLoop, %image);
}

function RevolverImage::onLight(%this, %obj, %slot)
{
	%state = %obj.getImageState(%slot);
	if (%state $= "Ready" || %state $= "Empty" || %state $= "opened")
	{
		%props = %obj.getItemProps();

		%obj.setImageAmmo(%slot, !%obj.getImageAmmo(%slot));
		%obj.setImageLoaded(%slot, false);
	}
	return 1; //We kinda HAVE to do this so you don't turn on light while reloading. You still can't turn on light when in Ready state anyway.
}

function RevolverImage::onEmptyFire(%this, %obj, %slot)
{
	if (%obj.getState() $= "Dead")
		return;

	%props = %obj.getItemProps();
	%props.currSlot = (%props.currSlot + 1) % 6;
	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function RevolverImage::onFire(%this, %obj, %slot)
{
	if (%obj.getState() $= "Dead")
		return;

	if(!%obj.suiciding) //If we're not firing through suicide
		Parent::onFire(%this, %obj, %slot);
	%obj.suiciding = ""; //reset the var, just in case
	%props = %obj.getItemProps();
	%props.slot[%props.currSlot] = 2;
	%props.currSlot = (%props.currSlot + 1) % 6;

	%obj.playThread(2, "shiftLeft");
	%obj.playThread(3, "shiftRight");

	%obj.applyComplexKnockback(1.75);
	%obj.applyComplexScreenshake(1);

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function RevolverImage::onSuicide(%this, %obj, %slot)
{
	%image = %obj.getMountedImage(%slot);
	%state = %obj.getImageState(%slot);
	if(%state !$= "Ready" && %state !$= "Empty")
	{
		return 1;
	}
	%props = %obj.getItemProps();
	if(%props.slot[%props.currSlot] != 1) //Either spent or empty
	{
		%obj.setImageTrigger(%slot, 1);
		%obj.playThread(2, plant);
		serverPlay3d(RevolverClickSound, %obj.getHackPosition());
	}
	else
	{
		//What you're about to see below is probably the ugliest thing I ever coded. ~Jack Noir
		%obj.playThread(2, shiftRight);
		%obj.playThread(3, shiftLeft);
		%obj.applyComplexKnockback(5);
		%props.slot[%props.currSlot] = 2;
		serverplay3d(revolverFireSound, %obj.getMuzzlePoint(%slot));
		%obj.suiciding = 1;
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

function RevolverImage::onEjectShell(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.slot[%props.currSlot] != 0)
	{
		%obj.ejectShell(Bullet357Item, 0.5, %props.slot[%props.currSlot] == 2);
		%props.slot[%props.currSlot] = 0;
	}
	%loaded = false;
	for (%i = 0; %i < 6; %i++)
	{
		if (%props.slot[%i] != 0)
		{
			%loaded = true;
			break;
		}
	}
	%props.currSlot = (%props.currSlot + 1) % 6;
	%obj.setImageLoaded(%slot, %loaded);
	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function RevolverImage::damage(%this, %obj, %col, %position, %normal)
{
	if (%col.getRegion(%position, true) $= "head")
	{
		ComplexHeadshotSFX.playFrom(%position, %col);

		%damage = 75;
		%damageType = $DamageType::RevolverHeadshot;
	}
	else
	{
		ComplexFleshImpactBulletSFX.playFrom(%position, %col);

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

	return "Gun Help for Revolver is work in progress.";
}

function RevolverImage::getDetailedGunHelp(%this, %obj, %slot, %hidden)
{
	%props = %obj.getItemProps();

	if (%obj.bulletCount[%this.item.bulletType] != -1)
		%text = "<just:center><font:Arial:20>\c6.357\c3:" SPC %obj.bulletCount[%this.item.bulletType];

	%kt_lmb = "Primary";
	%kt_rmb = "Jet    ";
	%kt_r   = "Light  ";

	%at_fire     = "Attempt Fire";
	%at_magazine = "Open Chamber";

	%ac_fire     = "\c6";
	%ac_magazine = "\c6";

	//Optional
	%kt_sl	= "ShiftL.";
	%kt_sr	= "ShiftR.";

	%ac_sl	= "\c7";
	%ac_sr	= "\c7";

	%at_sl	= "Spin Left";
	%at_sr	= "Spin Right";

	%state = %obj.getImageState(%slot);
	if (%state $= "Opening" || %state $= "Opened" || %state $= "ShellCheck" || %state $= "EjectShell")
	{
		%at_magazine = "Close Chamber";
		%at_fire = "Insert Shell ";
		%at_action = "Eject Shells ";
		%ac_action = "\c7";
		%full = true;
		%spent = false;
		for (%i = 0; %i < 6; %i++)
		{
			if (%props.slot[%i] == 2)
				%spent = true;
			if (%props.slot[%i] == 0)
				%full = false;
		}
		if (%spent)
			%ac_action = "\c6";
		if (%full)
			%ac_fire = "\c7";
		if (%obj.bulletCount[%this.item.bulletType] <= 0 && %obj.bulletCount[%this.item.bulletType] != -1)
			%ac_fire = "\c7";

		if (!%hidden)
		{
			%text = %text @ "<just:right><font:consolas:16>";
			%text = %text @ %ac_fire     @ %at_fire     @ "   " @ %kt_lmb @ " \n";
			%text = %text @ %ac_magazine @ %at_magazine @ "   " @ %kt_r   @ " \n";
			%text = %text @ %ac_action   @ %at_action   @ "   " @ %kt_rmb @ " \n";
		}

		%color[0] = "\c8";
		%color[1] = "\c6";
		%color[2] = "\c3";

		for ( %i = 0; %i <= 5; %i++ ) {
			%start[%i] = %color[%props.slot[%i]];

			if ( %i == %props.currSlot ) {
				if (%props.slot[%i] == 0)
					%start[%i] = "\c7";
				%start[%i] = "<shadow:0:3>" @ %start[%i];
				%end[%i] = "<shadow:0:0>";
			}
		}
		%text = %text @ "<just:center><font:Arial:35>";
		%text = %text @ %start[1] @		"O" @ %end[1] @ " " @ %start[2] @ "O" @ %end[2] @ "\n";
		%text = %text @ %start[0] @	"O" @ %end[0] @   "     " @ %start[3] @ "O" @ %end[3] @ "\n";
		%text = %text @ %start[5] @		"O" @ %end[5] @ " " @ %start[4] @ "O" @ %end[4];
		return %text;
	}
	if (!%hidden)
	{
		%text = %text @"<just:right><font:consolas:16>";
		%text = %text @ %ac_fire     @ %at_fire     @ "   " @ %kt_lmb @ " \n";
		%text = %text @ %ac_magazine @ %at_magazine @ "   " @ %kt_r   @ " \n";
		%text = %text @ %ac_sl @ %at_sl @ "   " @ %kt_sl	@ " \n";
		%text = %text @ %ac_sr @ %at_sr @ "   " @ %kt_sr	@ " \n";
	}
	return %text;
}


function RevolverImage::InsertShell(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (%obj.bulletCount[%this.item.bulletType] < 1 && %obj.bulletCount[%this.item.bulletType] != -1)
		return 0;

	%empty = false;
	for (%i = 0; %i < 6; %i++)
	{
		if (%props.slot[%i] == 0)
			%empty = true;
	}
	if (!%empty) return 0;

	if (%props.slot[%props.currSlot] == 0)
	{
		%obj.getMountedImage(0).insertSFX.play(%obj.getMuzzlePoint(0));
		%props.slot[%props.currSlot] = 1;
		if (%obj.bulletCount[%this.item.bulletType] != -1)
			%obj.bulletCount[%this.item.bulletType]--;
	}
	%props.currSlot = (%props.currSlot + 1) % 6;
	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
	return 1;
}

function Player::revolverInput(%this, %x, %y, %z, %rot)
{
	%props = %this.getItemProps();
	%state = %this.getImageState(0);
	if (%x == 1 && %y == 0 && %z == 0)
	{
		%loaded = false;
		for (%i = 0; %i < 6; %i++)
		{
			if (%props.slot[%i] != 0)
			{
				%loaded = true;
				break;
			}
		}
		if (%loaded)
			%this.setImageLoaded(%slot, true);
	}
	else if (%x == 0 && %y == -1 && %z == 0)
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
	else if (%x == 0 && %y == 0 && %z == -3 && %this.getImageState(0) $= "Opened")
	{
		if($Sim::Time - %this.lastBulletInsert < 0.1)
			return;
		%this.lastBulletInsert = $Sim::Time;
		if (%this.getMountedImage(0).InsertShell(%this))
			%playThread = "plant";
	}
	else if (%rot)
	{
		if (%props.slot[%props.currSlot] != 0 && %state $= "Opened")
		{
			if (%props.slot[%props.currSlot] == 1)
			{
				if (%this.bulletCount[%this.getMountedImage(0).item.bulletType] != -1)
					%this.bulletCount[%this.getMountedImage(0).item.bulletType]++;
				%this.getMountedImage(0).insertSFX.play(%this.getMuzzlePoint(0));
			}
			else
				%this.ejectShell(Bullet357Item, 0.5, 1);
			%props.slot[%props.currSlot] = 0;
		}
	}

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
	if (isObject(%this.client))
		%this.client.updateDetailedGunHelp();
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

	function serverCmdRotateBrick(%client, %rot)
	{
		%player = %client.player;

		if (!isObject(%player))
			return Parent::serverCmdRotateBrick(%client, %rot);

		if (!%player.getMountedImage(0).isRevolver)
			return Parent::serverCmdRotateBrick(%client, %rot);

		%player.revolverInput(0,0,0,%rot);
	}
};

activatePackage("RevolverInputPackage");
