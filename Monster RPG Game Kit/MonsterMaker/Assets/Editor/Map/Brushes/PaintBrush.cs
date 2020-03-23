using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[CustomGridBrush(true, true, true, "Paint Brush")]
	public class PaintBrush : GridBrush
	{
		public override void Paint(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			base.Paint(grid, brushTarget, position);

			UpdateProperties(grid);
		}

		public override void Erase(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			base.Erase(grid, brushTarget, position);
			
			UpdateProperties(grid);
		}

		public override void BoxFill(GridLayout grid, GameObject brushTarget, BoundsInt position)
		{
			base.BoxFill(grid, brushTarget, position);
			
			UpdateProperties(grid);
		}

		public override void BoxErase(GridLayout grid, GameObject brushTarget, BoundsInt position)
		{
			base.BoxErase(grid, brushTarget, position);
			
			UpdateProperties(grid);
		}

		public override void FloodFill(GridLayout grid, GameObject brushTarget, Vector3Int position)
		{
			base.FloodFill(grid, brushTarget, position);
			
			UpdateProperties(grid);
		}

		public override void Move(GridLayout grid, GameObject brushTarget, BoundsInt from, BoundsInt to)
		{
			base.Move(grid, brushTarget, from, to);
			
			UpdateProperties(grid);
		}
		
		public void UpdateProperties(GridLayout grid)
		{
			var properties = grid.GetComponent<MapProperties>();
			if (!properties)
				properties = grid.gameObject.AddComponent<MapProperties>();

			Undo.RecordObject(properties, "Properties Bounds Changed");

			properties.UpdateBounds();
		}
	}
}
