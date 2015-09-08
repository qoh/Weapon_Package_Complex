function Player::addTool(%this, %data, %props)
{
    %data = %data.getID();
    %maxTools = %this.getDataBlock().maxTools;

    for (%i = 0; %i < %maxTools; %i++)
    {
        if (!%this.tool[%i])
            break;

        if (!%data.customPickupMultiple && %this.tool[%i] == %data)
        {
            %props.delete();
            return -1;
        }
    }

    if (%i == %maxTools)
    {
        %props.delete();
        return -1;
    }

    %this.tool[%i] = %data;
    %this.itemProps[%i] = %props;

    if (isObject(%props))
        %props.onOwnerChange(%this);

    if (isObject(%this.client))
    {
        messageClient(%this.client, 'MsgItemPickup', '', %i, %data);

        if (%this.currTool == %i)
            serverCmdUseTool(%this.client, %i);
    }

    return %i;
}

function Player::removeTool(%this, %index, %ignoreProps)
{
    %this.tool[%index] = 0;

    if (!%ignoreProps && isObject(%this.itemProps[%index]))
        %this.itemProps[%index].delete();

    if (isObject(%this.client))
        messageClient(%this.client, 'MsgItemPickup', '', %index, 0);

    if (%this.currTool == %index)
        %this.unMountImage(0);
}
