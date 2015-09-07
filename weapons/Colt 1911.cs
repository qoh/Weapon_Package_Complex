datablock ItemData(Colt1911Item)
{
    shapeFile = "base/data/shapes/printGun.dts";
    canDrop = 1;
    uiName = "Colt 1911";
    image = Colt1911Image;

    itemPropsClass = "Colt1911Props";
};

function Colt1911Props::onRemove(%this)
{
    if (isObject(%this.magazine))
        %this.magazine.delete();
}

datablock ShapeBaseImageData(Colt1911Image)
{
    shapeFile = "base/data/shapes/printGun.dts";
    item = Colt1911Item;

    stateName[0] = "Activate";
    stateTimeoutValue[0] = 0.15;
    stateTransitionOnTimeout[0] = "CheckChamber";

    stateName[1] = "CheckChamber";
    stateTransitionOnLoaded[1] = "Ready";
    stateTransitionOnNotLoaded[1] = "Empty";

    stateName[2] = "Empty";
    stateTransitionOnLoaded[2] = "Ready";
    stateTransitionOnTriggerDown[2] = "EmptyFire";

    stateName[3] = "EmptyFire";
    stateScript[3] = "onEmptyFire";
    stateTransitionOnTriggerUp[3] = "Empty";

    stateName[4] = "Ready";
    stateTransitionOnTriggerDown[4] = "Fire";

    stateName[5] = "Fire";
    stateFire[5] = true;
    stateScript[5] = "onFire";
    stateTimeoutValue[5] = 0.15;
    stateWaitForTimeout[5] = true;
    stateTransitionOnTriggerUp[5] = "CheckChamber";
};

function Player::debugColt(%this)
{
    cancel(%this.debugColt);

    if (%this.getMountedImage(0) != Colt1911Image.getID())
        return;

    %props = %this.getItemProps(Colt1911Image.getID(), 0);

    %text = %props.loaded ? "loaded-chamber" : "empty-chamber";
    %text = %text SPC (isObject(%props.magazine) ? "mag=" @ %props.magazine.count : "no-mag");
    %text = %text SPC "state=" @ %this.getImageState(0);

    clientCmdBottomPrint("\c6" @ %text, 1, 1);
    %this.debugColt = %this.schedule(16, debugColt);
}

function Colt1911Image::onMount(%this, %obj, %slot)
{
    %obj.debugColt();

    %props = %obj.getItemProps(%this, %slot);

    %obj.setImageLoaded(%slot, %props.loaded);
    %obj.setImageAmmo(%slot, isObject(%props.magazine));
}

function Colt1911Image::onLight(%this, %obj, %slot)
{
    %props = %obj.getItemProps(%this, %slot);

    if (isObject(%props.magazine))
    {
        // remove magazine
        %props.magazine.delete();
        %obj.setImageAmmo(%slot, 0);
    }
    else
    {
        // insert magazine
        %props.magazine = new ScriptObject()
        {
            count = 12;
        };

        %obj.setImageAmmo(%slot, 1);
    }

    return 1;
}

function Colt1911Image::onTrigger(%this, %obj, %slot, %trigger, %state)
{
    %props = %obj.getItemProps(%this, %slot);

    if (%trigger == 4 && %state)
    {
        talk("manual chamber");

        if (!%props.loaded && %props.magazine.count >= 1)
        {
            %props.loaded = true;
            %props.magazine.count--;

            %obj.setImageLoaded(%slot, 1);
        }

        return 1;
    }

    return 0;
}

function Colt1911Image::onEmptyFire(%this, %obj, %slot)
{
    // click
    talk("click");
}

function Colt1911Image::onFire(%this, %obj, %slot)
{
    %props = %obj.getItemProps(%this, %slot);

    if (!%props.loaded)
    {
        // shouldn't happen
        return;
    }

    // fire bullet
    talk("fire");

    // since this is a semi-auto weapon, automatically chamber the next shell
    if (%props.magazine.count >= 1)
    {
        %props.magazine.count--;
    }
    else
    {
        %props.loaded = false;
        %obj.setImageLoaded(%slot, 0);
    }
}
