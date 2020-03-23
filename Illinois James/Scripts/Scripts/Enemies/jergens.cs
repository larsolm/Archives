datablock fxImageMapDatablock2D(Jergens) {
   mode = cell;
   cellWidth = 160;
   cellHeight = 128;
   textureName = "Data/Images/jergens.png";
};

datablock fxAnimationDatablock2D(JergensStand) {
   imageMap = Jergens;
   animationFrames = 0;
   animationTime = 1;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(JergensWalk) {
   imageMap = Jergens;
   animationFrames = "0 1 2 3";
   animationTime = 0.25;
   animationCycle = 1;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(JergensHit) {
   imageMap = Jergens;
   animationFrames = "5 6 7 8 0";
   animationTime = 0.25;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(JergensDie) {
   imageMap = Jergens;
   animationFrames = "20 21 22 23 24";
   animationTime = 0.5;
   animationCycle = 0;
   randomStart = 0;
};

datablock fxAnimationDatablock2D(JergensKick) {
   imageMap = Jergens;
   animationFrames = "10 11 12 13 15 16 17 18 0";
   animationTime = 0.5;
   animationCycle = 0;
   randomStart = 0;
};
