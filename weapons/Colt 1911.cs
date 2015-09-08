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
	iconName = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/icons/colt";
	rotate = false;
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

    canDrop = 1;
    uiName = "Colt 1911";
    image = Colt1911Image;

    itemPropsClass = "SimpleMagWeaponProps";
};

datablock ShapeBaseImageData(Colt1911Image)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/Colt_1911.dts";
    item = Colt1911Item;
    armReady = 1;

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
	stateWaitForTimeout[5] = true;
	stateAllowImageChange[5] = false;
	stateTransitionOnTriggerUp[5] = "Empty";

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
    %obj.setImageLoaded(%slot, %props.loaded);
}

function Colt1911Image::onReload(%this, %obj, %slot)
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

function Colt1911Image::onEmptyFire(%this, %obj, %slot)
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

function Colt1911Image::onFire(%this, %obj, %slot)
{
	if (%obj.getState() $= "Dead")
		return;

    %props = %obj.getItemProps();

    if (!%props.loaded)
        // shouldn't happen
        return;

	%obj.playThread(2, "shiftLeft");
	%obj.playThread(3, "shiftRight");

    // fire bullet
    %proj = new ScriptObject()
	{
		class = "ProjectileRayCast";
		superClass = "TimedRayCast";
		position = %obj.getMuzzlePoint(0);
		velocity = vectorScale(%obj.getMuzzleVector(%slot), cf_muzzlevelocity_ms(251));
		gravity = "0 0" SPC cf_bulletdrop_grams(15);
		lifetime = 3;
		mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType;
		exempt = %obj;
		sourceObject = %obj;
		sourceClient = %obj.client;
		damage = 16;
		damageType = $DamageType::Generic;
		hitExplosion = GunProjectile;
    };

    MissionCleanup.add(%proj);
    %proj.fire();

    // since this is a semi-auto weapon, automatically chamber the next shell
    if (%props.magazine.count >= 1)
    {
        %props.magazine.count--;
        serverPlay3D(Colt1911FireSound, %obj.getMuzzlePoint(%slot));
    }
    else
    {
        %props.loaded = false;
        %obj.setImageLoaded(%slot, 0);
        serverPlay3D(Colt1911FireLastSound, %obj.getMuzzlePoint(%slot));
    }
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

// function Colt1911Image::getDebugText(%this, %obj, %slot)
// {
//     %props = %obj.getItemProps();
//
//     %text = "\c6" @ (%props.loaded ? "loaded" : "empty");
//     %text = %text SPC (isObject(%props.magazine) ? "mag=" @ %props.magazine.count : "no-mag");
//     %text = %text SPC "state=" @ %obj.getImageState(%slot);
//
//     return %text;
// }
