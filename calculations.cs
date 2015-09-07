// Reference gravity
$CF_G = -9.8;

// meter <-> torque unit
$CF_M_TO_TU = 2.5;
$CF_TU_TO_M = 1 / $CF_M_TO_TU;

function cf_muzzlevelocity_ms(%speed)
{
    return %speed * $CF_M_TO_TU;
}

function cf_bulletdrop_grams(%weight)
{
    return %weight * $CF_G;
    // should be: return %weight / 1000 * $CF_G * $CF_M_TO_TU;
}
