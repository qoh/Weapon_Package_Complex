addDamageType("Remington870",
	'<bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_remmington> %1',
	'%2 <bitmap:Add-Ons/Weapon_Package_Complex/assets/icons/ci_remmington> %1',
	0.2, 1);

datablock AudioProfile(Remington870FireSound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/remington870/fire.wav";
    description = AudioClose3d;
    preload = true;
};

datablock AudioProfile(Remington870PumpSound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/remington870/pump.wav";
    description = AudioClose3d;
    preload = true;
};

datablock AudioProfile(Remington870InsertSound)
{
    fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/remington870/reload.wav";
    description = AudioClose3d;
    preload = true;
};

datablock ItemData(Remington870Item)
{
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington870.dts";

    mass = 1;
	density = 0.2;
	elasticity = 0.2;
	friction = 0.6;

    uiName = "Remington 870";
    iconName = "Add-Ons/Weapon_Package_Complex/assets/icons/remmington";
    image = Remington870Image;
    canDrop = true;
};

datablock ShapeBaseImageData(Remington870Image)
{
    className = "TimeSliceRayWeapon";
    shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/remmington870.dts";

    fireMuzzleVelocity = cf_muzzlevelocity_ms(472.44);
	fireVelInheritFactor = 0.75;
	fireGravity = cf_bulletdrop_grams(25);
	fireHitExplosion = GunProjectile;
    fireCount = 8;
	fireSpread = 11;

    item = Remington870Item;
    armReady = true;
	speedScale = 0.795;

    stateName[0] = "Activate";
    stateSequence[0] = "activate";
    stateTimeoutValue[0] = 0.15;
    stateTransitionOnTimeout[0] = "Empty";

    stateName[1] = "Empty";
    stateSequence[1] = "root";
    stateTransitionOnTriggerDown[1] = "EmptyFire";
    stateTransitionOnLoaded[1] = "Ready";
    stateTransitionOnAmmo[1] = "Pump";

    stateName[2] = "EmptyFire";
    stateScript[2] = "onEmptyFire";
    stateTimeoutValue[2] = 0.13;
	stateWaitForTimeout[2] = true;
    stateTransitionOnTriggerUp[2] = "Empty";

    stateName[3] = "Ready";
    stateSequence[1] = "root";
    stateTransitionOnTriggerDown[3] = "Fire";
    stateTransitionOnNotLoaded[3] = "Empty";
    stateTransitionOnAmmo[3] = "Pump";

    stateName[4] = "Fire";
    stateFire[4] = true;
    stateSound[4] = Remington870FireSound;
    stateScript[4] = "onFire";
    stateSequence[4] = "Fire";
    stateEmitter[4] = advSmallBulletFireEmitter;
	stateEmitterTime[4] = 0.05;
	stateEmitterNode[4] = "muzzleNode";
    stateTimeoutValue[4] = 0.12;
    stateTransitionOnTimeout[4] = "Smoke";

    stateName[5] = "Smoke";
    stateEmitter[5] = advSmallBulletSmokeEmitter;
	stateEmitterTime[5] = 0.05;
	stateEmitterNode[5] = "muzzleNode";
    stateTimeoutValue[5] = 0.1;
    stateTransitionOnTriggerUp[5] = "Empty";

    stateName[6] = "Pump";
    stateSound[6] = Remington870PumpSound;
    stateScript[6] = "onPump";
    stateSequence[6] = "Pump";
    stateTimeoutValue[6] = 0.3; // 0.2
    stateWaitForTimeout[6] = true;
    stateTransitionOnNoAmmo[6] = "Empty";
};

function Remington870Image::onMount(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (%props.chamber $= "")
        %props.chamber = 0;

    %obj.setImageLoaded(%slot, %props.chamber == 1);
}

function Remington870Image::onEmptyFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    // if (%props.count >= 1)
    // {
    //     %obj.setImageAmmo(%slot, true);
    //     return;
    // }

    serverPlay3D(RevolverClickSound, %obj.getMuzzlePoint(%slot));
}

function Remington870Image::onFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    Parent::onFire(%this, %obj, %slot);

    %obj.playThread(2, "shiftLeft");
    %obj.playThread(3, "shiftRight");

    %props.chamber = 2;
    %obj.setImageLoaded(%slot, false);

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function Remington870Image::onPump(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (%props.chamber != 0)
    {
        %obj.ejectShell(Bullet45Item, 1.35, %props.chamber == 2);
        %props.chamber = 0;
    }

    if (%props.count >= 1)
    {
        %props.count--;
        %props.chamber = 1;
        // insert shell
    }

    %obj.setImageLoaded(%slot, %props.chamber == 1);
    %obj.setImageAmmo(%slot, false);

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function Remington870Image::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    if (%trigger == 4 && %state && %obj.getImageState(%slot) !$= "Pump")
    {
        %obj.setImageAmmo(%slot, true);
        return 1;
    }

    return 0;
}

function Remington870Image::onLight(%this, %obj, %slot)
{
    %props = %obj.getItemProps();

    if (%props.count < 6)
    {
		if ($Sim::Time - %obj.lastRemingtonInsert <= 0.2)
			return 1;

		%obj.lastRemingtonInsert = $Sim::Time;

        %props.count++;
        %obj.playThread(2, "plant");
        serverPlay3D(Remington870InsertSound, %obj.getMuzzlePoint(%slot));

		if (isObject(%obj.client))
			%obj.client.updateDetailedGunHelp();

        return 1;
    }

    return 0;
}

function Remington870Image::damage(%this, %obj, %col, %position, %normal)
{
	%damage = 9;
	%damageType = $DamageType::Remington870;

	if (!$NoCrouchDamageBonus && %col.isCrouched())
		%damage /= 2;

	%col.damage(%obj, %position, %damage, %damageType);
}

// function Remington870Image::getDebugText(%this, %obj, %slot)
// {
//     %props = %obj.getItemProps();
//     %text = "\c6" @ (%props.chamber == 2 ? "spent" : (%props.chamber == 1 ? "loaded" : "empty")) @ " \c3";
//
//     for (%i = 0; %i < %props.count; %i++) %text = %text @ "o";
//     %text = %text @ "\c7";
//     for (0; %i < 6; %i++) %text = %text @ "o";
//     %text = %text @ "\n";
//
//     return %text;
// }

function Remington870Image::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (%props.chamber == 1)
		return "Your gun is ready to fire. Left click to shoot. You will need to pump the action afterwards to chamber the next bullet.";

	if (%props.count < 1)
		return "Your gun has no more bullets stored. Press the light key to insert one.";

	return "Your gun has bullets stored, but no bullet is chambered. Right click to pump the action and chamber the next bullet.";
}

function Remington870Image::getDetailedGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	%kt_lmb = "Primary";
	%kt_rmb = "Jet    ";
	%kt_r   = "Light  ";

	%at_fire     = "Dryfire     ";
	%at_action   = "Pump Action ";
	%at_magazine = "Insert Round";

	%ac_fire     = "\c7";
	%ac_action   = "\c7";
	%ac_magazine = "\c7";

	if (%props.chamber == 1)
		%at_fire = "Fire        ";

	if (%props.chamber == 1)
		%ac_fire = "\c6";
	else
	{
		if (%props.count >= 1)
			%ac_action = "\c6";
		else
			%ac_magazine = "\c6";
	}

	if (%props.chamber == 2)
		%ac_action = "\c6";

	if (%props.count < 6)
		%ac_magazine = "\c6";

	%text = "<just:right><font:consolas:16>";
	%text = %text @ %ac_fire     @ %at_fire     @ "   " @ %kt_lmb @ " \n";
	%text = %text @ %ac_action   @ %at_action   @ "   " @ %kt_rmb @ " \n";
	%text = %text @ %ac_magazine @ %at_magazine @ "   " @ %kt_r   @ " \n";
	return %text;
}
