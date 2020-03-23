#ifndef _LEVEL_EDITOR_H_
#define _LEVEL_EDITOR_H_

#include "gui/guiControl.h"

class LevelEditor : public GuiControl {
private:
   typedef GuiControl Parent;
   
public:
   DECLARE_CONOBJECT(LevelEditor);
   LevelEditor();

   //Parental methods
   bool onAdd();

   //input events
   void onMouseDown(const GuiEvent& event);
   void onMouseDragged(const GuiEvent& event);
};

#endif
