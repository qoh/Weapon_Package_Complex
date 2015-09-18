datablock ItemData(Bullet45Item)
{
	shapeFile = "./assets/shapes/items/45_bullet.dts";
	uiName = "Shell: .45";

	mass = 0.5;
	density = 0.1;
	elasticity = 0.4;
	friction = 0.2;

	isBullet = true;
	playerCapacity = 36;

	shellCollisionThreshold = 2;
	shellCollisionSFX = GenericShellSFX;
};

datablock ItemData(Bullet357Item : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/357_bullet.dts";
	uiName = "Shell: .357";

	playerCapacity = 36;
};

datablock ItemData(Bullet30Item : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/30-06bullet.dts";
	uiName = "Shell: .30-06";

	playerCapacity = 36;
};

datablock ItemData(BulletBuckshotItem : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/buckshot.dts";
	uiName = "Shell: Buckshot";

	playerCapacity = 36;

	shellCollisionSFX = BuckshotShellSFX;
	spentShell = BulletBuckshotSpentItem;
};

datablock ItemData(BulletBuckshotSpentItem : BulletBuckshotItem)
{
	shapeFile = "./assets/shapes/items/buckshot_spent.dts";
	uiName = "";

	isBullet = false;
};

datablock ItemData(Bullet357Item : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/357_bullet.dts";
	uiName = "Shell: .357";

	playerCapacity = 36;
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
	%item.canPickup = 0;

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
	%capacity = %data.playerCapacity - %this.bulletCount[%data];
	%added = getMin(%capacity, %count);
	%count = getMax(0, %count - %capacity);

	%this.bulletCount[%data] += %added;

	if (%added > 0)
	{
		//
	}

	return %count;
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

		if (!isObject(%ray) || !(%ray.getType() & $TypeMasks::ItemObjectType))
			return;

		%data = %ray.getDataBlock();

		if (!%data.isBullet || %ray.spent)
			return;

		if (%this.addBullets(%data, 1) == 0)
			%ray.delete();
	}
};

activatePackage("ComplexBulletPackage");
