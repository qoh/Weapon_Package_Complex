// by Port

package TimedCustomFire
{
	function WeaponImage::onFire(%this, %obj, %slot)
	{
		if (!%this.timedCustomFire)
			return Parent::onFire(%this, %obj, %slot);

		%obj.hasShotOnce = 1;

		if (%this.minShotTime > 0)
		{
			if (getSimTime() - %obj.lastFireTime < %this.minShotTime)
				return 0;

			%obj.lastFireTime = getSimTime();
		}

		%client = %obj.client;

		if (isObject(%client.miniGame))
		{
			if (getSimTime() - %client.lastF8Time < 3000)
				return 0;
		}

		%muzzlePoint = %obj.getMuzzlePoint(%slot);
		%muzzleVector = %obj.getMuzzleVector(%slot);

		%eyePoint = %obj.getEyePoint();
		%eyeVector = %obj.getEyeVector();

		%muzzleVelocity = %this.fireSpeed;

		if (%this.melee)
		{
			%origin = %eyePoint;
			%vector = vectorScale(%obj.getMuzzleVector(%slot), 20);
			%mask =
				$TypeMasks::StaticObjectType |
				$TypeMasks::StaticShapeObjectType |
				$TypeMasks::FxBrickObjectType |
				$TypeMasks::VehicleObjectType |
				$TypeMasks::PlayerObjectType;

			%ray = containerRayCast(%eyePoint, vectorAdd(%eyePoint, %vector), %mask, %obj);

			if (%ray)
			{
				%pos = getWords(%ray, 1, 3);

				%diffEye = vectorDist(%eyePoint, %pos);
				%diffMuzzle = vectorDist(%muzzlePoint, %pos);

				%muzzleVelocity *= %diffEye / %diffMuzzle;
			}
		}
		else
		{
			%origin = %muzzlePoint;

			if (%obj.isFirstPerson())
			{
				%vector = vectorScale(%eyeVector, 5);
				%mount = %obj.getObjectMount();

				%mask =
					$TypeMasks::StaticObjectType |
					$TypeMasks::StaticShapeObjectType |
					$TypeMasks::FxBrickObjectType |
					$TypeMasks::VehicleObjectType |
					$TypeMasks::PlayerObjectType;

				%ray = containerRayCast(%eyePoint, vectorAdd(%eyePoint, %vector), %mask, %mount, %obj);

				if (%ray)
				{
					%pos = getWords(%ray, 1, 3);
					%targetVector = vectorSub(%pos, %eyePoint);
					//%eyeToMuzzle = vectorSub(%start, %origin);
					if (vectorLen(%targetVector) < 3.1)
					{
						%muzzleVector = %obj.getEyeVector();
						%origin = %eyePoint;
					}
				}
			}
		}

		%playerVelocity = %obj.getVelocity();
		%dot = vectorDot(%eyeVector, %obj.getMuzzleVector(%slot));

		if (%dot < 0 && vectorLen(%playerVelocity) < 14)
			%inheritFactor = 0;
		else if (isFunction(%this.getName(), "getInheritFactor"))
			%inheritFactor = %this.getInheritFactor(%obj, %slot);
		else
			%inheritFactor = %this.velInheritFactor;

		if (isFunction(%this.getName(), "getProjectileCount"))
			%count = %this.getProjectileCount();
		else
			%count = %this.projectileCount $= "" ? 1 : %this.projectileCount;

		%baseVector = %muzzleVector;

		for (%i = 0; %i < %count; %i++)
		{
			if (isFunction(%this.getName(), "getProjectileSpread"))
				%spread = %this.getProjectileSpread(%obj, %slot);
			else
				%spread = %this.projectileSpread;

			%scalars = getRandomScalar() SPC getRandomScalar() SPC getRandomScalar();
			%spread = vectorScale(%scalars, mDegToRad(%spread / 2));

			%matrix = matrixCreateFromEuler(%spread);
			%vector = matrixMulVector(%matrix, %baseVector);

			%velocity = %this.fireSpeed * getWord(%obj.getScale(), 2);
			%velocity = vectorScale(%vector, %velocity);
			%velocity = vectorAdd(%velocity, vectorScale(%playerVelocity, %inheritFactor));

			// %proj = new ScriptObject()
			// {
			// 	datablock = %data;
			//
			// 	initialVelocity = %velocity;
			// 	initialPosition = %origin;
			//
			// 	sourceObject = %obj;
			// 	sourceSlot = %slot;
			//
			// 	client = %client;
			// };
			//
			// MissionCleanup.add(%projectile);
			// %projectile.setScale(%obj.getScale());

			// talk("sourceObject =" SPC %obj SPC %obj.getClassName());
			// talk("sourceClient =" SPC %obj.client SPC %obj.client.getClassName());

			%proj = new ScriptObject()
			{
				class = "ProjectileRayCast";
				superClass = "TimedRayCast";

				position = %origin;
				velocity = %velocity;

				lifetime = %this.fireLifetime;
				gravity = %this.fireGravity;

				mask = %this.fireMask $= "" ? $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType | $TypeMasks::TerrainObjectType : %this.fireMask;
				exempt = %obj;
				sourceObject = %obj;
				sourceClient = %obj.client;
				damage = %this.directDamage;
				damageType = %this.directDamageType;
				damageRef = %this;
				hitExplosion = %this.projectile;
			};

			MissionCleanup.add(%proj);
			%proj.fire();
		}
	}
};

activatePackage("TimedCustomFire");

function getRandomScalar()
{
	return getRandom() * 2 - 1;
}
