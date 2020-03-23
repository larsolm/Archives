#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

namespace
{
	float NormalizeRotation(float rotation)
	{
		while (rotation < -180)
			rotation += 360;

		while (rotation >= 180)
			rotation -= 360;

		return rotation;
	}
}

void SpongeMover::Update(float elapsed)
{
	if (!_sponge)
		return;

	if (_gameManager->Paused)
		return;

	Spatial2D* spatial = _sponge->Entity()->GetComponent<Spatial2D>();
	CollisionComponent* collision = _sponge->Entity()->GetComponent<CollisionComponent>();
	
	List<Vector2> startPolygon = Polygon::Transform(collision->Polygon, spatial->Offset, spatial->Position, spatial->Size, spatial->Rotation);
	
	ProcessMovement(elapsed, spatial);
	
	List<Vector2> spongePolygon = Polygon::Transform(collision->Polygon, spatial->Offset, spatial->Position, spatial->Size, spatial->Rotation);
	
	ProcessAction(elapsed);
	ProcessFields(spatial, startPolygon, spongePolygon);
	ProcessCollision(spatial, spongePolygon);
	ProcessPits(elapsed, spatial, spongePolygon);
	ProcessScale(spatial, collision);

	//if (CurrentGame()->Input()->IsButtonDown(0, Input::ControllerButton::Back))
	//	CurrentGame()->Quit();
}

void SpongeMover::Reset()
{
	_jumpVelocity = 0;
	_velocity = Vector2(0, 0);
	_angularVelocity = 0;
	_currentPit = nullptr;
	_currentHeight = 1;
}

void SpongeMover::Respawn(Spatial2D* spatial)
{
	spatial->Position = _spawnPosition;
	Reset();
}

void SpongeMover::ProcessPits(float elapsed, Spatial2D* spatial, const List<Vector2>& spongePolygon)
{
	if (_currentHeight >= 1)
	{
		const List<Pit*>& pits = CurrentGame()->World().GetObjects<Pit>().AllObjects();
		bool found = false;
		for (Pit* pit : pits)
		{
			Spatial2D* other = pit->Entity()->GetComponent<Spatial2D>();
			List<Vector2> otherPolygon = Polygon::Transform(pit->Polygon, other->Offset, other->Position, other->Size, other->Rotation);

			Vector2 penetration, normal;
			if (Collision::IsColliding(spongePolygon, otherPolygon, penetration, normal))
			{
				if (penetration.Length() > spatial->Size.Length() / 2)
				{
					found = true;

					if (_currentPit == nullptr)
					{
						_currentPit = pit;

						Rectangle bounds;
						other->GetBoundingBox(bounds);
						float degrees = Trigonometry::ToDegrees(atan2(-penetration.Y, -penetration.X));
						if (degrees < 45 || degrees >= 315)
						{
							_spawnPosition.X = bounds.X + bounds.Width + pit->RightSpawnOffset.X;
							_spawnPosition.Y = bounds.Y + (bounds.Height / 2) + pit->RightSpawnOffset.Y;
						}
						else if (degrees < 135)
						{
							_spawnPosition.X = bounds.X + (bounds.Width / 2) + pit->TopSpawnOffset.X;
							_spawnPosition.Y = bounds.Y + bounds.Height + pit->TopSpawnOffset.Y;
						}
						else if (degrees < 225)
						{
							_spawnPosition.X = bounds.X + pit->LeftSpawnOffset.X;
							_spawnPosition.Y = bounds.Y + (bounds.Height / 2) + pit->LeftSpawnOffset.Y;
						}
						else if (degrees < 315)
						{
							_spawnPosition.X = bounds.X + (bounds.Width / 2) + pit->BottomSpawnOffset.X;
							_spawnPosition.Y = bounds.Y + pit->BottomSpawnOffset.Y;
						}
					}
				}
			}
		}

		if (!found)
			_currentPit = nullptr;
	}

	if (_currentPit != nullptr && _currentHeight <= 1)
	{
		_jumpVelocity -= 0.25f * elapsed;
		_currentHeight += _jumpVelocity;

		if (_currentHeight <= 0)
			Respawn(spatial);
	}
}

void SpongeMover::ProcessScale(Spatial2D* spatial, CollisionComponent* collision)
{
	Vector2 previousSize = spatial->Size;

	if (_currentHeight > 1)
		spatial->Size = _sponge->MinimumSize * (1 + ((_sponge->JumpScale / 10000) * pow(_currentHeight, 2)));
	else if (_currentHeight < 1)
		spatial->Size = _sponge->MinimumSize * _currentHeight;
	else
		spatial->Size = _sponge->MinimumSize;

	_heightScale = spatial->Size.X / previousSize.X;

	if (_heightScale != 1)
	{
		for (Vector2& point : collision->Polygon)
		{
			point.X /= _heightScale;
			point.Y /= _heightScale;
		}
	}
}

void SpongeMover::ProcessFields(Spatial2D* spatial, const List<Vector2>& startPolygon, const List<Vector2>& endPolygon)
{
	List<Vector2> polygon = Polygon::FindConvexHull(startPolygon, endPolygon);

	if (_currentHeight == 1)
	{
		if (_gameManager->SpongeCapacity() < 1 || _gameManager->CurrentLevel.DirtCapacity <= 0)
		{
			const List<ClearField*>& dirts = _clearFields->ObjectsWithTag("Dirt");
			for (ClearField* dirt : dirts)
				_clearFieldManager->ClearPolygon(_gameManager, dirt, polygon);
		}

		if (_gameManager->CurrentLevel.UncoverableObjects > 0)
		{
			const List<ClearField*>& uncoverables = _clearFields->ObjectsWithTag("Uncoverable");
			for (ClearField* uncoverable : uncoverables)
			{
				if (uncoverable->ClearedSpace[0] >= uncoverable->TotalSpace[0])
					continue;

				_clearFieldManager->ClearPolygon(nullptr, uncoverable, polygon);

				if (uncoverable->ClearedSpace[0] >= uncoverable->TotalSpace[0])
					_gameManager->UncoveredObjects++;
			}
		}
	}
}

void SpongeMover::ProcessCollision(Spatial2D* spatial, const List<Vector2>& spongePolygon)
{
	if (_currentHeight == 1)
	{
		const List<CollisionComponent*>& bucktes = CurrentGame()->World().GetObjects<CollisionComponent>().ObjectsWithTag("Bucket");
		for (CollisionComponent* bucket : bucktes)
		{
			Spatial2D* other = bucket->Entity()->GetComponent<Spatial2D>();
			List<Vector2> otherPolygon = Polygon::Transform(bucket->Polygon, other->Offset, other->Position, other->Size, other->Rotation);

			Vector2 penetration, normal;
			if (Collision::IsColliding(spongePolygon, otherPolygon, penetration, normal))
			{
				for (size_t i = 0; i < 3; i++)
					_gameManager->HeldDirt[i] = 0;
			}
		}

		const List<CollisionComponent*>& collectables = CurrentGame()->World().GetObjects<CollisionComponent>().ObjectsWithTag("Collectable");
		CollisionComponent* toRemove = nullptr;
		for (CollisionComponent* collectable : collectables)
		{
			Spatial2D* other = collectable->Entity()->GetComponent<Spatial2D>();
			List<Vector2> otherPolygon = Polygon::Transform(collectable->Polygon, other->Offset, other->Position, other->Size, other->Rotation);

			Vector2 penetration, normal;
			if (Collision::IsColliding(spongePolygon, otherPolygon, penetration, normal))
			{
				_gameManager->FoundObjects++;
				toRemove = collectable;
			}
		}

		if (toRemove != nullptr)
			CurrentGame()->RemoveObject(toRemove->Entity());
	}

	const List<CollisionComponent*>& walls = CurrentGame()->World().GetObjects<CollisionComponent>().ObjectsWithTag("Static");
	for (CollisionComponent* wall : walls)
	{
		Spatial2D* other = wall->Entity()->GetComponent<Spatial2D>();
		List<Vector2> otherPolygon = Polygon::Transform(wall->Polygon, other->Offset, other->Position, other->Size, other->Rotation);

		Vector2 penetration, normal;
		if (Collision::IsColliding(spongePolygon, otherPolygon, penetration, normal))
		{
			Vector2 direction = -_velocity;
			float angle = direction.AngleTo(normal);
			Vector2 velocity = direction.Rotate(-angle * 2);
			_velocity = velocity;
			spatial->Position = spatial->Position - penetration;
		}
	}

	if (_gameManager->CurrentLevel.Laps > 0)
	{
		FinishLine* finishLine = CurrentGame()->World().GetObjects<FinishLine>().ObjectNamed("FinishLine");
		Spatial2D* other = finishLine->Entity()->GetComponent<Spatial2D>();
		List<Vector2> otherPolygon = Polygon::Transform(finishLine->Polygon, other->Offset, other->Position, other->Size, other->Rotation);

		Vector2 penetration, normal;
		if (Collision::IsColliding(spongePolygon, otherPolygon, penetration, normal))
		{
			if (!_onFinishLine)
			{
				if (abs(_velocity.AngleTo(finishLine->Direction)) <= 90)
					_gameManager->LapsCompleted++;
			}
			_onFinishLine = true;
		}
		else
		{
			if (_onFinishLine)
			{
				if (abs(_velocity.AngleTo(finishLine->Direction)) >= 90)
					_gameManager->LapsCompleted--;
			}
			_onFinishLine = false;
		}
	}
}

void SpongeMover::ProcessMovement(float elapsed, Spatial2D* spatial)
{
	if (_currentHeight >= 1)
	{
		//MoveWithMotion(elapsed, _gameManager->LevelFinished());
		MoveWithController(_gameManager->LevelFinished());

		spatial->Position = spatial->Position + _velocity * elapsed;
		spatial->Rotation = spatial->Rotation + _angularVelocity * elapsed;
	}
}


void SpongeMover::MoveWithMotion(float elapsed, bool disableInput)
{
	Spatial2D* spatial = _sponge->Entity()->GetComponent<Spatial2D>();

	const Input::MotionState& motion = CurrentGame()->MotionInput()->GetMotionState();
	
	Vector2 move = Vector2(0, 0);
	if (!disableInput)
		move = Vector2(motion.Pitch, motion.Roll);
	
	if (CurrentGame()->KeyboardMouseInput()->WasLeftMouseJustPressed())
		_actionPressed = true;
	else
		_actionPressed = false;

	if (move.Length() > 0.25f)
	{
		float targetAngle = Trigonometry::ToDegrees(atan2(move.Y, move.X)) - 90;
		float angleDifference = NormalizeRotation(spatial->Rotation - targetAngle);
		float maneuverability = _sponge->Maneuverability * elapsed;

		if (angleDifference > maneuverability)
			spatial->Rotation -= maneuverability;
		else if (angleDifference < -maneuverability)
			spatial->Rotation += maneuverability;
		else
			spatial->Rotation = targetAngle;
	}

	float length = move.Length();
	if (length <= 0.25f)
		move.Set(0, 0);
	else if ((length > 0.25f) && length < 0.75f)
		move = move.NormalizeTo(0.5f);
	else if (length >= 0.75f)
		move = move.Normalize();

	if (_currentHeight == 1)
		_velocity = _velocity + move * _sponge->Acceleration;
	_velocity = _velocity * _sponge->Friction;

	if (_velocity.Length() > _sponge->MaxSpeed)
		_velocity = _velocity.NormalizeTo(_sponge->MaxSpeed);
}

void SpongeMover::MoveWithController(bool disableInput)
{
	Spatial2D* spatial = _sponge->Entity()->GetComponent<Spatial2D>();

	Vector2 moveVector = Vector2(0, 0);
	if (!disableInput)
		moveVector = CurrentGame()->ControllerInput()->GetLeftStickPosition(0);

	if (CurrentGame()->ControllerInput()->WasButtonJustPressed(0, Input::ControllerButton::A))
		_actionPressed = true;
	else
		_actionPressed = false;
	
	if (moveVector.Length() != 0)
	{
		if (_currentHeight == 1)
			_velocity = _velocity + (moveVector * _sponge->Acceleration);

		if (_velocity.Length() > _sponge->MaxSpeed)
			_velocity = _velocity * _sponge->MaxSpeed * (1.0f / _velocity.Length());

		float moveAngle = Trigonometry::ToDegrees(atan2(moveVector.Y, moveVector.X)) - 90;
		float angleDifference = NormalizeRotation(spatial->Rotation - moveAngle);

		if (angleDifference < 0)
			_angularVelocity += _sponge->Maneuverability;
		else if (angleDifference > 0)
			_angularVelocity -= _sponge->Maneuverability;

		if (_angularVelocity > _sponge->MaxAngularVelocity)
			_angularVelocity = _sponge->MaxAngularVelocity;
		else if (_angularVelocity < -_sponge->MaxAngularVelocity)
			_angularVelocity = -_sponge->MaxAngularVelocity;
	}
	else
	{
		_velocity = _velocity * _sponge->Friction;
		_angularVelocity *= _sponge->Friction;

		if (_velocity.Length() < _sponge->Acceleration)
			_velocity = Vector2(0, 0);
	}
}

void SpongeMover::ProcessAction(float elapsed)
{
	if (_sponge->CurrentType == SpongeType::Normal)
		return;
	else if (_sponge->CurrentType == SpongeType::Jumping)
		Jump(elapsed);
	else if (_sponge->CurrentType == SpongeType::Speed)
		Speed(elapsed);
	else if (_sponge->CurrentType == SpongeType::Burst)
		Burst(elapsed);
}

void SpongeMover::Jump(float elapsed)
{
	if (_currentHeight < 1)
		return;

	if (_actionPressed && _currentHeight == 1)
		_jumpVelocity += _sponge->JumpHeight;

	if (_currentHeight >= 1)
	{
		_jumpVelocity -= _sponge->Gravity * elapsed;
		_currentHeight += _jumpVelocity * elapsed;
	}

	if (_currentHeight < 1)
	{
		_jumpVelocity = 0;
		_currentHeight = 1;
	}
}

void SpongeMover::Speed(float elapsed)
{

}

void SpongeMover::Burst(float elapsed)
{

}

void SpongeMover::Flip(float elapsed)
{

}
