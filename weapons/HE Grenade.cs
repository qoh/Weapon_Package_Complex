datablock ItemData(HEGrenadeItem)
{
    shapeFile = "base/data/shapes/printGun.dts";
    canDrop = true;
    uiName = "HE Grenade";
    image = HEGrenadeImage;

    fuseTime = 3000;
    itemPropsClass = "HEGrenadeProps";
    itemPropsAlways = true;
};

datablock ShapeBaseImageData(HEGrenadeImage)
{
    shapeFile = "base/data/shapes/printGun.dts";
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
        %props.pinOut = true;
    else if (%props.pinOut)
        %props.startSchedule(HEGrenadeItem.fuseTime);
}
