datablock fxImageMapDatablock2D(Joe) {
   mode = cell;
   cellWidth = 160;
   cellHeight = 112;
   textureName = "Data/Images/joe.png";
};

datablock fxAnimationDatablock2D(JoeStand) {
   imageMap = Joe;
   animationFrames = 0;
   animationTime = 1;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(JoeWalk) {
   imageMap = Joe;
   animationFrames = "0 1 2 3";
   animationTime = 1;
   animationCycle = 1;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(JoeHit) {
   imageMap = Joe;
   animationFrames = "4 5 6 7";
   animationTime = 0.5;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(JoeDie) {
   imageMap = Joe;
   animationFrames = "20 21 22 23 24 25 26 27 28 29";
   animationTime = 1;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(JoeKick) {
   imageMap = Joe;
   animationFrames = "8 9 10 11 12 13 14 15 16 17 18";
   animationTime = 0.75;
   animationCycle = 0;
   randomStart = 0;
};
