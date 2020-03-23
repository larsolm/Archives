// Load up the default preferences
exec("./defaultPrefs.cs");
exec("./bind.cs");

// Required by the engine
setRandomSeed();

// Create the game window
exec("./canvas.cs");
InitializeCanvas("Whip Game");
exec("Data/GUI/guiProfiles.cs");

// Start up the game
exec("./game.cs");
