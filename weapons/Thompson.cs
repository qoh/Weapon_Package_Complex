datablock AudioProfile(ThompsonFireSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/thompson/fire.wav";
	description = AudioClose3d;
	preload = true;
};
datablock AudioProfile(ThompsonFireLastSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/thompson/fire_last.wav";
	description = AudioClose3d;
	preload = true;
};

datablock ItemData(ThompsonItem)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/thompson_mg.dts";
	iconName = "Add-Ons/Weapon_Package_Complex/assets/icons/thompson";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

    canDrop = 1;
    uiName = "Thompson SMG";
    image = ThompsonImage;

    itemPropsClass = "SimpleMagWeaponProps";
};

function ThompsonItem::onAdd(%this, %obj)
{
	Parent::onAdd(%this, %obj);

	if (!isObject(%obj.itemProps.magazine))
		%obj.hideNode("clip");
}

datablock ShapeBaseImageData(ThompsonImage)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/thompson_mg.dts";
    item = ThompsonItem;
    armReady = 1;

    stateName[0] = "Activate";
	stateSequence[0] = "activate";
    stateTimeoutValue[0] = 0.2;
    stateTransitionOnTimeout[0] = "CheckChamber";

    stateName[1] = "CheckChamber";
    stateTransitionOnLoaded[1] = "Ready";
    stateTransitionOnNotLoaded[1] = "Empty";

    stateName[2] = "Empty";
	stateSequence[2] = "noammo";
    stateTransitionOnLoaded[2] = "Ready";
    stateTransitionOnTriggerDown[2] = "EmptyFire";
	stateTransitionOnAmmo[2] = "PullBackSlide";

    stateName[3] = "EmptyFire";
    stateScript[3] = "onEmptyFire";
	stateSequence[3] = "emptyFire";
	stateTimeoutValue[3] = 0.13;
	stateWaitForTimeout[3] = true;
	stateAllowImageChange[3] = false;
    stateTransitionOnTriggerUp[3] = "Empty";

    stateName[4] = "Ready";
	stateSequence[4] = "root";
	stateTransitionOnAmmo[4] = "PullBackSlide";
    stateTransitionOnTriggerDown[4] = "Fire";

    stateName[5] = "Fire";
    stateFire[5] = true;
    stateScript[5] = "onFire";
	stateSequence[5] = "Fire";
    stateTimeoutValue[5] = 0.03;
	stateEmitter[5] = advBigBulletFireEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
	stateAllowImageChange[5] = false;
    stateTransitionOnTimeout[5] = "Smoke";

	stateName[6] = "Smoke";
	stateEmitter[6] = advBigBulletSmokeEmitter;
	stateEmitterTime[6] = 0.05;
	stateEmitterNode[6] = "muzzleNode";
	stateTimeoutValue[6] = 0.05;
	stateAllowImageChange[6] = false;
	stateTransitionOnTimeout[6] = "CheckChamber";

	stateName[7] = "PullBackSlide";
	stateScript[7] = "onPullBackSlide";
	stateSound[7] = ComplexBoltSound;
	stateSequence[7] = "Fire";
	stateTimeoutValue[7] = 0.2;
	stateTransitionOnTimeout[7] = "CheckTrigger";

	stateName[8] = "CheckTrigger";
	stateTransitionOnTriggerUp[8] = "CheckChamber";
};

function ThompsonImage::onMount(%this, %obj, %slot)
{
    %obj.debugWeapon();

    %props = %obj.getItemProps();
    %obj.setImageLoaded(%slot, %props.loaded);
}

function ThompsonImage::onPullBackSlide(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.loaded)
	{
		// eject shell
		%props.loaded = false;
	}

	if (%props.magazine.count >= 1)
	{
		%props.magazine.count--;
		%props.loaded = true;

		%obj.playThread(2, "plant");
	}

	%obj.setImageAmmo(%slot, false);
	%obj.setImageLoaded(%slot, %props.loaded);
}

function ThompsonImage::onEmptyFire(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.magazine.count >= 1)
	{
		%obj.setImageAmmo(%slot, true);
		return;
	}

	%obj.playThread(2, "plant");

    serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function ThompsonImage::onFire(%this, %obj, %slot)
{
	if (%obj.getState() $= "Dead")
		return;

    %props = %obj.getItemProps();

    if (!%props.loaded)
        // shouldn't happen
        return;

	%obj.playThread(2, "plant");

    // fire bullet
    %proj = new ScriptObject()
	{
		class = "ProjectileRayCast";
		superClass = "TimedRayCast";
		position = %obj.getMuzzlePoint(0);
		velocity = vectorScale(%obj.getMuzzleVector(%slot), cf_muzzlevelocity_ms(285));
		gravity = "0 0" SPC cf_bulletdrop_grams(15);
		lifetime = 3;
		mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;
		exempt = %obj;
		sourceObject = %obj;
		sourceClient = %obj.client;
		damage = 6;
		damageType = $DamageType::Generic;
		hitExplosion = GunProjectile;
    };

    MissionCleanup.add(%proj);
    %proj.fire();

    if (%props.magazine.count >= 1)
    {
        %props.magazine.count--;
        serverPlay3D(ThompsonFireSound, %obj.getMuzzlePoint(%slot));
    }
    else
    {
        %props.loaded = false;
        %obj.setImageLoaded(%slot, false);
        serverPlay3D(ThompsonFireLastSound, %obj.getMuzzlePoint(%slot));
    }
}

function ThompsonImage::onLight(%this, %obj, %slot)
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

function ThompsonImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    %props = %obj.getItemProps();

    if (%trigger == 4 && %state && %obj.getImageState(%slot) !$= "Reload")
    {
		%obj.setImageAmmo(%slot, true);
        return 1;
    }

    return 0;
}

// function ThompsonImage::getDebugText(%this, %obj, %slot)
// {
//     %props = %obj.getItemProps();
//
//     %text = "\c6" @ (%props.loaded ? "loaded" : "empty");
//     %text = %text SPC (isObject(%props.magazine) ? "mag=" @ %props.magazine.count : "no-mag");
//     %text = %text SPC "state=" @ %obj.getImageState(%slot);
//
//     return %text;
// }
