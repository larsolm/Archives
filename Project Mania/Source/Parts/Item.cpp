#include "Pch.h"
#include "Data/ItemData.h"
#include "Parts/Item.h"

void Item::OnInitialize()
{
	ItemData = static_cast<struct ItemData*>(Data);
}
