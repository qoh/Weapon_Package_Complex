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
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/m1garand.dts";

    item = M1GarandItem;
    armReady = true;

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
    stateTimeoutValue[4] = 0.15;
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
    stateTransitionOnTriggerUp[5] = "Empty";

    stateName[6] = "Reload";
    stateSound[6] = ComplexBoltSound;
    stateSequence[6] = "eject";
    stateScript[6] = "onReload";
    stateTimeoutValue[6] = 0.2;
    stateAllowImageChange[6] = false;
    stateTransitionOnTimeout[6] = "Empty";
};

function M1GarandImage::onMount(%this, %obj, %slot)
{
    %props = %obj.getItemProps();
    %obj.setImageLoaded(%slot, %props.loaded);
}

function M1GarandImage::onEmptyFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (%props.magazine.count >= 1)
    {
        %obj.setImageAmmo(%slot, true);
        return;
    }

    %obj.playThread(2, "shiftLeft");
    %obj.playThread(3, "shiftRight");

    serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function M1GarandImage::onFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    // fire bullet
    %proj = new ScriptObject()
	{
		class = "ProjectileRayCast";
		superClass = "TimedRayCast";
		position = %obj.getMuzzlePoint(0);
		velocity = vectorScale(%obj.getMuzzleVector(%slot), cf_muzzlevelocity_ms(853));
		gravity = "0 0" SPC cf_bulletdrop_grams(12);
		lifetime = 3;
		mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;
		exempt = %obj;
		sourceObject = %obj;
		sourceClient = %obj.client;
		damage = 40;
		damageType = $DamageType::Generic;
		hitExplosion = GunProjectile;
    };

    MissionCleanup.add(%proj);
    %proj.fire();

    if (%props.magazine.count >= 1)
    {
        %props.magazine.count--;
        serverPlay3D(M1GarandFireSound, %obj.getMuzzlePoint(%slot));
    }
    else
    {
        %props.loaded = false;
        serverPlay3D(M1GarandFireLastSound, %obj.getMuzzlePoint(%slot));
        %obj.setImageLoaded(%slot, false);
    }
}

function M1GarandImage::onReload(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (%props.loaded)
    {
        %props.loaded = false;
        // eject shell
    }

    if (%props.magazine.count >= 1)
    {
        %props.magazine.count--;
        %props.loaded = true;

        %obj.playThread(2, "plant");
    }

    %obj.setImageLoaded(%slot, %props.loaded);
    %obj.setImageAmmo(%slot, false);
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
