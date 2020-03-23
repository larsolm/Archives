#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

void ScrollContainer::Start()
{
	_camera = CurrentGame()->World().GetObjects<Camera2D>().ObjectNamed("UiCamera");

	const List<Spatial2D*>& spatials = CurrentGame()->World().GetObjects<Spatial2D>().ObjectsWithTag(ContainerName);
	for (Spatial2D* spatial : spatials)
	{
		int index = spatial->Entity()->GetComponent<IndexComponent>()->Index;
		spatial->Position.X = LeftBounds + (index * Spacing);
		spatial->Position.Y = 25;
	}

	_rightBounds = (spatials.Count() - 1) * Spacing;
}

ScrollContainer::~ScrollContainer()
{
	_camera = nullptr;
}

void ScrollContainer::Update(float elapsed)
{
	if (_camera == nullptr)
		return;

	const Input::TouchState& touchState = CurrentGame()->TouchInput()->GetTouchState();
	Vector2 worldPosition = _camera->ScreenToWorld(touchState.X, touchState.Y, touchState.Width, touchState.Height);

	if (CurrentGame()->TouchInput()->WasTouchJustPressed())
	{
		_dragging = true;
		_scrolling = 0;
		_previousX = worldPosition.X;
	}
	else if (CurrentGame()->TouchInput()->WasTouchJustReleased())
	{
		_dragging = false;
		if (_scrollX < LeftBounds)
		{
			_destinationX = LeftBounds;
		}
		else if (_scrollX > _rightBounds)
		{
			_destinationX = _rightBounds;
		}
		else
		{
			int mod = static_cast<int>(_scrollX) % static_cast<int>(Spacing);
			_destinationX = mod > Spacing / 2 ? static_cast<int>(_scrollX) + (Spacing - mod) : static_cast<int>(_scrollX) - mod;
		}
		
		if (_scrollX != _destinationX)
		{
			_scrolling = _scrollX > _destinationX ? 1 : -1;
		}
	}
	
	if (_dragging)
	{
		float change = worldPosition.X - _previousX;

		ScrollChildren(change);

		_previousX = worldPosition.X;
	}
	else if (_scrolling != 0)
	{
		ScrollChildren(_scrolling * ScrollSpeed * elapsed);

		if (_scrollX < _destinationX + (ScrollSpeed * elapsed) && _scrollX > _destinationX - (ScrollSpeed * elapsed))
		{
			_scrolling = 0;
			ScrollChildren(_scrollX - _destinationX);
		}
	}
}

void ScrollContainer::ScrollChildren(float amount)
{
	_scrollX -= amount;

	const List<Spatial2D*>& spatials = CurrentGame()->World().GetObjects<Spatial2D>().ObjectsWithTag(ContainerName);
	for (Spatial2D* spatial : spatials)
		spatial->Position.X += amount;
}

template<> void Pargon::CreateMembers<GrungySponge::ScrollContainer>(Reflection::Type* type)
{
	Serialization::SerializeAsClass<ScrollContainer>();

	type->AddField("ContainerName", &ScrollContainer::ContainerName);
	type->AddField("LeftBounds", &ScrollContainer::LeftBounds);
	type->AddField("Spacing", &ScrollContainer::Spacing);
	type->AddField("PositionY", &ScrollContainer::PositionY);
	type->AddField("ScrollSpeed", &ScrollContainer::ScrollSpeed);
}