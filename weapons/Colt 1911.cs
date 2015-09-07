datablock AudioProfile(Colt1911FireSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/m1911_fire.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Colt1911FireLastSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/m1911_fireLast.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Colt1911SlidepullSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/pistol_slidepull.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Colt1911ClipInSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/pistol_clipin.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(Colt1911ClipOutSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/pistol_clipout.wav";
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

    itemPropsClass = "Colt1911Props";
};

function Colt1911Props::onRemove(%this)
{
    if (isObject(%this.magazine))
        %this.magazine.delete();
}

datablock ShapeBaseImageData(Colt1911Image)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/Colt_1911.dts";
    item = Colt1911Item;
    armReady = 1;

    stateName[0] = "Activate";
	stateSequence[0] = "activate";
    stateTimeoutValue[0] = 0.15;
    stateTransitionOnTimeout[0] = "CheckChamber";

    stateName[1] = "CheckChamber";
    stateTransitionOnLoaded[1] = "Ready";
    stateTransitionOnNotLoaded[1] = "Empty";

    stateName[2] = "Empty";
	stateSequence[2] = "noammo";
    stateTransitionOnLoaded[2] = "Ready";
    stateTransitionOnTriggerDown[2] = "EmptyFire";

    stateName[3] = "EmptyFire";
    stateScript[3] = "onEmptyFire";
	stateSequence[3] = "emptyFire";
	stateTimeoutValue[3] = 0.13;
	stateWaitForTimeout[3] = true;
	stateAllowImageChange[3] = false;
    stateTransitionOnTriggerUp[3] = "Empty";

    stateName[4] = "Ready";
	stateSequence[4] = "root";
    stateTransitionOnTriggerDown[4] = "Fire";

    stateName[5] = "Fire";
    stateFire[5] = true;
    stateScript[5] = "onFire";
	stateSequence[5] = "Fire";
    stateTimeoutValue[5] = 0.12;
	stateEmitter[5] = advSmallBulletFireEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
	stateAllowImageChange[5] = false;
    stateTransitionOnTimeout[5] = "Smoke";

	stateName[6] = "Smoke";
	stateEmitter[6] = advSmallBulletSmokeEmitter;
	stateEmitterTime[6] = 0.05;
	stateEmitterNode[6] = "muzzleNode";
	stateTimeoutValue[6] = 0.1;
	stateWaitForTimeout[6] = true;
	stateAllowImageChange[6] = false;
	stateTransitionOnTriggerUp[6] = "CheckChamber";
};

function Colt1911Image::getDebugText(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    %text = "\c6" @ (%props.loaded ? "loaded" : "empty");
    %text = %text SPC (isObject(%props.magazine) ? "mag=" @ %props.magazine.count : "no-mag");
    %text = %text SPC "state=" @ %obj.getImageState(%slot);

    return %text;
}

function Colt1911Image::onMount(%this, %obj, %slot)
{
    %obj.debugWeapon();

    %props = %obj.getItemProps();

    %obj.setImageLoaded(%slot, %props.loaded);
    %obj.setImageAmmo(%slot, isObject(%props.magazine));
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

    %obj.setImageAmmo(%slot, isObject(%props.magazine));
    return 1;
}

function Colt1911Image::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    %props = %obj.getItemProps();

    if (%trigger == 4 && %state)
    {
        if (!%props.loaded && %props.magazine.count >= 1)
        {
            serverPlay3D(Colt1911SlidepullSound, %obj.getMuzzlePoint(%slot));

            %props.loaded = true;
            %props.magazine.count--;

            %obj.setImageLoaded(%slot, 1);
			%obj.playThread(2, "plant");
        }

        return 1;
    }

    return 0;
}

function Colt1911Image::onEmptyFire(%this, %obj, %slot)
{
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

	%obj.playThread(2, "shiftAway");

    // fire bullet
    %proj = new ScriptObject() {
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
