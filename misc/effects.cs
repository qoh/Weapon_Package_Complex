datablock ExplosionData(QuietGunExplosion : GunExplosion)
{
    soundProfile = "";
};

datablock ProjectileData(QuietGunProjectile : GunProjectile)
{
    uiName = "";
    explosion = QuietGunExplosion;
};
