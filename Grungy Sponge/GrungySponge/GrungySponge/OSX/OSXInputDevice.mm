#include "OSXInputDevice.h"

#import <GameController/GCController.h>

using namespace Pargon;
using namespace Pargon::Input;

bool OSXInputDevice::Startup()
{
    Device::Startup();
    
    NSArray* controllers = [GCController controllers];
    
    for (GCController* controller in controllers)
    {
        unsigned int controllerIndex = (int)controller.playerIndex;
        GCExtendedGamepad* profile = controller.extendedGamepad;
        
        profile.buttonA.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::A);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::A);
        };
        
        profile.buttonB.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::B);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::B);
        };
        
        profile.buttonX.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::X);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::X);
        };
        
        profile.buttonY.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::Y);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::Y);
        };
        
        profile.dpad.left.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::DPadLeft);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::DPadLeft);
        };
        
        profile.dpad.right.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::DPadRight);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::DPadRight);
        };
        
        profile.dpad.up.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::DPadUp);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::DPadUp);
        };
        
        profile.dpad.down.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::DPadDown);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::DPadDown);
        };
        
        profile.leftShoulder.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::LeftBumper);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::LeftBumper);
        };
        
        profile.rightShoulder.valueChangedHandler = ^(GCControllerButtonInput *button, float value, BOOL pressed)
        {
            if (pressed)
                PressButton(controllerIndex, Pargon::Input::ControllerButton::RightBumper);
            else
                ReleaseButton(controllerIndex, Pargon::Input::ControllerButton::RightBumper);
        };
        
        profile.leftTrigger.valueChangedHandler = ^(GCControllerButtonInput* button, float value, BOOL pressed)
        {
            SetLeftTrigger(controllerIndex, value, 0.1f);
        };
        
        profile.rightTrigger.valueChangedHandler = ^(GCControllerButtonInput* button, float value, BOOL pressed)
        {
            SetRightTrigger(controllerIndex, value, 0.1f);
        };
        
        /*profile.leftThumbstick.xAxis.valueChangedHandler = ^(GCControllerAxisInput* axis, float value)
        {
            SetLeftStickX(controllerIndex, value);
        };
        
        profile.leftThumbstick.yAxis.valueChangedHandler = ^(GCControllerAxisInput* axis, float value)
        {
            SetLeftStickY(controllerIndex, value);
        };
        
        profile.rightThumbstick.xAxis.valueChangedHandler = ^(GCControllerAxisInput* axis, float value)
        {
            SetRightStickX(controllerIndex, value);
        };
        
        profile.rightThumbstick.yAxis.valueChangedHandler = ^(GCControllerAxisInput* axis, float value)
        {
            SetRightStickY(controllerIndex, value);
        };*/
    }
    
    return true;
}

void OSXInputDevice::Shutdown()
{
}

void OSXInputDevice::Update(float elapsed)
{
    
}