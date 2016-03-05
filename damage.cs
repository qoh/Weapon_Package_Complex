datablock ItemData(EmptyItem)
{
  shapeFile = "base/data/shapes/empty.dts";

  gravityMod = 0;
  friction = 0;
};

function EmptyItem::onCollision(%this, %obj, %col)
{
  %obj.setCollisionTimeout(%col);
}

function EmptyItem::onPickup()
{
}

function Player::displayDamage(%this)
{
  cancel(%this.displayDamage);

  %damage = %this.receivedDamage;
  %this.receivedDamage = 0;

  if (%damage < 6)
  {
    return;
  }

  %obj = new Item()
  {
    datablock = EmptyItem;
  };

  MissionCleanup.add(%obj);

  %obj.setTransform(%this.getHackPosition());
  %obj.setVelocity("0 0 1.6");

  %obj.setCollisionTimeout(%this);
  %obj.schedule(3000, delete);

  if (%damage < 0)
  {
    %obj.setShapeName("+" @ mFloor(mAbs(%damage)));
    %obj.setShapeNameColor("0 1 0");
  }
  else
  {
    %r = mClampF(%damage / 50, 0, 1);
    %b = 1 - mClampF((%damage - 50) / 50, 0, 1);

    %obj.setShapeName("-" @ mFloor(%damage));
    %obj.setShapeNameColor(%r SPC 200 SPC %b);
  }
}

package DamageIndicatorPackage
{
  function Armor::damage(%this, %obj, %source, %pos, %damage, %type)
  {
    cancel(%obj.displayDamage);

    %obj.receivedDamage += %damage;
    %obj.displayDamage = %obj.schedule(0, "displayDamage");

    Parent::damage(%this, %obj, %source, %pos, %damage, %type);
  }
};

// activatePackage("DamageIndicatorPackage");
