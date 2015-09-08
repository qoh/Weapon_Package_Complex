datablock AudioProfile(ColtWalkerCockSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds_west/coltwalker/cock.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(ColtWalkerFireSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds_west/coltwalker/fire.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(ColtWalkerInsertSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds_west/coltwalker/insert.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(ColtWalkerLeverSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds_west/coltwalker/lever.wav";
	description = AudioClose3d;
	preload = true;
};

datablock ItemData(ColtWalkerItem)
{
    shapeFile = "Add-Ons/Weapon_Gun/pistol.dts";
    uiName = "Colt Walker";

    canDrop = true;
    image = ColtWalkerImage;

    mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

    itemPropsClass = "RevolverProps";
};

datablock ShapeBaseImageData(ColtWalkerImage)
{
    shapeFile = "Add-Ons/Weapon_Gun/pistol.dts";

    item = ColtWalkerItem;
    armReady = true;
	isRevolver = true;

	insertSound0 = ColtWalkerInsertSound;
	numInsertSound = 1;

    stateName[0] = "Activate";
    stateTimeoutValue[0] = 0.15;
    stateAllowImageChange[0] = false;
    stateTransitionOnTimeout[0] = "CheckChamber";

    stateName[2] = "Empty";
    stateTransitionOnLoaded[2] = "Ready";
    stateTransitionOnAmmo[2] = "Opening";
    stateTransitionOnTriggerDown[2] = "EmptyFire";

    stateName[3] = "EmptyFire";
	stateSound[3] = RevolverClickSound;
    stateScript[3] = "onEmptyFire";
    stateTimeoutValue[3] = 0.13;
    stateWaitForTimeout[3] = true;
    stateAllowImageChange[3] = false;
    stateTransitionOnTriggerUp[3] = "CheckChamber";

    stateName[4] = "Ready";
    stateTransitionOnNotLoaded[4] = "Empty";
    stateTransitionOnAmmo[4] = "Opening";
    stateTransitionOnTriggerDown[4] = "Fire";

    stateName[5] = "Fire";
    stateFire[5] = true;
	stateSound[5] = ColtWalkerFireSound;
    stateScript[5] = "onFire";
    stateEmitter[5] = advBigBulletFireEmitter;
    stateEmitterTime[5] = 0.1;
    stateEmitterNode[5] = "muzzleNode";
    stateTimeoutValue[5] = 0.3;
    stateAllowImageChange[5] = false;
    stateTransitionOnTimeout[5] = "Smoke";

    stateName[6] = "Smoke";
    stateEmitter[6] = gunSmokeEmitter;
    stateEmitterTime[6] = 0.5;
    stateEmitterNode[6] = "muzzleNode";
    stateTimeoutValue[6] = 0.5;
    stateWaitForTimeout[6] = true;
    stateAllowImageChange[6] = false;
    stateTransitionOnTriggerUp[6] = "HammerUp";

	stateName[7] = "HammerUp";
	stateTransitionOnTriggerDown[7] = "Cock";

    stateName[8] = "Cock";
    stateSound[8] = ColtWalkerCockSound;
    stateTimeoutValue[8] = 0.3;
    stateAllowImageChange[8] = false;
	stateWaitForTimeout[8] = true;
    stateTransitionOnTriggerUp[8] = "CheckChamber";

    stateName[9] = "Opening";
    stateLoadedFlag[9] = "NotLoaded"; // doesn't seem to work
    stateTimeoutValue[9] = 0.25;
    stateAllowImageChange[9] = false;
    stateTransitionOnTimeout[9] = "Opened";

    stateName[10] = "Opened";
    stateTransitionOnLoaded[10] = "ShellCheck";
    stateTransitionOnNoAmmo[10] = "Closing";

    stateName[11] = "Closing";
    stateTimeoutValue[11] = 0.25;
    stateAllowImageChange[11] = false;
    stateTransitionOnTimeout[11] = "HammerUp";

    stateName[12] = "ShellCheck";
    stateTransitionOnNoAmmo[12] = "Closing";
    stateTransitionOnNotLoaded[12] = "Opened";
    stateTransitionOnLoaded[12] = "EjectShell";

    stateName[13] = "EjectShell";
    stateScript[13] = "onEjectShell";
    stateTimeoutValue[13] = 0.1;
    stateTransitionOnTimeout[13] = "ShellCheck";

    stateName[20] = "CheckChamber";
    stateScript[20] = "onCheckChamber";
    stateTimeoutValue[20] = 0.01;
    stateTransitionOnTimeout[20] = "Empty";
};

function ColtWalkerImage::getDebugText(%this, %obj, %slot)
{
    return RevolverImage::getDebugText(%this, %obj, %slot);
}

function ColtWalkerImage::onCheckChamber(%this, %obj, %slot)
{
    RevolverImage::onCheckChamber(%this, %obj, %slot);
}

function ColtWalkerImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    RevolverImage::onTrigger(%this, %obj, %slot, %trigger, %state);
}

function ColtWalkerImage::onLight(%this, %obj, %slot)
{
    RevolverImage::onLight(%this, %obj, %slot);
}

function ColtWalkerImage::onEmptyFire(%this, %obj, %slot)
{
    RevolverImage::onFire(%this, %obj, %slot);
}

function ColtWalkerImage::onFire(%this, %obj, %slot)
{
    RevolverImage::onFire(%this, %obj, %slot);
}

function ColtWalkerImage::onEjectShell(%this, %obj, %slot)
{
    RevolverImage::onEjectShell(%this, %obj, %slot);
}