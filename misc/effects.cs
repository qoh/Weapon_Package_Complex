datablock ExplosionData(QuietGunExplosion : GunExplosion)
{
    soundProfile = "";
};

datablock ProjectileData(QuietGunProjectile : GunProjectile)
{
    uiName = "";
    explosion = QuietGunExplosion;
};

datablock ExplosionData(ScreenShakeExplosion)
{
    // explosionShape = "";
    lifeTimeMS = 150;

    shakeCamera = true;
    camShakeFreq = "3 3 3";
    camShakeAmp = "0.6 0.8 0.7";
    camShakeDuration = 0.5;
    camShakeRadius = 10.0;
};

datablock ProjectileData(ScreenShakeProjectile)
{
    explosion = ScreenShakeExplosion;
};
