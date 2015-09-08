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
