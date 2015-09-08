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

datablock ItemData(M24RifleItem)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington700_sniper.dts";
    mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

    canDrop = 1;
    uiName = "M24 Rifle";
    image = M24RifleImage;

    itemPropsClass = "M24RifleProps";
};

function M24RifleProps::onAdd(%this)
{
    %this.chamber = 0;
}

datablock ShapeBaseImageData(M24RifleImage)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington700_sniper.dts";
    item = M24RifleItem;
    armReady = 1;

    stateName[0] = "Ready";
    stateTransitionOnTriggerDown[0] = "Cycle";

    stateName[1] = "Cycle";
    stateScript[1] = "onCycle";
    stateTimeoutValue[1] = 0.65;
    stateAllowImageChange[1] = false;
    stateWaitForTimeout[1] = true;
    stateTransitionOnTriggerUp[1] = "Ready";
};

function M24RifleImage::getDebugText(%this, %obj, %slot)
{
    %props = %obj.getItemProps();
    return "\c6chamber=" @ (%props.chamber == 1 ? "ready" : (%props.chamber == -1 ? "spent" : "empty")) @ " mag=" @ (isObject(%props.magazine) ? %props.magazine.count : "none") @ "\n";
}

function M24RifleImage::onLight(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (isObject(%props.magazine))
    {
        %obj.giveMagazineProps(%props.magazine);
		%props.magazine = "";
    }
    else
    {
        %props.magazine = %obj.takeMagazineProps(%this.item);

        if (!isObject(%props.magazine) && isObject(%obj.client))
            messageClient(%obj.client, '', '\c6You don\'t have any magazines for this weapon.');
    }

    return 1;
}

function M24RifleImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    if (%trigger == 4 && %state && %obj.getImageState(%slot) $= "Ready")
        %obj.mountImage(M24RifleScopeImage, %slot);

    return 0;
}

function M24RifleImage::onCycle(%this, %obj, %slot)
{
    serverPlay3D(SniperReloadSound, %obj.getMuzzlePoint(%slot));

    %props = %obj.getItemProps();

    if (%props.chamber != 0)
    {
        %obj.playThread(2, "shiftRight");
        %props.chamber = 0;
    }

    if (%props.magazine.count >= 1)
    {
        %obj.playThread(3, "shiftLeft");

        %props.chamber = 1;
        %props.magazine.count--;
    }
    else if (isObject(%obj.client))
        messageClient(%client, '', "\c6There are no more cartridges left to load. Insert a magazine.");
}

datablock ShapeBaseImageData(M24RifleScopeImage)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington700_sniper_scoped_v3.dts";
    item = M24RifleItem;
    armReady = 1;

    eyeOffset = "0 0.175 -0.8";

    stateName[0] = "Activate";
    stateAllowImageChange[0] = false;
    stateTimeoutValue[0] = 0.3;
    stateTransitionOnTimeout[0] = "CheckLoaded";

    stateName[1] = "Deactivate";
    stateAllowImageChange[1] = false;
    stateTimeoutValue[1] = 0.3;
    stateTransitionOnTimeout[1] = "DeactivateDone";

    stateName[2] = "DeactivateDone";
    stateScript[2] = "onDeactivateDone";

    stateName[3] = "CheckLoaded";
    stateAllowImageChange[3] = false;
    stateTransitionOnLoaded[3] = "Ready";
    stateTransitionOnNotLoaded[3] = "Empty";

    stateName[4] = "Empty";
    stateTransitionOnAmmo[4] = "Deactivate";
    stateTransitionOnTriggerDown[4] = "EmptyFire";

    stateName[5] = "EmptyFire";
    stateScript[5] = "onEmptyFire";
    stateAllowImageChange[5] = false;
    stateTimeoutValue[5] = 0.25;
    stateWaitForTimeout[5] = true;
    stateTransitionOnTriggerUp[5] = "Empty";

    stateName[6] = "Ready";
    stateTransitionOnAmmo[6] = "Deactivate";
    stateTransitionOnTriggerDown[6] = "Fire";

    stateName[7] = "Fire";
    stateFire[7] = true;
    stateScript[7] = "onFire";
    stateAllowImageChange[7] = false;
    stateTimeoutValue[7] = 0.25;
    stateWaitForTimeout[7] = true;
    stateTransitionOnTriggerUp[7] = "Empty";
};

function M24RifleScopeImage::getDebugText(%this, %obj, %slot)
{
    return M24RifleImage::getDebugText(%this, %obj, %slot);
}

function M24RifleScopeImage::onMount(%this, %obj, %slot)
{
    %props = %obj.getItemProps();
    %obj.setImageLoaded(%slot, %props.chamber == 1);
}

function M24RifleScopeImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    if (%trigger == 4 && %state)
        %obj.setImageAmmo(%slot, 1);

    return 0;
}

function M24RifleScopeImage::onDeactivateDone(%this, %obj, %slot)
{
    %obj.mountImage(M24RifleImage, %slot, false);
}

function M24RifleScopeImage::onEmptyFire(%this, %obj, %slot)
{
    serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function M24RifleScopeImage::onFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (%props.chamber != 1)
        return; // shouldn't happen

    %obj.playThread(2, "plant");

    // fire bullet
    %proj = new ScriptObject() {
      class = "ProjectileRayCast";
      superClass = "TimedRayCast";
      position = %obj.getMuzzlePoint(0);
      velocity = vectorScale(%obj.getMuzzleVector(%slot), cf_muzzlevelocity_ms(790 * (3/2)));
      gravity = "0 0" SPC cf_bulletdrop_grams(10); // 7.62x51mm NATO
      lifetime = 3;
      mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;
      exempt = %obj;
      sourceObject = %obj;
      sourceClient = %obj.client;
      damage = 60;
      damageType = $DamageType::Generic;
      hitExplosion = GunProjectile;
    };

    MissionCleanup.add(%proj);
    %proj.fire();

    serverPlay3D($SniperFireSound[getRandom(2)], %obj.getMuzzlePoint(%slot));

    %props.chamber = -1;
}
