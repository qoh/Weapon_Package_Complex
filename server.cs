exec("./support/imagetrigger.cs");
exec("./support/itemprops.cs");
exec("./support/itemfuncs.cs");
exec("./support/timedraycast.cs");
exec("./support/hitregion.cs");
// exec("./support/lagcompensation.cs");
exec("./support/movespeed.cs");

exec("./adventure_Effects.cs");

exec("./calculations.cs");
exec("./magazines.cs");

datablock AudioProfile(ComplexBoltSound)
{
	fileName = "./assets/sounds/bolt.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ComplexClipInSound)
{
	fileName = "./assets/sounds/clipin.wav";
	description = AudioClose3d;
	preload = true;
};

datablock AudioProfile(ComplexClipOutSound)
{
	fileName = "./assets/sounds/clipout.wav";
	description = AudioClose3d;
	preload = true;
};

exec("./weapons/HE Grenade.cs");
exec("./weapons/Colt 1911.cs");
exec("./weapons/Thompson.cs");
exec("./weapons/Revolver.cs");
exec("./weapons/Colt Walker.cs");
exec("./weapons/M1 Garand.cs");
exec("./weapons/M24 Rifle.cs");
exec("./weapons/Remington 870.cs");

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
