function ShapeBaseImageData::getStateIndex(%this, %name)
{
    for (%i = 0; %i < 31; %i++)
    {
        if (%this.stateName[%i] $= %name)
            return %i;
    }

    return -1;
}

function Player::getSpeedScale(%this)
{
    %scale = 1;
    return %scale;

    for (%i = 0; %i < 4; %i++)
    {
        %image = %this.getMountedImage(%i);

        if (%image.speedScale !$= "")
            %scale *= %image.speedScale;

        %state = %this.getImageState(%i);

        if (!isObject(%image) || %state $= "")
            continue;

        %index = %image.getStateIndex(%state);

        if (%index != -1 && %image.stateSpeedScale[%index] !$= "")
            %scale *= %image.stateSpeedScale[%index];
    }

    return %scale;
}

function Player::updateSpeedScale(%this)
{
    %data = %this.getDataBlock();
    %scale = %this.getSpeedScale();

    %this.setMaxForwardSpeed(%data.maxForwardSpeed * %scale);
    %this.setMaxBackwardSpeed(%data.maxBackwardSpeed * %scale);
    %this.setMaxSideSpeed(%data.maxSideSpeed * %scale);
    %this.setMaxCrouchForwardSpeed(%data.maxForwardCrouchSpeed * %scale);
    %this.setMaxCrouchBackwardSpeed(%data.maxBackwardCrouchSpeed * %scale);
    %this.setMaxCrouchSideSpeed(%data.maxSideCrouchSpeed * %scale);
    %this.setMaxUnderwaterForwardSpeed(%data.maxUnderwaterForwardSpeed * %scale);
    %this.setMaxUnderwaterBackwardSpeed(%data.maxUnderwaterBackwardSpeed * %scale);
    %this.setMaxUnderwaterSideSpeed(%data.maxUnderwaterSideSpeed * %scale);
}

package SpeedScalePackage
{
    function Player::mountImage(%this, %image, %slot, %loaded, %skinTag)
    {
        Parent::mountImage(%this, %image, %slot, %loaded, %skinTag);
        %this.updateSpeedScale();
    }

    function Player::unMountImage(%this, %slot)
    {
        Parent::unMountImage(%this, %slot);
        %this.updateSpeedScale();
    }
};

activatePackage("SpeedScalePackage");
