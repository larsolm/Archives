#pragma once

#include "Pargon/Types/Array.h"
#include "Pargon/Types/String.h"
#include "Pargon/Types/Buffer.h"
#include "Pargon/Data/Resource.h"
#include "Pargon/Data/DataObject.h"
#include "Pargon/Graphics/Texture.h"
#include "GrungySponge/Sponge/Sponge.h"

namespace GrungySponge
{
	class ClearField : public Pargon::Component
	{
	public:
		int Width;
		int Height;
		Pargon::Array<int, 3> TotalSpace;
        Pargon::Array<int, 3> ClearedSpace;

		Pargon::String MaskFilename;
        Pargon::Graphics::Texture Mask;
	};
}

template<> void Pargon::CreateMembers<GrungySponge::ClearField>(Reflection::Type* type);
