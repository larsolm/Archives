#include "Pch.h"
#include "Character/Character.h"
#include "Character/Processor.h"
#include "Character/Processors/PlayerProcessor.h"
#include "Character/Processors/EnemyProcessor.h"
#include "Game/Game.h"

DataMap<Processor> Processor::ProcessorMap
{
	DataMapEntry<Processor>{ "Character", &CreateInstance<Processor, Processor> },
	DataMapEntry<Processor>{ "Player", &CreateInstance<Processor, PlayerProcessor> },
	DataMapEntry<Processor>{ "Enemy", &CreateInstance<Processor, EnemyProcessor> },
};

void Processor::UpdateDepth(float elapsed)
{
	// This part will be updated in more general trigger place eventually.
	{
		Character->CanMoveToFront = false;
		Character->CanMoveToMiddle = false;
		Character->CanMoveToBack = false;
		Character->ForceMiddle = false;

		auto triggers = Character->Node().DepthTriggers.All();
		for (auto& trigger : triggers)
		{
			if (Shapes::IsContained(Character->ProjectedPosition, trigger->Polygon))
				trigger->Execute(*Character);
		}
	}

	if (Character->Controller->Vertical != 0.0f)
	{
		if (Character->CanMoveToBack || Character->CanMoveToFront || Character->CanMoveToMiddle)
		{
			switch (Character->Depth)
			{
			case CollisionDepth::Back:
			{
				if (Character->Controller->Vertical < 0.0f)
				{
					if (Character->CanMoveToFront)
						Character->Depth = CollisionDepth::Front;
					else if (Character->CanMoveToMiddle)
						Character->Depth = CollisionDepth::Middle;
				}

				break;
			}
			case CollisionDepth::Middle:
			{
				if (Character->Controller->Vertical < 0.0f && Character->CanMoveToFront)
					Character->Depth = CollisionDepth::Front;
				else if (Character->Controller->Vertical > 0.0f && Character->CanMoveToBack)
					Character->Depth = CollisionDepth::Back;

				break;
			}
			case CollisionDepth::Front:
			{
				if (Character->Controller->Vertical > 0.0f)
				{
					if (Character->CanMoveToBack)
						Character->Depth = CollisionDepth::Back;
					else if (Character->CanMoveToMiddle)
						Character->Depth = CollisionDepth::Middle;
				}

				break;
			}
			}
		}
		else if (Character->ForceMiddle)
		{
			Character->Depth = CollisionDepth::Middle;
		}
	}
	else
	{
		if (Character->ForceMiddle)
			Character->Depth = CollisionDepth::Middle;
	}
}

void Processor::ApplyDamage(float amount)
{
	if (Character->Shield > 0.0f)
	{
		Character->Shield -= amount;

		if (Character->Shield <= 0.0f)
		{
			Character->Shield = 0.0;
			OnShieldDepleted();
		}
	}
	else
	{
		Character->Health -= amount;

		if (Character->Health <= 0.0f)
		{
			Character->Health = 0.0f;
			OnDeath();
		}
	}
}
