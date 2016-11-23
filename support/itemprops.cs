function Player::getItemProps(%this, %slot)
{
	if (%slot $= "")
	{
		if (%this.currTool == -1)
			return 0;

		%slot = %this.currTool;
	}

	if (!isObject(%this.tool[%slot]))
		return 0;
	if (!isObject(%this.itemProps[%slot]))
		%this.itemProps[%slot] = %this.tool[%slot].newItemProps(%this, %slot);
	else if (%this.itemProps[%slot].sourceItemData != %this.tool[%slot])
		echo(" --+ BUG ALERT BUG ALERT: " @ %this.tool[%slot].getName() @ " has props for " @ %this.itemProps[%slot].sourceItemData.getName());

	return %this.itemProps[%slot];
}

function Item::getItemProps(%this)
{
	if (!isObject(%this.itemProps))
		%this.itemProps = %this.getDataBlock().newItemProps();

	return %this.itemProps;
}

function ItemData::newItemProps(%this, %player, %slot)
{
	%props = new ScriptObject()
	{
		class = %this.itemPropsClass;
		superClass = "ItemProps";

		sourceItemData = %this.getId();
		sourcePlayer = %player;
		sourceClient = %player.client;

		itemSlot = %slot;
	};

	%props.onOwnerChange(%player);
	return %props;
}

function ItemProps::onOwnerChange(%this, %owner)
{
	%this.owner = %owner;
}

if (!isFunction("ItemData", "onRemove"))
	eval("function ItemData::onRemove(){}");

package ItemPropsPackage
{
	function Player::setDataBlock(%this, %data)
	{
		%maxTools = %this.getDataBlock().maxTools;

		for (%i = %data.maxTools; %i < %maxTools; %i++)
		{
			if (isObject(%this.itemProps[%i]))
				%this.itemProps[%i].delete();
		}

		Parent::setDataBlock(%this, %data);
	}

	function Player::mountImage(%this, %image, %slot, %loaded, %skinTag)
	{
		Parent::mountImage(%this, %image, %slot, %loaded, %skinTag);

		if (%this.getMountedImage(%slot).item.itemPropsAlways)
			%this.getItemProps(%this.getMountedImage(%slot), %slot);

		%this.debugWeapon();
	}

	function Armor::onRemove(%this, %player)
	{
		Parent::onRemove(%this, %player);

		for (%i = 0; %i < %this.maxTools; %i++)
		{
			if (isObject(%player.itemProps[%i]))
				%player.itemProps[%i].delete();
		}
	}

	function ItemData::onAdd(%this, %obj)
	{
		Parent::onAdd(%this, %obj);

		if (isObject($DroppedItemProps))
		{
			%obj.itemProps = $DroppedItemProps;
			$DroppedItemProps.onOwnerChange(%obj);
			$DroppedItemProps = "";
		}
	}

	function ItemData::onRemove(%this, %obj)
	{
		Parent::onRemove(%this, %obj);

		if (isObject(%obj.itemProps))
			%obj.itemProps.delete();
	}

	function ItemData::onPickup(%this, %col, %obj)
	{
		if (!isObject(%col.itemProps) && !%col.getDataBlock().customPickupAlways)
		{
			return Parent::onPickup(%this, %col, %obj);
		}
		%client = %obj.client;
		%i = $complexI;
		$complexI = "";

		%data = %col.getDataBlock();
		%obj.tool[%i] = %data;

		if (isObject(%col.itemProps))
		{
			%obj.itemProps[%i] = %col.itemProps;

			// improve this later
			%col.itemProps.itemSlot = %i;
			%col.itemProps.onOwnerChange(%obj);
			%col.itemProps = "";
		}

		messageClient(%client, 'MsgItemPickup', '', %i, %data);

		if (%col.isStatic())
		{
			%col.Respawn();
		}
		else
			%col.delete();

		return 1;
	}

	function Player::pickup(%obj, %col)
	{
		if (%obj.getState() !$= "Dead" && %obj.getDamagePercent() < 1 &&
			isObject(%col) && %col.getClassName() $= "Item" && (isObject(%col.itemProps) || %col.getDataBlock().customPickupAlways))
		{
			if (%col.canPickup == 0)
				return;

			%client = %obj.client;
			%data = %col.getDataBlock();

			if (!isObject(%client))
				return;

			%miniGame = %client.miniGame;

			if (isObject(%miniGame) && %miniGame.WeaponDamage == 1)
			{
				if (getSimTime() - %client.lastF8Time < 5000)
					return;
			}

			for (%i = 0; %i < %obj.getDataBlock().maxTools; %i++)
			{
				if (!isObject(%obj.tool[%i]))
					break;

				if (!%data.customPickupMultiple && %obj.tool[%i] == %data)
					return;
			}

			if (%i == %obj.getDataBlock().maxTools)
				return;

			if (miniGameCanUse(%obj, %col) == 0)
			{
				if (isObject(%col.spawnBrick))
					%ownerName = %col.spawnBrick.getGroup().name;

				if ($lastError == $LastError::MiniGameDifferent)
				{
					if (isObject(%client.miniGame))
						%msg = "This item is not part of the mini-game.";
					else
						%msg = "This item is part of a mini-game.";
				}
				else if ($lastError == $LastError::MiniGameNotYours)
					%msg = "You do not own this item.";
				else if ($lastError == $LastError::NotInMiniGame)
					%msg = "This item is not part of the mini-game.";
				else
					%msg = %ownerName @ " does not trust you enough to use this item.";

				commandToClient(%client, 'CenterPrint', %msg, 1);
				return;
			}

			//%brick = %col.spawnBrick;
			//if(isObject(%brick) && isFunction(%brick.getClassName(), "onItemPickup"))
			//{
			//	%brick.onItemPickup(%client);
			//}

			$complexI = %i;
			return %data.onPickup(%col, %obj);
		}
		return Parent::pickup(%obj, %col);
	}

	function serverCmdDropTool(%client, %index)
	{
		%player = %client.player;

		if (isObject(%player.tool[%index]) && isObject(%player.itemProps[%index]))
			$DroppedItemProps = %player.itemProps[%index];

		Parent::serverCmdDropTool(%client, %index);

		if (!isObject(%player.tool[%index]) && isObject(%player.itemProps[%index]))
		{
			if (isObject($DroppedItemProps))
				$DroppedItemProps.delete();
			else
				$DroppedItemProps = "";

			%player.itemProps[%index] = "";
		}
	}

	function Armor::onCollision(%this, %obj, %col, %velocity, %speed)
	{
		if (isObject(%col) && %col.getClassName() $= "Item" && (isObject(%col.itemProps) || %col.getDataBlock().customPickupAlways))
		{
			%obj.pickup(%col);
			return;
		}
		Parent::onCollision(%this, %obj, %col, %velocity, %speed);
	}

	function serverCmdUseTool(%client, %index)
	{
		%player = %client.player;

		if (isObject(%player) && !isObject(%player.tool[%index].image))
		{
			%player.unMountImage(0);
			fixArmReady(%player);
			return;
		}

		Parent::serverCmdUseTool(%client, %index);
	}

	function MiniGameSO::forceEquip(%this, %slot)
	{
		for (%i = 0; %i < %this.numMembers; %i++)
		{
			%player = %this.member[%i].player;

			if (!isObject(%player))
				continue;

			if (%player.tool[%slot] != %this.startEquip[%slot])
			{
				if (isObject(%player.itemProps[%slot]))
					%player.itemProps[%slot].delete();

				if (%this.startEquip[%slot].itemPropsAlways)
					%player.itemProps[%slot] = %this.startEquip[%slot].newItemProps(%player, %slot);
			}
		}

		Parent::forceEquip(%this, %slot);
	}
};

activatePackage("ItemPropsPackage");
