function ShapeBaseImageData::onTrigger(%this, %obj, %slot, %trigger, %state)
{
	return 0;
}

function ShapeBaseImageData::onLight(%this, %obj, %slot)
{
	return 0;
}

function ShapeBaseImageData::onSuicide(%this, %obj, %slot)
{
	return 0;
}

function ShapeBaseImageData::onDrop(%this, %obj, %slot)
{
	return 0;
}

package ImageTriggerPackage
{
	function serverCmdLight(%client)
	{
		%player = %client.player;

		if (isObject(%player) && !isObject(%player.light))
		{
			for (%i = 0; %i < 4; %i++)
			{
				%image = %player.getMountedImage(%i);

				if (isObject(%image) && %image.onLight(%player, %i))
					%stop = 1;
			}
		}

		if (!%stop)
			Parent::serverCmdLight(%client);
	}

	function serverCmdSuicide(%client)
	{
		%player = %client.player;

		if (isObject(%player))
		{
			for (%i = 0; %i < 4; %i++)
			{
				%image = %player.getMountedImage(%i);

				if (isObject(%image) && %image.onSuicide(%player, %i))
					%stop = 1;
			}
		}

		if (!%stop)
			Parent::serverCmdSuicide(%client);
	}

	function Armor::onTrigger(%this, %obj, %slot, %state)
	{
		for (%i = 0; %i < 4; %i++)
		{
			%image = %obj.getMountedImage(%i);

			if (isObject(%image) && %image.onTrigger(%obj, %i, %slot, %state))
				%stop = 1;
		}

		if (!%stop)
			Parent::onTrigger(%this, %obj, %slot, %state);
	}

	function serverCmdDropTool(%client, %slot) {
		%player = %client.player;
		if (isObject(%player)) {
			for (%i = 0; %i < 4; %i++)
			{
				%image = %player.getMountedImage(%i);
				if (isObject(%image) && %player.tool[%slot] == %image.item && %image.onDrop(%obj, %i, %slot))
					%stop = 1;
			}
		}
		if (!%stop)
			parent::serverCmdDropTool(%client, %slot);
	}
};

activatePackage("ImageTriggerPackage");
