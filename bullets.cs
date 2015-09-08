datablock ItemData(Bullet45Item)
{
	shapeFile = "./assets/shapes/items/45_bullet.dts";
    uiName = "Shell: .45";
    isBullet = true;
	mass = 0.5;
	density = 0.1;
	elasticity = 0.4;
	friction = 0.2;
};

function Player::ejectShell(%this, %data, %distance, %spent)
{
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

function Item::monitorShellVelocity(%this, %before)
{
	cancel(%this.monitorShellVelocity);
	%now = vectorLen(%this.getVelocity());

	if (%before - %now >= 2)
		serverPlay3D("advReloadCasingSound" @ getRandom(1, 3), %this.getPosition());

	%this.monitorShellVelocity = %this.schedule(50, "monitorShellVelocity", %now);
}

function eulerToAxis(%euler)
{
	%euler = VectorScale(%euler,$pi / 180);
	%matrix = MatrixCreateFromEuler(%euler);
	return getWords(%matrix,3,6);
}
