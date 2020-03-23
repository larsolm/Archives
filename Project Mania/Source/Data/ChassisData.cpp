#include "Pch.h"
#include "Data/ChassisData.h"
#include "Game/Game.h"

void ChassisData::FromBlueprint(BlueprintReader& reader)
{
	Data::FromBlueprint(reader);

	String skeleton;

	reader.ReadChild("Skeleton", skeleton);

	Skeleton = Game()->Data.GetSkeletonData(skeleton);
	Attachments.Clear();

	if (reader.MoveDown("Attachments"))
	{
		for (auto valid = reader.FirstChild(); valid; valid = reader.NextChild())
		{
			auto& attachment = Attachments.Increment();

			String part;
			List<String> components;
			List<String> augments;

			reader.ReadChild("Bone", attachment.Bone);
			reader.ReadChild("Part", part);
			reader.ReadChild("Components", components);
			reader.ReadChild("Augments", augments);

			auto& bone = Skeleton->GetBone(attachment.Bone);
			switch (bone.AttachmentType)
			{
				case AttachmentType::Equipment: attachment.Part = Game()->Data.GetEquipmentData(part); break;
				case AttachmentType::Item: attachment.Part = Game()->Data.GetItemData(part); break;
				case AttachmentType::Weapon: attachment.Part = Game()->Data.GetWeaponData(part); break;
				case AttachmentType::None: break;
			}

			for (auto& component : components)
				attachment.Components.Add(Game()->Data.GetComponentData(component));

			for (auto& augment : augments)
				attachment.Augments.Add(Game()->Data.GetAugmentData(augment));
		}

		reader.MoveUp();
	}
}
