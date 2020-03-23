function CreateTrigger(%function, %pos, %size) {
   %x = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
   %x.setPosition(%pos);
   %x.setSize(%size);
   %x.setImageMap(Trigger);
   %x.imap = "Trigger";
   %x.type = "trigger";
   %x.func = %function;
   %x.setGroup(1);
   %x.setLayer(1);
   %x.setCollisionActive(false, true);
   %x.setCollisionMasks(BIT(6) | BIT(7) | BIT(8), BIT(6) | BIT(7) | BIT(8));
   %x.setCollisionCallback(true);

   Mission.add(%x);

   if ($editorOn) Canvas.popDialog(NewTriggerGUI);
}

function LoadLevelStreets1() {
   LoadLevel("streets1");
}

function LoadLevelStreets2() {
   LoadLevel("streets2");
}

function LoadLevelStreets3() {
   LoadLevel("streets3");
}

function LoadLevelFreeway() {
   LoadLevel("freeway");
}

function LoadLevelBuilding() {
   LoadLevel("building");
}

function LoadLevelLobby() {
   LoadLevel("lobby");
}

function LoadLevelRoof() {
   LoadLevel("roof");
}

function LoadLevelEnd() {
   LoadLevel("end");
}

function CreateHouseEnemies1() {
   CreateEnemy("Pete", "50 -250", 7, 30);
   CreateEnemy("Pete", "70 -250", 6, 30);
}

function CreateHouseEnemies2() {
   CreateEnemy("Pete", "350 -25", 6, 40);
   CreateEnemy("Pete", "350 25", 7, 30);
   CreateEnemy("Pete", "350 0", 5, 50);
}
