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

exec("./weapons/HE Grenade.cs");
exec("./weapons/Colt 1911.cs");
exec("./weapons/Revolver.cs");
exec("./weapons/Colt Walker.cs");
exec("./weapons/M24 Rifle.cs");

function Player::debugWeapon(%this)
{
    cancel(%this.debugWeapon);

    if (isObject(%this.client))
    {
        %image = %this.getMountedImage(0);

        if (isObject(%image))
        {
            %text = "\c6" @ %image.getName() @ " in " @ %this.getImageState(0) @ "\n";

            if (isFunction(%image.getName(), "getDebugText"))
                %text = %text @ %image.getDebugText(%this, 0);

            %this.client.bottomPrint(%text, 0.3, 1);
        }
    }

    %this.debugWeapon = %this.schedule(50, "debugWeapon");
}
