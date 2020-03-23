#include "GrungySponge/GrungySponge.h"
#include "App.h"

#include <ppltasks.h>

#include "Pargon/FileSystem/FileSystem.h"
#include "Pargon/Graphics/Apis/Direct3d11/Direct3d11.h"
#include "Pargon/Graphics/Apis/WinRt/WinRt.h"
#include "Pargon/Audio/Apis/XAudio2/XAudio2.h"
#include "Pargon/Input/Apis/Event/EventKeyboardMouseDevice.h"
#include "Pargon/Input/Apis/Emulated/MouseToTouchEmulator.h"
#include "Pargon/Input/Apis/Win32/XInputControllerDevice.h"
#include "Pargon/Networking/Apis/WinSock2/WinSock2.h"

using namespace GrungySponge;

using namespace Pargon;
using namespace Pargon::Application;
using namespace Pargon::Audio;
using namespace Pargon::Graphics;
using namespace Pargon::Input;
using namespace Pargon::Networking;

using namespace concurrency;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Core;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::UI::Core;
using namespace Windows::UI::Input;
using namespace Windows::System;
using namespace Windows::Foundation;
using namespace Windows::Graphics::Display;

namespace
{
	auto GetStdString(Platform::String^ string)
	{
		auto data = string->Data();

		char chars[1024];
		wcstombs(chars, data, 1024);

		return std::string(chars);
	}
}

[Platform::MTAThread]
int main(Platform::Array<Platform::String^>^)
{
	Windows::Storage::StorageFolder^ applicationFolder = Windows::ApplicationModel::Package::Current->InstalledLocation;
	Windows::Storage::StorageFolder^ userFolder = Windows::Storage::ApplicationData::Current->LocalFolder;
	Windows::Storage::StorageFolder^ temporaryFolder = Windows::Storage::ApplicationData::Current->TemporaryFolder;

	auto applicationDirectory = GetStdString(applicationFolder->Path);
	auto userDirectory = GetStdString(userFolder->Path);
	auto temporaryDirectory = GetStdString(temporaryFolder->Path);

	FileSystem::SetDirectories(userDirectory, applicationDirectory + "\\GrungySponge_assets", temporaryDirectory);

	GrungySponge::Include();

	auto direct3DApplicationSource = ref new Direct3DApplicationSource();
	CoreApplication::Run(direct3DApplicationSource);
	return 0;
}

IFrameworkView^ Direct3DApplicationSource::CreateView()
{
	return ref new App();
}

App::App()
{
}

void App::Initialize(CoreApplicationView^ applicationView)
{
	applicationView->Activated += ref new TypedEventHandler<CoreApplicationView^, IActivatedEventArgs^>(this, &App::OnActivated);

	CoreApplication::Suspending += ref new EventHandler<SuspendingEventArgs^>(this, &App::OnSuspending);
	CoreApplication::Resuming += ref new EventHandler<Platform::Object^>(this, &App::OnResuming);
}

void App::SetWindow(CoreWindow^ window)
{
	window->KeyDown += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Core::CoreWindow ^, Windows::UI::Core::KeyEventArgs ^>(this, &GrungySponge::App::OnKeyDown);
	window->KeyUp += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Core::CoreWindow ^, Windows::UI::Core::KeyEventArgs ^>(this, &GrungySponge::App::OnKeyUp);

	window->PointerPressed += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Core::CoreWindow ^, Windows::UI::Core::PointerEventArgs ^>(this, &GrungySponge::App::OnPointerPressed);
	window->PointerReleased += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Core::CoreWindow ^, Windows::UI::Core::PointerEventArgs ^>(this, &GrungySponge::App::OnPointerReleased);
	window->PointerMoved += ref new Windows::Foundation::TypedEventHandler<Windows::UI::Core::CoreWindow ^, Windows::UI::Core::PointerEventArgs ^>(this, &GrungySponge::App::OnPointerMoved);

	window->SizeChanged += ref new TypedEventHandler<CoreWindow^, WindowSizeChangedEventArgs^>(this, &App::OnWindowSizeChanged);
	window->VisibilityChanged += ref new TypedEventHandler<CoreWindow^, VisibilityChangedEventArgs^>(this, &App::OnVisibilityChanged);
	window->Closed += ref new TypedEventHandler<CoreWindow^, CoreWindowEventArgs^>(this, &App::OnWindowClosed);

	DisplayInformation^ currentDisplayInformation = DisplayInformation::GetForCurrentView();

	currentDisplayInformation->DpiChanged += ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnDpiChanged);
	currentDisplayInformation->OrientationChanged += ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnOrientationChanged);

	DisplayInformation::DisplayContentsInvalidated += ref new TypedEventHandler<DisplayInformation^, Object^>(this, &App::OnDisplayContentsInvalidated);
}

void App::Load(Platform::String^ entryPoint)
{
	if (!_game)
	{
		auto window = CoreWindow::GetForCurrentThread();

		_game = std::make_unique<Pargon::Game>();
		_game->AddGraphics<Direct3d11GraphicsDevice, WinRtDirect3d11Canvas>(reinterpret_cast<IUnknown*>(window), window->Bounds.Width, window->Bounds.Height);
		_game->AddAudio<XAudio2Device>();
		_game->AddTouchInput<MouseToTouchEmulator<EventKeyboardMouseDevice>>();
		_game->AddControllerInput<XInputControllerDevice>();
		_game->AddTools<WinSock2TcpServer>();
		_game->LoadGame("GrungySponge.game");
	}
}

void App::Run()
{
	while (!m_windowClosed)
	{
		if (m_windowVisible)
		{
			CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessAllIfPresent);
			Pargon::Application::RunMainLoop();
		}
		else
		{
			CoreWindow::GetForCurrentThread()->Dispatcher->ProcessEvents(CoreProcessEventsOption::ProcessOneAndAllPending);
		}
	}
}

void App::Uninitialize()
{
}

void App::OnActivated(CoreApplicationView^ applicationView, IActivatedEventArgs^ args)
{
	CoreWindow::GetForCurrentThread()->Activate();
}

void App::OnSuspending(Platform::Object^ sender, SuspendingEventArgs^ args)
{
	SuspendingDeferral^ deferral = args->SuspendingOperation->GetDeferral();

	create_task([this, deferral]()
	{
		deferral->Complete();
	});
}

void App::OnResuming(Platform::Object^ sender, Platform::Object^ args)
{
}

void App::OnWindowSizeChanged(CoreWindow^ sender, WindowSizeChangedEventArgs^ args)
{
}

void App::OnVisibilityChanged(CoreWindow^ sender, VisibilityChangedEventArgs^ args)
{
	m_windowVisible = args->Visible;
}

void App::OnWindowClosed(CoreWindow^ sender, CoreWindowEventArgs^ args)
{
	m_windowClosed = true;
}

void App::OnDpiChanged(DisplayInformation^ sender, Object^ args)
{
}

void App::OnOrientationChanged(DisplayInformation^ sender, Object^ args)
{
}

void App::OnDisplayContentsInvalidated(DisplayInformation^ sender, Object^ args)
{
}

namespace
{
	auto TranslateKey(Windows::System::VirtualKey windows)
	{
		auto key = static_cast<size_t>(windows);

		if (key >= 48 && key <= 57)
		{
			return static_cast<Pargon::Input::Key>(key - 22);
		}
		else if (key >= 65 && key <= 90)
		{
			return static_cast<Pargon::Input::Key>(key - 65);
		}
		else if (key >= 112 && key <= 123)
		{
			return static_cast<Pargon::Input::Key>(key - 76);
		}
		else if (key >= 96 && key <= 105)
		{
			return static_cast<Pargon::Input::Key>(key - 48);
		}
		else
		{
			switch (key)
			{
				case 27: return Pargon::Input::Key::Escape;
				case 9: return Pargon::Input::Key::Tab;
				case 32: return Pargon::Input::Key::Space;
				case 8: return Pargon::Input::Key::Backspace;
				case 13: return Pargon::Input::Key::Enter;
				case 46: return Pargon::Input::Key::Delete;
				case 16: return Pargon::Input::Key::LeftShift;
				case 160: return Pargon::Input::Key::LeftShift;
				case 161: return Pargon::Input::Key::RightShift;
				case 17: return Pargon::Input::Key::LeftControl;
				case 162: return Pargon::Input::Key::LeftControl;
				case 163: return Pargon::Input::Key::RightControl;
				case 91: return Pargon::Input::Key::LeftCommand;
				case 92: return Pargon::Input::Key::RightCommand;
				case 164: return Pargon::Input::Key::LeftAlt;
				case 165: return Pargon::Input::Key::RightAlt;
				case 20: return Pargon::Input::Key::CapsLock;
				case 192: return Pargon::Input::Key::Tilde;
				case 187: return Pargon::Input::Key::Plus;
				case 189: return Pargon::Input::Key::Minus;
				case 219: return Pargon::Input::Key::LeftBracket;
				case 221: return Pargon::Input::Key::RightBracket;
				case 220: return Pargon::Input::Key::Slash;
				case 191: return Pargon::Input::Key::Backslash;
				case 188: return Pargon::Input::Key::Comma;
				case 190: return Pargon::Input::Key::Period;
				case 186: return Pargon::Input::Key::Colon;
				case 222: return Pargon::Input::Key::Quote;
				case 37: return Pargon::Input::Key::Left;
				case 39: return Pargon::Input::Key::Right;
				case 38: return Pargon::Input::Key::Up;
				case 40: return Pargon::Input::Key::Down;
				case 36: return Pargon::Input::Key::Home;
				case 35: return Pargon::Input::Key::End;
				case 33: return Pargon::Input::Key::PageUp;
				case 34: return Pargon::Input::Key::PageDown;
				case 110: return Pargon::Input::Key::NumberPeriod;
				case 107: return Pargon::Input::Key::NumberPlus;
				case 109: return Pargon::Input::Key::NumberMinus;
				case 12: return Pargon::Input::Key::NumberEquals;
				case 111: return Pargon::Input::Key::NumberDivide;
				case 106: return Pargon::Input::Key::NumberMultiply;
				default: return Pargon::Input::Key::Unknown;
			}
		}
	}
}

void GrungySponge::App::OnKeyDown(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::KeyEventArgs ^args)
{
	auto key = TranslateKey(args->VirtualKey);
	_game->KeyboardMouseInput()->PressKey(key);
}

void GrungySponge::App::OnKeyUp(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::KeyEventArgs ^args)
{
	auto key = TranslateKey(args->VirtualKey);
	_game->KeyboardMouseInput()->ReleaseKey(key);
}

void GrungySponge::App::OnPointerPressed(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::PointerEventArgs ^args)
{
	_game->KeyboardMouseInput()->SetMouseLeftButton(true);
}

void GrungySponge::App::OnPointerReleased(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::PointerEventArgs ^args)
{
	_game->KeyboardMouseInput()->SetMouseLeftButton(false);
}

void GrungySponge::App::OnPointerMoved(Windows::UI::Core::CoreWindow ^sender, Windows::UI::Core::PointerEventArgs ^args)
{
	auto x = args->CurrentPoint->Position.X;
	auto y = args->CurrentPoint->Position.Y;

	_game->KeyboardMouseInput()->SetMousePosition(static_cast<int>(x), static_cast<int>(y));
}
