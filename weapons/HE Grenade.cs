datablock AudioProfile(PinOutSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/pinOut.wav";
	description = AudioClose3D;
	preload = true;
};

datablock AudioProfile(GrenadeLeverOffSound)
{
	fileName = "base/data/sound/clickSuperMove.wav";
	description = AudioClosest3d;
	preload = true;
};

new ScriptObject(HEGrenadeBounceSFX)
{
	class = "SFXEffect";

	file[main, 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/he_bounce-1.mono.wav";
};

datablock AudioProfile(HEGrenadeBounceSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/he_bounce-1.mono.wav";
	description = AudioClose3D;
	preload = true;
};

datablock AudioProfile(HEGrenadeBeepSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/beep.wav";
	description = AudioClose3D;
	preload = true;
};

datablock AudioProfile(HEGrenadeDrawSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/he_draw.wav";
	description = AudioClose3D;
	preload = true;
};

datablock AudioProfile(HEGrenadeThrowSound)
{
	fileName = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/grenade_throw.wav";
	description = AudioClose3D;
	preload = true;
};

new ScriptObject(HEGrenadeExplodeSFX)
{
	class = "SFXEffect";

	file["close", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/explode3.mono.wav";
	file["close", 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/explode4.mono.wav";
	file["close", 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/explode5.mono.wav";
	filterDistanceMax["close"] = 64;
	use2D["close"] = "source";

	file["distant", 1] = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/explode3_distant.wav";
	file["distant", 2] = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/explode4_distant.wav";
	file["distant", 3] = "Add-Ons/Weapon_Package_Complex/assets/sounds/hegrenade/explode5_distant.wav";
	filterDistanceMin["distant"] = 64;
	use2D["distant"] = "always";
};

datablock ParticleData(HEGrenadeBlastParticle)
{
	dragCoefficient		= 1.0;
	windCoefficient		= 0.0;
	gravityCoefficient	= -0.2;
	inheritedVelFactor	= 0.1;
	constantAcceleration	= 0.0;
	lifetimeMS		= 4000;
	lifetimeVarianceMS	= 3990;
	spinSpeed		= 10.0;
	spinRandomMin		= -50.0;
	spinRandomMax		= 50.0;
	useInvAlpha		= true;
	animateTexture		= false;
	//framesPerSec		= 1;

	textureName		= "base/data/particles/cloud";
	//animTexName		= "~/data/particles/cloud";

	// Interpolation variables
	colors[0]	= "0.2 0.2 0.2 1.0";
	colors[1]	= "0.25 0.25 0.25 0.2";
	colors[2]	= "0.4 0.4 0.4 0.0";

	sizes[0]	= 2.0;
	sizes[1]	= 10.0;
	sizes[2]	= 13.0;

	times[0]	= 0.0;
	times[1]	= 0.1;
	times[2]	= 1.0;
};

datablock ParticleEmitterData(HEGrenadeBlastEmitter)
{
	ejectionPeriodMS = 7;
	periodVarianceMS = 0;
	lifeTimeMS	   = 21;
	ejectionVelocity = 10;
	velocityVariance = 1.0;
	ejectionOffset   = 1.0;
	thetaMin         = 0;
	thetaMax         = 0;
	phiReferenceVel  = 0;
	phiVariance      = 90;
	overrideAdvance = false;
	particles = "HEGrenadeBlastParticle";
};

datablock ExplosionData(HEGrenadeBlastExplosion)
{
	explosionShape = "Add-Ons/Weapon_Rocket_Launcher/explosionsphere1.dts";
	lifeTimeMS = 150;

	particleEmitter = HEGrenadeBlastEmitter;
	particleDensity = 10;
	particleRadius = 1;

	faceViewer = true;
	explosionScale = "1 1 1";

	shakeCamera = true;
	camShakeFreq = "7.0 8.0 7.0";
	camShakeAmp = "1.0 1.0 1.0";
	camShakeDuration = 0.5;
	camShakeRadius = 15.0;

	lightStartRadius = 0;
	lightEndRadius = 0;
	lightStartColor = "0.45 0.3 0.1";
	lightEndColor = "0 0 0";

	impulseRadius = 17;
	impulseForce = 4000;

	damageRadius = 16;
	radiusDamage = 200;
};

datablock ProjectileData(HEGrenadeBlastProjectile)
{
	explosion = HEGrenadeBlastExplosion;

	directDamageType = $DamageType::RocketDirect;
	radiusDamageType = $DamageType::RocketRadius;

	brickExplosionRadius = 10;
	brickExplosionImpact = false;
	brickExplosionForce = 25;
	brickExplosionMaxVolume = 100;
	brickExplosionMaxVolumeFloating = 60;
};

datablock ProjectileData(HEGrenadeProjectile)
{
	projectileShapeName = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/grenade_projectile.dts";
	ballRadius = 0.15;

	isBallistic = true;
	gravityMod = 1;
	bounceElasticity = 0.4;
	bounceFriction = 0.3;

	lifeTime = 5000;
	armingDelay = 5000;
	fadeDelay = 5000;

	explodeOnDeath = true; //Calls the "onExplode" function on HEGrenadeProjectile

	brickExplosionRadius = 0;
	brickExplosionImpact = false;
	brickExplosionForce = 0;
	brickExplosionMaxVolume = 0;
	brickExplosionMaxVolumeFloating = 0;
};

function HEGrenadeProjectile::onExplode(%this, %obj) {
	%item = new item() {
		dataBlock = HEGrenadeItem;
		sourceObject = %obj.sourceObject;
		client = %obj.client;
		miniGame = %obj.client.miniGame;
	};
	%item.setTransform(%obj.getTransform());
	%item.setVelocity(%obj.getVelocity());
	%item.props = %obj.itemProps;
}

function HEGrenadeProjectile::onCollision(%this, %obj, %col, %fade, %position, %normal)
{
	if (%col.getType() & $TypeMasks::PlayerObjectType)
	{
		%z1 = getWord(%position, 2);
		%z2 = getWord(%col.getPosition(), 2);

		%p1 = setWord(%position, 2, 0);
		%p2 = setWord(%col.getPosition(), 2, 0);

		%dot = vectorDot(vectorNormalize(vectorSub(%p1, %p2)), %col.getForwardVector());

		if (%z2 - %z1 > -0.5 || %dot >= 0.1)
		{
			if (isObject(%col.client))
			{
				if (%col.addTool(HEGrenadeItem, %obj.itemProps) != -1)
					%obj.delete();
			}

			return;
		}

		if (miniGameCanDamage(%obj, %col) == 1)
			%col.damage(%obj, %position, 2, $DamageType::Suicide);
	}
	if (VectorLen(%obj.getVelocity()) <= 3 && !isEventPending(%obj.itemProps.schedule)) //We're not planning to explode anytime soon
	{
		%obj.explode(); //Turn into item, actually. We're not exploding yet.
	}

	HEGrenadeBounceSFX.play(%position);
}

datablock ItemData(HEGrenadeItem)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/grenade.dts";
	iconName = "Add-Ons/Weapon_Package_Complex/assets/icons/grenade";
	mass = 1;
	drag = 0.2;
	density = 0.2;
	elasticity = 0.5;
	friction = 0.6;
	emap = true;

	canDrop = true;
	uiName = "HE Grenade";
	image = HEGrenadeImage;

	fuseTime = 4000;
	itemPropsClass = "HEGrenadeProps";
	itemPropsAlways = true;

	customPickupAlways = true;
	customPickupMultiple = true;

	shellCollisionThreshold = 2;
	shellCollisionSFX = HEGrenadeBounceSFX;
};

function HEGrenadeItem::onAdd(%this,%obj)
{
	parent::onAdd(%this,%obj);
	if (%obj.itemProps.pinOut || isEventPending(%obj.itemProps.schedule))
		%obj.hideNode("ring");
}

datablock ItemData(HEGrenadePinItem) {
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/grenade_pin.dts";
	rotate = false;
	mass = 0.5;
	density = 0.1;
	elasticity = 0.4;
	friction = 0.2;
	emap = true;

	uiName = "";
	canPickUp = false;

	shellCollisionThreshold = 2;
	shellCollisionSFX = GrenadePinSFX;
};

function HEGrenadePinItem::onAdd(%this, %obj) {
	parent::onAdd(%this, %obj);
	%obj.canPickUp = false;
}

datablock ShapeBaseImageData(HEGrenadeImage)
{
	shapeFile = "Add-Ons/Weapon_Package_Complex/assets/shapes/weapons/grenade.dts";

	item = HEGrenadeItem;
	armReady = 1;
	speedScale = 0.9;

	stateName[0] = "Activate";
	stateSound[0] = HEGrenadeDrawSound;
	stateSequence[0] = "activate";
	stateTimeoutValue[0] = 0.2;
	stateAllowImageChange[0] = false;
	stateTransitionOnNotLoaded[0] = "NoPin";
	stateTransitionOnTimeout[0] = "Ready";

	stateName[1] = "Ready";
	stateTransitionOnTriggerDown[1] = "Charge";
	stateTransitionOnNotLoaded[1] = "NoPin";

	stateName[2] = "NoPin";
	stateSequence[2] = "noring";
	// stateAllowImageChange[2] = false;
	stateTransitionOnTriggerDown[2] = "Charge";

	stateName[3] = "Charge";
	stateScript[3] = "onCharge";
	stateTimeoutValue[3] = 0.45;
	stateAllowImageChange[3] = false;
	stateWaitForTimeout[3] = false;
	stateTransitionOnTriggerUp[3] = "ChargeAbort";
	stateTransitionOnTimeout[3] = "ChargeReady";

	stateName[4] = "ChargeAbort";
	stateScript[4] = "onChargeAbort";
	stateTimeoutValue[4] = 0.3;
	stateAllowImageChange[4] = false;
	stateTransitionOnTimeout[4] = "Ready";

	stateName[5] = "ChargeReady";
	stateScript[5] = "onChargeReady";
	stateAllowImageChange[5] = false;
	stateTransitionOnTriggerUp[5] = "Fire";

	stateName[6] = "Fire";
	stateScript[6] = "onFire";
	stateFire[6] = true;
};

function HEGrenadeProps::onRemove(%this)
{
	cancel(%this.schedule);
}

function HEGrenadeProps::onOwnerChange(%this, %owner)
{
	if (isEventPending(%this.schedule))
		%time = getTimeRemaining(%this.schedule);
	else if (%this.pinOut)
		%time = HEGrenadeItem.fuseTime;

	Parent::onOwnerChange(%this, %owner);

	%isPlayerOwner = %owner.getType() & ($TypeMasks::PlayerObjectType);

	if (%isPlayerOwner)
		%this.lastOwningPlayer = %owner;

	if (%this.damagePlayer $= "" || (!%isPlayerOwner && isObject(%this.lastOwningPlayer)))
	{
		%this.damagePlayer = %this.lastOwningPlayer;

		if (isObject(%this.lastOwningPlayer.client))
			%this.damageClient = %this.lastOwningPlayer.client;
	}

	if (%time !$= "")
		%this.startSchedule(%time);
}

function HEGrenadeProps::startSchedule(%this, %time)
{
	cancel(%this.schedule);
	cancel(%this.beepSchedule);

	%this.schedule = %this.owner.schedule(%time, "detonateHEGrenade", %this);

	if (%time >= 1000)
		%this.beepSchedule = %this.schedule(%time - 1000, "beep");
}

function HEGrenadeProps::beep(%this)
{
	// serverPlay3D(HEGrenadeBeepSound, %this.owner.getPosition());
}

function spawnHEGrenadeExplosion(%position, %object, %damagePlayer, %damageClient)
{
	%projectile = new Projectile()
	{
		// datablock = RocketLauncherProjectile;
		datablock = HEGrenadeBlastProjectile;

		initialPosition = %position;
		initialVelocity = "0 0 0";

		sourceObject = %damagePlayer;
		sourceSlot = 0;

		client = %damageClient;
	};

	MissionCleanup.add(%projectile);
	%projectile.explode();

	HEGrenadeExplodeSFX.playFrom(%position, %object);
}

function Player::detonateHEGrenade(%this, %props)
{
	%this.tool[%props.itemSlot] = 0;
	%this.itemProps[%props.itemSlot] = 0;

	if (isObject(%this.client))
		messageClient(%this.client, 'MsgItemPickup', '', %props.itemSlot, 0);
	%this.unMountImage(0);
	spawnHEGrenadeExplosion(%this.getHackPosition(), %this, %props.damagePlayer, %props.damageClient);

	if (isObject(%props)) // player might be deleted after death
		%props.delete();
}

function Item::detonateHEGrenade(%this, %props)
{
	spawnHEGrenadeExplosion(%this.getPosition(), %this, %props.damagePlayer, %props.damageClient);
	%this.exploded = true;
	%this.delete();
}

function Projectile::detonateHEGrenade(%this, %props)
{
	spawnHEGrenadeExplosion(%this.getPosition(), %this, %props.damagePlayer, %props.damageClient);
	%this.exploded = true;
	%this.delete();
}

function HEGrenadeImage::onMount(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	if (%props.pinOut == true)
		%obj.setImageLoaded(%slot, 0);
}

function HEGrenadeImage::onUnMount(%this, %obj, %slot)
{
	%obj.playThread(2, "root");
}

function HEGrenadeImage::onCharge(%this, %obj, %slot)
{
	%obj.playThread(2, "spearReady");

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function HEGrenadeImage::onChargeReady(%this, %obj, %slot)
{
	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function HEGrenadeImage::onChargeAbort(%this, %obj, %slot)
{
	%obj.playThread(2, "root");

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function HEGrenadeImage::onFire(%this, %obj, %slot)
{
	serverPlay3D(HEGrenadeThrowSound, %obj.getMuzzlePoint(%slot));
	%obj.playThread(2, "spearThrow");
	%props = %obj.getItemProps();

	%velocity = vectorAdd(vectorScale(%obj.getEyeVector(), 20), vectorScale(%obj.getVelocity(), 0.5));

	// %item = new Item()
	// {
	//     datablock = HEGrenadeItem;
	//     position = %obj.getEyePoint();
	//     itemProps = %props;
	// };

	%item = new Projectile()
	{
		datablock = HEGrenadeProjectile;
		initialPosition = %obj.getMuzzlePoint(%slot);
		initialVelocity = %velocity;
		sourceObject = %obj;
		sourceSlot = %slot;
		client = %obj.client;
		itemProps = %props;
	};

	MissionCleanup.add(%item);

	// %item.setCollisionTimeout(%obj);
	// %item.setVelocity(%velocity);
	// %item.schedule(9000, "fadeOut");
	// %item.schedule(10000, "delete");

	%obj.itemProps[%obj.currTool] = "";

	%props.onOwnerChange(%item);

	%obj.removeTool(%obj.currTool, true);
	%obj.unMountImage(%slot);
}

function HEGrenadeImage::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	%props = %obj.getItemProps();

	if (isEventPending(%props.schedule) || %trigger != 4)
		return;

	if (%state)
	{
		%props.pinOut = true;
		%obj.playThread(2, "shiftRight");
		serverPlay3D(PinOutSound, %obj.getMuzzlePoint(0));
		%obj.setImageLoaded(%slot, 0);
		%item = new Item() {
			dataBlock = HEGrenadePinItem;
			position = %obj.getMuzzlePoint(0);
		};
		%item.setCollisionTimeout(%obj);
		%item.setVelocity(vectorAdd(%obj.getVelocity(), vectorCross("0 0 -1", vectorScale(%obj.getForwardVector(), 5))));
		%item.schedulePop();
	}
	else if (%props.pinOut)
	{
		%props.startSchedule(HEGrenadeItem.fuseTime);
		%obj.playThread(3, "shiftLeft");
		serverPlay3D(GrenadeLeverOffSound, %obj.getMuzzlePoint(0));
	}

	if (isObject(%obj.client))
		%obj.client.updateDetailedGunHelp();
}

function HEGrenadeImage::getGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();

	if (isEventPending(%props.schedule))
		return "This grenade is primed! Press Ctrl+W (or the custom 'Drop Item' key) to quickly dispose of it. Make sure you run away!";

	if (%props.pinOut)
		return "You pulled the pin and you're clutching the lever. The grenade will prime itself as you release right click or throw it (hold LMB or Ctrl+W).";

	return "To throw a grenade, either hold left click or tap Ctrl+W. To prime a grenade, tap right click. It will explode 3 seconds after being primed.";
}

function HEGrenadeImage::getDetailedGunHelp(%this, %obj, %slot)
{
	%props = %obj.getItemProps();
	%state = %obj.getImageState(%slot);

	%text = "<just:right><font:consolas:16>";

	%throw_text = "Throw   ";
	%lob_text   = "Lob     ";

	%lob_key    = "Drop Item      ";

	if (%state $= "ChargeReady")
		%throw_key = "release Primary";
	else
		%throw_key  = "hold Primary   ";

	if (%props.pinOut)
	{
		%pin_text = "Prime   ";
		%pin_key = "release Jet    ";
	}
	else
	{
		%pin_text   = "Pull Pin";
		%pin_key    = "Jet            ";
	}

	%text = "<just:right><font:consolas:16>";
	%text = %text @ "\c6" @ %throw_text @ "   " @ %throw_key @ " \n";
	%text = %text @ "\c6" @ %lob_text   @ "   " @ %lob_key   @ " \n";

	if (!isEventPending(%props.schedule))
	   %text = %text @ "\c6" @ %pin_text   @ "   " @ %pin_key   @ " \n";

	return %text;
}