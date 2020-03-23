datablock fxImageMapDatablock2D(Pig) {
   mode = cell;
   cellWidth = 112;
   cellHeight = 128;
   textureName = "Data/Images/pig.png";
};

datablock fxAnimationDatablock2D(PigStand) {
   imageMap = Pig;
   animationFrames = 0;
   animationTime = 1;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(PigWalk) {
   imageMap = Pig;
   animationFrames = "0 1 2 3";
   animationTime = 1;
   animationCycle = 1;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(PigHit) {
   imageMap = Pig;
   animationFrames = "16 17 18 19 20 21 22 23";
   animationTime = 0.5;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(PigDie) {
   imageMap = Pig;
   animationFrames = "24 25 26 27 28 29 30 31 32 33 34 35 36 37 38 39";
   animationTime = 1;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(PigKick) {
   imageMap = Pig;
   animationFrames = "4 5 6 7 8 9 10 11 12 13 14";
   animationTime = 0.75;
   animationCycle = 0;
   randomStart = 0;
};
