exec("./Enemies/pete.cs");
exec("./Enemies/pig.cs");
exec("./Enemies/jergens.cs");
exec("./Enemies/joe.cs");

$enemy::hitDelay = 0.7;

function CreateEnemy(%type, %pos, %speed, %life) {
   if ($editorOn) {
      CreateEditorEnemy(%type, %pos, %speed, %life);
      return;
   }

   %enemy = new fxAnimatedSprite2D() {
      scenegraph = MainSceneGraph;
   };
   %enemy.setPosition(%pos);
   %enemy.setSize("48 48");
   %enemy.setGroup(5);
   %enemy.setLayer(5);
   %enemy.setCollisionActive(true, true);
   %enemy.setCollisionMasks(BIT(6) | BIT(7) | BIT(8) | BIT(9), BIT(6) | BIT(7) | BIT(8) | BIT(9));
   %enemy.setCollisionCallback(true);
   %enemy.playAnimation(%type @ "Stand");
   
   // Collision resolution helper variables
   %enemy.type = "enemy";
   %enemy.hit = false;
   %enemy.time = 0;
   %enemy.kick = false;
   
   // Attributes
   %enemy.imap = %type;
   %enemy.life = %life;
   %enemy.startLife = %life;
   %enemy.speed = %speed * 0.15;
   %enemy.leftDown = false;
   %enemy.rightDown = false;
   %enemy.upDown = false;
   %enemy.downDown = false;
   %enemy.move = true;
   Mission.add(%enemy);
   Enemies.add(%enemy);
   echo(%enemy.getPosition());
}

function DoAI(%enemy, %xTarget, %yTarget, %time) {
   %enemy.setRotation(angleBetween2D(%enemy.getPosition(), %xTarget SPC %yTarget));
   // Check if %enemy is kicking right now
   if (%enemy.kick) {
      if (%enemy.getIsAnimationFinished()) {
         %enemy.kick = false;
         %enemy.playAnimation(%enemy.imap @ "Stand");
      }
   }
   %xPos = %enemy.getPositionX();
   %yPos = %enemy.getPositionY();
   if (%enemy.kick $= false) {
      if ((mAbs(%xPos - %xTarget) < 32) && (mAbs(%yPos - %yTarget) < 32)) {
         %enemy.kick = true;
         %enemy.playAnimation(%enemy.imap @ "Kick");
         if (%enemy.leftDown) GoLeft(0, %enemy);
         if (%enemy.rightDown) GoRight(0, %enemy);
         if (%enemy.downDown) GoDown(0, %enemy);
         if (%enemy.upDown) GoUp(0, %enemy);
      } else {
         if (%xPos < %xTarget) {
            if (!%enemy.rightDown) GoRight(1, %enemy);
            if (%enemy.leftDown) GoLeft(0, %enemy);
         } else {
            if (!%enemy.leftDown) GoLeft(1, %enemy);
            if (%enemy.rightDown) GoRight(0, %enemy);
         }
         if (%yPos < %yTarget) {
            if (!%enemy.downDown) GoDown(1, %enemy);
            if (%enemy.upDown) GoUp(0, %enemy);
         } else {
            if (!%enemy.upDown) GoUp(1, %enemy);
            if (%enemy.downDown) GoDown(0, %enemy);
         }
      }
   }
}

function MoveEnemyRight(%enemy) {
   %enemy.setPositionX(%enemy.getPositionX() + %enemy.speed);
   %enemy.schedRight = schedule(32, 0, MoveEnemyRight, %enemy);
}

function StopEnemyRight(%enemy) {
   cancel(%enemy.schedRight);
}

function MoveEnemyLeft(%enemy) {
   %enemy.setPositionX(%enemy.getPositionX() - %enemy.speed);
   %enemy.schedLeft = schedule(32, 0, MoveEnemyLeft, %enemy);
}

function StopEnemyLeft(%enemy) {
   cancel(%enemy.schedLeft);
}

function MoveEnemyUp(%enemy) {
   %enemy.setPositionY(%enemy.getPositionY() - %enemy.speed);
   %enemy.schedUp = schedule(32, 0, MoveEnemyUp, %enemy);
}

function StopEnemyUp(%enemy) {
   cancel(%enemy.schedUp);
}

function MoveEnemyDown(%enemy) {
   %enemy.setPositionY(%enemy.getPositionY() + %enemy.speed);
   %enemy.schedDown = schedule(32, 0, MoveEnemyDown, %enemy);
}

function StopEnemyDown(%enemy) {
   cancel(%enemy.schedDown);
}

function GoRight(%val, %enemy) {
   if (%val && %enemy.move) {
      %enemy.rightDown = true;
      %enemy.playAnimation(%enemy.imap @ "Walk");
      MoveEnemyRight(%enemy);
   } else if (!%val) {
      %enemy.rightDown = false;
      StopEnemyRight(%enemy);
   }
}

function GoLeft(%val, %enemy) {
   if (%val && %enemy.move) {
      %enemy.leftDown = true;
      %enemy.playAnimation(%enemy.imap @ "Walk");
      MoveEnemyLeft(%enemy);
   } else if (!%val) {
      %enemy.leftDown = false;
      StopEnemyLeft(%enemy);
   }
}

function GoDown(%val, %enemy) {
   if (%val && %enemy.move) {
      %enemy.downDown = true;
      %enemy.playAnimation(%enemy.imap @ "Walk");
      MoveEnemyDown(%enemy);
   } else if (!%val) {
      %enemy.downDown = false;
      StopEnemyDown(%enemy);
   }
}

function GoUp(%val, %enemy) {
   if (%val && %enemy.move) {
      %enemy.upDown = true;
      %enemy.playAnimation(%enemy.imap @ "Walk");
      MoveEnemyUp(%enemy);
   } else if (!%val) {
      %enemy.upDown = false;
      StopEnemyUp(%enemy);
   }
}

function UpdateEnemies(%time) {
   %count = Enemies.getCount();
   for (%i = 0; %i < %count; %i++) {
      %enemy = Enemies.getObject(%i);
      DoAI(%enemy, $guy.getPositionX(), $guy.getPositionY(), %time);

      // Update the hit status
      // Enemies can only be hit once per "$enemy::hitDelay" seconds
      // This is to make sure they are only hurt by the first whip section
      // that hits them
      if (%enemy.hit) {
         if (MainSceneGraph.getSceneTime() - %enemy.time > $enemy::hitDelay) {
            %enemy.hit = false;
         }
      }
   }
}

function CollideEnemyGuy(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts) {
   if (%srcObj.kick) {
      GuyHit(10);
   }
}

function EnemyHit(%enemy, %power) {
   %enemy.life = %enemy.life - %power;
   EnemyLifeBar.extent = ((%enemy.life / %enemy.startLife) * 100) SPC "25";
   EnemyName.setText(%enemy.imap);
   if ((%enemy.life < 1) && (!%enemy.dead)) {
      %enemy.move = false;
      %enemy.dead = true;
      cancel(%enemy.schedRight);
      cancel(%enemy.schedLeft);
      cancel(%enemy.schedUp);
      cancel(%enemy.schedDown);
      %enemy.playAnimation(%enemy.imap @ "Die");
      schedule(1000, 0, EnemyDie, %enemy);
   } else if (%enemy.life > 0) {
      %enemy.time = MainSceneGraph.getSceneTime();
      %enemy.hit = true;
      %enemy.playAnimation(%enemy.imap @ "Hit");
   }
}

function EnemyDie(%enemy) {
   Enemies.remove(%enemy);
   Mission.remove(%enemy);
   %enemy.delete();
}

function CollideEnemyBoundary(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts) {
   %x = %srcObj.getPositionX();
   %y = %srcObj.getPositionY();
   %x2 = %dstObj.getPositionX();
   %y2 = %dstObj.getPositionY();

   if (%x > (%x2 + (%dstObj.getSizeX() * 0.4))) {
      %srcObj.setPositionX(%x - %srcObj.speed);
      GoLeft(0, %srcObj);
   }
   if (%x < (%x2 - (%dstObj.getSizeX() * 0.4))) {
      %srcObj.setPositionX(%x + %srcObj.speed);
      GoRight(0, %srcObj);
   }
   if (%y > (%y2 + (%dstObj.getSizeY() * 0.4))) {
      %srcObj.setPositionY(%y + %srcObj.speed);
      GoUp(0, %srcObj);
   }
   if (%y < (%y2 + (%dstObj.getSizeY() * 0.4))) {
      %srcObj.setPositionY(%y - %srcObj.speed);
      GoDown(0, %srcObj);
   }
}
