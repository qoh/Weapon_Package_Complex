exec("./support/imagetrigger.cs");
exec("./support/itemprops.cs");
exec("./support/timedraycast.cs");
exec("./support/hitregion.cs");
// exec("./support/lagcompensation.cs");

exec("./calculations.cs");

exec("./weapons/HE Grenade.cs");
exec("./weapons/Colt 1911.cs");
exec("./weapons/M24 Rifle.cs");

function Player::debugWeapon(%this)
{
    cancel(%this.debugWeapon);

    if (isObject(%this.client))
    {
        %image = %this.getMountedImage(0);

        if (isObject(%image) && isFunction(%image.getName(), "getDebugText"))
            %this.client.bottomPrint(%image.getDebugText(%this, 0), 0.3, 1);
    }
    
    %this.debugWeapon = %this.schedule(50, "debugWeapon");
}
