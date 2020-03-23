using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	[AddComponentMenu("Monster Maker/UI/Selection Control")]
	public class SelectionControl : MonoBehaviour
	{
		[Tooltip("The prefab that this object will duplicate for each of the prompt's options.")] public SelectionItem Item;

		protected InstructionListener _listener;
		protected object[] _options;
		protected SelectionItem[] _items;
		protected int _currentSelection;

		public bool IsActivated { get { return _listener != null && isActiveAndEnabled; } }

		public void Activate(InstructionListener listener, object[] options)
		{
			_listener = listener;
			_options = options;
			_currentSelection = -1;

			// POOL THIS!
			_items = _options == null ? null : new SelectionItem[_options.Length];

			if (Item && _items != null)
			{
				for (var i = 0; i < _options.Length; i++)
				{
					var item = Instantiate(Item, transform);
					item.Create(i, _options[i]);
					item.Blur();

					_items[i] = item;
				}
			}

			ItemsCreated();
			UpdateSelection(0);
		}

		public void Deactivate()
		{
			if (_items != null)
			{
				foreach (var item in _items)
					Destroy(item.gameObject);
			}

			_items = null;
			_listener = null;
			_options = null;
		}

		public virtual void Accept()
		{
			Select();
		}

		public virtual void Cancel()
		{
			_listener.Dismiss();
		}

		public virtual void Left()
		{
		}

		public virtual void Right()
		{
		}

		public virtual void Up()
		{
		}

		public virtual void Down()
		{
		}

		protected virtual void ItemsCreated()
		{
		}

		protected virtual void Awake()
		{
			gameObject.SetActive(false);
		}

		protected virtual void Select()
		{
			_listener.Select(_options[_currentSelection]);
		}

		protected virtual void UpdateSelection(int index)
		{
			if (_currentSelection != index && _items != null)
			{
				if (_currentSelection >= 0 && _currentSelection < _items.Length) _items[_currentSelection].Blur();
				if (index >= 0 && _currentSelection < _items.Length) _items[index].Focus();

				_currentSelection = index;
			}
		}
	}
}
