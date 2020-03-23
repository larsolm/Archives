#pragma once

#include "Pargon/Types/Types.h"

#include "SuperMarioWar/Player.h"

namespace SuperMarioWar
{
	enum class PlayerType
	{
		Human,
		Bot,
		Inactive,
		Count
	};

	enum class Team
	{
		Red,
		Green,
		Blue,
		Yellow
	};

	struct PlayerSettings
	{
		PlayerType PlayerType;
		InputType InputType;
		Pargon::String Character;
		Team TeamColor;
		bool Ready;
	};
	
	enum class GameType
	{
		Classic,
		Deathmatch,
		Chicken
	};

	class GameSettings
	{
	public:
		auto GetPlayerType(int index) const -> Pargon::String;
		void SetPlayerType(int index, PlayerType type);
		void SetCharacter(int index, Pargon::String character);

		void IncrementPlayerType(int playerIndex, int increment);

		auto GetPlayerSettings(int index) const -> const PlayerSettings&;

		GameType GameType = GameType::Classic;

	private:
		Pargon::Array<PlayerSettings, 4> _playerSettings;
	};
}

inline
auto SuperMarioWar::GameSettings::GetPlayerSettings(int index) const -> const PlayerSettings&
{
	return _playerSettings[index];
}

inline
auto SuperMarioWar::GameSettings::GetPlayerType(int index) const -> Pargon::String
{
	return Pargon::ToString<PlayerType>(_playerSettings[index].PlayerType);
}

inline
void SuperMarioWar::GameSettings::SetPlayerType(int index, PlayerType type)
{
	_playerSettings[index].PlayerType = type;
}

inline
void SuperMarioWar::GameSettings::SetCharacter(int index, Pargon::String character)
{
	_playerSettings[index].Character = character;
}

template<> void Pargon::CreateMembers<SuperMarioWar::GameSettings>(Pargon::Reflection::Type* type);
template<> void Pargon::CreateMembers<SuperMarioWar::PlayerType>(Pargon::Reflection::Type* type);
