function resetCanvas() {
   if (isObject(Canvas)) {
      Canvas.repaint(); 
   }
}

function InitializeCanvas(%windowName) {
   if (!createCanvas(%windowName)) {
      quit();
   }
}

function CursorOn() {
   lockMouse(false);
   Canvas.cursorOn();
   Canvas.setCursor(MainCursor); 
}

function CursorOff() {
   lockMouse(true);
   Canvas.cursorOff();
}

package CanvasCursor {
function GuiCanvas::CheckCursor(%this) {
   %cursorShouldBeOn = false;
   for(%i = 0; %i < %this.getCount(); %i++) {
      %control = %this.getObject(%i);
      if(%control.noCursor $= "") {
         %cursorShouldBeOn = true;
         break;
      }
   }
   if (%cursorShouldBeOn != %this.isCursorOn()) {
      if (%cursorShouldBeOn) CursorOn();
      else CursorOff();
   }
}

function GuiCanvas::setContent(%this, %ctrl) {
   Parent::setContent(%this, %ctrl);
   %this.CheckCursor();
}

function GuiCanvas::pushDialog(%this, %ctrl, %layer) {
   Parent::pushDialog(%this, %ctrl, %layer);
   %this.CheckCursor();
}

function GuiCanvas::popDialog(%this, %ctrl) {
   Parent::popDialog(%this, %ctrl);
   %this.CheckCursor();
}

function GuiCanvas::popLayer(%this, %layer) {
   Parent::popLayer(%this, %layer);
   %this.CheckCursor();
}
}; ActivatePackage(CanvasCursor);
