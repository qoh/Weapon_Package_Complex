function Player::takeMagazineProps(%this, %item)
{
    %index = %this.findMagazine(%item);

    if (%index == -1)
        return 0;

    %props = %this.itemProps[%index];

    %this.itemProps[%index] = "";
    %this.removeTool(%index);

    return %props;
}

function Player::giveMagazineProps(%this, %props)
{
    if (isObject(%this.client) && %props.count < 1)
    {
        if (%this.client.discardEmptyMagazine == 2)
        {
            %props.delete();
            return;
        }

        if (%this.client.discardEmptyMagazine == 1 && %this.findMagazine(%props.weapon) != -1)
        {
            %props.delete();
            return;
        }
    }

    %maxTools = %this.getDataBlock().maxTools;

    for (%i = 0; %i < %maxTools; %i++)
    {
        if (!isObject(%this.tool[%i]))
            break;
    }

    if (%i == %maxTools)
    {
        // todo: drop mag
        %props.delete();
        return;
    }

    %this.tool[%i] = %props.sourceItemData.getID();
    %this.itemProps[%i] = %props;

    if (isObject(%this.client))
        messageClient(%this.client, 'MsgItemPickup', '', %i, %props.sourceItemData.getID(), 1);
}

function Player::findMagazine(%this, %item)
{
    %item = %item.getID();
    %maxTools = %this.getDataBlock().maxTools;

    %bestSlot = -1;
    %bestCount = 0;

    for (%i = 0; %i < %maxTools; %i++)
    {
        %props = %this.getItemProps(%i);

        // if (!isObject(%props))
        // {
        //     talk("-- no props");
        //     continue;
        // }
        //
        // if (%props.weapon $= "")
        // {
        //     talk("-- weapon is \"\"");
        //     continue;
        // }
        //
        // if (%props.weapon.getID() != %item)
        // {
        //     talk("-- not the right ammo type");
        //     continue;
        // }
        //
        // if (%props.count <= %bestCount)
        // {
        //     talk("-- count <= bestCount");
        //     continue;
        // }
        //
        // %bestSlot = %i;
        // %bestCount = %props.count;

        if (isObject(%props.weapon) && %props.weapon.getID() == %item && %props.count > %bestCount)
        {
            %bestSlot = %i;
            %bestCount = %props.count;
        }
    }

    return %bestSlot;
}

function SimpleMagWeaponProps::onRemove(%this)
{
    if (isObject(%this.magazine))
        %this.magazine.delete();
}

function MagazineProps::onAdd(%this)
{
    %data = %this.sourceItemData;

    %this.capacity = %data.magSize;
    %this.count = %data.magSize;
    %this.weapon = %data.magWeapon;
    %this.type = %data.magType;

    %this.name = %data.cartName;
    %this.weight = %data.cartWeight;
}

datablock ShapeBaseImageData(MagazineImage)
{
    shapeFile = "base/data/shapes/empty.dts";
};

function Player::displayMagazineStats(%this)
{
    if (%this.getMountedImage(0) == MagazineImage.getID() && isObject(%this.client))
    {
        %props = %this.getItemProps();
        %text = "<just:center><font:palatino linotype:30>\c3" @ %props.capacity @ "-round<font:palatino linotype:24>\c6 " @ %props.type @ " magazine\n";
        %text = %text @ "\c6Contains <font:palatino linotype:30>\c3" @ %props.count @ "x <font:palatino linotype:24>\c6" @ %props.name @ " (" @ %props.weight @ "g)\n";
        %text = %text @ "\c6Used with <font:palatino linotype:30>\c3" @ %props.weapon.uiName @ "\n";
        commandToClient(%this.client, 'CenterPrint', %text, 0);
    }
}

function MagazineImage::onMount(%this, %obj, %slot)
{
    %obj.displayMagazineStats();
}

function MagazineImage::onUnMount(%this, %obj, %slot)
{
    if (isObject(%obj.client))
        commandToClient(%obj.client, 'ClearCenterPrint');
}

datablock ItemData(MagazineItem_45ACP_x7)
{
    image = MagazineImage;
    itemPropsClass = "MagazineProps";
    customPickupAlways = true;
    customPickupMultiple = true;
    canDrop = true;

    mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;
	emap = true;

    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/items/colt_magazine.dts";
    iconName = "./assets/icons/colt_clip";
    uiName = "M 12x .45 ACP";

    magSize = 12;
    magWeapon = Colt1911Item;
    magType = "extended detachable box";

    cartName = "11.43x23mm .45 ACP";
    cartWeight = 15;
};

datablock ItemData(MagazineItem_M24A1 : MagazineItem_45ACP_x7)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/items/m1garand_clip.dts";
    iconName = "./assets/icons/m1garand_clip";
    uiName = "M M24A1";

    magSize = 5;
    magWeapon = M24RifleItem;
    magType = "internal";

    cartName = "7.62x51mm NATO";
    cartWeight = 10;
};

datablock ItemData(MagazineItem_45ACP_x20_SMG : MagazineItem_45ACP_x7)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/items/thompson_clip.dts";
    iconName = "./assets/icons/thompson_clip";
    uiName = "M 20x .45 ACP";

    magSize = 20;
    magWeapon = ThompsonItem;
    magType = "stick magazine";

    cartName = "11.43x23mm .45 ACP";
    cartWeight = 15;
};

package MagazineStatsPackage
{
    function serverCmdUseTool(%client, %slot)
    {
        Parent::serverCmdUseTool(%client, %slot);

        if (isObject(%client.player))
            %client.player.displayMagazineStats();
    }
};

activatePackage("MagazineStatsPackage");
