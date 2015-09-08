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
};

datablock ShapeBaseImageData(Remington870Image)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington870.dts";

    item = Remington870Item;
    armReady = true;

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
    stateTransitionOnTimeout[2] = "Empty";

    stateName[3] = "Ready";
    stateSequence[1] = "root";
    stateTransitionOnTriggerDown[3] = "Fire";
    stateTransitionOnNotLoaded[3] = "Empty";
    stateTransitionOnAmmo[3] = "Pump";

    stateName[4] = "Fire";
    stateFire[4] = true;
    stateSound[4] = Remington870FireSound;
    stateScript[4] = "onFire";
    stateSequence[4] = "Fire";
    stateEmitter[4] = advSmallBulletFireEmitter;
	stateEmitterTime[4] = 0.05;
	stateEmitterNode[4] = "muzzleNode";
    stateTimeoutValue[4] = 0.12;
    stateTransitionOnTimeout[4] = "Smoke";

    stateName[5] = "Smoke";
    stateEmitter[5] = advSmallBulletSmokeEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
    stateTimeoutValue[5] = 0.1;
    stateTransitionOnTriggerUp[5] = "Empty";

    stateName[6] = "Pump";
    stateSound[6] = Remington870PumpSound;
    stateScript[6] = "onPump";
    stateSequence[6] = "Pump";
    stateTimeoutValue[6] = 0.2;
    stateWaitForTimeout[6] = true;
    stateTransitionOnNoAmmo[6] = "Empty";
};

function Remington870Image::onMount(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (%props.chamber $= "")
        %props.chamber = 0;

    %obj.setImageLoaded(%slot, %props.chamber == 1);
}

function Remington870Image::onPump(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (%props.chamber != 0)
    {
        // eject shell
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
}

function Remington870Image::onEmptyFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (%props.count >= 1)
    {
        %obj.setImageAmmo(%slot, true);
        return;
    }

    %obj.playThread(2, "shiftLeft");
    %obj.playThread(3, "shiftRight");

    serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function Remington870Image::onFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    %obj.playThread(2, "shiftLeft");
    %obj.playThread(3, "shiftRight");

    %props.chamber = 2;
    %obj.setImageLoaded(%slot, false);
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

    if (%props.count < 4)
    {
        %props.count++;
        %obj.playThread(2, "plant");
        serverPlay3D(Remington870InsertSound, %obj.getMuzzlePoint(%slot));
        return 1;
    }

    return 0;
}

function Remington870Image::getDebugText(%this, %obj, %slot)
{
    %props = %obj.getItemProps();
    %text = "\c6" @ (%props.chamber == 2 ? "spent" : (%props.chamber == 1 ? "loaded" : "empty")) @ " \c3";

    for (%i = 0; %i < %props.count; %i++) %text = %text @ "o";
    %text = %text @ "\c7";
    for (0; %i < 4; %i++) %text = %text @ "o";
    %text = %text @ "\n";

    return %text;
}
