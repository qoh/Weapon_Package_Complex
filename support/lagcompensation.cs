function LatencyCompensator::onAdd(%this)
{
    %this.tracked = new SimSet();
    %this.snapshotIndex = 0;
}

function LatencyCompensator::onRemove(%this)
{
    %this.tracked.delete();
}

function LatencyCompensator::tick(%this)
{
    %count = %this.tracked.getCount();

    for (%i = 0; %i < %count; %i++)
        %this.tracked.getObject(%i).createSnapshot(%this.snapshotIndex);

    %this.snapshotIndex++;
    %this.snapshotIndex %= %this.snapshotCount;

    %this.schedule = %this.schedule(%this.snapshotInterval, "tick");
}

function LatencyCompensator::enterClient(%this, %client)
{
    if (!isObject(%client) || %client.isLocal() || %client.isAIControlled() || !%client.noLagComp)
        return false;

    return %this.enter(%client.getPing());
}

function LatencyCompensator::enter(%this, %ping)
{
    %window = mFloor(%ping / %this.snapshotInterval) - 1;

    if (%window <= 0)
        return false;

    %index = %this.snapshotIndex - getMin(%this.snapshotCount, %window);

    // TODO: stop using a loop here and use math instead
    while (%index < 0)
        %index += %this.snapshotCount;

    %count = %this.tracked.getCount();

    for (%i = 0; %i < %count; %i++)
        %this.tracked.getObject(%i).enterSnapshot(%index);

    return true;
}

function LatencyCompensator::exit(%this, %entered)
{
    if (!%entered)
        return;

    %count = %this.tracked.getCount();

    for (%i = 0; %i < %count; %i++)
        %this.tracked.getObject(%i).exitSnapshot();
}

function Player::createSnapshot(%this, %index)
{
    %this.isSnapshot[%index] = true;
    %this.snapshotTransform[%index] = %this.getTransform();
    // %this.snapshotScale[%index] = %this.getScale();
    // %this.snapshotCrouched[%index] = %this.isCrouched();
}

function Player::enterSnapshot(%this, %index)
{
    if (!%this.isSnapshot[%index])
        return;

    %this.originalSnapshot = true;
    %this.originalTransform = %this.getTransform();
    // %this.originalScale = %this.getScale();
    // %this.originalCrouched = %this.isCrouched();

    %this.setTransform(%this.snapshotTransform[%index]);
    // %this.setScale(%this.snapshotScale[%index]);
    //
}

function Player::exitSnapshot(%this)
{
    if (!%this.originalSnapshot)
        return;

    %this.originalSnapshot = false;
    %this.setTransform(%this.originalTransform);
    // %this.setScale(%this.originalScale);
    //
}

function serverCmdLagComp(%client, %arg)
{
    if (%arg $= "on")
        %client.noLagComp = false;
    else if (%arg $= "off")
        %client.noLagComp = true;

    if (%client.noLagComp)
        messageClient(%client, '', "\c6Lag compensation is \c0disabled \c6for you. Use \c3/LagComp on \c6to enable it.");
    else
        messageClient(%client, '', "\c6Lag compensation is \c2enabled \c6for you. Use \c3/LagComp off \c6to disable it.");
}

if (!isFunction("Armor", "onAdd"))
    eval("function Armor::onAdd(){}");

package LagCompPackage
{
    function Armor::onAdd(%this, %obj)
    {
        Parent::onAdd(%this, %obj);

        if (isObject(LagComp))
            LagComp.tracked.add(%obj);
    }
};

if (!isObject(LagComp))
{
    new ScriptObject(LagComp)
    {
        class = "LatencyCompensator";

        snapshotInterval = 25;
        snapshotCount = 20;
    };

    LagComp.tick();
}

activatePackage("LagCompPackage");
