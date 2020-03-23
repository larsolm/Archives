$numWhipSections = 16;

datablock fxImageMapDatablock2D(WhipSection) {
   mode = full;
   textureName = "Data/Images/whipSection.png";
};

function CreateWhip() {
   $whip = new Whip() { scenegraph = MainSceneGraph; };
   $whip.setPosition("0 0");
   $whip.setSize("3 3");
   $whip.setGroup(7);
   $whip.setLayer(7);

   for (%i = 0; %i < $numWhipSections; %i++) {
      $section[%i] = new fxStaticSprite2D() { scenegraph = MainSceneGraph; };
      if (%i < 5) $section[%i].setSize("4 4");
      else if (%i < 10) $section[%i].setSize("3 3");
      else if (%i < 15) $section[%i].setSize("2 2");
      else $section[%i].setSize("5 5");
      $section[%i].index = %i * 2;
      $section[%i].setImageMap(WhipSection);
      $section[%i].setGroup(7);
      $section[%i].setLayer(7);
      $section[%i].setCollisionActive(true, false);
      $section[%i].setCollisionMasks(BIT(1) | BIT(5), BIT(1) | BIT(5));
      $section[%i].setCollisionCallback(true);
      $section[%i].type = "whipSection";
   }
}

function UpdateWhip() {
   // Each section of the whip is represented by a mass in the engine
   // and thus follows the position of that mass
   for (%i = 0; %i < $numWhipSections; %i++) {
      $section[%i].setPosition($whip.GetMassPosition(%i * 2)); 
   }
}

// Called by on collision when the whip hits an enemy
// The whip should be %srcObj and the enemy %dstObj
function CollideWhipEnemy(%srcObj, %dstObj, %srcRef, %dstRef, %time, %normal, %contactCount, %contacts) {
   if (!%dstObj.hit) {
      EnemyHit(%dstObj, vectorLength2D($whip.getMassVelocity(%srcObj.index)));
   }
}
