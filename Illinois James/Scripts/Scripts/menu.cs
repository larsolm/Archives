exec("Data/GUI/ggIntro.gui");
exec("Data/GUI/prIntro.gui");
exec("Data/GUI/menu.gui");
exec("Data/GUI/intro.gui");

function GGIntroGUI::onWake() {
   Canvas.setCursor(MainCursor);
   $sched = schedule(5000, 0, LoadPRIntro);
}

function GGIntroGUI::click() {
   cancel($sched);
   LoadPRIntro();
}

function LoadPRIntro() {
   Canvas.setContent(PRIntroGUI);
}

function PRIntroGUI::onWake() {
   $sched = schedule(5000, 0, LoadMenu);
}

function PRIntroGUI::click() {
   cancel($sched);
   LoadMenu();
}

function LoadMenu() {
   Canvas.setContent(MenuGUI);
}

function PlayGame() {
   Canvas.setContent(IntroGUI1);
}

function IntroGUI1::onWake() {
   $sched = schedule(12000, 0, LoadIntro2);
}

function IntroGui1::click() {
   cancel($sched);
   LoadIntro2();
}

function LoadIntro2() {
   Canvas.setContent(IntroGUI2);
}

function IntroGUI2::onWake() {
   $sched = schedule(8000, 0, LoadIntro3);
}

function IntroGui2::click() {
   cancel($sched);
   LoadIntro3();
}

function LoadIntro3() {
   Canvas.setContent(IntroGUI3);
}

function IntroGUI3::onWake() {
   $sched = schedule(14000, 0, LoadGame);
}

function IntroGui3::click() {
   cancel($sched);
   LoadGame();
}

function LoadGame() {
   StartGame();
   cancel($sched);
   Canvas.setContent(MainGUI);
   Canvas.pushDialog(IntroGUI4);
   schedule(7500, 0, CloseIntro);
}

function CloseIntro() {
   Canvas.popDialog(IntroGUI4);
}
