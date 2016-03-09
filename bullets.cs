datablock ItemData(Bullet45Item)
{
	shapeFile = "./assets/shapes/items/45_bullet.dts";
	uiName = "Shell: .45";

	mass = 0.5;
	density = 0.1;
	elasticity = 0.4;
	friction = 0.2;

	isBullet = true;
	bulletType = ".45";
	useAmmoPool = false;
	// playerCapacity = 36;
	canPickup = false;

	shellCollisionThreshold = 2;
	shellCollisionSFX = GenericShellSFX;

	amount = 1;
};

datablock ItemData(Bullet357Item : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/357_bullet.dts";
	uiName = "Shell: .357";
	bulletType = ".357";
	useAmmoPool = true;
	playerCapacity = 36;
	canPickup = false;
};

datablock ItemData(Bullet357PackItem : Bullet357Item)
{
	shapeFile = "./assets/shapes/items/357_pack.dts";
	uiName = "AmmoPack: .357";
	bulletType = ".357";
	amount = 12;
};


datablock ItemData(Bullet30Item : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/30-06bullet.dts";
	uiName = "Shell: .30-06";
	bulletType = ".30-06";
	// playerCapacity = 36;
	canPickup = false;
};

datablock ItemData(BulletBuckshotItem : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/buckshot.dts";
	uiName = "Shell: Buckshot";
	bulletType = "Buckshot";
	useAmmoPool = true;
	playerCapacity = 36;
	canPickup = false;

	shellCollisionSFX = BuckshotShellSFX;
	spentShell = BulletBuckshotSpentItem;
};

datablock ItemData(BulletBuckshotPackItem : BulletBuckshotItem)
{
	shapeFile = "./assets/shapes/items/buckshot_pack.dts";
	uiName = "AmmoPack: Buckshot";
	bulletType = "Buckshot";

	shellCollisionSFX = BuckshotShellSFX;
	spentShell = BulletBuckshotSpentItem;

	amount = 12;
};

datablock ItemData(BulletBuckshotSpentItem : BulletBuckshotItem)
{
	shapeFile = "./assets/shapes/items/buckshot_spent.dts";
	uiName = "";
	canPickup = false;
	isBullet = false;
};

function Player::ejectShell(%this, %data, %distance, %spent)
{
	if (%spent && isObject(%data.spentShell))
		%data = %data.spentShell;

	%item = new Item()
	{
		datablock = %data;
		isSpent = %spent;
	};

	if (!isObject(BulletGroup))
	{
		new SimGroup(BulletGroup);
		MissionCleanup.add(BulletGroup);
	}

	BulletGroup.add(%item);

	%item.schedulePop();

	if (%spent)
		%item.hideNode("bullet");

	%muzzleVector = %this.getMuzzleVector(0);
	%cross = vectorCross(%muzzleVector, "0 0 1");
	%position = vectorAdd(%this.getMuzzlePoint(0), vectorScale(%muzzleVector, -%distance));
	%vector = vectorAdd(vectorScale(%this.getEyeVector(), -2), "0 0 5");
	%vector = vectorAdd(%vector, %cross);

	%spread = 15;
	%scalars = getRandomScalar() SPC getRandomScalar() SPC getRandomScalar();
	%spread = vectorScale(%scalars, mDegToRad(%spread / 2));
	%matrix = matrixCreateFromEuler(%spread);

	%velocity = matrixMulVector(%matrix, %vector);
	%rotation = eulerToAxis("0 0" SPC getRandom() * 360 - 180);

	%item.setTransform(%position SPC %rotation);
	%item.setVelocity(%velocity);
	%item.setCollisionTimeout(%this);
}

function Player::addBullets(%this, %data, %count)
{
	if (%data.useAmmoPool)
	{
		%capacity = %data.playerCapacity - %this.bulletCount[%data.bulletType];
		%added = getMin(%capacity, %count);
		%count = getMax(0, %count - %capacity);

		%this.bulletCount[%data.bulletType] += %added;
	}
	else
	{
		%index = %this.findMagazineByBullet(%data);

		if (%index == -1)
		{
			// if (isObject(%this.client))
			// 	messageClient(%this.client, '', '\c6You have no magazines of the same bullet type in your inventory. Remember to take out the magazine out of your gun.');
			return 0;
		}

		if (%index == -2)
		{
			// if (isObject(%this.client))
			// 	messageClient(%this.client, '', '\c6All your magazines are full. You need to get an empty magazine first.');
			return 0;
		}

		%magazine = %this.itemProps[%index];

		%capacity = %magazine.capacity - %magazine.count;
		%added = getMin(%capacity, %count);

		%magazine.count += %added;
	}
	return %added TAB %magazine TAB %index;
}

function Item::monitorCollisionSounds(%this, %before)
{
	cancel(%this.monitorCollisionSounds);

	%data = %this.getDatablock();
	%now = vectorLen(%this.getVelocity());

	if (%before - %now >= %data.shellCollisionThreshold)
		%data.shellCollisionSFX.play(%this.getPosition());

	%this.monitorCollisionSounds = %this.schedule(50, "monitorCollisionSounds", %now);
}

function eulerToAxis(%euler)
{
	%euler = VectorScale(%euler, $pi / 180);
	return getWords(MatrixCreateFromEuler(%euler), 3, 6);
}

package ComplexBulletPackage
{
	function ItemData::onAdd(%this, %item)
	{
		Parent::onAdd(%this, %item);
		if (%this.canPickup !$= "")
			%item.canPickup = %this.canPickup;
		if (isObject(%this.shellCollisionSFX))
			%item.monitorCollisionSounds();
	}

	function Player::activateStuff(%this)
	{
		Parent::activateStuff(%this);

		%a = %this.getEyePoint();
		%b = vectorAdd(%a, vectorScale(%this.getEyeVector(), 6));

		%mask =
			$TypeMasks::StaticObjectType |
			$TypeMasks::FxBrickObjectType |
			$TypeMasks::VehicleObjectType |
			$TypeMasks::ItemObjectType;

		%ray = containerRayCast(%a, %b, %mask);
		%pos = getWords(%ray, 1, 3);
		initContainerRadiusSearch(
			%pos, 0.2,
			$TypeMasks::ItemObjectType
		);
		while (isObject(%col = containerSearchNext()))
		{
			%data = %col.getDataBlock();

			if (!%data.isBullet || %col.isSpent)
				continue;

			if (%this.bulletCount[%data.bulletType] == -1) //Infinite ammo detected
				continue;

			if (%amt = getField(%func = %this.addBullets(%data, %data.amount), 0) > 0)
			{
				RevolverInsertSFX.play(getWords(%col.getTransform(), 0, 2));
				if (isObject(%this.client))
				{
					if ($Sim::Time - %this.lastCenterPrint[%data.bulletType] <= 2)
						%amt = %this.lastAmt + %amt;
					if (%data.useAmmoPool)
						commandToClient(%this.client, 'CenterPrint', "\c2+"@ %amt @"\c3 "@ %data.bulletType @" \c6bullets.\n\c6You now have \c3" @ %this.bulletCount[%data.bulletType] @" bullets.", 2);
					else
						commandToClient(%this.client, 'CenterPrint', "\c2+"@ %amt @"\c3 "@ %data.bulletType @" \c6bullets.", 2);
					%this.lastAmt = %amt;
					%this.lastCenterPrint[%data.bulletType] = $Sim::Time;
				}
				%typeamt[%data.bulletType] += %amt;
				%col.delete();
			}
		}
	}
};

activatePackage("ComplexBulletPackage");
