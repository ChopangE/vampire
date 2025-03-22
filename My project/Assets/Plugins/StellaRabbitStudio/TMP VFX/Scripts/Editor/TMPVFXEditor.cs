#if UNITY_EDITOR
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace StellaRabbitStudio
{
    [CustomEditor(typeof(TMPVFX)), CanEditMultipleObjects]
    public class TMPVFXEditor : UnityEditor.Editor
    {
        private bool showUrpWarning = false;
        private double warningTime = 0f;

        public override void OnInspectorGUI()
        {
            TMPVFX myScript = (TMPVFX)target;
            
            // Style Settings
            GUIStyle titleStyle = new GUIStyle(EditorStyles.boldLabel)
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
                fixedWidth = 240
            };

            GUIStyle buttonStyle = new GUIStyle(GUI.skin.button)
            {
                fixedHeight = 22,
                padding = new RectOffset(5, 15, 3, 3),
                margin = new RectOffset(2, 2, 0, 5),
                normal = new GUIStyleState { textColor = new Color(0f, 1f, 0.769f) },
                hover = new GUIStyleState { textColor = new Color(0f, 1f, 1f) },
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.MiddleLeft,
                fontSize = 11,
                stretchWidth = true
            };

            // Title Box Start
            EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(15, 15, 5, 15),
                margin = new RectOffset(25, 25, 10, 10)
            });

            // Logo and Title
            EditorGUILayout.BeginHorizontal();
            {
                Texture2D iconTexture = (Texture2D)Resources.Load("Editor Textures/Logo");
                GUIContent titleContent = new GUIContent("  TMP VFX Component", iconTexture);
                GUILayout.Label(titleContent, titleStyle);
                
                // Version Information
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

            // Button Section
            EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(0, 0, 5, 5)
            });
            {
                
                if (GUILayout.Button(new GUIContent(" Documentation", EditorGUIUtility.IconContent("_Help").image), buttonStyle))
                {
                    Application.OpenURL("https://stellarabbitstudio.gitbook.io/stellarabbitstudio/tmp-vfx-shader/quickstart");
                }

                if (GUILayout.Button(new GUIContent(" Auto Add VFX Components", EditorGUIUtility.IconContent("d_Toolbar Plus").image), buttonStyle))
                {
                    AddEffectComponents();
                }

                if (GUILayout.Button(new GUIContent(" Deactivate All VFX", EditorGUIUtility.IconContent("d_Refresh").image), buttonStyle))
                {
                    bool successOperation = true;
                    for (int i = 0; i < targets.Length; i++)
                    {
                        successOperation &= ((TMPVFX)targets[i]).ClearAllKeywords();
                    }
                    if(successOperation) ShowNotification("TMPVFX: Clean SDF Font");
                }

                if (GUILayout.Button(new GUIContent(" Save Font (Clear VFX)", EditorGUIUtility.IconContent("SaveAs").image), buttonStyle))
                {
                    bool successOperation = true;
                    for (int i = 0; i < targets.Length; i++)
                    {
                        successOperation &= ((TMPVFX)targets[i]).SaveFont(true);
                    }
                    if(successOperation) ShowNotification("TMPVFX: Clean SDF Font");
                }

                if (GUILayout.Button(new GUIContent(" Save Font (Include VFX)", EditorGUIUtility.IconContent("SaveAs").image), buttonStyle))
                {
                    bool successOperation = true;
                    for(int i = 0; i < targets.Length; i++)
                    {
                        successOperation &= ((TMPVFX) targets[i]).SaveFont();
                    }
                    if(successOperation) ShowNotification("TMPVFX: Font Saved");
                }
            }
            EditorGUILayout.EndVertical();

            // URP Warning Message
            bool isUrp = false;
            Shader temp = TMPVFXShaderData.FindShader("TMP VFX");
            if (temp != null) isUrp = true;

            if (warningTime < EditorApplication.timeSinceStartup) showUrpWarning = false;
            if (isUrp) showUrpWarning = false;
            if (showUrpWarning)
            {
                EditorGUILayout.HelpBox(
                    "You can't set the URP 2D Renderer variant since you didn't import the URP package available in the asset root folder (SEE DOCUMENTATION)",
                    MessageType.Error,
                    true);
            }

            EditorGUILayout.EndVertical();
            DrawLine(Color.grey, 1, 3);

            EditorGUILayout.Space();

            // Component Removal Section
            EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(10, 10, 10, 10),
                margin = new RectOffset(25, 25, 5, 5)
            });
            {
                if (GUILayout.Button(new GUIContent(" Remove Component", EditorGUIUtility.IconContent("d_TreeEditor.Trash").image), buttonStyle))
                {
                    for(int i = targets.Length - 1; i >= 0; i--)
                    {
                        DestroyImmediate(targets[i] as TMPVFX);
                        ((TMPVFX)targets[i]).SetSceneDirty();
                    }
                    ShowNotification("TMPVFX: Component Removed");
                }

                if (GUILayout.Button(new GUIContent(" Remove Component And Font", EditorGUIUtility.IconContent("d_TreeEditor.Trash").image), buttonStyle))
                {
                    for(int i = targets.Length - 1; i >= 0; i--)
                    {
                        var tmpvfx = targets[i] as TMPVFX;
                        var tmpText = tmpvfx.GetComponent<TMP_Text>();
                        if (tmpText != null)
                        {
                            tmpText.font = null;
                        }
                        DestroyImmediate(tmpvfx);
                        tmpvfx.SetSceneDirty();
                    }
                    ShowNotification("TMPVFX: Component And Font Removed");
                }
            }
            EditorGUILayout.EndVertical();
        }
        private void SetCurrentShaderType(TMPVFX myScript)
        {
            string shaderName = "";
            TextMeshPro tm = myScript.GetComponent<TextMeshPro>();
            if (tm != null)
            {
                if(tm.fontMaterial == null) return;
                shaderName = tm.fontMaterial.shader.name;
            }
            else
            {
                Graphic img = myScript.GetComponent<Graphic>();
                if(img.material == null) return;
                if (img != null) shaderName = img.material.shader.name;
            }
            shaderName = shaderName.Replace("TMPVFX/", "");

            if (shaderName.Equals("TMPVFX")) myScript.currentShaderType = TMPVFX.ShaderTypes.Default;
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

        private void ShowNotification(string message)
        {
            SceneView.lastActiveSceneView?.ShowNotification(new GUIContent(message), 2f);
        }

        private void AddEffectComponents()
        {
            TMPVFX tmpvfx = target as TMPVFX;
            if (tmpvfx == null)
            {
                Debug.LogWarning("TMPVFX: Target component is null");
                return;
            }

            Material material = null;
            var tmpText = tmpvfx.GetComponent<TMP_Text>();
            if (tmpText == null)
            {
                Debug.LogWarning("TMPVFX: No TMP_Text component found on the target object");
            }
            else
            {
                material = tmpText.fontSharedMaterial;
                if (material == null)
                {
                    Debug.LogWarning("TMPVFX: No font material assigned to TMP_Text component");
                }
            }

            if (material != null)
            {
                if (material.GetFloat("_GLITCHEnabled") > 0.5f)
                {
                    if (tmpvfx.GetComponent<GlitchOnce>() != null)
                    {
                        Debug.Log("TMPVFX: GlitchOnce component already exists");
                    }
                    else
                    {
                        tmpvfx.gameObject.AddComponent<GlitchOnce>();
                        Debug.Log("TMPVFX: Successfully added GlitchOnce component");
                    }
                }
                else
                {
                    Debug.Log("TMPVFX: Glitch effect is not enabled in the material");
                }
            }

            ShowNotification("TMPVFX: Effect Components Added");
        }
    }
}
#endif