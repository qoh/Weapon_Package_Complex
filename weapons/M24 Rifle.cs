datablock AudioProfile(SniperFire01Sound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/sniper/Sniper_Fire01.wav";
    description = AudioClose3D;
    preload = true;
};

datablock AudioProfile(SniperFire02Sound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/sniper/Sniper_Fire02.wav";
    description = AudioClose3D;
    preload = true;
};

datablock AudioProfile(SniperFire03Sound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/sniper/Sniper_Fire03.wav";
    description = AudioClose3D;
    preload = true;
};

$SniperFireSound0 = SniperFire01Sound;
$SniperFireSound1 = SniperFire02Sound;
$SniperFireSound2 = SniperFire03Sound;

datablock AudioProfile(SniperReloadSound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/sniper/Sniper_Reload.wav";
    description = AudioClose3D;
    preload = true;
};

datablock AudioProfile(RevolverClickSound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/revolver_click.wav";
    description = AudioClose3D;
    preload = true;
};

datablock ItemData(M24RifleItem)
{
    shapeFile = "Add-Ons/Weapon_Gun/pistol.dts";
    mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

    canDrop = 1;
    armReady = 1;
    uiName = "M24 Rifle";
    image = M24RifleImage;
};

datablock ShapeBaseImageData(M24RifleImage)
{
    shapeFile = "Add-Ons/Weapon_Gun/pistol.dts";
    item = M24RifleItem;

    stateName[0] = "Activate";
    stateTimeoutValue[0] = 0.5;
    stateTransitionOnTimeout[0] = "CheckLoaded";

    stateName[1] = "CheckLoaded";
    stateTransitionOnLoaded[1] = "Ready";
    stateTransitionOnNotLoaded[1] = "Empty";

    stateName[2] = "Empty";
    stateTransitionOnAmmo[2] = "Cycle";
    stateTransitionOnTriggerDown[2] = "EmptyFire";

    stateName[3] = "EmptyFire";
    stateScript[3] = "onEmptyFire";
    stateTimeoutValue[3] = 0.25;
    stateWaitForTimeout[3] = true;
    stateTransitionOnTriggerUp[3] = "Empty";

    stateName[4] = "Cycle";
    stateScript[4] = "onCycle";
    stateTimeoutValue[4] = 0.65;
    stateWaitForTimeout[4] = true;
    stateTransitionOnNoAmmo[4] = "StopCycle";

    stateName[5] = "StopCycle";
    stateTransitionOnLoaded[5] = "Ready";
    stateTransitionOnNotLoaded[5] = "Empty";

    stateName[6] = "Ready";
    stateTransitionOnAmmo[6] = "Cycle";
    stateTransitionOnTriggerDown[6] = "Fire";

    stateName[7] = "Fire";
    stateFire[7] = true;
    stateScript[7] = "onFire";
    stateTimeoutValue[7] = 0.25;
    stateWaitForTimeout[7] = true;
    stateTransitionOnTriggerUp[7] = "Empty";
};

function M24RifleImage::getDebugText(%this, %obj, %slot)
{
    %props = %obj.getItemProps(%this, %slot);

    %text = "\c6chamber=" @ (%props.chamber == 1 ? "ready" : (%props.chamber == -1 ? "spent" : "empty"));
    %text = %text SPC (isObject(%props.magazine) ? "mag=" @ %props.magazine.count : "no-mag");
    %text = %text SPC "state=" @ %obj.getImageState(%slot);

    return %text;
}

function M24RifleImage::onMount(%this, %obj, %slot)
{
    %obj.debugWeapon();

    %props = %obj.getItemProps(%this, %slot);

    if (%props.chamber $= "")
        %props.chamber = 0;

    %obj.setImageLoaded(%slot, %props.chamber == 1);
    %obj.setImageAmmo(%slot, 0);
}

function M24RifleImage::onLight(%this, %obj, %slot)
{
    %props = %obj.getItemProps(%this, %slot);

    if (isObject(%props.magazine))
    {
        // remove magazine
        %props.magazine.delete();
    }
    else
    {
        // insert magazine
        %props.magazine = new ScriptObject()
        {
            count = 5;
        };
    }

    return 1;
}

function M24RifleImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    %props = %obj.getItemProps(%this, %slot);

    if (%trigger == 4 && %state)
    {
        // talk("manual chamber");
        //
        // if (!%props.loaded && %props.magazine.count >= 1)
        // {
        //     %props.ch
        //     %props.magazine.count--;
        //
        //     %obj.setImageLoaded(%slot, 1);
        // }

        if (%obj.getImageState(%slot) !$= "Cycle" && !%obj.getImageAmmo(%slot))
            %obj.setImageAmmo(%slot, 1);

        return 1;
    }

    return 0;
}

function M24RifleImage::onCycle(%this, %obj, %slot)
{
    serverPlay3D(SniperReloadSound, %obj.getMuzzlePoint(%slot));

    %props = %obj.getItemProps(%this, %slot);

    if (%props.chamber != 0)
    {
        talk("eject shell");
        %props.chamber = 0;
    }

    if (%props.magazine.count >= 1)
    {
        talk("insert shell");

        %props.chamber = 1;
        %props.magazine.count--;

        %obj.setImageLoaded(%slot, 1);
    }
    else
    {
        talk("mag empty");
        %obj.setImageLoaded(%slot, 0);
    }

    %obj.setImageAmmo(%slot, 0);
}

function M24RifleImage::onEmptyFire(%this, %obj, %slot)
{
    serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function M24RifleImage::onFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps(%this, %slot);

    if (%props.chamber != 1)
        return; // shouldn't happen

    // fire bullet
    serverPlay3D($SniperFireSound[getRandom(2)], %obj.getMuzzlePoint(%slot));
    talk("fire");

    %props.chamber = -1;
}
