datablock AudioProfile(PinOutSound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/pinOut.wav";
    description = AudioClose3D;
    preload = true;
};

datablock AudioProfile(GrenadeLeverOffSound)
{
	fileName = "base/data/sound/clickSuperMove.wav";
	description = AudioClosest3d;
	preload = true;
};

datablock ItemData(HEGrenadeItem)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/grenade.dts";
	mass = 1;
    drag = 0.2;
	density = 0.2;
	elasticity = 0.5;
	friction = 0.6;
	emap = true;

    canDrop = true;
    uiName = "HE Grenade";
    image = HEGrenadeImage;

    fuseTime = 3000;
    itemPropsClass = "HEGrenadeProps";
    itemPropsAlways = true;

    customPickupAlways = true;
    customPickupMultiple = true;
};

datablock ShapeBaseImageData(HEGrenadeImage)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/grenade.dts";

    item = HEGrenadeItem;
    armReady = 1;
    speedScale = 0.9;

    stateName[0] = "Activate";
    stateSequence[0] = "activate";
    stateTimeoutValue[0] = 0.2;
    stateAllowImageChange[0] = false;
    stateTransitionOnNotLoaded[0] = "NoPin";
    stateTransitionOnTimeout[0] = "Ready";

    stateName[1] = "Ready";
    stateTransitionOnTriggerDown[1] = "Charge";
    stateTransitionOnNotLoaded[1] = "NoPin";

    stateName[2] = "NoPin";
    stateSequence[2] = "noring";
    // stateAllowImageChange[2] = false;
    stateTransitionOnTriggerDown[2] = "Charge";

    stateName[3] = "Charge";
    stateScript[3] = "onCharge";
    stateTimeoutValue[3] = 0.45;
    stateAllowImageChange[3] = false;
    stateWaitForTimeout[3] = true;
    stateTransitionOnTriggerUp[3] = "ChargeAbort";
    stateTransitionOnTimeout[3] = "ChargeReady";

    stateName[4] = "ChargeAbort";
    stateScript[4] = "onChargeAbort";
    stateTimeoutValue[4] = 0.3;
    stateAllowImageChange[4] = false;
    stateTransitionOnTimeout[4] = "Ready";

    stateName[5] = "ChargeReady";
    stateScript[5] = "onChargeReady";
    stateAllowImageChange[5] = false;
    stateTransitionOnTriggerUp[5] = "Fire";

    stateName[6] = "Fire";
    stateScript[6] = "onFire";
    stateFire[6] = true;
};

function HEGrenadeProps::onRemove(%this)
{
    cancel(%this.schedule);
}

function HEGrenadeProps::onOwnerChange(%this, %owner)
{
    if (isEventPending(%this.schedule))
        %time = getTimeRemaining(%this.schedule);
    else if (%this.pinOut)
        %time = HEGrenadeItem.fuseTime;

    Parent::onOwnerChange(%this, %owner);

    if (%owner.getType() & ($TypeMasks::PlayerObjectType))
    {
        %this.damagePlayer = %owner;

        if (isObject(%owner.client))
            %this.damageClient = %owner.client;
    }

    if (%time !$= "")
        %this.startSchedule(%time);
}

function HEGrenadeProps::startSchedule(%this, %time)
{
    cancel(%this.schedule);
    %this.schedule = %this.owner.schedule(%time, "detonateHEGrenade", %this);
}

function spawnHEGrenadeExplosion(%position, %damagePlayer, %damageClient)
{
    %projectile = new Projectile()
    {
        datablock = RocketLauncherProjectile;

        initialPosition = %position;
        initialVelocity = "0 0 0";

        sourceObject = %damagePlayer;
        sourceSlot = 0;

        client = %damageClient;
    };

    MissionCleanup.add(%projectile);
    %projectile.explode();
}

function Player::detonateHEGrenade(%this, %props)
{
    %this.tool[%props.itemSlot] = 0;
    %this.itemProps[%props.itemSlot] = 0;

    if (isObject(%this.client))
        messageClient(%this.client, 'MsgItemPickup', '', %props.itemSlot, 0);

    spawnHEGrenadeExplosion(%this.getHackPosition(), %props.damagePlayer, %props.damageClient);

    if (isObject(%props)) // player might be deleted after death
        %props.delete();
}

function Item::detonateHEGrenade(%this, %props)
{
    spawnHEGrenadeExplosion(%this.getPosition(), %props.damagePlayer, %props.damageClient);
    %this.delete();
}

function HEGrenadeImage::onUnMount(%this, %obj, %slot)
{
    %obj.playThread(2, "root");
}

function HEGrenadeImage::onCharge(%this, %obj, %slot)
{
    %obj.playThread(2, "spearReady");

    if (isObject(%obj.client))
        %obj.client.updateDetailedGunHelp();
}

function HEGrenadeImage::onChargeReady(%this, %obj, %slot)
{
    if (isObject(%obj.client))
        %obj.client.updateDetailedGunHelp();
}

function HEGrenadeImage::onChargeAbort(%this, %obj, %slot)
{
    %obj.playThread(2, "root");

    if (isObject(%obj.client))
        %obj.client.updateDetailedGunHelp();
}

function HEGrenadeImage::onFire(%this, %obj, %slot)
{
    %obj.playThread(2, "spearThrow");
    %props = %obj.getItemProps();

    %velocity = vectorAdd(vectorScale(%obj.getEyeVector(), 20), vectorScale(%obj.getVelocity(), 0.5));

    %item = new Item()
    {
        datablock = HEGrenadeItem;
        position = %obj.getEyePoint();
        itemProps = %props;
    };

    MissionCleanup.add(%item);

    %item.setCollisionTimeout(%obj);
    %item.setVelocity(%velocity);
    %item.schedule(9000, "fadeOut");
    %item.schedule(10000, "delete");

    %props.onOwnerChange(%item);

    %obj.removeTool(%obj.currTool, true);
    %obj.unMountImage(%slot);
}

function HEGrenadeImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    %props = %obj.getItemProps();

    if (isEventPending(%props.schedule) || %trigger != 4)
        return;

    if (%state)
    {
        %props.pinOut = true;
        %obj.playThread(2, "shiftRight");
        serverPlay3D(PinOutSound, %obj.getMuzzlePoint(0));
        // need to drop pin item
    }
    else if (%props.pinOut)
    {
        %props.startSchedule(HEGrenadeItem.fuseTime);
        %obj.playThread(3, "shiftLeft");
        serverPlay3D(GrenadeLeverOffSound, %obj.getMuzzlePoint(0));
    }

    if (isObject(%obj.client))
        %obj.client.updateDetailedGunHelp();
}

function HEGrenadeImage::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

    if (isEventPending(%props.schedule))
        return "This grenade is primed! Press Ctrl+W (or the custom 'Drop Item' key) to quickly dispose of it. Make sure you run away!";

    if (%props.pinOut)
        return "You pulled the pin and you're clutching the lever. The grenade will prime itself as you release right click or throw it (hold LMB or Ctrl+W).";

    return "To throw a grenade, either hold left click or tap Ctrl+W. To prime a grenade, tap right click. It will explode 3 seconds after being primed.";
}

function HEGrenadeImage::getDetailedGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
    %state = %obj.getImageState(%slot);

    %text = "<just:right><font:consolas:16>";

    %throw_text = "Throw   ";
    %lob_text   = "Lob     ";

    %lob_key    = "Drop Item      ";

    if (%state $= "ChargeReady")
        %throw_key = "release Primary";
    else
        %throw_key  = "hold Primary   ";

    if (%props.pinOut)
    {
        %pin_text = "Prime   ";
        %pin_key = "release Jet    ";
    }
    else
    {
        %pin_text   = "Pull Pin";
        %pin_key    = "Jet            ";
    }

	%text = "<just:right><font:consolas:16>";
	%text = %text @ "\c6" @ %throw_text @ "   " @ %throw_key @ " \n";
	%text = %text @ "\c6" @ %lob_text   @ "   " @ %lob_key   @ " \n";

    if (!isEventPending(%props.schedule))
	   %text = %text @ "\c6" @ %pin_text   @ "   " @ %pin_key   @ " \n";

	return %text;
}
