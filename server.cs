exec("./support/sfx.cs");
exec("./support/imagetrigger.cs");
exec("./support/itemprops.cs");
exec("./support/itemfuncs.cs");
exec("./support/timedraycast.cs");
exec("./support/timedfiring.cs");
exec("./support/lagcompensation.cs");
exec("./support/hitregion.cs");
exec("./support/movespeed.cs");

exec("./adventure_Effects.cs");

exec("./calculations.cs");
exec("./magazines.cs");
exec("./bullets.cs");
exec("./sounds.cs");

// temporary
datablock PlayerData(DeathmatchPlayer : PlayerStandardArmor)
{
	uiName = "Deathmatch Player";

	canJet = false;
	airControl = 0.25;
	mass = 120;

	maxForwardSpeed = 10;
	maxSideSpeed = 9;
	maxBackwardSpeed = 8;

	maxForwardCrouchSpeed = 3;
	maxSideCrouchSpeed = 3;
	maxBackwardCrouchSpeed = 2;
};

exec("./weapons/HE Grenade.cs");
exec("./weapons/Colt 1911.cs");
exec("./weapons/Thompson.cs");
exec("./weapons/Revolver.cs");
exec("./weapons/Colt Walker.cs");
exec("./weapons/M1 Garand.cs");
exec("./weapons/M24 Rifle.cs");
exec("./weapons/Remington 870.cs");

function GameConnection::updateDetailedGunHelp(%this, %stop)
{
	%player = %this.player;

	if (%stop || %this.disableDetailedGunHelp || !isObject(%player))
	{
		if (%this.displayingGunHelp)
		{
			commandToClient(%this, 'ClearCenterPrint');
			%this.displayingGunHelp = false;
		}

		return;
	}

	%image = %player.getMountedImage(0);

	if (!isObject(%image) || !isFunction(%image.getName(), "getDetailedGunHelp"))
	{
		if (%this.displayingGunHelp)
		{
			commandToClient(%this, 'ClearCenterPrint');
			%this.displayingGunHelp = false;
		}

		return;
	}

	commandToClient(%this, 'CenterPrint', %image.getDetailedGunHelp(%player, 0), 0);
	%this.displayingGunHelp = true;
}

function serverCmdGunHelp(%client)
{
	%player = %client.player;

	if (!isObject(%player))
		return;

	%image = %player.getMountedImage(0);

	if (!isObject(%image))
		return;

	if (!isFunction(%image.getName(), "getGunHelp"))
		return;

	messageClient(%client, '', "\c4Gun Help\c6: " @ %image.getGunHelp(%player, 0));
}

function Player::debugWeapon(%this)
{
    cancel(%this.debugWeapon);

    if (isObject(%this.client))
    {
        %image = %this.getMountedImage(0);

        if (isObject(%image) && isFunction(%image.getName(), "getDebugText"))
        {
            %text = "\c6" @ %image.getName() @ " in " @ %this.getImageState(0) @ "\n";
            %text = %text @ %image.getDebugText(%this, 0);

            %this.client.bottomPrint(%text, 0.3, 1);
        }
    }

    %this.debugWeapon = %this.schedule(50, "debugWeapon");
}

package GunHelpPackage
{
	function Player::mountImage(%this, %image, %slot, %loaded, %skinTag)
	{
		%client = %this.client;

		if (isObject(%client))
			%client.updateDetailedGunHelp(true);

		Parent::mountImage(%this, %image, %slot, %loaded, %skinTag);

		if (isObject(%client))
			%client.updateDetailedGunHelp();
	}

	function Player::unMountImage(%this, %slot)
	{
		Parent::unMountImage(%this, %slot);

		if (isObject(%this.client))
			%this.client.updateDetailedGunHelp(true);
	}
};

activatePackage("GunHelpPAckage");
