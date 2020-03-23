exec("./menu.cs");
exec("./guy.cs");
exec("./whip.cs");
exec("./boomerang.cs");
exec("./enemy.cs");
exec("./level.cs");
exec("./trigger.cs");
exec("./Editor/editor.cs");
exec("Data/GUI/game.gui");

Canvas.setContent(GGIntroGUI);

function BIT(%num) {
   return 1 << %num;
}

function StartGame() {
   // Create the main scene graph
   new fxSceneGraph2D(MainSceneGraph);
   SceneWindowGUI.setSceneGraph(MainSceneGraph);
   // Screen dimensions are from (-400, -300) to (400, 300)
   // For some reason this makes good sense to me
   SceneWindowGUI.setCurrentCameraPosition("0 0 640 480");

   CreateGuy();
   CreateWhip();
   CreateBoomerang();
   LoadLevel("house");
}

function fxSceneObject2D::onCollision(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts) {
   if ((%srcObj == $guy) && (%dstObj.type $= "boundary")) {
      CollideGuyBoundary(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts);
   } else if ((%srcObj.type $= "whipSection") && (%dstObj.type $= "enemy")) {
      CollideWhipEnemy(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts);
   } else if ((%srcObj.type $= "boomerang") && (%dstObj.type $= "boundary")) {
      CollideBoomerangBoundary(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts);
   } else if ((%srcObj.type $= "enemy") && (%dstObj == $guy)) {
      CollideEnemyGuy(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts);
   } else if ((%srcObj.type $= "boomerang") && (%dstObj.type $= "enemy")) {
      CollideBoomerangEnemy(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts);
   } else if ((%srcObj == $guy) && (%dstObj.type $= "enemy")) {
      CollideGuyBoundary(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts);
   } else if ((%srcObj.type $= "enemy") && (%dstObj.type $= "boundary")) {
      CollideEnemyBoundary(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts);
   } else if ((%srcObj == $guy) && (%dstObj.type $= "trigger")) {
      CollideGuyTrigger(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts);
   }
}

$prevSceneTime = 0;

// Called every frame by the engine
function fxSceneGraph2D::onUpdateScene(%this) {
   if (!$editorOn) {
      UpdateWhip();
      UpdateEnemies(%this.getSceneTime() - $prevSceneTime);
      UpdateBoomerang(%this.getSceneTime() - $prevSceneTime);
   }
   $prevSceneTime = %this.getSceneTime();
}

function DestroyScene() {
   if (isObject(MainSceneGraph)) MainSceneGraph.delete();   
}
