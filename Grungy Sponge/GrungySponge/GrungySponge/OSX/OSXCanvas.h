#pragma once

#include "Pargon/Graphics/Graphics.h"
#include "GLView.h"

namespace Pargon
{
    namespace Graphics
    {
        class OSXCanvas : public Canvas
        {
        private:
            GLView* _view;

        public:
            OSXCanvas(GLView* view);

            virtual int Width() override;
            virtual int Height() override;
        };
    }
}
