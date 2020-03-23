$Gui::fontCacheDirectory = expandFilename("./Cache");

new GuiControlProfile(GuiDefaultProfile) {
	tab = false;
	canKeyFocus = false;
	hasBitmapArray = false;
	mouseOverSelected = false;

	opaque = false;
	fillColor =  "212 208 220";
	fillColorHL = "220 220 220";
	fillColorNA = "220 220 220";

	border = false;
	borderColor = "0 0 0";
	borderColorHL = "128 128 128";
	borderColorNA = "64 64 64";

	fontType = "Arial";
	fontSize = 14;

	fontColor = "0 0 0";
	fontColorHL = "255 255 255";
	fontColorNA = "192 192 192";
	fontColorSEL = "200 200 200";

	bitmap = "./window";
	bitmapBase = "";
	textOffset = "0 0";

	modal = true;
	justify = "left";
	autoSizeWidth = false;
	autoSizeHeight = false;
	returnTab = false;
	numbersOnly = false;
	cursorColor = "0 0 0 255";

	soundButtonDown = "";
	soundButtonOver = "";
};

new GuiControlProfile(GuiButtonProfile) {
   opaque = true;
   border = true;
   fontColor = "0 0 0";
   fontColorHL = "32 100 100";
   fixedExtent = true;
   justify = "center";
	canKeyFocus = false;
};

new GuiControlProfile(GuiContentProfile) {
	fillColor = "255 255 255";
	opaque = true;
};

new GuiControlProfile(GuiWindowProfile) {
	opaque = true;
	border = 2;
	fontColor = "255 255 255";
	fontColorHL = "0 0 0";
	bitmap = "./window";
	textOffset = "6 6";
	hasBitmapArray = true;
	justify = "left";
};

new GuiControlProfile(GuiInputProfile) {
   tab = true;
	canKeyFocus = true;
};

new GuiControlProfile(GuiTextEditProfile) {
   opaque = true;
   fillColor = "255 255 255";
   fillColorHL = "128 128 128";
   border = 3;
   borderThickness = 2;
   borderColor = "0 0 0";
   fontColor = "0 0 0";
   fontColorHL = "255 255 255";
   fontColorNA = "128 128 128";
   textOffset = "0 2";
   autoSizeWidth = false;
   autoSizeHeight = true;
   tab = true;
   canKeyFocus = true;
};

new GuiCursor(MainCursor) {
	hotSpot = "1 1";
	bitmapName = "./cursor";
};
