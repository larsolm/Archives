$boomerang::speed = 512;
$boomerang::delay = 0;

datablock fxImageMapDatablock2D(Boomerang) {
   mode = full;
   textureName = "Data/Images/boomerang.png";
};

// Creates a boomerang
function CreateBoomerang() {
   $boomerang = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
   $boomerang.setSize("24 24");
   $boomerang.setPosition("0 0");
   $boomerang.setImageMap(Boomerang);
   
   // Collision information
   $boomerang.setGroup(8);
   $boomerang.setLayer(8);
   $boomerang.setCollisionActive(true, false);
   $boomerang.setCollisionMasks(BIT(1) | BIT(5) | BIT(9), BIT(1) | BIT(5) | BIT(9));
   $boomerang.setCollisionCallback(true);
   $boomerang.type = "boomerang";
   
   // The state of the boomerang - either thrown or not
   $boomerang.thrown = false;
}

function ThrowBoomerang(%val) {
   if ((%val) && (!$boomerang.thrown)) {
      // Change the boomerang's state
      $boomerang.thrown = true;

      // The boomerang is thrown the direction the guy is facing
      // That can only be one of eight ways so I'm calculating it this
      // way instead of with trig
      %rot = $guy.getRotation();
      if ((%rot < 0) && (%rot > -180)) {
         %x = -1;
      } else if ((%rot > 0) && (%rot < 180)) {
         %x = 1;
      } else {
         %x = 0;
      }
      if ((%rot > -90) && (%rot < 90)) {
         %y = -1;
      } else if ((%rot < -90) || (%rot > 90)) {
         %y = 1;
      } else {
         %y = 0;
      }
      $boomerang.setLinearVelocityX(%x * $boomerang::speed);
      $boomerang.setLinearVelocityY(%y * $boomerang::speed);
   }
}

// Called every frame to update the position of the boomerang
// This should be handled in the engine by a custom object, but I'm lazy
function UpdateBoomerang() {
   if ($boomerang.thrown) {
      %x = $boomerang.getPositionX() + (Mission.xVel * 0.01);
      %y = $boomerang.getPositionY() + (Mission.yVel * 0.01);
      $boomerang.setPosition(%x SPC %y);
      $boomerang::delay++;
      $boomerang.setRotation($boomerang.getRotation() + 15);
      %dir = vectorNormalise2D($boomerang.getPosition());
      %dir = vectorScale2D(%dir, 10);
      $boomerang.setLinearVelocity(vectorSub2D($boomerang.getLinearVelocity(), %dir));
      if (($boomerang.getPositionX() > -35) && ($boomerang.getPositionX() < 35) && ($boomerang.getPositionY() > -35) && ($boomerang.getPositionY() < 35) && ($boomerang::delay > 25)) {
         $boomerang.setLinearVelocity("0 0");
         $boomerang::delay = 0;
         $boomerang.setPosition("0 0");
         $boomerang.thrown = false;
      }
   }
}

// Called when a boomerang collides with a boundary object (wall, rock, etc)
// The boomerang is the collider (%srcObj) and the boundary is the
// collidee (%dstObj)
function CollideBoomerangBoundary(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts) {
   if ($boomerang.thrown) {
      $boomerang.setLinearVelocity(vectorScale2D(%normal, 100));
   }
}

function CollideBoomerangEnemy(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts) {
   if ($boomerang.thrown) {
      $boomerang.setLinearVelocity(vectorScale2D(%normal, 100));
      if (!%dstObj.hit) {
         EnemyHit(%dstObj, 10);
      }
   }
}
