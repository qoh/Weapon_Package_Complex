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
    %this.pathTracer = createShape(CylinderGlowShapeData, "1 1 1 0.9");
}

function TimedRayCast::onEnd(%this)
{
    if (isObject(%this.pathTracer))
        %this.pathTracer.schedule(500, "delete");

    %this.delete();
}

function TimedRayCast::fire(%this)
{
    %this.onStart();
    %this.step(0, $Sim::Time);
}

$color0 = "1 0 0";
$color1 = "1 1 0";
$color2 = "0 1 0";
$color3 = "0 1 1";
$color4 = "0 0 1";
$color5 = "1 0 1";

function TimedRayCast::step(%this, %i, %prevTime)
{
    %dt = $Sim::Time - %prevTime;

    %a = %this.position;
    %b = vectorAdd(%a, vectorScale(%this.velocity, %dt));

    if (isObject(%this.pathTracer))
        %this.pathTracer.transformLine(%a, %b, 0.01);

    // %shape = createShape(CylinderGlowShapeData, $color[%i % 6] SPC 1, 2000);
    // %shape.transformLine(%a, %b, 0.1);

    %this.position = %b;
    %this.velocity = vectorAdd(%this.velocity, vectorScale(%this.gravity, %dt));
    // %this.velocity = vectorAdd(%this.velocity, vectorScale(mSin($Sim::Time * 16) SPC 0 SPC mCos($Sim::Time * 16), 4000 * %dt));

    // initContainerRadiusSearch(%this.position, 64, $TypeMasks::PlayerObjectType);
    //
    // while (isObject(%find = containerSearchNext()))
    // {
    //     if (%find == %this.exempt)
    //         continue;
    //
    //     %this.velocity = vectorAdd(%this.velocity, vectorSub(%find.getHackPosition(), %this.position));
    //     break;
    // }

    if (isObject(LagComp) && isObject(%this.sourceClient))
        %entered = LagComp.enterClient(%this.sourceClient);

    %result = containerRayCast(%a, %b, %this.mask,
        %this.exempt, %this.exempt2, %this.exempt3,
        %this.exempt4, %this.exempt5, %this.exempt6);

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

function ProjectileRayCast::onCollision(%this, %col, %position, %normal)
{
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

    return false;
}
