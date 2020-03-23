datablock fxImageMapDatablock2D(Pete) {
   mode = cell;
   cellWidth = 128;
   cellHeight = 170;
   textureName = "Data/Images/pete.png";
};

datablock fxAnimationDatablock2D(PeteStand) {
   imageMap = Pete;
   animationFrames = 0;
   animationTime = 1;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(PeteWalk) {
   imageMap = Pete;
   animationFrames = "0 1 2 3";
   animationTime = 0.25;
   animationCycle = 1;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(PeteHit) {
   imageMap = Pete;
   animationFrames = "4 5 6 0";
   animationTime = 0.25;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(PeteDie) {
   imageMap = Pete;
   animationFrames = "8 9 10 11 12 13 14 15";
   animationTime = 1.0;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(PeteKick) {
   imageMap = Pete;
   animationFrames = "16 17 18 19 20 21 22";
   animationTime = 0.5;
   animationCycle = 0;
   randomStart = 0;
};
