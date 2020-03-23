#include "Pch.h"
#include "Game/Duration.h"

auto Duration::FromBuffer(BufferReader& reader) -> bool
{
	reader.Read(_ticks);
	return true;
}

void Duration::ToBuffer(BufferWriter& writer) const
{
	writer.Write(_ticks);
}

auto Duration::FromBlueprint(BlueprintReader& reader) -> bool
{
	reader.Read(_ticks);
	return true;
}

void Duration::ToBlueprint(BlueprintWriter& writer) const
{
	writer.Write(Ticks());
}

auto Duration::FromString(StringReader& reader) -> bool
{
	auto intValue = 0;

	if (reader.Parse("{}_ticks", intValue))
		_ticks = intValue;

	return true;
}

void Duration::ToString(StringWriter& writer) const
{
	writer.Format("{}_ticks", Ticks());
}
