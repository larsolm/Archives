using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class UndoScope : GUI.Scope
	{
		private SerializedObject _serializedObject;
		private Object _object;

		public UndoScope(Object objectToUndo)
		{
			_object = objectToUndo;
			EditorGUI.BeginChangeCheck();
			Undo.RecordObject(objectToUndo, objectToUndo.name + " changed");
		}

		public UndoScope(SerializedObject serializedObject)
		{
			_serializedObject = serializedObject;
			serializedObject.Update();
		}

		protected override void CloseScope()
		{
			if (_serializedObject != null)
			{
				_serializedObject.ApplyModifiedProperties();
			}
			else
			{
				Undo.FlushUndoRecordObjects();

				if (EditorGUI.EndChangeCheck())
					EditorUtility.SetDirty(_object);
			}
		}
	}

	public class EditObjectScope : GUI.Scope
	{
		private SerializedObject _serializedObject;
		
		public EditObjectScope(SerializedObject serializedObject)
		{
			_serializedObject = serializedObject;
			serializedObject.ApplyModifiedProperties();
		}

		protected override void CloseScope()
		{
			_serializedObject.Update();
		}
	}
}
