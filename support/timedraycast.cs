// Usage (in ::onFire):
//
// %proj = new ScriptObject() {
//   class = "ProjectileRayCast";
//   superClass = "TimedRayCast";
//   position = %obj.getMuzzlePoint(0);
//   velocity = vectorScale(%obj.getMuzzleVector(0), 400);
//   lifetime = 2;
//   mask = $TypeMasks::PlayerObjectType | $TypeMasks::FxBrickObjectType;
//   exempt = %obj;
//   sourceObject = %obj;
//   sourceClient = %obj.client;
//   damage = 10;
//   damageType = $DamageType::Generic;
// };
//
// MissionCleanup.add(%proj);
// %proj.fire();

// exec("./shapes/debug/init.cs");

function TimedRayCast::onAdd(%this)
{
    if (%this.tickInterval $= "")
        %this.tickInterval = 16;
}

function TimedRayCast::onCollision(%this, %col, %position, %normal)
{
    return false;
}

function TimedRayCast::onMiss(%this)
{
}

function TimedRayCast::onStart(%this)
{
    // %this.pathTracer = createShape(CylinderGlowShapeData, "1 1 1 0.9");
    %this.pathTracer = createShape(BulletShapeData, "1 1 0.5 1");
    %a = %this.position;
    %b = vectorAdd(%a, vectorScale(%this.velocity, %this.tickInterval / 1000));
    %result = containerRayCast(%a, %b, %this.mask,
        %this.exempt, %this.exempt2, %this.exempt3,
        %this.exempt4, %this.exempt5, %this.exempt6);
    if(%result)
        %newpos = getWords(%result, 1, 3);
    else
        %newpos = %b;
    %this.pathTracer.transformLine(%a, %newpos, 0.5);
}

function TimedRayCast::onEnd(%this)
{
    if (isObject(%this.pathTracer))
        %this.pathTracer.schedule(64, "delete");

    %this.delete();
}

function TimedRayCast::onMove(%this, %a, %b, %ray)
{
    if (isObject(%this.pathTracer))
        %this.pathTracer.transformLine(%a, %b, 0.5);
}

function TimedRayCast::fire(%this)
{
    %this.onStart();
    %this.step(0, $Sim::Time);
}

function TimedRayCast::step(%this, %i, %prevTime)
{
    %dt = $Sim::Time - %prevTime;

    %a = %this.position;
    %b = vectorAdd(%a, vectorScale(%this.velocity, %dt));

    %this.position = %b;
    %this.velocity = vectorAdd(%this.velocity, vectorScale(%this.gravity, %dt));

    if (isObject(LagComp) && isObject(%this.sourceClient))
        %entered = LagComp.enterClient(%this.sourceClient);

    %result = containerRayCast(%a, %b, %this.mask,
        %this.exempt, %this.exempt2, %this.exempt3,
        %this.exempt4, %this.exempt5, %this.exempt6);

    if (%dt > 0)
    {
        if (%result)
            %bb = getWords(%result, 1, 3);
        else
            %bb = %b;

        %this.onMove(%a, %bb, %result, %i);
    }

    if (%entered)
        LagComp.exit(%entered);

    %hitObj = getWord(%result, 0);
    %hitPos = getWords(%result, 1, 3);
    %hitVec = getWords(%result, 4, 6);

    if (%result && !%this.onCollision(%hitObj, %hitPos, %hitVec))
    {
        %this.onEnd();
        return;
    }

    %this.lifetime -= %dt;

    if (%this.lifetime <= 0)
    {
        %this.onMiss();
        %this.onEnd();
        return;
    }

    %this.schedule(%this.tickInterval, "step", %i + 1, $Sim::Time);
}

exec("Add-Ons/Weapon_RayGun/shapes/debug/init.cs");

$color0 = "1 0 0";
$color1 = "1 1 0";
$color2 = "0 1 0";
$color3 = "0 1 1";
$color4 = "0 0 1";
$color5 = "1 0 1";

function ProjectileRayCast::onMove(%this, %a, %b, %ray, %i)
{
    // %shape = createShape(CylinderGlowShapeData, $color[%i % 6] SPC 1, 500);
    // %shape.transformLine(%a, %b, 0.1);
    // if (isObject(%this.pathTracer) && %dt != 0)
    //     %this.pathTracer.transformLine(%a, %b, 0.5);

    %maxDist = 2.5;

    if (isObject(%this.nearMissSFX))
    {
        %center = vectorScale(vectorAdd(%a, %b), 0.5);
        %length = vectorLen(%center);

        initContainerRadiusSearch(%center, %length + %maxDist * 32, $TypeMasks::PlayerObjectType);

        while (isObject(%obj = containerSearchNext()))
        {
            if (%obj == %this.exempt || %obj == firstWord(%ray) || !isObject(%obj.client))
                continue;

            %p = %obj.getEyePoint();
            %ab = vectorSub(%b, %a);
            %ap = vectorSub(%p, %a);

            %project = vectorDot(%ap, %ab) / vectorDot(%ab, %ab);

            if (%project < 0 || %project > 1)
                continue;

            %j = vectorAdd(%a, vectorScale(%ab, %project));
            %distance = vectorDist(%p, %j);

            // %shape = createShape(CylinderGlowShapeData, %distance <= %maxDist ? "1 1 1 1" : "0 0 0 1", 4000);
            // %shape.transformLine(%p, %j, 0.1);

            if (%distance <= %maxDist)
                ComplexNearMissSFX.playTo(%obj.client, %center);
        }
    }

    Parent::onMove(%this, %a, %b, %ray);
}

function ProjectileRayCast::onCollision(%this, %col, %position, %normal)
{
    %isPlayer = %col.getType() & $TypeMasks::PlayerObjectType;

    if (isObject(%this.hitExplosion))
    {
        %projectile = new Projectile()
        {
            datablock = %this.hitExplosion;
            initialPosition = %position;
            initialVelocity = "0 0 0";
        };

        MissionCleanup.add(%projectile);
        %projectile.explode();
    }

    if (isObject(%this.hitSound))
        serverPlay3D(%this.hitSound, %position);

    // if (isObject(%this.hitPlayerSFX) && %isPlayer)
    //     %this.hitPlayerSFX.playFrom(%position, %col);

    if (isObject(%this.hitOtherSFX) && !%isPlayer)
        %this.hitOtherSFX.playFrom(%position, %col);

    if (isObject(%this.ricSFX) && !%isPlayer && getRandom() < %this.ricChance)
        %this.ricSFX.playFrom(%position, %col);

    if (isObject(%this.hitDecal))
        %doNothing = 1;
        // spawnDecal(...);

    if (%col.getType() & ($TypeMasks::PlayerObjectType | $TypeMasks::VehicleObjectType))
    {
        if (miniGameCanDamage(%this.sourceClient, %col) == 1)
        {
            if (isObject(%this.damageRef) && isFunction(%this.damageRef.getName(), "damage"))
                %this.damageRef.damage(%this.sourceObject, %col, %position, %normal);
            else
                %col.damage(%this.sourceObject, %position, %this.damage, %this.damageType);
        }
    }

    if (isObject(%this.damageRef) && isFunction(%this.damageRef.getName(), "onCollision"))
        return %this.damageRef.onCollision(%this, %col, %position, %normal);

    return false;
}
