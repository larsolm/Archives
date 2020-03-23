exec("./editor.gui");

$editorCreated = false;
$editorOn = false;
$scrollSpeed = 16;

$picked = "";

new SimSet(EditorGroup) {
};

function EditorGUI::onMouseDown(%this, %x, %y) {
   $prevX = %x * 0.8;
   $prevY = %y * 0.8;
   $picked = getWord(MainSceneGraph.pickPoint((%x * 0.8) SPC (%y * 0.8)), 0);
   if ($picked) {
      $xOffset = (%x * 0.8) - getWord($picked.getPosition(), 0);
      $yOffset = (%y * 0.8) - getWord($picked.getPosition(), 1);
      %xSize = $picked.getSizeX() * 0.5;
      %ySize = $picked.getSizeY() * 0.5;
      %xDif = %xSize - $xOffset;
      %yDif = %ySize - $yOffset;

      $scale = "";
      if (%xDif < 10) $scale = "right";
      else if (%xDif > ($picked.getSizeX() - 10)) $scale = "left";
      if (%yDif < 10) {
         if ($scale $= "right") $scale = "rightdown";
         else if ($scale $= "left") $scale = "leftdown";
         else $scale = "down";
      } else if (%yDif > ($picked.getSizeY() - 10)) {
         if ($scale $= "right") $scale = "rightup";
         else if ($scale $= "left") $scale = "leftup";
         else $scale = "up";
      }
   }
}

function EditorGUI::onMouseDragged(%this, %x, %y) {
   %x = %x * 0.8;
   %y = %y * 0.8;
   %xChange = %x - $prevX;
   %yChange = %y - $prevY;
   if ($scale $= "") {
      $picked.setPosition((%x - $xOffset) SPC (%y - $yOffset));
   } else {
      if ($scale $= "down") {
         if ($picked.getSizeY() + %yChange > 5) {
            $picked.setSizeY($picked.getSizeY() + %yChange);
            $picked.setPositionY($picked.getPositionY() + (%yChange * 0.5));
         }
      } else if ($scale $= "up") {
         if ($picked.getSizeY() - %yChange > 5) {
            $picked.setSizeY($picked.getSizeY() - %yChange);
            $picked.setPositionY($picked.getPositionY() + (%yChange * 0.5));
         }
      } else if ($scale $= "left") {
         if ($picked.getSizeX() - %xChange > 5) {
            $picked.setSizeX($picked.getSizeX() - %xChange);
            $picked.setPositionX($picked.getPositionX() + (%xChange * 0.5));
         }
      } else if ($scale $= "right") {
         if ($picked.getSizeX() + %xChange > 5) {
            $picked.setSizeX($picked.getSizeX() + %xChange);
            $picked.setPositionX($picked.getPositionX() + (%xChange * 0.5));
         }
      } else if ($scale $= "rightdown") {
         if ($picked.getSizeX() + %xChange > 5) {
            $picked.setSizeX($picked.getSizeX() + %xChange);
            $picked.setPositionX($picked.getPositionX() + (%xChange * 0.5));
         }
         if ($picked.getSizeY() + %yChange > 5) {
            $picked.setSizeY($picked.getSizeY() + %yChange);
            $picked.setPositionY($picked.getPositionY() + (%yChange * 0.5));
         }
      } else if ($scale $= "leftdown") {
         if ($picked.getSizeX() - %xChange > 5) {
            $picked.setSizeX($picked.getSizeX() - %xChange);
            $picked.setPositionX($picked.getPositionX() + (%xChange * 0.5));
         }
         if ($picked.getSizeY() + %yChange > 5) {
            $picked.setSizeY($picked.getSizeY() + %yChange);
            $picked.setPositionY($picked.getPositionY() + (%yChange * 0.5));
         }
      } else if ($scale $= "rightup") {
         if ($picked.getSizeX() + %xChange > 5) {
            $picked.setSizeX($picked.getSizeX() + %xChange);
            $picked.setPositionX($picked.getPositionX() + (%xChange * 0.5));
         }
         if ($picked.getSizeY() - %yChange > 5) {
            $picked.setSizeY($picked.getSizeY() - %yChange);
            $picked.setPositionY($picked.getPositionY() + (%yChange * 0.5));
         }
      } else if ($scale $= "leftup") {
         if ($picked.getSizeX() - %xChange > 5) {
            $picked.setSizeX($picked.getSizeX() - %xChange);
            $picked.setPositionX($picked.getPositionX() + (%xChange * 0.5));
         }
         if ($picked.getSizeY() - %yChange > 5) {
            $picked.setSizeY($picked.getSizeY() - %yChange);
            $picked.setPositionY($picked.getPositionY() + (%yChange * 0.5));
         }
      }
   }
   $prevX = %x;
   $prevY = %y;
}

function OpenEditor() {
   if (!$editorCreated) {
      new fxSceneGraph2D(MainSceneGraph);
      EditorSceneWindow.setSceneGraph(MainSceneGraph);
      EditorSceneWindow.setCurrentCameraPosition("320 240 640 480");

      $editorCreated = true;
   }

   $editorOn = true;
   Canvas.setContent(LevelEditorGUI);
   Canvas.pushDialog(LevelSelectGUI);
}

function CreateBoundary(%name, %size) {
   %x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
   %x.setPosition("400 300");
   %x.setSize(%size);
   %x.setImageMap(%name);
   %x.setGroup(9);
   %x.setLayer(9);
   %x.imap = %name;
   %x.type = "boundary";

   Mission.add(%x);

   Canvas.popDialog(NewBoundaryGUI);
}

function SaveLevel(%name) {
   %file = new FileObject();
   %file.openForWrite("Data/Levels/" @ %name @ ".cs");

   %file.writeLine("new SimSet(Mission) { xVel = 0; yVel = 0; };");
   %file.writeLine("new SimSet(Enemies) { };");
   %file.writeLine("");

   %file.writeLine("datablock fxImageMapDatablock2D(Trigger) {");
   %file.writeLine("   mode = full;");
   %file.writeLine("   textureName = \"Data/Images/trigger.png\";");
   %file.writeLine("};");
   %file.writeLine("");

   %file.writeLine("datablock fxImageMapDatablock2D(Enemy) {");
   %file.writeLine("   mode = full;");
   %file.writeLine("   textureName = \"Data/Images/Enemy.png\";");
   %file.writeLine("};");
   
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      if (%obj.type $= "boundary") {
         %file.writeLine("");
         %file.writeLine("%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };");
         %file.writeLine("%x.setSize(\"" @ %obj.getSize() @ "\");");
         %file.writeLine("%x.setPosition(\"" @ %obj.getPosition() @ "\");");
         %file.writeLine("%x.setImageMap(" @ %obj.imap @ ");");
         %file.writeLine("%x.imap = \"" @ %obj.imap @ "\";");
         %file.writeLine("%x.type = \"" @ %obj.type @ "\";");
         %file.writeLine("%x.setGroup(9);");
         %file.writeLine("%x.setLayer(9);");
         %file.writeLine("%x.setCollisionActive(false, true);");
         %file.writeLine("%x.setCollisionMasks(BIT(5) | BIT(6) | BIT(8), BIT(5) | BIT(6) | BIT(8));");
         %file.writeLine("%x.setCollisionCallback(true);");
         %file.writeLine("Mission.add(%x);");
      } else if (%obj.type $= "enemy") {
         %file.writeLine("");
         %file.writeLine("CreateEnemy(\"" @ %obj.imap @ "\", \"" @ %obj.getPosition() @ "\", " @ %obj.speed @ ", " @ %obj.life @ ");");
      } else if (%obj.type $= "trigger") {
         %file.writeLine("");
         %file.writeLine("CreateTrigger(\"" @ %obj.func @ "\", \"" @ %obj.getPosition() @ "\", \"" @ %obj.getSize() @ "\");");
      } else if (%obj.type $= "background") {
         %file.writeLine("");
         %file.writeLine("%x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };");
         %file.writeLine("%x.setSize(\"" @ %obj.getSize() @ "\");");
         %file.writeLine("%x.setPosition(\"" @ %obj.getPosition() @ "\");");
         %file.writeLine("%x.setImageMap(" @ %obj.imap @ ");");
         %file.writeLine("%x.imap = \"" @ %obj.imap @ "\";");
         %file.writeLine("%x.type = \"" @ %obj.type @ "\";");
         %file.writeLine("%x.setGroup(31);");
         %file.writeLine("%x.setLayer(31);");
         %file.writeLine("Mission.add(%x);");
      }
   }

   %file.close();
   %file.delete();
}

function DeleteObject() {
   Mission.remove($picked);
   $picked.delete();
}

function ScrollLeft() {
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      %x = %obj.getPositionX() + $scrollSpeed;
      %y = %obj.getPositionY();
      %obj.setPosition(%x SPC %y);
   }
}

function ScrollUpLeft() {
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      %x = %obj.getPositionX() + $scrollSpeed;
      %y = %obj.getPositionY() + $scrollSpeed;
      %obj.setPosition(%x SPC %y);
   }
}

function ScrollUp() {
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      %x = %obj.getPositionX();
      %y = %obj.getPositionY() + $scrollSpeed;
      %obj.setPosition(%x SPC %y);
   }
}

function ScrollUpRight() {
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      %x = %obj.getPositionX() - $scrollSpeed;
      %y = %obj.getPositionY() + $scrollSpeed;
      %obj.setPosition(%x SPC %y);
   }
}

function ScrollRight() {
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      %x = %obj.getPositionX() - $scrollSpeed;
      %y = %obj.getPositionY();
      %obj.setPosition(%x SPC %y);
   }
}

function ScrollDownRight() {
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      %x = %obj.getPositionX() - $scrollSpeed;
      %y = %obj.getPositionY() - $scrollSpeed;
      %obj.setPosition(%x SPC %y);
   }
}

function ScrollDown() {
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      %x = %obj.getPositionX();
      %y = %obj.getPositionY() - $scrollSpeed;
      %obj.setPosition(%x SPC %y);
   }
}

function ScrollDownLeft() {
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      %x = %obj.getPositionX() + $scrollSpeed;
      %y = %obj.getPositionY() - $scrollSpeed;
      %obj.setPosition(%x SPC %y);
   }
}

function CreateImage(%image, %size) {
   %x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
   %x.setPosition("400 300");
   %x.setSize(%size);
   %x.setImageMap(%image);
   %x.imap = %image;
   %x.setLayer(31);
   %x.setGroup(31);
   %x.type = "background";

   Mission.add(%x);

   Canvas.popDialog(NewImageGUI);
}

function CreateEnemySpawn(%type, %speed, %life) {
   %x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
   %x.setPosition("400 300");
   %x.setSize("32 32");
   %x.setImageMap(Enemy);
   %x.imap = %type;
   %x.type = "enemy";
   %x.speed = %speed;
   %x.life = %life;

   Mission.add(%x);

   Canvas.popDialog(NewEnemyGUI);
}

function CreateEditorEnemy(%type, %pos, %speed, %life) {
   %x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
   %x.setPosition(%pos);
   %x.setSize("32 32");
   %x.setImageMap(Enemy);
   %x.imap = %type;
   %x.type = "enemy";
   %x.speed = %speed;
   %x.life = %life;

   Mission.add(%x);
}
