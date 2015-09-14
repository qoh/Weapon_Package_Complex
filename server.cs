forceRequiredAddOn("Weapon_Gun");

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

exec("./sounds.cs");
exec("./calculations.cs");
exec("./magazines.cs");
exec("./bullets.cs");
exec("./damage.cs");
exec("./misc/effects.cs");

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
exec("./weapons/Micro Uzi.cs");

function SimObject::getYaw(%this) {
	%vector = %this.getForwardVector();
	return mATan(getWord(%vector, 0), getWord(%vector, 1));
}

function SimObject::getPitch(%this) {
	%vector = %this.getMuzzleVector(0);

	%x = getWord(%vector, 0);
	%y = getWord(%vector, 1);
	%z = getWord(%vector, 2);

	return mATan(%z, mSqrt(%x * %x + %y * %y));
}

function Player::applyComplexKnockback(%this, %h)
{
	%v = %this.getMuzzleVector(0);
	%this.addVelocity(vectorScale(setWord(%v, 2, -1), -%h));
}

function Player::applyComplexScreenshake(%this, %h)
{
	%projectile = new Projectile()
	{
		datablock = ScreenShakeProjectile;

		initialPosition = %this.getEyePoint();
		initialVelocity = "0 0 0";
	};

	MissionCleanup.add(%projectile);

	%projectile.setScale("1 1" SPC %h);
	%projectile.explode();
}

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

function serverCmdToggleGunHelp(%this, %tog)
{
	if(%tog $= "")
		%tog = !%this.disableDetailedGunHelp;
	%tog = MClampF(%tog, 0, 1);
	%this.disableDetailedGunHelp = %tog;
	messageClient(%client, '', "\c4Gun Preferences\c6: You have " @ (%tog ? "enabled" : "disabled") @ " on-screen gun help.");
}

function serverCmdDiscardEmptyMagazines(%this, %val)
{
	if(%val $= "")
	{
		messageClient(%client, '', "\c4Gun Preferences\c6: Specify a number from 0 to 2 to change your magazine preferences.");
		messageClient(%client, '', "\c4Gun Preferences\c6: 0 will not drop empty magazines for you, 1 will drop empty magazines unless it's your last, and 2 will always throw away empty magazines.");
		return;
	}
	%val = MClampF(%val, 0, 2);
	%this.discardEmptyMagazine = %val;
	messageClient(%client, '', "\c4Gun Preferences\c6: " @ %text);
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
