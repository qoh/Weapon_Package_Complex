function strStart(%str, %search)
{
    return getSubStr(%str, 0, strlen(%search)) $= %search;
}

function strEnd(%str, %search)
{
    %len1 = strlen(%str);
    %len2 = strlen(%search);

    return strlen(%len1) >= strlen(%len2) && getSubStr(%str, %len1 - %len2, %len2) $= %search;
}

function strStartCut(%str, %search)
{
    return getSubStr(%str, strlen(%search), strlen(%str));
}

function strirpos(%str, %search)
{
    %len = strlen(%search);

    for (%i = strlen(%str) - %len; %i >= 0; %i--)
    {
        if (getSubStr(%str, %i, %len) $= %search)
            return %i;
    }

    return -1;
}

function SFXEffect::onAdd(%this)
{
    %name = %this.getName();

    if (%name $= "")
    {
        error("ERROR: SFXEffect objects must have a name!");
        %this.delete();
        return;
    }

    // If this is a duplicate, delete the old one.
    %this.setName("");
    %original = nameToID(%name);
    %this.setName(%name);

    if (%original != -1 && %original.class $= "SFXEffect")
        %original.delete();

    // Go through fields to discover all layers
    // We'll have to use local variables for everything because this looks
    // through the fields of %this, and we don't want to mess it up.
    for (%i = 0; 1; %i++)
    {
        %tag = %this.getTaggedField(%i);

        if (%tag $= "")
            break;

        %field = getField(%tag, 0);
        %value = getFields(%tag, 1, getFieldCount(%tag));

        // This part is really repetitive because we can't use fields here
        // Sorry! :P
        // Technically it could be cleaned up with globals but that's a whole
        // other can of worms
        if (strStart(%field, "file"))
        {
            %field = strStartCut(%field, "file");
            %index = strirpos(%field, "_");

            if (%index != -1)
            {
                %handle = getSubStr(%field, %index + 1, strlen(%field));
                %field = getSubStr(%field, 0, %index);
            }
            else
                %handle = "";

            if (!%isLayer[%field])
            {
                %isLayer[%field] = true;
                %layer[%maxLayer++] = %field;
            }

            %sample[%field, %maxSample[%field]++] = %handle;
        }
        else if (strStart(%field, "description"))
        {
            %field = strStartCut(%field, "description");
            %index = strirpos(%field, "_");

            if (%index != -1)
            {
                %handle = getSubStr(%field, %index + 1, strlen(%field));
                %field = getSubStr(%field, 0, %index);
            }

            if (!%isLayer[%field])
            {
                %isLayer[%field] = true;
                %layer[%maxLayer++] = %field;
            }

            %sample[%field, %maxSample[%field]++] = %handle;
        }
        else if (strStart(%field, "profile"))
        {
            %field = strStartCut(%field, "profile");
            %index = strirpos(%field, "_");

            if (%index != -1)
            {
                %handle = getSubStr(%field, %index + 1, strlen(%field));
                %field = getSubStr(%field, 0, %index);
            }

            if (!%isLayer[%field])
            {
                %isLayer[%field] = true;
                %layer[%maxLayer++] = %field;
            }

            %sample[%field, %maxSample[%field]++] = %handle;
        }
        // Now that I think about it, maybe globals would be better...
        // else if (strStart(%field, "useSource2D"))
        // {
        //     %field = strStartCut(%field, "useSource2D");
        //
        //     if (!%isLayer[%field])
        //     {
        //         %isLayer[%field] = true;
        //         %layer[%maxLayer++] = %field;
        //     }
        // }
        // else if (strStart(%field, "distanceThreshold"))
        // {
        //     %field = strStartCut(%field, "distanceThreshold");
        //
        //     if (!%isLayer[%field])
        //     {
        //         %isLayer[%field] = true;
        //         %layer[%maxLayer++] = %field;
        //     }
        // }
    }

    // Thank god that's over, now we can start making a mess with fields!
    %this.layerCount = 0;

    for (%layerIndex = 1; %layerIndex <= %maxLayer; %layerIndex++)
    {
        %layerName = %layer[%layerIndex];
        %layerMaxSample = %maxSample[%layerName];

        if (%layerMaxSample < 1)
        {
            // TODO: Do this after trying to load samples so that invalid samples don't count
            warn("SFXEffect " @ %name @ " has no samples on layer " @ %layerName @ ", ignoring layer...");
            continue;
        }

        %this.layerName[%this.layerCount] = %layerName;
        %this.layerCount++;
        %this.sampleCount[%layerName] = 0;

        for (%sampleIndex = 1; %sampleIndex <= %layerMaxSample; %sampleIndex++)
        {
            %sampleName = %sample[%layerName, %sampleIndex];

            if (%this.profile[%layerName, %sampleName] $= "")
            {
                if (%this.file[%layerName, %sampleName] $= "")
                {
                    warn("SFXEffect " @ %name @ " has no file or profile for sample " @ %sampleName @ " on layer " @ %layerName @ ", ignoring sample...");
                    continue;
                }

                %fileName = %this.file[%layerName, %sampleName];
                %description = AudioClose3d;
                %preload = true;

                if (%this.description[%layerName, %sampleName] !$= "")
                    %description = %this.description[%layerName, %sampleName];

                // Should probably getSafeVariableName the %name
                %profileName = "__SFXEffect_AudioProfile_" @ %name @ "_" @ getSafeVariableName(%layerName) @ "_" @ getSafeVariableName(%sampleName);
                %profile = nameToID(%profileName);

                if (!isObject(%profile))
                {
                    %profileName = "__SFXEffect_AudioProfile_" @ getSafeVariableName(%fileName) @ "_" @ getSafeVariableName(%description.getName());
                    echo("making " @ %profileName);
                    eval("datablock AudioProfile(" @ %profileName @ "){fileName=%fileName;description=%description;preload=%preload;};");
                    %profile = nameToID(%profileName);
                }

                if (!isObject(%profile))
                {
                    warn("SFXEffect " @ %name @ " failed to create profile for sample " @ %sampleName @ " on layer " @ %layerName @ ", ignoring sample...");
                    continue;
                }

                %this.profile[%layerName, %sampleName] = %profile;
            }

            %this.sampleHandle[%layerName, %this.sampleCount[%layerName]] = %sampleName;
            %this.sampleCount[%layerName]++;
        }
    }
}

function SFXEffect::playTo(%this, %client, %position, %source)
{
    %control = %client.getControlObject();

    if (!isObject(%control))
    {
        %client.debugSFX(%this, "not playing: no control object");
        return;
    }

    %isSource = %client == %source;
    %distance = vectorDist(%position, %control.getPosition());

    for (%i = 0; %i < %this.layerCount; %i++)
    {
        %layerName = %this.layerName[%i];

        if (%this.sampleCount[%layerName] < 1)
        {
            %client.debugSFX(%this, "not playing " @ %layerName @ ": no samples");
            continue;
        }

        switch$ (%this.filterListener[%layerName])
        {
            case "source": if (!%isSource) { %client.debugSFX(%this, "not playing " @ %layerName @ ": filterListener"); continue; }
            case "other": if (%isSource) { %client.debugSFX(%this, "not playing " @ %layerName @ ": filterListener"); continue; }
        }

        if (%this.filterDistanceMin[%layerName] !$= "" && %distance < %this.filterDistanceMin[%layerName])
        {
            %client.debugSFX(%this, "not playing " @ %layerName @ ": filterDistanceMin" SPC %distance);
            continue;
        }

        if (%this.filterDistanceMax[%layerName] !$= "" && %distance > %this.filterDistanceMax[%layerName])
        {
            %client.debugSFX(%this, "not playing " @ %layerName @ ": filterDistanceMax" SPC %distance);
            continue;
        }

        %use2D = false;

        switch$ (%this.use2D[%layerName])
        {
            case "always" or "1": %use2D = true;
            case "source": %use2D = %isSource;
            case "other": %use2D = !%isSource;
            default: %use2D = false;
        }

        %sampleIndex = getRandom(%this.sampleCount[%layerName] - 1);
        %sampleHandle = %this.sampleHandle[%layerName, %sampleIndex];

        %profile = %this.profile[%layerName, %sampleHandle];

        if (!isObject(%profile))
        {
            %client.debugSFX(%this, "not playing " @ %layerName @ ": invalid sample " @ %sampleHAndle);
            continue;
        }

        %client.debugSFX(%this, "playing " @ %layerName @ ": " @ %use2D SPC %sampleHandle);

        if (%use2D)
            %client.play2D(%profile);
        else
            %client.play3D(%profile, %position);
    }
}

function SFXEffect::play(%this, %position, %source)
{
    %count = ClientGroup.getCount();

    for (%i = 0; %i < %count; %i++)
        %this.playTo(ClientGroup.getObject(%i), %position, %source);
}

function SFXEffect::playFrom(%this, %position, %object)
{
    if (!isFunction(%object.getClassName(), "getControllingClient"))
        return %this.play(%position, 0);

    return %this.play(%position, %object.getControllingClient());
}

function GameConnection::debugSFX(%this, %sfx, %text)
{
    if (%this.debugSFX || %this.debugSFX[%sfx.getName()])
        messageClient(%this, '', "\c4debugSFX (" @ %sfx.getName() @ ")\c6: " @ %text);
}
