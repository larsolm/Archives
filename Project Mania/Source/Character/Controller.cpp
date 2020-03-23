#include "Pch.h"
#include "Character/Controller.h"
#include "Character/Controllers/PlayerController.h"
#include "Character/Controllers/EnemyController.h"

DataMap<Controller> Controller::ControllerMap
{
	DataMapEntry<Controller>{ "Character", &CreateInstance<Controller, Controller> },
	DataMapEntry<Controller>{ "Player", &CreateInstance<Controller, PlayerController> },
	DataMapEntry<Controller>{ "Enemy", &CreateInstance<Controller, EnemyController> }
};
