using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public abstract class PropertyControl
	{
		public abstract void Setup(SerializedProperty property, FieldInfo fieldInfo);
		public abstract float GetHeight(SerializedProperty property, GUIContent label);
		public abstract void Draw(Rect position, SerializedProperty property, GUIContent label);

		public T GetObject<T>(SerializedProperty property) where T : class
		{
			var obj = (object)property.serializedObject.targetObject;
			var elements = property.propertyPath.Replace("Array.data[", "[").Split('.');

			foreach (var element in elements)
			{
				if (element.StartsWith("["))
				{
					var indexString = element.Substring(1, element.Length - 2);
					var index = Convert.ToInt32(indexString);

					obj = GetIndexed(obj, index);
				}
				else
				{
					obj = obj.GetType().GetField(element).GetValue(obj);
				}
			}

			return obj as T;
		}

		private object GetIndexed(object obj, int index)
		{
			var array = obj as Array;
			if (array != null)
				return array.GetValue(index);

			var list = obj as IList;
			if (list != null)
				return list[index];

			return null;
		}
	}

	public class ControlDrawer<T> : PropertyDrawer where T : PropertyControl, new()
	{
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			var control = GetControl(property);
			return control.GetHeight(property, label);
		}

		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			var control = GetControl(property);
			control.Draw(position, property, label);
		}

		private Dictionary<string, T> _controls = new Dictionary<string, T>();

		private T GetControl(SerializedProperty property)
		{
			T control;
			var path = property.serializedObject.targetObject.name + property.propertyPath;

			if (!_controls.TryGetValue(path, out control))
			{
				control = new T();
				control.Setup(property, fieldInfo);
				_controls.Add(path, control);
			}

			return control;
		}
	}
}
