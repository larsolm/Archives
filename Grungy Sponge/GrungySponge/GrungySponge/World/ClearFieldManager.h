#pragma once

#include "Pargon/GameCore/GameCore.h"
#include "Pargon/Graphics/Texture.h"
#include "Pargon/Types/String.h"
#include "Pargon/Types/Dictionary.h"
#include "GrungySponge/World/ClearField.h"

namespace Pargon
{
	template<typename T> class List;
	class Vector2;
	class Spatial2D;
	struct Color;
	class CollisionComponent;
}

namespace GrungySponge
{
	class GameManager;

	class ClearFieldManager : public Pargon::GameObject
	{
	protected:        
		Pargon::GameListReference<ClearField> _clearFields;

		bool Clear(GameManager* gameManager, ClearField* field, Pargon::Color* colors, int x, int y);

	public:
		int ClearPolygon(GameManager* gameManager, ClearField* field, const Pargon::List<Pargon::Vector2>& polygon);

		void LoadLevel();
		void Reset();
		
		int TotalSpace(const Pargon::String& tag, int channel);
		int ClearedSpace(const Pargon::String& tag, int channel);
	};
}
