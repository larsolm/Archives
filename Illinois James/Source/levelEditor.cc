#include "console/console.h"
#include "WhipGame/levelEditor.h"

IMPLEMENT_CONOBJECT(LevelEditor);

LevelEditor::LevelEditor() {
}

bool LevelEditor::onAdd() {
   if(!Parent::onAdd()) return false;
   return true;
}

void LevelEditor::onMouseDown(const GuiEvent& event) {
   Con::executef(this, 3, "onMouseDown", Con::getIntArg(event.mousePoint.x), Con::getIntArg(event.mousePoint.y));
}

void LevelEditor::onMouseDragged(const GuiEvent& event) {
   Con::executef(this, 3, "onMouseDragged", Con::getIntArg(event.mousePoint.x), Con::getIntArg(event.mousePoint.y));
}