using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace PiRhoSoft.MonsterMaker
{
	public class StyleEditorWindow : EditorWindow
	{
		public GUIStyle Style;
		public GUIContent Content;
		public GUILayoutOption[] Options;
		public Color Color;
		public Color BackgroundColor;
		public Color ContentColor;

		public GUILayoutOption Width;
		public GUILayoutOption Height;
		public GUILayoutOption MinWidth;
		public GUILayoutOption MinHeight;
		public GUILayoutOption MaxWidth;
		public GUILayoutOption MaxHeight;
		public GUILayoutOption ExpandWidth;
		public GUILayoutOption ExpandHeight;

		private StyleList _styles;
		private int _selectedStyle = 0;
		private float _width = EditorGUIUtility.fieldWidth;
		private float _height = EditorGUIUtility.singleLineHeight;
		private float _minWidth = 0.0f;
		private float _minHeight = 0.0f;
		private float _maxWidth = float.MaxValue;
		private float _maxHeight = float.MaxValue;
		private bool _expandWidth = false;
		private bool _expandHeight = false;

		private bool _useWidth = false;
		private bool _useHeight = false;
		private bool _useMinWidth = false;
		private bool _useMinHeight = false;
		private bool _useMaxWidth = false;
		private bool _useMaxHeight = false;
		private bool _useExpandWidth = false;
		private bool _useExpandHeight = false;

		private int _previewControl = 0;
		private bool _useLayout = true;
		private Vector2 _area;

		private VerticalSplitterControl _splitter;
		private TexturePickerControl _normal;
		private TexturePickerControl _hover;
		private TexturePickerControl _focused;
		private TexturePickerControl _active;
		private TexturePickerControl _onNormal;
		private TexturePickerControl _onHover;
		private TexturePickerControl _onFocused;
		private TexturePickerControl _onActive;

		private Vector2 _leftScrollPosition;
		private Vector2 _rightScrollPosition;

		private static string[] _controlTypes = new string[]
		{
			"GUI/Box",
			"GUI/Button",
			"GUI/HorizontalScrollbar",
			"GUI/HorizontalSlider",
			"GUI/Label",
			"GUI/PasswordField",
			"GUI/RepeatButton",
			"GUI/SelectionGrid",
			"GUI/TextArea",
			"GUI/TextField",
			"GUI/Toggle",
			"GUI/Toolbar",
			"GUI/VerticalScrollbar",
			"GUI/VerticalSlider",
			"GUI/Window",
			"EditorGUI/BoundsField",
			"EditorGUI/BoundsIntField",
			"EditorGUI/ColorField",
			"EditorGUI/CurveField",
			"EditorGUI/DoubleField",
			"EditorGUI/DropDownButton",
			"EditorGUI/DropShadowLabel",
			"EditorGUI/EnumFlagsField",
			"EditorGUI/EnumPopup",
			"EditorGUI/FloatField",
			"EditorGUI/Foldout",
			"EditorGUI/HelpBox",
			"EditorGUI/InspectorTitlebar",
			"EditorGUI/IntField",
			"EditorGUI/IntPopup",
			"EditorGUI/IntSlider",
			"EditorGUI/LabelField",
			"EditorGUI/LayerField",
			"EditorGUI/LongField",
			"EditorGUI/MaskField",
			"EditorGUI/MinMaxSlider",
			"EditorGUI/ObjectField",
			"EditorGUI/PasswordField",
			"EditorGUI/Popup",
			"EditorGUI/PrefixLabel",
			"EditorGUI/ProgressBar",
			"EditorGUI/RectField",
			"EditorGUI/RectIntField",
			"EditorGUI/SelectableLabel",
			"EditorGUI/Slider",
			"EditorGUI/TagField",
			"EditorGUI/TextArea",
			"EditorGUI/TextField",
			"EditorGUI/Toggle",
			"EditorGUI/ToggleLeft",
			"EditorGUI/Vector2Field",
			"EditorGUI/Vector2IntField",
			"EditorGUI/Vector3Field",
			"EditorGUI/Vector3IntField",
			"EditorGUI/Vector4Field"
		};

		[MenuItem("Window/PiRho Soft/Style Editor")]
		public static void Open()
		{
			GetWindow<StyleEditorWindow>("Style Editor").Show();
		}

		void OnEnable()
		{
			_splitter = new VerticalSplitterControl(300.0f, this);
			_normal = new TexturePickerControl("Normal Image");
			_hover = new TexturePickerControl("Hover Image");
			_focused = new TexturePickerControl("Focused Image");
			_active = new TexturePickerControl("Active Image");
			_onNormal = new TexturePickerControl("On Normal Image");
			_onHover = new TexturePickerControl("On Hover Image");
			_onFocused = new TexturePickerControl("On Focused Image");
			_onActive = new TexturePickerControl("On Active Image");
		}

		void OnGUI()
		{
			if (_styles == null)
				_styles = EnumerateStyles();

			if (Style == null)
				Style = GetStyle(null, GUIStyle.none);

			if (Content == null)
				ResetContent();

			_splitter.OnGui();

			using (new GUILayout.AreaScope(_splitter.LeftSide))
			{
				using (var scroll = new EditorGUILayout.ScrollViewScope(_leftScrollPosition))
				{
					using (var changes = new EditorGUI.ChangeCheckScope())
					{
						_selectedStyle = EditorGUILayout.Popup("Select Style", _selectedStyle, _styles.Names);

						if (changes.changed)
							Style = GetStyle(_styles.Skins[_selectedStyle], _styles.Styles[_selectedStyle]);
					}

					EditorGUILayout.LabelField("Positioning", EditorStyles.boldLabel);
					Style.margin = RectOffsetField("Margin", Style.margin);

					EditorGUILayout.LabelField("Sizing", EditorStyles.boldLabel);
					Style.fixedWidth = EditorGUILayout.FloatField("Fixed Width", Style.fixedWidth);
					Style.fixedHeight = EditorGUILayout.FloatField("Fixed Height", Style.fixedHeight);
					Style.stretchWidth = EditorGUILayout.Toggle("Stretch Width", Style.stretchWidth);
					Style.stretchHeight = EditorGUILayout.Toggle("Stretch Height", Style.stretchHeight);

					EditorGUILayout.LabelField("Background", EditorStyles.boldLabel);
					Style.border = RectOffsetField("Border", Style.border);
					Style.overflow = RectOffsetField("Overflow", Style.overflow);
					Style.normal.background = _normal.OnGui();
					Style.onNormal.background = _onNormal.OnGui();
					Style.hover.background = _hover.OnGui();
					Style.onHover.background = _onHover.OnGui();
					Style.focused.background = _focused.OnGui();
					Style.onFocused.background = _onFocused.OnGui();
					Style.active.background = _active.OnGui();
					Style.onActive.background = _onActive.OnGui();

					EditorGUILayout.LabelField("Content", EditorStyles.boldLabel);
					Style.padding = RectOffsetField("Padding", Style.padding);
					Style.contentOffset = EditorGUILayout.Vector2Field("Content Offset", Style.contentOffset);
					Style.clipping = (TextClipping)EditorGUILayout.EnumPopup("Clipping", Style.clipping);

					EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
					Style.font = (Font)EditorGUILayout.ObjectField("Font", Style.font, typeof(Font), false);
					Style.fontSize = EditorGUILayout.IntSlider("Size", Style.fontSize, 1, 200);
					Style.fontStyle = (FontStyle)EditorGUILayout.EnumPopup("Style", Style.fontStyle);
					Style.alignment = (TextAnchor)EditorGUILayout.EnumPopup("Alignment", Style.alignment);
					Style.wordWrap = EditorGUILayout.Toggle("Word Wrap", Style.wordWrap);
					Style.richText = EditorGUILayout.Toggle("Use Rich Text", Style.richText);
					Style.normal.textColor = EditorGUILayout.ColorField("Normal Color", Style.normal.textColor);
					Style.onNormal.textColor = EditorGUILayout.ColorField("On Normal Color", Style.onNormal.textColor);
					Style.hover.textColor = EditorGUILayout.ColorField("Hovered Color", Style.hover.textColor);
					Style.onHover.textColor = EditorGUILayout.ColorField("On Hovered Color", Style.onHover.textColor);
					Style.focused.textColor = EditorGUILayout.ColorField("Focused Color", Style.focused.textColor);
					Style.onFocused.textColor = EditorGUILayout.ColorField("On Focused Color", Style.onFocused.textColor);
					Style.active.textColor = EditorGUILayout.ColorField("Active Color", Style.active.textColor);
					Style.onActive.textColor = EditorGUILayout.ColorField("On Active Color", Style.onActive.textColor);

					EditorGUILayout.LabelField("Image", EditorStyles.boldLabel);
					Style.imagePosition = (ImagePosition)EditorGUILayout.EnumPopup("Position", Style.imagePosition);

					_leftScrollPosition = scroll.scrollPosition;
				}
			}

			using (new GUILayout.AreaScope(_splitter.RightSide))
			{
				using (var scroll = new EditorGUILayout.ScrollViewScope(_rightScrollPosition))
				{
					EditorGUILayout.LabelField("Content", EditorStyles.boldLabel);

					Content.image = (Texture)EditorGUILayout.ObjectField("Image Content", Content.image, typeof(Texture), false);
					Content.text = GUILayout.TextArea(Content.text);
					Color = EditorGUILayout.ColorField("Color", Color);
					BackgroundColor = EditorGUILayout.ColorField("Background Color", BackgroundColor);
					ContentColor = EditorGUILayout.ColorField("Content Color", ContentColor);

					EditorGUILayout.LabelField("Layout Options", EditorStyles.boldLabel);

					if (FloatLayoutOptionField("Width", Width, ref _useWidth, ref _width)) Width = _useWidth ? GUILayout.Width(_width) : null;
					if (FloatLayoutOptionField("Height", Height, ref _useHeight, ref _height)) Height = _useHeight ? GUILayout.Height(_height) : null;
					if (FloatLayoutOptionField("Minimum Width", MinWidth, ref _useMinWidth, ref _minWidth)) MinWidth = _useMinWidth ? GUILayout.MinWidth(_minWidth) : null;
					if (FloatLayoutOptionField("Minimum Height", MinHeight, ref _useMinHeight, ref _minHeight)) MinHeight = _useMinHeight ? GUILayout.MinHeight(_minHeight) : null;
					if (FloatLayoutOptionField("Maximum Width", MaxWidth, ref _useMaxWidth, ref _maxWidth)) MaxWidth = _useMaxWidth ? GUILayout.MaxWidth(_maxWidth) : null;
					if (FloatLayoutOptionField("Maximum Height", MaxHeight, ref _useMaxHeight, ref _maxHeight)) MaxHeight = _useMaxHeight ? GUILayout.MaxHeight(_maxHeight) : null;
					if (BoolLayoutOptionField("Expand Width", ExpandWidth, ref _useExpandWidth, ref _expandWidth)) ExpandWidth = _useExpandWidth ? GUILayout.ExpandWidth(_expandWidth) : null;
					if (BoolLayoutOptionField("Expand Height", ExpandHeight, ref _useExpandHeight, ref _expandHeight)) ExpandHeight = _useExpandHeight ? GUILayout.ExpandHeight(_expandHeight) : null;

					EditorGUILayout.LabelField("Preview", EditorStyles.boldLabel);
					_previewControl = EditorGUILayout.Popup("Control Type", _previewControl, _controlTypes);
					_useLayout = EditorGUILayout.Toggle("Use Layout", _useLayout);

					if (!_useLayout)
						_area = EditorGUILayout.Vector2Field("Size", _area);

					EditorHelper.Separator(Color.black);

					var color = GUI.color;
					var backgroundColor = GUI.backgroundColor;
					var contentColor = GUI.contentColor;

					GUI.color = Color;
					GUI.backgroundColor = BackgroundColor;
					GUI.contentColor = ContentColor;

					var rect = new Rect();

					if (!_useLayout)
					{
						rect = GUILayoutUtility.GetRect(0, 0);
						rect.width = _area.x;
						rect.height = _area.y;
						GUI.DrawTexture(rect, Texture2D.whiteTexture);
					}

					switch (_previewControl)
					{
						case 0: DrawBox(rect); break;
						case 1: DrawButton(rect); break;
						case 2: DrawHorizontalScrollbar(rect); break;
						case 3: DrawHorizontalSlider(rect); break;
						case 4: DrawLabel(rect); break;
						case 5: DrawPasswordField(rect); break;
						case 6: DrawRepeatButton(rect); break;
						case 7: DrawSelectionGrid(rect); break;
						case 8: DrawTextArea(rect); break;
						case 9: DrawTextField(rect); break;
						case 10: DrawToggle(rect); break;
						case 11: DrawToolbar(rect); break;
						case 12: DrawVerticalScrollbar(rect); break;
						case 13: DrawVerticalSlider(rect); break;
						case 14: DrawWindow(rect); break;
						case 15: DrawEditorBoundsField(rect); break;
						case 16: DrawEditorBoundsIntField(rect); break;
						case 17: DrawEditorColorField(rect); break;
						case 18: DrawEditorCurveField(rect); break;
						case 19: DrawEditorDoubleField(rect); break;
						case 20: DrawEditorDropDownButton(rect); break;
						case 21: DrawEditorDropShadowLabel(rect); break;
						case 22: DrawEditorEnumFlagsField(rect); break;
						case 23: DrawEditorEnumPopup(rect); break;
						case 24: DrawEditorFloatField(rect); break;
						case 25: DrawEditorFoldout(rect); break;
						case 26: DrawEditorHelpBox(rect); break;
						case 27: DrawEditorInspectorTitlebar(rect); break;
						case 28: DrawEditorIntField(rect); break;
						case 29: DrawEditorIntPopup(rect); break;
						case 30: DrawEditorIntSlider(rect); break;
						case 31: DrawEditorLabelField(rect); break;
						case 32: DrawEditorLayerField(rect); break;
						case 33: DrawEditorLongField(rect); break;
						case 34: DrawEditorMaskField(rect); break;
						case 35: DrawEditorMinMaxSlider(rect); break;
						case 36: DrawEditorObjectField(rect); break;
						case 37: DrawEditorPasswordField(rect); break;
						case 38: DrawEditorPopup(rect); break;
						case 39: DrawEditorPrefixLabel(rect); break;
						case 40: DrawEditorProgressBar(rect); break;
						case 41: DrawEditorRectField(rect); break;
						case 42: DrawEditorRectIntField(rect); break;
						case 43: DrawEditorSelectableLabel(rect); break;
						case 44: DrawEditorSlider(rect); break;
						case 45: DrawEditorTagField(rect); break;
						case 46: DrawEditorTextArea(rect); break;
						case 47: DrawEditorTextField(rect); break;
						case 48: DrawEditorToggle(rect); break;
						case 49: DrawEditorToggleLeft(rect); break;
						case 50: DrawEditorVector2Field(rect); break;
						case 51: DrawEditorVector2IntField(rect); break;
						case 52: DrawEditorVector3Field(rect); break;
						case 53: DrawEditorVector3IntField(rect); break;
						case 54: DrawEditorVector4Field(rect); break;
					}

					GUI.color = color;
					GUI.backgroundColor = backgroundColor;
					GUI.contentColor = contentColor;

					_rightScrollPosition = scroll.scrollPosition;
				}
			}
		}

		private class StyleList
		{
			public string[] Names;
			public GUISkin[] Skins;
			public GUIStyle[] Styles;
		}

		private StyleList EnumerateStyles()
		{
			var names = new List<string>();
			var skins = new List<GUISkin>();
			var styles = new List<GUIStyle>();

			var builtInSkins = Resources.FindObjectsOfTypeAll<GUISkin>();

			names.Add("Blank");
			styles.Add(new GUIStyle());

			foreach (var skin in builtInSkins)
			{
				names.Add(skin.name + "/Box (" + skin.box.name + ")");
				names.Add(skin.name + "/Label (" + skin.label.name + ")");
				names.Add(skin.name + "/Text Field (" + skin.textField.name + ")");
				names.Add(skin.name + "/Text Area (" + skin.textArea.name + ")");
				names.Add(skin.name + "/Button (" + skin.button.name + ")");
				names.Add(skin.name + "/Toggle (" + skin.toggle.name + ")");
				names.Add(skin.name + "/Horizontal Scrollbar (" + skin.horizontalScrollbar.name + ")");
				names.Add(skin.name + "/Horizontal Scrollbar Left Button (" + skin.horizontalScrollbarLeftButton.name + ")");
				names.Add(skin.name + "/Horizontal Scrollbar Right Button (" + skin.horizontalScrollbarRightButton.name + ")");
				names.Add(skin.name + "/Horizontal Scrollbar Thumb (" + skin.horizontalScrollbarThumb.name + ")");
				names.Add(skin.name + "/Vertical Scrollbar (" + skin.verticalScrollbar.name + ")");
				names.Add(skin.name + "/Vertical Scrollbar Up Button (" + skin.verticalScrollbarUpButton.name + ")");
				names.Add(skin.name + "/Vertical Scrollbar Down Button (" + skin.verticalScrollbarDownButton.name + ")");
				names.Add(skin.name + "/Vertical Scrollbar Thumb (" + skin.verticalScrollbarThumb.name + ")");
				names.Add(skin.name + "/Horizontal Slider (" + skin.horizontalSlider.name + ")");
				names.Add(skin.name + "/Horizontal Slider Thumb (" + skin.horizontalSliderThumb.name + ")");
				names.Add(skin.name + "/Vertical Slider (" + skin.verticalSlider.name + ")");
				names.Add(skin.name + "/Vertical Slider Thumb (" + skin.verticalSliderThumb.name + ")");
				names.Add(skin.name + "/Scroll View (" + skin.scrollView.name + ")");
				names.Add(skin.name + "/Window (" + skin.window.name + ")");

				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);
				skins.Add(skin);

				styles.Add(skin.box);
				styles.Add(skin.label);
				styles.Add(skin.textField);
				styles.Add(skin.textArea);
				styles.Add(skin.button);
				styles.Add(skin.toggle);
				styles.Add(skin.horizontalScrollbar);
				styles.Add(skin.horizontalScrollbarLeftButton);
				styles.Add(skin.horizontalScrollbarRightButton);
				styles.Add(skin.horizontalScrollbarThumb);
				styles.Add(skin.verticalScrollbar);
				styles.Add(skin.verticalScrollbarUpButton);
				styles.Add(skin.verticalScrollbarDownButton);
				styles.Add(skin.verticalScrollbarThumb);
				styles.Add(skin.horizontalSlider);
				styles.Add(skin.horizontalSliderThumb);
				styles.Add(skin.verticalSlider);
				styles.Add(skin.verticalSliderThumb);
				styles.Add(skin.scrollView);
				styles.Add(skin.window);

				for (var i = 0; i < skin.customStyles.Length; i++)
				{
					var customStyle = skin.customStyles[i];
					var name = string.IsNullOrEmpty(customStyle.name) ? i.ToString() : customStyle.name;

					names.Add(skin.name + "/Custom/" + name);
					skins.Add(skin);
					styles.Add(customStyle);
				}
			}

			return new StyleList { Names = names.ToArray(), Skins = skins.ToArray(), Styles = styles.ToArray() };
		}

		private GUIStyle GetStyle(GUISkin skin, GUIStyle template)
		{
			var style = new GUIStyle(template);

			_normal.Texture = style.normal.background;
			_hover.Texture = style.hover.background;
			_focused.Texture = style.focused.background;
			_active.Texture = style.active.background;
			_onNormal.Texture = style.onNormal.background;
			_onHover.Texture = style.onHover.background;
			_onFocused.Texture = style.onFocused.background;
			_onActive.Texture = style.onActive.background;

			if (style.font == null && skin != null)
				style.font = skin.font;

			if (style.fontSize == 0)
				style.fontSize = 20;

			return style;
		}

		private void ResetContent()
		{
			Content = new GUIContent();
			Options = null;
			Color = GUI.color;
			BackgroundColor = GUI.backgroundColor;
			ContentColor = GUI.contentColor;
		}

		private RectOffset RectOffsetField(string label, RectOffset rect)
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PrefixLabel(label);
				GUILayout.Label("L");
				rect.left = EditorGUILayout.IntField(rect.left);
				GUILayout.Label("R");
				rect.right = EditorGUILayout.IntField(rect.right);
				GUILayout.Label("T");
				rect.top = EditorGUILayout.IntField(rect.top);
				GUILayout.Label("B");
				rect.bottom = EditorGUILayout.IntField(rect.bottom);
			}

			return rect;
		}

		private bool FloatLayoutOptionField(string label, GUILayoutOption option, ref bool use, ref float value)
		{
			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel(label);
					use = EditorGUILayout.Toggle(use);

					if (use)
						value = EditorGUILayout.FloatField(value);

					return changes.changed;
				}
			}
		}

		private bool BoolLayoutOptionField(string label, GUILayoutOption option, ref bool use, ref bool value)
		{
			using (var changes = new EditorGUI.ChangeCheckScope())
			{
				using (new EditorGUILayout.HorizontalScope())
				{
					EditorGUILayout.PrefixLabel(label);
					use = EditorGUILayout.Toggle(use);

					if (use)
						value = EditorGUILayout.Toggle(value);

					return changes.changed;
				}
			}
		}

		private void DrawBox(Rect rect)
		{
			if (_useLayout)
				GUILayout.Box(Content, Style);
			else
				GUI.Box(rect, Content, Style);
		}

		private void DrawButton(Rect rect)
		{
			if (_useLayout)
				GUILayout.Button(Content, Style);
			else
				GUI.Button(rect, Content, Style);
		}

		private void DrawHorizontalScrollbar(Rect rect)
		{
		}

		private void DrawHorizontalSlider(Rect rect)
		{
		}

		private void DrawLabel(Rect rect)
		{
			if (_useLayout)
				GUILayout.Label(Content, Style);
			else
				GUI.Label(rect, Content, Style);
		}

		private void DrawPasswordField(Rect rect)
		{
		}

		private void DrawRepeatButton(Rect rect)
		{
		}

		private void DrawSelectionGrid(Rect rect)
		{
		}

		private void DrawTextArea(Rect rect)
		{
			if (_useLayout)
				Content.text = GUILayout.TextArea(Content.text, Style);
			else
				Content.text = GUI.TextArea(rect, Content.text, Style);
		}

		private void DrawTextField(Rect rect)
		{
			if (_useLayout)
				Content.text = GUILayout.TextField(Content.text, Style);
			else
				Content.text = GUI.TextField(rect, Content.text, Style);
		}

		private bool _toggle = false;

		private void DrawToggle(Rect rect)
		{
			if (_useLayout)
				_toggle = GUILayout.Toggle(_toggle, Content, Style);
			else
				_toggle = GUI.Toggle(rect, _toggle, Content, Style);
		}

		private void DrawToolbar(Rect rect)
		{
		}

		private void DrawVerticalScrollbar(Rect rect)
		{
		}

		private void DrawVerticalSlider(Rect rect)
		{
		}

		private void DrawWindow(Rect rect)
		{
		}

		private void DrawEditorBoundsField(Rect rect)
		{
		}

		private void DrawEditorBoundsIntField(Rect rect)
		{
		}

		private void DrawEditorColorField(Rect rect)
		{
		}

		private void DrawEditorCurveField(Rect rect)
		{
		}

		private void DrawEditorDoubleField(Rect rect)
		{
		}

		private void DrawEditorDropDownButton(Rect rect)
		{
		}

		private void DrawEditorDropShadowLabel(Rect rect)
		{
		}

		private void DrawEditorEnumFlagsField(Rect rect)
		{
		}

		private void DrawEditorEnumPopup(Rect rect)
		{
		}

		private void DrawEditorFloatField(Rect rect)
		{
		}

		private void DrawEditorFoldout(Rect rect)
		{
		}

		private void DrawEditorHelpBox(Rect rect)
		{
		}

		private void DrawEditorInspectorTitlebar(Rect rect)
		{
		}

		private void DrawEditorIntField(Rect rect)
		{
		}

		private void DrawEditorIntPopup(Rect rect)
		{
		}

		private void DrawEditorIntSlider(Rect rect)
		{
		}

		private void DrawEditorLabelField(Rect rect)
		{
		}

		private void DrawEditorLayerField(Rect rect)
		{
		}

		private void DrawEditorLongField(Rect rect)
		{
		}

		private void DrawEditorMaskField(Rect rect)
		{
		}

		private void DrawEditorMinMaxSlider(Rect rect)
		{
		}

		private void DrawEditorObjectField(Rect rect)
		{
		}

		private void DrawEditorPasswordField(Rect rect)
		{
		}

		private void DrawEditorPopup(Rect rect)
		{
		}

		private void DrawEditorPrefixLabel(Rect rect)
		{
		}

		private void DrawEditorProgressBar(Rect rect)
		{
		}

		private void DrawEditorRectField(Rect rect)
		{
		}

		private void DrawEditorRectIntField(Rect rect)
		{
		}

		private void DrawEditorSelectableLabel(Rect rect)
		{
		}

		private void DrawEditorSlider(Rect rect)
		{
		}

		private void DrawEditorTagField(Rect rect)
		{
		}

		private void DrawEditorTextArea(Rect rect)
		{
		}

		private void DrawEditorTextField(Rect rect)
		{
		}

		private void DrawEditorToggle(Rect rect)
		{
		}

		private void DrawEditorToggleLeft(Rect rect)
		{
		}

		private void DrawEditorVector2Field(Rect rect)
		{
		}

		private void DrawEditorVector2IntField(Rect rect)
		{
		}

		private void DrawEditorVector3Field(Rect rect)
		{
		}

		private void DrawEditorVector3IntField(Rect rect)
		{
		}

		private void DrawEditorVector4Field(Rect rect)
		{
		}
	}

	public class VerticalSplitterControl
	{
		public Rect LeftSide;
		public Rect RightSide;

		public float MinimumLeftWidth = 100;
		public float MinimumRightWidth = 100;
		public float HandleWidth = 5;
		public float HandlePosition;

		private bool _dragging;
		private EditorWindow _owner;

		public VerticalSplitterControl(float startingHandlePosition, EditorWindow owner)
		{
			HandlePosition = startingHandlePosition;
			_owner = owner;
		}

		public void OnGui()
		{
			var halfWidth = HandleWidth * 0.5f;
			var handleRect = new Rect(HandlePosition - halfWidth, 0, HandleWidth, _owner.position.height);
			GUI.Box(handleRect, "");

			LeftSide = new Rect(0, 0, HandlePosition - halfWidth, _owner.position.height);
			RightSide = new Rect(HandlePosition + halfWidth, 0, _owner.position.width - HandlePosition - halfWidth, _owner.position.height);

			if (Event.current != null)
			{
				switch (Event.current.rawType)
				{
					case EventType.MouseDown:
						if (handleRect.Contains(Event.current.mousePosition))
							_dragging = true;

						break;
					case EventType.MouseDrag:
						if (_dragging)
						{
							HandlePosition = Event.current.mousePosition.x;

							if (HandlePosition < MinimumLeftWidth)
								HandlePosition = MinimumLeftWidth;
							else if (HandlePosition > (_owner.position.width - MinimumRightWidth))
								HandlePosition = (_owner.position.width - MinimumRightWidth);

							_owner.Repaint();
						}

						break;
					case EventType.MouseUp:
						_dragging = false;
						_owner.Repaint();
						break;
				}
			}

			if (_dragging)
				EditorGUIUtility.AddCursorRect(new Rect(0, 0, _owner.position.width, _owner.position.height), MouseCursor.SplitResizeLeftRight);
			else
				EditorGUIUtility.AddCursorRect(handleRect, MouseCursor.SplitResizeLeftRight);
		}
	}

	public class TexturePickerControl
	{
		public Texture2D Texture;
		private GUIContent _label;

		public TexturePickerControl(string label)
		{
			_label = new GUIContent(label);
		}

		public Texture2D OnGui()
		{
			using (new EditorGUILayout.HorizontalScope())
			{
				EditorGUILayout.PrefixLabel(_label);
				EditorGUILayout.LabelField(Texture != null ? Texture.name : "");

				var rect = GUILayoutUtility.GetLastRect();

				if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
					TexturePickerWindow.Show(this);
			}

			return Texture;
		}
	}

	public class TexturePickerWindow : EditorWindow
	{
		public static TexturePickerControl Control;

		private static float _spacing = 6.0f;
		private static float _size = 32.0f;
		private static int _scale = 2;
		private static float _rightGutter = 16.0f;

		private static Texture2D[] _textures;
		private static Vector2 _scrollPosition;

		public static void Show(TexturePickerControl control)
		{
			Control = control;
			_textures = Resources.FindObjectsOfTypeAll<Texture2D>();
			GetWindow<TexturePickerWindow>("Texture Picker").ShowAuxWindow();
		}

		void OnGUI()
		{
			GUILayout.Space(10.0f);
			_scale = EditorGUILayout.IntSlider("Scale", _scale, 1, 10, GUILayout.MaxWidth(500.0f));
			GUILayout.Space(10.0f);

			var x = 0.0f;
			var size = _scale * _size;
			var viewWidth = EditorGUIUtility.currentViewWidth;

			using (var scroll = new EditorGUILayout.ScrollViewScope(_scrollPosition))
			{
				x += _spacing;

				EditorGUILayout.BeginHorizontal();

				foreach (var texture in _textures)
				{
					try
					{
						GUILayout.Box(texture.name, GUIStyle.none, GUILayout.Width(size), GUILayout.Height(size));
						var rect = GUILayoutUtility.GetLastRect();
						GUI.DrawTexture(rect, texture, ScaleMode.ScaleToFit);
						x += size + _spacing;

						if (Event.current.type == EventType.MouseUp && rect.Contains(Event.current.mousePosition))
						{
							Control.Texture = texture;
							Close();
						}

						if (x + size + _spacing + _rightGutter > viewWidth)
						{
							x = 0.0f;
							GUILayout.FlexibleSpace();
							EditorGUILayout.EndHorizontal();
							GUILayout.Space(_spacing);
							EditorGUILayout.BeginHorizontal();
						}
						else
						{
							GUILayout.Space(_spacing);
						}
					}
					catch
					{
					}
				}

				EditorGUILayout.EndHorizontal();
				_scrollPosition = scroll.scrollPosition;
			}
		}
	}
}
