datablock AudioProfile(PinOutSound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/pinOut.wav";
    description = AudioClose3D;
    preload = true;
};

datablock ItemData(HEGrenadeItem)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/grenade.dts";
	mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

    canDrop = true;
    armReady = 1;
    uiName = "HE Grenade";
    image = HEGrenadeImage;

    fuseTime = 3000;
    itemPropsClass = "HEGrenadeProps";
    itemPropsAlways = true;
};

datablock ShapeBaseImageData(HEGrenadeImage)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/grenade.dts";
    item = HEGrenadeItem;
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

function HEGrenadeImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    %props = %obj.getItemProps(%this, %slot);

    if (isEventPending(%props.schedule) || %trigger != 4)
        return;

    if (%state)
    {
        %props.pinOut = true;
        serverPlay3D(PinOutSound, %obj.getMuzzlePoint(0));
    }
    else if (%props.pinOut)
        %props.startSchedule(HEGrenadeItem.fuseTime);
}
