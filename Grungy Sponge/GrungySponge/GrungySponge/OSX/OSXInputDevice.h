#include "Pargon/Input/Input.h"

namespace Pargon
{
    namespace Input
    {
        class OSXInputDevice : public Device
        {
        public:
            virtual bool Startup() override;
            virtual void Shutdown() override;
            virtual void Update(float elapsed) override;
        };
    }
}