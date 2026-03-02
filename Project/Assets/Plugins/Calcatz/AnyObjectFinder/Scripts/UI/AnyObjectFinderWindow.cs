using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace Calcatz.AnyObjectFinder {

    public class AnyObjectFinderWindow : EditorWindow {

        public delegate void BindResultItemGUICallback(VisualElement visualElement, int index, ObjectFinder.ResultItem resultItem);

        [SerializeField] private ObjectFinder m_finder = new ObjectFinder();
        [SerializeField] private FolderSelectionWindow folderSelectionWindow;

        [SerializeField] private List<ObjectFinder.ResultItem> result = null;

        private BaseSearchBy currentSearchByInstance;

        private System.Type typeToSearch = null;
        [SerializeField] private string typeSearchTerm = "";

        private Button locationButton;
        private VisualElement customMethodArea;

        private VisualElement resultArea;
        private ListView foundList;

        private SerializedObject serializedObject;

        [MenuItem("Tools/Any Object Finder")]
        private static void OpenWindow() {
            var window = GetWindow<AnyObjectFinderWindow>();
            window.Show();
        }

        private void OnEnable() {
            titleContent.text = "Any Object Finder";
            titleContent.image = EditorGUIUtility.FindTexture("d_Search Icon");
            minSize = new Vector2(601.6f, 138);
            if (!string.IsNullOrEmpty(m_finder.typeNameToSearch)) {
                typeToSearch = System.Type.GetType(m_finder.typeNameToSearch);
            }
        }

        private void CreateGUI() {
            serializedObject = new SerializedObject(this);
            rootVisualElement.Bind(serializedObject);

            VisualElement root = new VisualElement()
                .AOF_Margin(4f)
                .AOF_BorderRadius(4f)
                .AOF_BackgroundColor(new Color(0.2f, 0.2f, 0.2f, 1.0f))
                .AOF_Grow();

            root.Add(CreateFiltersSection());

            resultArea = new VisualElement()
                .AOF_Grow();
            if (resultArea != null) {
                UpdateResultArea();
            }
            root.Add(resultArea);

            rootVisualElement.Add(root);
        }

        private VisualElement CreateFiltersSection() {
            var filtersSection = new VisualElement()
                .AOF_Padding(4f)
                .AOF_BorderRadius(4f)
                .AOF_BackgroundColor(new Color(0.27f, 0.27f, 0.27f, 1.0f))
                .AOF_TextAlignment(TextAnchor.MiddleLeft);

            var topRow = new VisualElement()
                .AOF_Horizontal();

            var col1 = new VisualElement()
                .AOF_Vertical();
            var col2 = new VisualElement()
                .AOF_Vertical()
                .AOF_Grow(1f);

            var row1 = new VisualElement()
                .AOF_Horizontal();
            var row2 = new VisualElement()
                .AOF_Horizontal()
                .AOF_Margin(2f, 0, 0, 0);
            var row3 = new VisualElement()
                .AOF_Horizontal()
                .AOF_Margin(2f, 0, 0, 0);

            col1.Add(row1);
            col1.Add(row2);
            topRow.Add(col1);
            topRow.Add(col2);

            filtersSection.Add(topRow);
            filtersSection.Add(row3);

            var filterLabel = new Label("Filters:")
                .AOF_FixedWidth(45f)
                .AOF_FontStyle(FontStyle.Bold);
            row1.Add(filterLabel);

            var searchMethodPopup = CreateSearchMethodPopup();
            row1.Add(searchMethodPopup);

            row1.Add(CreateTargetFlagsField());

            row1.Add(new Label("Type"));
            row1.Add(CreateObjectTypePopup());


            row2.Add(new VisualElement().AOF_FixedWidth(45f));
            row2.Add(new Label("Location"));
            locationButton = CreateLocationButton();
            row2.Add(locationButton);

            row2.Add(CreateTraversalModeFlagsField());


            row3.Add(new VisualElement().AOF_FixedWidth(45f));

            col2.Add(CreateFilterByFieldNameArea());

            customMethodArea = new VisualElement()
                .AOF_Horizontal()
                .AOF_Grow();
            row3.Add(customMethodArea);

            //var flexibleSpace = new VisualElement().AOF_Grow();
            //row2.Add(flexibleSpace);

            searchMethodPopup.RegisterValueChangedCallback(valueChange => {
                UpdateCustomMethodArea(valueChange.newValue);
            });
            searchMethodPopup.value = typeof(SearchByStringValue);
            UpdateCustomMethodArea(searchMethodPopup.value);

            Button searchButton = DrawingUtils.CreateSearchButton(Search);
            row3.Add(searchButton);

            return filtersSection;
        }

        private VisualElement CreateFilterByFieldNameArea() {
            return new IMGUIContainer(() => {
                EditorGUI.BeginChangeCheck();
                bool toggleVal = m_finder.filterByFieldName;
                toggleVal = EditorGUILayout.ToggleLeft("Filter by Field Name", m_finder.filterByFieldName, GUILayout.MaxWidth(140));
                if (EditorGUI.EndChangeCheck()) {
                    Undo.RecordObject(this, "toggle filter by field name");
                    m_finder.filterByFieldName = toggleVal;
                    EditorUtility.SetDirty(this);
                }

                if (m_finder.filterByFieldName) {
                    var labelWidth = EditorGUIUtility.labelWidth;
                    EditorGUIUtility.labelWidth = 70;
                    EditorGUI.BeginChangeCheck();
                    var fieldName = EditorGUILayout.TextField("Field Name:", m_finder.fieldNameFilter);
                    if (EditorGUI.EndChangeCheck()) {
                        Undo.RecordObject(this, "change field name filter");
                        m_finder.fieldNameFilter = fieldName;
                        EditorUtility.SetDirty(this);
                    }
                    EditorGUIUtility.labelWidth = labelWidth;
                }
            }).AOF_Margin(2, 2, 2, 2)
              .AOF_Grow(1f);
        }

        private void UpdateCustomMethodArea(System.Type newType) {
            customMethodArea.Clear();
            if (newType == null) return;
            if (serializedObject == null) serializedObject = new SerializedObject(this);
            currentSearchByInstance = System.Activator.CreateInstance(newType) as BaseSearchBy;
            currentSearchByInstance.CreateGUI(customMethodArea, serializedObject.FindProperty("m_finder.searchArguments"));
        }

        private static PopupField<System.Type> CreateSearchMethodPopup() {
            var searchMethodPopup = new PopupField<System.Type>();
#if UNITY_2020_3_OR_NEWER
            searchMethodPopup.choices = ObjectFinder.GetAvailableSearchMethods();
#else
            var basePopupType = searchMethodPopup.GetType().BaseType;
            var choicesProp = basePopupType.GetProperty("choices", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            choicesProp.SetValue(searchMethodPopup, ObjectFinder.GetAvailableSearchMethods());
#endif
            searchMethodPopup.formatListItemCallback = type => {
                return ObjectNames.NicifyVariableName(type.Name);
            };
            searchMethodPopup.formatSelectedValueCallback = type => {
                return type == null ? "(Select a Search Method)" : ObjectNames.NicifyVariableName(type.Name);
            };
            searchMethodPopup.AOF_Shrink(0.5f);
            return searchMethodPopup;
        }

        private VisualElement CreateTargetFlagsField() {
            var searchTargetFlags = new EnumFlagsField("Target", m_finder.searchTarget);
            searchTargetFlags.labelElement.style.minWidth = 40;
            searchTargetFlags.labelElement.style.width = 40;
            searchTargetFlags.AOF_Shrink(0.5f);
            searchTargetFlags.RegisterValueChangedCallback(valueChange => {
                Undo.RecordObject(this, "change search target");
                m_finder.searchTarget = (ObjectFinder.SearchTarget)valueChange.newValue;
                searchTargetFlags.Q(className: "unity-base-popup-field__text").style.color = (int)m_finder.searchTarget == 0 ? (Color)new Color32(255, 112, 112, 255) : (Color)new Color32(238, 238, 238, 255);
                EditorUtility.SetDirty(this);
            });
            searchTargetFlags.Q(className: "unity-base-popup-field__text").style.color = (int)m_finder.searchTarget == 0 ? (Color)new Color32(255, 112, 112, 255) : (Color)new Color32(238, 238, 238, 255);
            return searchTargetFlags;
        }

        private VisualElement CreateTraversalModeFlagsField() {
            var traversalModeFlags = new EnumFlagsField("Traversal Mode", m_finder.traversalMode);
            traversalModeFlags.labelElement.style.minWidth = 90;
            traversalModeFlags.labelElement.style.width = 90;
            traversalModeFlags.AOF_Shrink(0.5f);
            traversalModeFlags.RegisterValueChangedCallback(valueChange => {
                Undo.RecordObject(this, "change traversal mode");
                m_finder.traversalMode = (ObjectFinder.TraversalMode)valueChange.newValue;
                traversalModeFlags.Q(className: "unity-base-popup-field__text").style.color = (int)m_finder.traversalMode == 0 ? (Color)new Color32(255, 112, 112, 255) : (Color)new Color32(238, 238, 238, 255);
                EditorUtility.SetDirty(this);
            });
            traversalModeFlags.Q(className: "unity-base-popup-field__text").style.color = (int)m_finder.traversalMode == 0 ? (Color)new Color32(255, 112, 112, 255) : (Color)new Color32(238, 238, 238, 255);
            return traversalModeFlags;
        }

        private IMGUIContainer CreateObjectTypePopup() {
            var objectTypePopup = new IMGUIContainer(()=> {
                if (GUILayout.Button(typeToSearch == null? "All Types" : typeToSearch.Name, EditorStyles.popup)) {
                    var availableTypes = ObjectFinder.GetAvailableObjectTypes();
                    var genericMenu = new GenericMenu();
                    genericMenu.AddItem(new GUIContent("All Types"), false, () => {
                        Undo.RecordObject(this, "change type to search");
                        typeToSearch = null;
                        m_finder.typeNameToSearch = "";
                        EditorUtility.SetDirty(this);
                    });
                    genericMenu.AddSeparator("");
                    foreach (var type in availableTypes) {
                        genericMenu.AddItem(new GUIContent(type.FullName.Replace('.', '/')), false, () => {
                            Undo.RecordObject(this, "change type to search");
                            typeToSearch = type;
                            m_finder.typeNameToSearch = type.AssemblyQualifiedName;
                            EditorUtility.SetDirty(this);
                        });
                    }
                    GenericMenuPopup.Show(genericMenu, "Select Type to Search", Event.current.mousePosition, null, typeSearchTerm, newTerm => {
                        typeSearchTerm = newTerm;
                        EditorUtility.SetDirty(this);
                    });
                }
            });
            objectTypePopup
                .AOF_Shrink(0.5f)
                .AOF_Margin(1, 4, 1, 1);
            return objectTypePopup;
        }

        private Button CreateLocationButton() {
            var searchFoldersButton = new Button(() => {
                if (folderSelectionWindow == null) {
                    folderSelectionWindow = FolderSelectionWindow.CreateNewWindow();
                }
                folderSelectionWindow.parentWindow = this;
                folderSelectionWindow.Show();
                folderSelectionWindow.Focus();
            });
            UpdateLocationButtonText(searchFoldersButton);
            searchFoldersButton.AOF_Shrink(0.5f);
            return searchFoldersButton;
        }

        private void Search() {
            if (currentSearchByInstance != null) {
                if ((int)m_finder.searchTarget == 0) {
                    EditorUtility.DisplayDialog("Can't Start Searching", "Please specify at least one Target.", "Close");
                    return;
                }
                if ((int)m_finder.selectedFolders.Count == 0) {
                    EditorUtility.DisplayDialog("Can't Start Searching", "Please specify at least one folder location.", "Close");
                    return;
                }

                result = m_finder.Search(currentSearchByInstance, typeToSearch);
                UpdateResultArea();
            }
            else {
                EditorUtility.DisplayDialog("Can't Start Searching", "Search By Method is not specified", "Close");
            }
        }

        private void UpdateResultArea() {
            resultArea.Clear();
            var resultList = CreateResultList(result, resultItem => {
                EditorGUIUtility.PingObject(resultItem.targetObject);
            }, currentSearchByInstance.OnCreateResultItemGUI, currentSearchByInstance.OnBindResultItemGUI);

            resultArea.Add(resultList);
        }

        public List<string> GetSelectedFolders() {
            return m_finder.selectedFolders;
        }

        public void SetSelectedFolders(List<string> selectedFolders) {
            m_finder.selectedFolders = selectedFolders;
            if (locationButton != null) {
                UpdateLocationButtonText(locationButton);
            }
        }

        private void UpdateLocationButtonText(Button button) {
            if (m_finder.selectedFolders.Count == 0) {
                button.text = "-Select Folder-";
                button.style.color = (Color)new Color32(255, 112, 112, 255);
            }
            else if (m_finder.selectedFolders.Count == 1) {
                button.text = System.IO.Path.GetFileNameWithoutExtension(m_finder.selectedFolders[0]);
                button.style.color = (Color)new Color32(238, 238, 238, 255);
            }
            else {
                button.text = "Multiple Folders";
                button.style.color = (Color)new Color32(238, 238, 238, 255);
            }
        }


        private ListView CreateResultList(List<ObjectFinder.ResultItem> result, System.Action<ObjectFinder.ResultItem> _onItemChoosen, System.Action<VisualElement> onCreateResultItemGUI, BindResultItemGUICallback onBindResultItemGUI) {

            System.Action<VisualElement, int> bindItem = (e, i) => {
                onBindResultItemGUI?.Invoke(e, i, result[i]);
            };

            foundList = new ListView(result, 25, ()=>MakeResultItem(onCreateResultItemGUI), bindItem) {
                name = "found-list",
                style = {
                    //marginTop = new StyleLength(new Length(CommandNode.verticalSpacing, LengthUnit.Pixel)),
                    flexGrow = 1f,
                    flexShrink = 0,
                    flexBasis = 0,
                    //backgroundColor = new StyleColor(NodesContainer.backgroundColor * 0.5f)
                }
            };
            foundList.AOF_Margin(5);

            foundList.selectionType = SelectionType.Single;

#if UNITY_2022_2_OR_NEWER
            foundList.itemsChosen += _obj => {
                foreach (var elm in _obj) {
                    _onItemChoosen(elm as ObjectFinder.ResultItem);
                }
            };
#elif UNITY_2020_1_OR_NEWER
            foundList.onItemsChosen += _obj => {
                foreach(var elm in _obj) {
                    _onItemChoosen(elm as ObjectFinder.ResultItem);
                }
            };
#else
            foundList.onItemChosen += _obj => {
                _onItemChoosen(_obj as ObjectFinder.ResultItem);
            };
#endif

            return foundList;
        }

        private static VisualElement MakeResultItem(System.Action<VisualElement> onCreateItemGUI) {
            var box = new VisualElement() {
                style = {
                    flexDirection = FlexDirection.Row,
                    flexGrow = 1f,
                    flexShrink = 0,
                    flexBasis = 0,
                    borderBottomColor = new Color(0.5f, 0.5f, 0.5f, 0.8f),
                    borderBottomWidth = new StyleFloat(1)
                }
            };
            onCreateItemGUI?.Invoke(box);
            return box;
        }

    }

}