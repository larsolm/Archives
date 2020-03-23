#include "Pch.h"
#include "Data/Data.h"

void Data::FromBlueprint(BlueprintReader& reader)
{
	reader.ReadChild("DisplayName", DisplayName);
}
