#include "OSXCanvas.h"

using namespace Pargon;
using namespace Pargon::Graphics;

OSXCanvas::OSXCanvas(GLView* view) : Canvas(),
_view(view)
{
}

int OSXCanvas::Width()
{
    return _view.bounds.size.width;
}

int OSXCanvas::Height()
{
    return _view.bounds.size.height;
}

//void OSXCanvas::Quit()
//{
//    [NSApp performSelector:@selector(terminate:) withObject:nil afterDelay:0.0];
//}
