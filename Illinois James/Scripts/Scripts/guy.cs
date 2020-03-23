$guy::speed = 125;
$leftDown = 0;
$rightDown = 0;
$upDown = 0;
$downDown = 0;

function UnHit() {
   $guy.hit = false;
}

function UnMove() {
   $guy.move = true;
   if ($leftDown) OnLeft(1);
   if ($rightDown) OnRight(1);
   if ($upDown) OnUp(1);
   if ($downDown) OnDown(1);
}

function Respawn() {
   $guy.move = true;
   $guy.dead = false;
   $guy.hit = true;
   schedule(2000, 0, UnHit);
   $guy.life = 100;
   LifeBar.extent = "100 25";
   $guy.playAnimation(GuyStand);
}

function GuyHit(%power) {
   if (!$guy.hit) {
      $guy.life = $guy.life - %power;
      LifeBar.extent = $guy.life SPC "25";
      $guy.hit = true;
      $guy.move = false;
      if ($guy.life < 1) {
         GuyDie();
      } else {
         schedule(500, 0, UnMove);
         schedule(1000, 0, UnHit);
         $guy.playAnimation(GuyHit);
      }
   }
}

function GuyDie() {
   $guy.move = false;
   $guy.dead = true;
   $guy.playAnimation(GuyDie);
   schedule(5000, 0, Respawn);
}   

datablock fxImageMapDatablock2D(Guy) {
   mode = cell;
   cellWidth = 128;
   cellHeight = 128;
   textureName = "Data/Images/illinoisJames.png";
};

datablock fxAnimationDatablock2D(GuyStand) {
   imageMap = Guy;
   animationFrames = 0;
   animationTime = 1;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(GuyWalk) {
   imageMap = Guy;
   animationFrames = "0 1 2 3";
   animationTime = 0.25;
   animationCycle = 1;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(GuyHit) {
   imageMap = Guy;
   animationFrames = "4 5 6 7";
   animationTime = 0.25;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(GuyDie) {
   imageMap = Guy;
   animationFrames = "8 9 10 11 12 13 14 15";
   animationTime = 0.5;
   animationCycle = 0;
   randomStart = 0;
};

function CreateGuy() {
   $guy = new fxAnimatedSprite2D() {
      scenegraph = MainSceneGraph;
   };
   $guy.setPosition("0 0");
   $guy.setSize("48 48");
   $guy.setGroup(6);
   $guy.setLayer(6);
   $guy.setCollisionActive(true, true);
   $guy.setCollisionScale(".75 .75");
   $guy.setCollisionMasks(BIT(1) | BIT(5) | BIT(9), BIT(1) | BIT(5) | BIT(9));
   $guy.setCollisionCallback(true);
   $guy.playAnimation(GuyStand);
   $guy.hit = false;
   $guy.dead = false;
   $guy.life = 100;
   $guy.move = true;
}

function StopWorldLeft() {
   if (Mission.xVel > 0) {
      Mission.xVel = 0;
      %count = Mission.getCount();
      for(%i = 0; %i < %count; %i++) {
         %obj = Mission.getObject(%i);
         %obj.setLinearVelocityX(0);
      }
   }
}

function StopWorldUp() {
   if (Mission.yVel > 0) {
      %count = Mission.getCount();
      for(%i = 0; %i < %count; %i++) {
         %obj = Mission.getObject(%i);
         Mission.yVel = 0;
         %obj.setLinearVelocityY(0);
      }
   }
}

function StopWorldRight() {
   if (Mission.xVel < 0) {
      %count = Mission.getCount();
      for(%i = 0; %i < %count; %i++) {
         %obj = Mission.getObject(%i);
         Mission.xVel = 0;
         %obj.setLinearVelocityX(0);
      }
   }
}

function StopWorldDown() {
   if (Mission.yVel < 0) {
      %count = Mission.getCount();
      for(%i = 0; %i < %count; %i++) {
         %obj = Mission.getObject(%i);
         Mission.yVel = 0;
         %obj.setLinearVelocityY(0);
      }
   }
}

function MoveWorldLeft() {
   %count = Mission.getCount();
   for(%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      Mission.xVel = $guy::speed;
      %obj.setLinearVelocityX($guy::speed);
   }
}

function MoveWorldUp(%val) {
   %count = Mission.getCount();
   for(%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      Mission.yVel = $guy::speed;
      %obj.setLinearVelocityY($guy::speed);
   }
}

function MoveWorldRight(%val) {
   %count = Mission.getCount();
   for(%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      Mission.xVel = -$guy::speed;
      %obj.setLinearVelocityX(-$guy::speed);
   }
}

function MoveWorldDown(%val) {
   %count = Mission.getCount();
   for(%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      Mission.yVel = -$guy::speed;
      %obj.setLinearVelocityY(-$guy::speed);
   }
}
      
function OnLeft(%val) {
   if (%val && $guy.move) {
      $leftDown = true;
      $guy.playAnimation(GuyWalk);
      MoveWorldLeft();
      if ($downDown) $guy.setRotation(-135);
      else if ($upDown) $guy.setRotation(-45);
      else $guy.setRotation(-90);
   } else if (!%val) {
      $leftDown = false;
      StopWorldLeft();
      if ($downDown) $guy.setRotation(180);
      else if ($upDown) $guy.setRotation(0);
      else if ($rightDown) $guy.setRotation(90);
      else $guy.playAnimation(GuyStand);
   }
}

function OnUp(%val) {
   if (%val && $guy.move) {
      $upDown = true;
      $guy.playAnimation(GuyWalk);
      MoveWorldUp();
      if ($leftDown) $guy.setRotation(-45);
      else if ($rightDown) $guy.setRotation(45);
      else $guy.setRotation(0);
   } else if (!%val) {
      $upDown = false;
      StopWorldUp();
      if ($downDown) $guy.setRotation(180);
      else if ($leftDown) $guy.setRotation(-90);
      else if ($rightDown) $guy.setRotation(90);
      else $guy.playAnimation(GuyStand);
   }
}

function OnRight(%val) {
   if (%val && $guy.move) {
      $rightDown = true;
      $guy.playAnimation(GuyWalk);
      MoveWorldRight();
      if ($downDown) $guy.setRotation(135);
      else if ($upDown) $guy.setRotation(45);
      else $guy.setRotation(90);
   } else if (!%val) {
      $rightDown = false;
      StopWorldRight();
      if ($downDown) $guy.setRotation(180);
      else if ($upDown) $guy.setRotation(0);
      else if ($leftDown) $guy.setRotation(-90);
      else $guy.playAnimation(GuyStand);
   }
}

function OnDown(%val) {
   if (%val && $guy.move) {
      $downDown = true;
      $guy.playAnimation(GuyWalk);
      MoveWorldDown();
      if ($leftDown) $guy.setRotation(-135);
      else if ($rightDown) $guy.setRotation(135);
      else $guy.setRotation(180);
   } else if (!%val) {
      $downDown = false;
      StopWorldDown();
      if ($upDown) $guy.setRotation(0);
      else if ($leftDown) $guy.setRotation(-90);
      else if ($rightDown) $guy.setRotation(90);
      else $guy.playAnimation(GuyStand);
   }
}

function CollideGuyBoundary(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts) {
   // If the left side was hit
   if (%srcObj.getPositionX() < %dstObj.getPositionX()) {
      StopWorldRight();
   }
   // If the right side was hit
   if (%srcObj.getPositionX() > (%dstObj.getPositionX() + (%dstObj.getSizeX() * 0.5))) {
      StopWorldLeft();
   }
   // If the top was hit
   if (%srcObj.getPositionY() < (%dstObj.getPositionY() - (%dstObj.getSizeY() * 0.5))) {
      StopWorldDown();
   }
   // If the bottom was hit
   if (%srcObj.getPositionY() > (%dstObj.getPositionY() + (%dstObj.getSizeY() * 0.5))) {
      StopWorldUp();
   }
}

function CollideGuyTrigger(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts) {
   eval(%dstObj.func);
   Mission.remove(%dstObj);
   %dstObj.removeFromScene();
}
