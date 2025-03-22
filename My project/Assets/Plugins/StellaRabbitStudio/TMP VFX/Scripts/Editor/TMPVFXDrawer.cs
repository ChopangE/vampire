#if UNITY_EDITOR
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;

namespace StellaRabbitStudio
{
	public class TMPVFXDrawer : TMPro.EditorUtilities.TMP_SDFShaderGUI
	{
		private Material targetMat;

		private GUIStyle propertiesStyle, bigLabelStyle, smallLabelStyle, toggleButtonStyle, titleHelpBoxStyle, foldoutStyle, foldoutInsideStyle;
		private Texture2D iconTexture;
		private const int bigFontSize = 16, smallFontSize = 11;
		private string[] oldKeyWords;
		private int effectCount = 1;
		private Material originalMaterialCopy;
		private MaterialEditor matEditor;
		private MaterialProperty[] matProperties;
		private uint[] materialDrawers = new uint[] { 1, 2, 4 };
		private bool[] currEnabledDrawers;
		private bool DefaultTMPSettingsDrawer;
		private const uint advancedConfigDrawer = 0;
		private const uint colorFxShapeDrawer = 1;
		private const uint uvFxShapeDrawer = 2;

		// Define the property range of the effect
		private struct EffectPropertyRange
		{
			public int start;
			public int end;
			public bool noReset; 

			public EffectPropertyRange(int start, int end, bool noReset = false)
			{
				this.start = start;
				this.end = end;
				this.noReset = noReset;
			}
		}
		private GameObject lastSelectedGameObject;
		public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			if (lastSelectedGameObject == null)
				lastSelectedGameObject = Selection.activeGameObject;
			foldoutStyle = new GUIStyle(GUI.skin.GetStyle("MiniPopup"));
			foldoutStyle.fontStyle = FontStyle.Bold;
			foldoutStyle.margin = new RectOffset(15, 15, 5, 5);
			foldoutStyle.padding = new RectOffset(7, 5, 5, 5);
			foldoutStyle.fixedHeight = 30;
			foldoutStyle.fontStyle = FontStyle.Bold;

			DefaultTMPSettingsDrawer = EditorGUILayout.BeginFoldoutHeaderGroup(DefaultTMPSettingsDrawer, new GUIContent("  Show Default TMP Settings", (Texture)Resources.Load("Editor Textures/DefaultSettings")), foldoutStyle);
			EditorGUILayout.EndFoldoutHeaderGroup();
			if (DefaultTMPSettingsDrawer) base.OnGUI(materialEditor, properties);

			// public Styles
			foldoutInsideStyle = new GUIStyle(GUI.skin.GetStyle("MiniPopup"));
			foldoutInsideStyle.fontStyle = FontStyle.Bold;
			foldoutInsideStyle.margin = new RectOffset(20, 15, 2, 2);
			foldoutInsideStyle.padding = new RectOffset(7, 5, 3, 3);
			foldoutInsideStyle.fixedHeight = 25;
			foldoutInsideStyle.fontSize = 11;

			bigLabelStyle = new GUIStyle(EditorStyles.boldLabel);
			bigLabelStyle.fontSize = bigFontSize;
			matEditor = materialEditor;
			matProperties = properties;
			targetMat = materialEditor.target as Material;
			effectCount = 1;
			oldKeyWords = targetMat.shaderKeywords;
			propertiesStyle = new GUIStyle(EditorStyles.helpBox);
			propertiesStyle.margin = new RectOffset(0, 0, 0, 0);
			smallLabelStyle = new GUIStyle(EditorStyles.boldLabel);
			smallLabelStyle.fontSize = smallFontSize;
			toggleButtonStyle = new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, richText = true };
			currEnabledDrawers = new bool[materialDrawers.Length];
			titleHelpBoxStyle = new GUIStyle(EditorStyles.helpBox)
			{
				padding = new RectOffset(15, 15, 5, 5),
				margin = new RectOffset(25, 25, 10, 10),
				fixedHeight = 120,
				stretchWidth = true
			};
			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
			{
				fixedHeight = 22,
				padding = new RectOffset(5, 15, 3, 3),
				margin = new RectOffset(2, 2, 0, 5),
				normal = new GUIStyleState
				{
					textColor = new Color(0f, 1f, 0.769f),
				},
				hover = new GUIStyleState
				{
					textColor = new Color(0f, 1f, 1f),
				},
				fontStyle = FontStyle.Bold,
				alignment = TextAnchor.MiddleLeft,
				fontSize = 11,
				stretchWidth = true
			};

			uint iniDrawers = (uint)ShaderGUI.FindProperty("_EditorDrawers", matProperties).floatValue;
			for (int i = 0; i < materialDrawers.Length; i++) currEnabledDrawers[i] = (materialDrawers[i] & iniDrawers) > 0;

			iconTexture = (Texture2D)Resources.Load("Editor Textures/Logo");

			EditorGUILayout.Separator();
			DrawLine(Color.grey, 1, 3);

			// TMP VFX Main Title
			EditorGUILayout.BeginHorizontal(titleHelpBoxStyle);
			{
				EditorGUILayout.BeginVertical();
				{
					// Title Section
					EditorGUILayout.BeginHorizontal();
					{
						GUIContent titleContent = new GUIContent("  TMP VFX Shader Settings", iconTexture);
						GUILayout.Label(titleContent, new GUIStyle(EditorStyles.boldLabel)
						{
							fontSize = 18,
							alignment = TextAnchor.MiddleLeft,
							imagePosition = ImagePosition.ImageLeft,
							richText = true,
							fixedHeight = 30,
							wordWrap = false,
							normal = new GUIStyleState { textColor = new Color(0f, 1f, 0.769f) },
							hover = new GUIStyleState { textColor = new Color(0f, 1f, 1f) },
							margin = new RectOffset(0, 0, 10, 10),
							padding = new RectOffset(0, 0, 0, 10),
							fixedWidth = 280
						});

						// Add version information
						GUIStyle versionStyle = new GUIStyle(EditorStyles.label)
						{
							fontSize = 12,
							alignment = TextAnchor.MiddleRight,
							normal = new GUIStyleState { textColor = new Color(0.7f, 0.7f, 0.7f) },
							padding = new RectOffset(0, 5, 10, 0)
						};

						GUILayout.FlexibleSpace(); // Reserve space between title and version
						GUILayout.Label(TMPVFXShaderData.versionString, versionStyle);
					}
					EditorGUILayout.EndHorizontal();

					EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox)
					{
						padding = new RectOffset(10, 10, 10, 10),
						margin = new RectOffset(0, 0, 5, 5),
						stretchWidth = true
					});
					{
						GUIContent documentationContent = new GUIContent(" Documentation", EditorGUIUtility.IconContent("_Help").image);
						if (GUILayout.Button(documentationContent, buttonStyle))
						{
							OpenDocumentation();
						}

						GUIContent addComponentContent = new GUIContent(" Add TMP VFX Component", EditorGUIUtility.IconContent("_Popup").image);
						if (GUILayout.Button(addComponentContent, buttonStyle))
						{
							AddTMPVFXComponent();
						}
					}
					EditorGUILayout.EndVertical();
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndHorizontal();

			currEnabledDrawers[colorFxShapeDrawer] =
			EditorGUILayout.BeginFoldoutHeaderGroup(currEnabledDrawers[colorFxShapeDrawer], new GUIContent("  Show Color Effects", (Texture)Resources.Load("Editor Textures/Color")), foldoutStyle);
			EditorGUILayout.EndFoldoutHeaderGroup();
			if (currEnabledDrawers[colorFxShapeDrawer])
			{
				Outline("Outline", "OUTLINE_ON");
				DrawEffect("Glow", "GLOW_ON", new EffectPropertyRange(34, 38, true));
				DrawEffect("CRT", "CRT_ON", new EffectPropertyRange(66, 88), true);
				DrawEffect("RGB Split", "RGBSPLIT_ON", new EffectPropertyRange(89, 91), true);
				Underlay("Underlay", "UNDERLAY_ON");
			}

			currEnabledDrawers[uvFxShapeDrawer] =
			EditorGUILayout.BeginFoldoutHeaderGroup(currEnabledDrawers[uvFxShapeDrawer], new GUIContent("  Show VFX Effects", (Texture)Resources.Load("Editor Textures/Alpha")), foldoutStyle);
			EditorGUILayout.EndFoldoutHeaderGroup();
			if (currEnabledDrawers[uvFxShapeDrawer])
			{
				Glitch("Glitch", "GLITCH_ON");

				// You Found Easter Egg! Our next feature is "Hand Drawn" and "Wave"
			}

			SetAndSaveEnabledDrawers(iniDrawers);
		}

		private void SetAndSaveEnabledDrawers(uint iniDrawers)
		{
			uint currDrawers = 0;
			for (int i = 0; i < currEnabledDrawers.Length; i++)
			{
				if (currEnabledDrawers[i]) currDrawers |= materialDrawers[i];
			}

			if (iniDrawers != currDrawers) ShaderGUI.FindProperty("_EditorDrawers", matProperties).floatValue = currDrawers;
		}

		private bool DrawEffectToggleButton(string keyword, bool isPropertyToggle = false)
		{
			bool isEnabled = IsPropertyEnabled(keyword, isPropertyToggle);

			GUIStyle buttonStyle = new GUIStyle(GUI.skin.button);
			buttonStyle.fixedWidth = 40;
			buttonStyle.margin = new RectOffset(0, 0, 5, 0);
			buttonStyle.normal.textColor = isEnabled ? Color.green : Color.white;
			buttonStyle.fontStyle = FontStyle.Bold;

			if (GUILayout.Button(isEnabled ? "ON" : "OFF", buttonStyle))
			{
				SetPropertyEnabled(keyword, !isEnabled, isPropertyToggle);
			}

			return isEnabled;
		}

		// Function to draw the effect header and return the toggle state
		private bool DrawEffectHeader(string inspector, string keyword, bool currentToggle, bool isPropertyToggle = false)
		{
			bool ini = currentToggle;

			GUIContent effectNameLabel = new GUIContent();
			effectNameLabel.tooltip = keyword + " (C#)";
			effectNameLabel.text = "  " + effectCount + ". " + inspector;
			effectNameLabel.image = (Texture)Resources.Load("Editor Textures/" + inspector);

			EditorGUILayout.BeginHorizontal();
			{
				DrawEffectToggleButton(keyword, isPropertyToggle);
				currentToggle = EditorGUILayout.BeginFoldoutHeaderGroup(currentToggle, effectNameLabel, foldoutInsideStyle);
			}
			EditorGUILayout.EndHorizontal();

			effectCount++;
			if (ini != currentToggle)
			{
				SetPropertyEnabled(keyword, currentToggle, isPropertyToggle);
			}

			return currentToggle;
		}

		// Function to draw the properties of the effect
		private void DrawEffectProperties(EffectPropertyRange range)
		{
			EditorGUILayout.BeginVertical(propertiesStyle);
			{
				for (int i = range.start; i <= range.end; i++)
				{
					DrawProperty(i, range.noReset);
				}
			}
			EditorGUILayout.EndVertical();
		}

		// Integrated function to draw the effect
		private void DrawEffect(string inspector, string keyword, EffectPropertyRange propertyRange, bool isPropertyToggle = false)
		{
			bool toggle = IsPropertyEnabled(keyword, isPropertyToggle);
			toggle = DrawEffectHeader(inspector, keyword, toggle, isPropertyToggle);

			if (toggle)
			{
				 DrawEffectProperties(propertyRange);
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}


		private void DrawProperty(int index, bool noReset = false)
		{
			MaterialProperty targetProperty = matProperties[index];

			EditorGUILayout.BeginHorizontal();
			{
				GUIContent propertyLabel = new GUIContent();
				propertyLabel.text = targetProperty.displayName;
				propertyLabel.tooltip = targetProperty.name + " (C#)";

				matEditor.ShaderProperty(targetProperty, propertyLabel);

				if (!noReset)
				{
					GUIContent resetButtonLabel = new GUIContent();
					resetButtonLabel.text = "R";
					resetButtonLabel.tooltip = "Resets to default value";
					if (GUILayout.Button(resetButtonLabel, GUILayout.Width(20))) ResetProperty(targetProperty);
				}
			}
			EditorGUILayout.EndHorizontal();
		}

		private void ResetProperty(MaterialProperty targetProperty)
		{
			if (originalMaterialCopy == null) originalMaterialCopy = new Material(targetMat.shader);
			if (targetProperty.type == MaterialProperty.PropType.Float || targetProperty.type == MaterialProperty.PropType.Range)
			{
				targetProperty.floatValue = originalMaterialCopy.GetFloat(targetProperty.name);
			}
			else if (targetProperty.type == MaterialProperty.PropType.Vector)
			{
				targetProperty.vectorValue = originalMaterialCopy.GetVector(targetProperty.name);
			}
			else if (targetProperty.type == MaterialProperty.PropType.Color)
			{
				targetProperty.colorValue = originalMaterialCopy.GetColor(targetProperty.name);
			}
			else if (targetProperty.type == MaterialProperty.PropType.Texture)
			{
				targetProperty.textureValue = originalMaterialCopy.GetTexture(targetProperty.name);
			}
		}

		private bool DrawEffectSubKeywordToggle(string inspector, string keyword, bool setCustomConfigAfter = false)
		{
			GUIContent propertyLabel = new GUIContent();
			propertyLabel.text = inspector;
			propertyLabel.tooltip = keyword + " (C#)";

			bool ini = oldKeyWords.Contains(keyword);
			bool toggle = ini;
			toggle = GUILayout.Toggle(toggle, propertyLabel);
			if (ini != toggle)
			{
				if (toggle) targetMat.EnableKeyword(keyword);
				else targetMat.DisableKeyword(keyword);
			}

			return toggle;
		}

		private void Save()
		{
			if (!Application.isPlaying) EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
			EditorUtility.SetDirty(targetMat);
		}

		private void DrawLine(Color color, int thickness = 2, int padding = 10)
		{
			Rect r = EditorGUILayout.GetControlRect(GUILayout.Height(padding + thickness));
			r.height = thickness;
			r.y += (padding / 2);
			r.x -= 2;
			r.width += 6;
			EditorGUI.DrawRect(r, color);
		}

		private void Outline(string inspector, string keyword)
		{
			bool toggle = IsPropertyEnabled(keyword, false);
			toggle = DrawEffectHeader(inspector, keyword, toggle);

			if (toggle)
			{
				SetPropertyEnabled(keyword, true, false);
				EditorGUILayout.BeginVertical(propertiesStyle);
				{
					DrawProperty(5, true);
					DrawProperty(9);
					DrawProperty(10);
					DrawEffectSubKeywordToggle("Outline High Resolution?", "OUTBASE8DIR_ON");

					DrawLine(Color.grey, 1, 3);
					bool outlineTexture = DrawEffectSubKeywordToggle("Outline uses texture?", "OUTTEX_ON");
					if (outlineTexture)
					{
						DrawProperty(6);
						DrawProperty(7);
						DrawProperty(8);
					}
				}
				EditorGUILayout.EndVertical();
			}
			else SetPropertyEnabled(keyword, false, false);
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		private void Underlay(string inspector, string keyword)
		{
			bool toggle = IsPropertyEnabled(keyword, false);
			toggle = DrawEffectHeader(inspector, keyword, toggle);

			if (toggle)
			{
				SetPropertyEnabled("UNDERLAY_ON", true, false);
				EditorGUILayout.BeginVertical(propertiesStyle);
				{
					DrawProperty(29, true);
					DrawProperty(30);
					DrawProperty(31);
					DrawProperty(32);
					DrawProperty(33);

					DrawLine(Color.grey, 1, 3);
					bool underlayInner = DrawEffectSubKeywordToggle("Underlay Inner?", "UNDERLAY_INNER");
				}
				EditorGUILayout.EndVertical();
			}
			else SetPropertyEnabled("UNDERLAY_ON", false, false);
			EditorGUILayout.EndFoldoutHeaderGroup();
		}
		private void Glitch(string inspector, string keyword)
		{
			bool toggle = IsPropertyEnabled(keyword, true);
			toggle = DrawEffectHeader(inspector, keyword, toggle, true);

			if (toggle)
			{
				EditorGUILayout.BeginVertical(propertiesStyle);
				{
					MaterialProperty verticalGlitchProp = ShaderGUI.FindProperty("_VerticalGlitchOnOff", matProperties);
					
					EditorGUILayout.BeginHorizontal();
					matEditor.ShaderProperty(verticalGlitchProp, new GUIContent("Vertical Glitch Mode"));
					EditorGUILayout.EndHorizontal();
					
					if (verticalGlitchProp.floatValue > 0.5f)
					{
						DrawProperty(95);
						DrawProperty(96);
						DrawProperty(97);
					}
					else 
					{
						DrawProperty(93);
						DrawProperty(94);
						DrawProperty(95);
						DrawProperty(96);
						DrawProperty(97);
					}
				}
				EditorGUILayout.EndVertical();
			}
			EditorGUILayout.EndFoldoutHeaderGroup();
		}

		private void OpenDocumentation()
		{
			Application.OpenURL("https://stellarabbitstudio.gitbook.io/stellarabbitstudio/tmp-vfx-shader/quickstart");
		}

		private void AddTMPVFXComponent()
		{
			if (lastSelectedGameObject != null)
			{
				var tmpText = lastSelectedGameObject.GetComponent<TMPro.TMP_Text>();
				if (tmpText != null)
				{
					var vfxComponent = lastSelectedGameObject.AddComponent<TMPVFX>();
					EditorUtility.SetDirty(lastSelectedGameObject);
				}
				else
				{
					EditorUtility.DisplayDialog("TMP VFX", "Selected object must have a TextMeshPro component!", "OK");
				}
			}
			else
			{
				EditorUtility.DisplayDialog("TMP VFX", "Please select a GameObject first!", "OK");
			}
		}

		private string GetPropertyNameFromKeyword(string keyword)
		{
			return "_" + keyword.Replace("_ON", "Enabled");
		}

		private bool IsPropertyEnabled(string keyword, bool isPropertyToggle)
		{
			if (isPropertyToggle)
			{
				string propertyName = GetPropertyNameFromKeyword(keyword);
				MaterialProperty property = ShaderGUI.FindProperty(propertyName, matProperties);
				return property.floatValue > 0.5f;
			}
			return oldKeyWords.Contains(keyword);
		}

		private void SetPropertyEnabled(string keyword, bool enabled, bool isPropertyToggle)
		{
			bool valueChanged = false;
			
			if (isPropertyToggle)
			{
				string propertyName = GetPropertyNameFromKeyword(keyword);
				MaterialProperty property = ShaderGUI.FindProperty(propertyName, matProperties);
				float newValue = enabled ? 1 : 0;
				if (property.floatValue != newValue)
				{
					property.floatValue = newValue;
					valueChanged = true;
				}
			}
			else
			{
				bool currentlyEnabled = oldKeyWords.Contains(keyword);
				if (currentlyEnabled != enabled)
				{
					if (enabled)
						targetMat.EnableKeyword(keyword);
					else
						targetMat.DisableKeyword(keyword);
					valueChanged = true;
				}
			}

			// 실제로 값이 변경되었을 때만 Save 호출
			if (valueChanged)
			{
				Save();
			}
		}
	}
}
#endif