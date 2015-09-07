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
    armReady = 1;
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

    stateName[0] = "Activate";
    stateTimeoutValue[0] = 0.15;
    stateTransitionOnTimeout[0] = "CheckChamber";

    stateName[1] = "CheckChamber";
    stateTransitionOnLoaded[1] = "Ready";
    stateTransitionOnNotLoaded[1] = "Empty";

    stateName[2] = "Empty";
    stateTransitionOnLoaded[2] = "Ready";
    stateTransitionOnTriggerDown[2] = "EmptyFire";

    stateName[3] = "EmptyFire";
    stateScript[3] = "onEmptyFire";
    stateTransitionOnTriggerUp[3] = "Empty";

    stateName[4] = "Ready";
    stateTransitionOnTriggerDown[4] = "Fire";

    stateName[5] = "Fire";
    stateFire[5] = true;
    stateScript[5] = "onFire";
    stateTimeoutValue[5] = 0.15;
    stateWaitForTimeout[5] = true;
    stateTransitionOnTriggerUp[5] = "CheckChamber";
};

function Colt1911Image::getDebugText(%this, %obj, %slot)
{
    %props = %obj.getItemProps(%this, %slot);

    %text = "\c6" @ %props.loaded ? "loaded" : "empty";
    %text = %text SPC (isObject(%props.magazine) ? "mag=" @ %props.magazine.count : "no-mag");
    %text = %text SPC "state=" @ %obj.getImageState(%slot);

    return %text;
}

function Colt1911Image::onMount(%this, %obj, %slot)
{
    %obj.debugWeapon();

    %props = %obj.getItemProps(%this, %slot);

    %obj.setImageLoaded(%slot, %props.loaded);
    %obj.setImageAmmo(%slot, isObject(%props.magazine));
}

function Colt1911Image::onLight(%this, %obj, %slot)
{
    %props = %obj.getItemProps(%this, %slot);

    if (isObject(%props.magazine))
    {
        // remove magazine
        %props.magazine.delete();
        %obj.setImageAmmo(%slot, 0);

        serverPlay3D(Colt1911ClipOutSound, %obj.getMuzzlePoint(%slot));
    }
    else
    {
        // insert magazine
        %props.magazine = new ScriptObject()
        {
            count = 12;
        };

        %obj.setImageAmmo(%slot, 1);
        serverPlay3D(Colt1911ClipInSound, %obj.getMuzzlePoint(%slot));
    }

    return 1;
}

function Colt1911Image::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    %props = %obj.getItemProps(%this, %slot);

    if (%trigger == 4 && %state)
    {
        if (!%props.loaded && %props.magazine.count >= 1)
        {
            serverPlay3D(Colt1911SlidepullSound, %obj.getMuzzlePoint(%slot));

            %props.loaded = true;
            %props.magazine.count--;

            %obj.setImageLoaded(%slot, 1);
        }

        return 1;
    }

    return 0;
}

function Colt1911Image::onEmptyFire(%this, %obj, %slot)
{
    // click
    talk("click");
    serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function Colt1911Image::onFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps(%this, %slot);
    talk("hey");

    if (!%props.loaded)
    {
        talk("not loaded??");
        // shouldn't happen
        return;
    }

    // fire bullet
    talk("fire");

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
