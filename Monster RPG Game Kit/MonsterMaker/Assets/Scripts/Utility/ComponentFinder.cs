using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public static class ComponentFinder
	{
		public static T GetComponentInScene<T>(int sceneIndex) where T : Component
		{
			return FindComponentsInScene<T>(sceneIndex).FirstOrDefault();
		}

		public static List<T> ListComponentsInScene<T>(int sceneIndex) where T : Component
		{
			return FindComponentsInScene<T>(sceneIndex).ToList();
		}

		public static IEnumerable<T> FindComponentsInScene<T>(int sceneIndex) where T : Component
		{
			return Object.FindObjectsOfType<T>().Where(o => o.gameObject.scene.buildIndex == sceneIndex);
		}

		public static T GetAsObject<T>(Object unityObject) where T : Object
		{
			var t = unityObject as T;
			if (t != null)
				return t;

			if (typeof(T) == typeof(GameObject))
				return GetAsGameObject(unityObject) as T;

			if (typeof(Component).IsAssignableFrom(typeof(T)))
				return GetAsComponent<T>(unityObject);

			return null;
		}

		public static GameObject GetAsGameObject(Object unityObject)
		{
			var gameObject = unityObject as GameObject;
			if (gameObject != null)
				return gameObject;

			var component = unityObject as Component;
			if (component != null)
				return component.gameObject;

			return null;
		}

		public static T GetAsComponent<T>(Object unityObject) where T : Object
		{
			var t = unityObject as Component as T;
			if (t != null)
				return t;

			var gameObject = unityObject as GameObject;
			if (gameObject != null)
				return gameObject.GetComponent<T>();

			var component = unityObject as Component;
			if (component != null)
				return component.GetComponent<T>();

			return null;
		}
	}
}
