#include "Pch.h"
#include "Data/EquipmentData.h"
#include "Parts/Equipment.h"

void Equipment::OnInitialize()
{
	EquipmentData = static_cast<struct EquipmentData*>(Data);
}