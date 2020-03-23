#include "GrungySponge/GrungySponge.h"

using namespace Pargon;
using namespace GrungySponge;

void ClearFieldManager::LoadLevel()
{
    Reset();
    
	for (ClearField* clearField : _clearFields->AllObjects())
	{
        auto& content = clearField->Mask.Content();
        
        int maskArea = content.Width * content.Height;
        
        clearField->Width = content.Width;
		clearField->Height = content.Height;
        
        for (int channel = 0; channel < 3; channel++)
            clearField->TotalSpace[channel] = maskArea;
        
        for (int i = 0; i < maskArea; i++)
        {
            for (int channel = 0; channel < 3; channel++)
            {
                if (content.Data[i * 4 + channel] == 0)
                    clearField->TotalSpace[channel]--;
            }
        }
	}
}

void ClearFieldManager::Reset()
{
	for (ClearField* clearField : _clearFields->AllObjects())
	{
		for (int channel = 0; channel < 3; channel++)
			clearField->ClearedSpace[channel] = 0;

        clearField->Mask.SetFilename(clearField->MaskFilename);
		auto& content = clearField->Mask.Content();
	}
}

namespace
{
	struct Scanline
	{
		int Min = std::numeric_limits<int>::max();
		int Max = std::numeric_limits<int>::min();
	};
}

int ClearFieldManager::ClearPolygon(GameManager* gameManager, ClearField* field, const List<Vector2>& polygon)
{
    Spatial2D* fieldSpatial = field->Entity()->GetComponent<Spatial2D>();

    auto& content = field->Mask.Data().ManualLoad("ClearFieldManager::ClearPolygon");
    
	Color* colors = reinterpret_cast<Color*>(content.Data.get());

	Rectangle fieldBounds;
	fieldSpatial->NaturalOffset.Set(0.0f, 0.0f);
	fieldSpatial->GetBoundingBox(fieldBounds);

	int minY = std::numeric_limits<int>::max();
	int maxY = std::numeric_limits<int>::min();

	List<Point2> points(polygon.Count());
	for (size_t i = 0; i < polygon.Count(); i++)
	{
		points[i].X = static_cast<int>(polygon[i].X - fieldBounds.X);
		points[i].Y = field->Height - static_cast<int>(polygon[i].Y - fieldBounds.Y);

		if (points[i].Y < minY)
			minY = points[i].Y;

		if (points[i].Y > maxY)
			maxY = points[i].Y;
	}

	if (minY < 0)
		minY = 0;

	if (maxY > field->Height - 1)
		maxY = field->Height - 1;

	int count = maxY - minY + 1;

	if (count <= 0)
		return 0;

	List<Scanline> scanlines(count);

	for (size_t i = 0; i < points.Count(); i++)
	{
		size_t n = (i + 1) % points.Count();
		if (points[i].Y == points[n].Y)
			continue;

		int xhIncrement = 1;
		int xlIncrement = 1;
		int yhIncrement = 1;
		int ylIncrement = 1;
		int dx = points[n].X - points[i].X;
		int dy = points[n].Y - points[i].Y;

		if (dx < 0)
		{
			dx = -dx;
			xhIncrement = xlIncrement = -1;
		}

		if (dy < 0)
		{
			dy = -dy;
			yhIncrement = ylIncrement = -1;
		}

		int dLong, dShort;
		if (dx >= dy)
		{
			dLong = dx;
			dShort = dy;
			ylIncrement = 0;
		}
		else
		{
			dLong = dy;
			dShort = dx;
			xlIncrement = 0;
		}

		int d = 2 * dShort - dLong;
		int dlIncrement = 2 * dShort;
		int dhIncrement = 2 * dShort - 2 * dLong;

		int x = points[i].X;
		int y = points[i].Y;
		for (int j = 0; j <= dLong; j++)
		{
			int index = y - minY;
			if (index >= 0 && index < static_cast<int>(scanlines.Count()))
			{
				if (x < scanlines[index].Min)
					scanlines[index].Min = x;

				if (x > scanlines[index].Max)
					scanlines[index].Max = x;
			}

			if (d >= 0)
			{
				x += xhIncrement;
				y += yhIncrement;
				d += dhIncrement;
			}
			else
			{
				x += xlIncrement;
				y += ylIncrement;
				d += dlIncrement;
			}
		}
	}

	int amountCleared = 0;
	for (int i = 0; i < count; i++)
	{
		for (int j = scanlines[i].Min; j <= scanlines[i].Max; j++)
		{
			if (Clear(gameManager, field, colors, j, minY + i))
				amountCleared++;
		}
	}

	return amountCleared;
}

bool ClearFieldManager::Clear(GameManager* gameManager, ClearField* field, Color* colors, int x, int y)
{
	if (x >= 0 && x < field->Width && y >= 0 && y < field->Height)
	{
		int channel = -1;
		int index = x + y * field->Width;
		if (colors[index].R != 0)
			channel = 0;
		else if (colors[index].G != 0)
			channel = 1;
		else if (colors[index].B != 0)
			channel = 2;

		if (channel >= 0)
		{
			memset(colors + index, 0, sizeof(Color));
			field->ClearedSpace[channel]++;

			if (gameManager != nullptr)
				gameManager->HeldDirt[channel]++;

			return true;
		}

		field->Mask.Reset();
	}

	return false;
}

int ClearFieldManager::TotalSpace(const String& tag, int channel)
{
	int totalSpace = 0;
	
	const List<ClearField*>& clearFields = _clearFields->ObjectsWithTag(tag);
	for (ClearField* clearField : clearFields)
		totalSpace += clearField->TotalSpace[channel];

	return totalSpace;
}

int ClearFieldManager::ClearedSpace(const String& tag, int channel)
{
	int clearedSpace = 0;

	const List<ClearField*>& clearFields = _clearFields->ObjectsWithTag(tag);
	for (ClearField* clearField : clearFields)
		clearedSpace += clearField->ClearedSpace[channel];

	return clearedSpace;
}
