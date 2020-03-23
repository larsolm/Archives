#pragma once

namespace Pargon
{
	class BufferView;
	class LogWriter;
}

namespace SuperMarioWar
{
	struct TilemapContent;

	namespace Ptm
	{
		auto Load(const Pargon::BufferView& buffer, TilemapContent& content, Pargon::LogWriter& log) -> bool;
	}
}
