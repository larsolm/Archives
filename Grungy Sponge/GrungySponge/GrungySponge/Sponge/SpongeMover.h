#pragma once

#include "Pargon/GameCore/GameCore.h"
#include "Pargon/Types/List.h"
#include "Pargon/Math/Math.h"
#include "GrungySponge/Game/GameManager.h"
#include "GrungySponge/Sponge/Sponge.h"
#include "GrungySponge/World/ClearFieldManager.h"
#include "GrungySponge/World/ClearField.h"
#include "GrungySponge/World/Pit.h"

namespace Pargon
{
	class Spatial2D;
	class CollisionComponent;
}

namespace GrungySponge
{
    class ClearField;
    
	class SpongeMover : public Pargon::Updater
	{
	private:
		Pargon::GameObjectReference<SpongeData> _sponge;
		Pargon::GameObjectReference<GameManager> _gameManager;
		Pargon::GameObjectReference<ClearFieldManager> _clearFieldManager;
		Pargon::GameListReference<ClearField> _clearFields;

		Pit* _currentPit = nullptr;
		
		Pargon::Vector2 _velocity = Pargon::Vector2(0, 0);
		Pargon::Vector2 _spawnPosition;
		float _jumpVelocity = 0;
		float _angularVelocity = 0;
		float _currentHeight = 1;
		float _heightScale = 1;

		bool _actionPressed = false;
		bool _onFinishLine = true;

	public:
		virtual void Update(float elapsed);
		void Reset();

	private:
		void ProcessMovement(float elapsed, Pargon::Spatial2D* spatial);
		void ProcessAction(float elapsed);
		void ProcessScale(Pargon::Spatial2D* spatial, Pargon::CollisionComponent* collision);
		void ProcessFields(Pargon::Spatial2D* spatial, const Pargon::List<Pargon::Vector2>& startPolygon, const Pargon::List<Pargon::Vector2>& endPolygon);
		void ProcessCollision(Pargon::Spatial2D* spatial, const Pargon::List<Pargon::Vector2>& spongePolygon);
		void ProcessPits(float elapsed, Pargon::Spatial2D* spatial, const Pargon::List<Pargon::Vector2>& spongePolygon);

		void MoveWithMotion(float elapsed, bool disableInput);
		void MoveWithController(bool disableInput);

		void Respawn(Pargon::Spatial2D* spatial);

		void Jump(float elapsed);
		void Speed(float elapsed);
		void Burst(float elapsed);
		void Flip(float elapsed);
	};
}

