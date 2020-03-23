using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class MovesControl
	{
		private List<Ability> _abilities;
		private List<Move> _moves;
		private EditableList<Move> _list = new EditableList<Move>();
		private List<VariableStoreControl> _traitStores = new List<VariableStoreControl>();
		private string[] _abilityNames;

		public MovesControl(List<Ability> abilities, List<Move> moves)
		{
			_abilities = abilities;
			_moves = moves;
			_traitStores = new List<VariableStoreControl>();

			var list = _list.Setup(moves, "Moves", "", false, true, false, true, true, DrawMove, RemoveMove);
			list.onAddDropdownCallback += AddMove;
			list.elementHeightCallback += GetHeight;

			foreach (var move in moves)
				MoveAdded(move);

			_abilityNames = _abilities.Select(ability => ability.name).ToArray();
		}

		private float GetHeight(int index)
		{
			return _traitStores[index].GetHeight(null, null);
		}

		public void Draw()
		{
			_list.DrawList();
		}

		private void DrawMove(Rect rect, int index)
		{
			_traitStores[index].Draw(rect, null, GUIContent.none);
		}

		private class AddPopup : PopupWindowContent
		{
			private MovesControl _control;

			public AddPopup(MovesControl control)
			{
				_control = control;
			}

			public override Vector2 GetWindowSize()
			{
				return new Vector2(200, EditorGUIUtility.singleLineHeight * 4);
			}

			public override void OnGUI(Rect rect)
			{
				EditorGUILayout.LabelField(_label);
				
				_ability = EditorGUILayout.Popup(_ability, _control._abilityNames);

				var create = GUILayout.Button(EditorHelper.CreateContent);

				if (create && _ability >= 0)
				{
					CreateMove(_control._abilities[_ability]);
					editorWindow.Close();
				}
			}

			private GUIContent _label = new GUIContent("New Move", "Add a new move to the species");
			private int _ability = -1;

			private void CreateMove(Ability ability)
			{
				var move = new Move(ability);
				Definitions.Instance.MoveTraits.Apply(move.Traits, false);

				_control._moves.Add(move);
				_control.MoveAdded(move);
			}
		}

		private void AddMove(Rect rect, ReorderableList list)
		{
			rect.y += EditorGUIUtility.singleLineHeight;
			PopupWindow.Show(rect, new AddPopup(this));
		}

		private void MoveAdded(Move move)
		{
			_traitStores.Add(new VariableStoreControl(move.Traits, move.Ability.name, "", false, false));
		}

		private void RemoveMove(int index)
		{
			_moves.RemoveAt(index);
			_traitStores.RemoveAt(index);
		}
	}
}
