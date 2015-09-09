function TimeSliceRayWeapon::onFire(%this, %obj, %slot)
{
    %count = 1;
    %lifetime = 3;
    %gravity = "0 0 0";

    %mask =
        $TypeMasks::FxBrickObjectType |
        $TypeMasks::StaticObjectType |
        $TypeMasks::PlayerObjectType |
        $TypeMAsks::VehicleObjectType;

    if (%this.fireCount !$= "")
        %count = %this.fireCount;

    if (%this.fireLifetime !$= "")
        %lifetime = %this.fireLifetime;

    if (%this.fireGravity !$= "")
        %gravity = "0 0" SPC %this.fireGravity;

    %basePosition = %obj.getMuzzlePoint(%slot);
    %baseVelocity = vectorScale(
        vectorAdd(
            vectorScale(%obj.getMuzzleVector(%slot), %this.fireMuzzleVelocity),
            vectorScale(%obj.getVelocity(), %this.fireVelInheritFactor)
        ), getWord(%obj.getScale(), 2));

    for (%i = 0; %i < %count; %i++)
    {
        %position = %basePosition;
        %velocity = %baseVelocity;

        %spread = %this.getFireSpread(%obj, %slot, %i);

        if (%spread != 0)
        {
            %scalars = getRandomScalar() SPC getRandomScalar() SPC getRandomScalar();
            %spread = vectorScale(%scalars, mDegToRad(%spread / 2));
            %velocity = matrixMulVector(matrixCreateFromEuler(%spread), %velocity);
        }

        %proj = new ScriptObject()
    	{
    		class = "ProjectileRayCast";
    		superClass = "TimedRayCast";

    		position = %position;
    		velocity = %velocity;
    		gravity = %gravity;
    		lifetime = %lifetime;
    		mask = %mask;
    		exempt = %obj;
    		sourceObject = %obj;
    		sourceClient = %obj.client;
            damageRef = %this;
    		hitExplosion = %this.fireHitExplosion;
            hitPlayerSFX = %this.fireHitPlayerSFX;
            hitOtherSFX = %this.fireHitOtherSFX;
            ricSFX = %this.fireRicSFX;
            ricChance = %this.fireRicChance $= "" ? 0.2 : %this.fireRicChance;
            nearMissSFX = %this.fireNearMissSFX;
        };

        MissionCleanup.add(%proj);
        %proj.fire();
    }
}

function ShapeBaseImageData::getFireSpread(%this, %obj, %slot, %index)
{
    if (%this.fireSpread !$= "")
        return %this.fireSpread;

    return 0;
}

function getRandomScalar()
{
	return getRandom() * 2 - 1;
}
