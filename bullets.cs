datablock ItemData(Bullet45Item)
{
	shapeFile = "./assets/shapes/items/45_bullet.dts";
	uiName = "Shell: .45";
	isBullet = true;
	mass = 0.5;
	density = 0.1;
	elasticity = 0.4;
	friction = 0.2;

	shellCollisionThreshold = 2;
	shellCollisionSFX = GenericShellSFX;
};

datablock ItemData(Bullet357Item : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/357_bullet.dts";
	uiName = "Shell: .357";
};

datablock ItemData(Bullet30Item : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/30-06bullet.dts";
	uiName = "Shell: .30-06";
};

datablock ItemData(BulletBuckshotItem : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/buckshot.dts";
	uiName = "Shell: Buckshot";
	shellCollisionSFX = BuckshotShellSFX;

	spentShell = BulletBuckshotSpentItem;
};

datablock ItemData(BulletBuckshotSpentItem : BulletBuckshotItem)
{
	shapeFile = "./assets/shapes/items/buckshot_spent.dts";
	uiName = "";
};

datablock ItemData(Bullet357Item : Bullet45Item)
{
	shapeFile = "./assets/shapes/items/357_bullet.dts";
	uiName = "Shell: .357";
};

function Player::ejectShell(%this, %data, %distance, %spent)
{
	if(%data.spentShell !$= "" && %spent)
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

	%item.schedule(14000, "fadeOut");
	%item.schedule(15000, "delete");

	%item.canPickup = 0;

	if (%spent)
	{
		%item.hideNode("bullet");
		// %item.setShapeName("Spent");
		// %item.setShapeNameColor("1 0 0 1");
	}
	// else
	// {
	//     %item.setShapeName("Unspent");
	//     %item.setShapeNameColor("0 1 0 1");
	// }

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
	// %scalars = getRandomScalar() SPC getRandomScalar() SPC getRandomScalar();
	// %spread = vectorScale(%scalars, mDegToRad(15 / 2));
	// %vector = vectorAdd(vectorScale(%this.getEyeVector(), -%distance), "0 0 5");
	//
	// %velocity = matrixMulVector(matrixCreateFromEuler(%spread), %vector);
	// %position = vectorAdd(%this.getMuzzlePoint(0), vectorScale(%this.getMuzzleVector(0), -%distance));
	// %rotation = eulerToAxis("0 0" SPC getRandom() * 360 - 180);
	//
	// %item.setVelocity(%velocity);
	// %item.setTransform(%position SPC %rotation);
}

function Item::monitorCollisionSounds(%this, %before)
{
	cancel(%this.monitorCollisionSounds);
	%now = vectorLen(%this.getVelocity());
	%db = %this.getDatablock();

	if (%before - %now >= %db.shellCollisionThreshold)
		%db.shellCollisionSFX.play(%this.getPosition());

	%this.monitorCollisionSounds = %this.schedule(50, "monitorCollisionSounds", %now);
}

function eulerToAxis(%euler)
{
	%euler = VectorScale(%euler,$pi / 180);
	%matrix = MatrixCreateFromEuler(%euler);
	return getWords(%matrix,3,6);
}

package shellSoundPackage
{
	function ItemData::onAdd(%this, %item)
	{
		parent::onAdd(%this, %item);
		if(%this.shellCollisionSFX !$= "")
			%item.monitorCollisionSounds();
	}
};
activatePackage(shellSoundPackage);