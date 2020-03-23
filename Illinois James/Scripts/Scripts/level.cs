$levelRunning = false;

function LoadLevel(%level) {
   if ($levelRunning) EndLevel();
   exec("Data/Levels/" @ %level @ "db.cs");
   exec("Data/Levels/" @ %level @ ".cs");
   $levelRunning = true;
}

function EndLevel() {
   %count = Mission.getCount();
   for (%i = 0; %i < %count; %i++) {
      %obj = Mission.getObject(%i);
      %obj.removeFromScene();
   }
   Mission.delete();
   Enemies.delete();
   $levelRunning = false;
}
