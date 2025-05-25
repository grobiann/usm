using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace USM
{
    public class USMWindow : EditorWindow
    {
        public UiStateMachineBehaviour CurrentUsmBehaviour => _currentUsmBehaviour;

        private UiStateMachineBehaviour[] _usmBehavioursInScene = new UiStateMachineBehaviour[0];
        private UiStateMachineBehaviour _currentUsmBehaviour;
        private Transform[] _currentUsmChildren;
        private Transform[] _currentUsmChildrenFiltered;

        // Variables for GUI representation
        private int _selectedUsmBehaviourIndex;
        private string _newStateName;
        private Vector2 _scrollPosition;
        private string _findObjectFilter;

        private const int STATE_OBJECT_ITEM_HEIGHT = 25;

        [MenuItem("Window/Tools/USM Window")]
        public static USMWindow ShowWindow()
        {
            return GetWindow<USMWindow>("USM Window");
        }

        private void OnEnable()
        {
            EditorSceneManager.sceneOpened += OnSceneChanged;
            PrefabStage.prefabStageOpened += OnPrefabStageChanged;
            PrefabStage.prefabStageClosing += OnPrefabStageChanged;

            InitializeUsmBehaviours();
        }

        private void OnDisable()
        {
            EditorSceneManager.sceneOpened -= OnSceneChanged;
            PrefabStage.prefabStageOpened -= OnPrefabStageChanged;
            PrefabStage.prefabStageClosing -= OnPrefabStageChanged;
        }

        private void OnSceneChanged(Scene scene, OpenSceneMode mode)
        {
            InitializeUsmBehaviours();
        }

        private void OnPrefabStageChanged(PrefabStage prefabStage)
        {
            InitializeUsmBehaviours();
        }

        private void OnGUI()
        {
            EnsureUsmDataValidity();

            StartGUI();

            // Create StateMachineGroupName field
            DrawStateMachineGUI();

            if (_currentUsmBehaviour == null)
            {
                EndGUI();
                return;
            }

            // Draw scroll area
            {
                _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.ExpandHeight(true));
                GUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

                // Left side content
                DrawUsmChildrenListGUI();

                // Draw vertical line
                GUILayout.Box(GUIContent.none, GUILayout.Width(2), GUILayout.ExpandHeight(true));

                // Right side content
                var states = _currentUsmBehaviour.Usm.States;
                for (int i = 0; i < states.Count; i++)
                {
                    var state = states[i];
                    DrawStateGUI(state);
                }
                DrawStateCreateGUI();

                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
            }

            if (GUILayout.Button("Save", GUILayout.ExpandWidth(true), GUILayout.Height(50)))
            {
                SaveCurrentState();
            }

            EndGUI();
        }

        #region GUI Draw
        private void StartGUI()
        {
            GUILayout.BeginHorizontal();
            GUILayout.Space(10);
            GUILayout.BeginVertical();
        }

        private void EndGUI()
        {
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        private void DrawStateMachineGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("StateMachine", GUILayout.Width(100));
            GUILayout.BeginHorizontal();

            int prevSelectedOption = _selectedUsmBehaviourIndex;
            string[] options = _usmBehavioursInScene.Select(x => x.name).ToArray();
            _selectedUsmBehaviourIndex = EditorGUILayout.Popup(_selectedUsmBehaviourIndex, options, GUILayout.Width(300));

            if (prevSelectedOption != _selectedUsmBehaviourIndex)
            {
                // ������ ����Ǿ��� �� ������ �۾� �߰�
                SelectUsmBehaviour(_usmBehavioursInScene[_selectedUsmBehaviourIndex]);
            }

            if (GUILayout.Button("Refresh", GUILayout.Width(70)))
            {
                InitializeUsmBehaviours();
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void DrawUsmChildrenListGUI()
        {
            float width = 310;
            Debug.Assert(_currentUsmBehaviour != null);

            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Select", GUILayout.Height(40), GUILayout.Width(width * 0.5f)))
            {
                SelectObjectInInspector(_currentUsmBehaviour.gameObject);
            }
            if (GUILayout.Button("Prefab Mode", GUILayout.Height(40), GUILayout.Width(width * 0.5f)))
            {
                OpenInPrefabMode();
            }
            GUILayout.EndHorizontal();
            GUILayout.Space(5);
            string prevFilter = _findObjectFilter;

            GUILayout.BeginHorizontal();
            GUILayout.Label("Filter:", GUILayout.Width(40));
            _findObjectFilter = GUILayout.TextField(_findObjectFilter, GUILayout.Width(width - 40 - 5));
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            if (prevFilter != _findObjectFilter)
            {
                RefreshUsmFilteredChildren();
            }
            GUILayout.Space(3);

            Debug.Assert(_currentUsmChildrenFiltered != null);
            foreach (var item in _currentUsmChildrenFiltered)
            {
                GUILayout.BeginHorizontal(GUILayout.Height(STATE_OBJECT_ITEM_HEIGHT));
                if (GUILayout.Button("Select", GUILayout.Width(50)))
                {
                    SelectObjectInInspector(item.gameObject);
                }

                string prevGameObjectName = item.gameObject.name;
                string curName = GUILayout.TextField(item.gameObject.name, GUILayout.Width(width - 50));
                if (prevGameObjectName != curName)
                {
                    if (IsUsmChangableInCurrentMode() == false)
                    {
                        OpenInPrefabMode();
                    }
                    else
                    {
                        ChangeGameObjectName(item.gameObject, curName);
                    }
                }

                bool contain = _currentUsmBehaviour.Usm.ActiveTargets.Contains(item.gameObject);
                var btnText = contain ? "Use" : "Not Use";
                var bgColor = contain ? Color.white : Color.gray;
                GUI.backgroundColor = bgColor;
                if (GUILayout.Button(btnText, GUILayout.Width(60)))
                {
                    if (IsUsmChangableInCurrentMode() == false)
                    {
                        OpenInPrefabMode();
                        return;
                    }
                    else
                    {
                        ChangeGameObjectUsability(_currentUsmBehaviour.Usm, item.gameObject, !contain);
                    }
                }
                GUI.backgroundColor = Color.white;
                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();
        }

        private void DrawStateGUI(UsmState state)
        {
            Debug.Assert(_currentUsmBehaviour != null);
            Debug.Assert(state != null);

            float width = 100;
            GUILayout.BeginVertical();
            GUI.backgroundColor = new Color(0.5f, 1.0f, 0.5f);
            if (GUILayout.Button("Test", GUILayout.Width(width), GUILayout.Height(40)))
            {
                _currentUsmBehaviour.SetState(state);
                EditorUtility.SetDirty(_currentUsmBehaviour.gameObject);
            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(5);
            string prevStateName = state.StateName;
            string stateName = GUILayout.TextField(state.StateName, GUILayout.Width(width));
            if (prevStateName != stateName)
            {
                if (IsUsmChangableInCurrentMode() == false)
                {
                    OpenInPrefabMode();
                }
                else
                {
                    ChangeStateName(state, stateName);
                }
            }
            GUILayout.Space(5);

            Debug.Assert(_currentUsmChildrenFiltered != null);
            foreach (var item in _currentUsmChildrenFiltered)
            {
                var go = item.gameObject;
                bool use = _currentUsmBehaviour.Usm.ActiveTargets.Contains(go);
                if (use)
                {
                    var active = state.IsActive(go);
                    var btnText = active ? "On" : "Off";
                    var bgColor = active ? Color.white : Color.gray;
                    GUI.backgroundColor = bgColor;
                    if (GUILayout.Button(btnText, GUILayout.Width(width), GUILayout.Height(STATE_OBJECT_ITEM_HEIGHT)))
                    {
                        if (IsUsmChangableInCurrentMode() == false)
                        {
                            OpenInPrefabMode();
                        }
                        else
                        {
                            ChangeGameObjectActivation(state, go, active);
                        }
                    }
                    GUI.backgroundColor = Color.white;
                }
            }

            GUI.backgroundColor = new Color(1.0f, 0.5f, 0.5f);
            if (GUILayout.Button("Delete", GUILayout.Width(width), GUILayout.Height(STATE_OBJECT_ITEM_HEIGHT)))
            {
                if (IsUsmChangableInCurrentMode() == false)
                {
                    OpenInPrefabMode();
                }
                else
                {
                    DeleteState(state);
                }

            }
            GUI.backgroundColor = Color.white;

            GUILayout.Space(10);

            GUILayout.EndVertical();
        }

        private void DrawStateCreateGUI()
        {
            Debug.Assert(_currentUsmBehaviour != null);

            GUILayout.BeginVertical();

            GUILayout.Space(44);
            GUILayout.Space(5);

            _newStateName = GUILayout.TextField(_newStateName, GUILayout.Width(100));
            GUILayout.Space(5);

            GUI.backgroundColor = Color.yellow;
            if (GUILayout.Button("Create", GUILayout.Height(STATE_OBJECT_ITEM_HEIGHT * 2.5f)))
            {
                if (IsUsmChangableInCurrentMode() == false)
                {
                    OpenInPrefabMode();
                }
                else
                {
                    CreateNewState(_newStateName);
                }
            }
            GUI.backgroundColor = Color.white;
            GUILayout.EndVertical();
        }
        #endregion

        #region GUI Input Controller
        private void ChangeGameObjectUsability(UiStateMachine usm, GameObject go, bool usable)
        {
            if (usable)
                usm.AddTarget(go);
            else
                usm.RemoveTarget(go);

            RefreshUsmFilteredChildren();
            EditorUtility.SetDirty(_currentUsmBehaviour.gameObject);
        }

        private void ChangeGameObjectActivation(UsmState state, GameObject go, bool active)
        {
            state.SetActive(go, !active);
            EditorUtility.SetDirty(_currentUsmBehaviour.gameObject);
        }

        private void ChangeStateName(UsmState state, string name)
        {
            state.StateName = name;
            EditorUtility.SetDirty(_currentUsmBehaviour.gameObject);
        }

        private void ChangeGameObjectName(GameObject go, string name)
        {
            go.name = name;
            EditorUtility.SetDirty(_currentUsmBehaviour.gameObject);
        }

        private bool CreateNewState(string name)
        {
            foreach (var state in _currentUsmBehaviour.Usm.States)
            {
                if (state.StateName == name)
                {
                    return false;
                }
            }

            var newState = new UsmState();
            newState.StateName = name;

            _currentUsmBehaviour.Usm.AddState(newState);
            EditorUtility.SetDirty(_currentUsmBehaviour.gameObject);

            int stateCount = _currentUsmBehaviour.Usm.States.Count;
            _newStateName = GetNewStateName(stateCount);
            return true;
        }

        private void DeleteState(UsmState state)
        {
            Undo.RecordObject(_currentUsmBehaviour, "Delete State");
            _currentUsmBehaviour.Usm.RemoveState(state);

            EditorUtility.SetDirty(_currentUsmBehaviour.gameObject);

            int stateCount = _currentUsmBehaviour.Usm.States.Count;
            _newStateName = GetNewStateName(stateCount);
        }

        private void SaveCurrentState()
        {
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
            {
                // Scene ���¸� ����
                EditorSceneManager.SaveOpenScenes();
                return;
            }

            // To save a prefab without a prompt. 
            MethodInfo savePrefabMethod = typeof(PrefabStage).GetMethod("Save", BindingFlags.NonPublic | BindingFlags.Instance);
            savePrefabMethod.Invoke(prefabStage, null);
        }
        #endregion

        private void InitializeUsmBehaviours()
        {
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                var root = prefabStage.prefabContentsRoot;
                var usm = root.GetComponent<UiStateMachineBehaviour>();
                if (usm == null)
                {
                    _usmBehavioursInScene = new UiStateMachineBehaviour[0];
                }
                else
                {
                    // Find all instances of UiStateMachine in prefab stage
                    _usmBehavioursInScene = usm.gameObject.GetComponentsInChildren<UiStateMachineBehaviour>(true);
                }
            }
            else
            {
                // Find all instances of UiStateMachine in the current scene
                _usmBehavioursInScene = FindObjectsOfType<UiStateMachineBehaviour>(true).Reverse().ToArray();
            }

            _currentUsmBehaviour = null;
            _currentUsmChildrenFiltered = null;
            if (_usmBehavioursInScene.Length > 0)
            {
                SelectUsmBehaviour(_usmBehavioursInScene[0]);
            }
        }

        public void SelectUsmBehaviour(UiStateMachineBehaviour usmBehaviour)
        {
            Debug.Assert(usmBehaviour != null);
            if (usmBehaviour.Usm == null)
            {
                Debug.LogWarning("There is no usm in behaviour");
                return;
            }

            _currentUsmBehaviour = usmBehaviour;
            _selectedUsmBehaviourIndex = _usmBehavioursInScene.ToList().IndexOf(usmBehaviour);
            EnsureUsmDataValidity();

            RefreshUsmChildren();

            int stateCount = _currentUsmBehaviour.Usm.States.Count;
            _newStateName = GetNewStateName(stateCount);
        }

        private void RefreshUsmChildren()
        {
            Debug.Assert(_currentUsmBehaviour != null);

            _currentUsmChildren = _currentUsmBehaviour
                .GetComponentsInChildren<Transform>(true)
                .Where(x => x != _currentUsmBehaviour.transform)
                .ToArray();

            RefreshUsmFilteredChildren();
        }

        private void RefreshUsmFilteredChildren()
        {
            Debug.Assert(_currentUsmBehaviour != null);
            Debug.Assert(_currentUsmChildren != null);

            if (string.IsNullOrEmpty(_findObjectFilter) == false)
            {
                var loweredFilter = _findObjectFilter.ToLower();

                _currentUsmChildrenFiltered = _currentUsmChildren
                    .Where(child => child.gameObject.name.ToLower().Contains(loweredFilter))
                    .OrderByDescending(item => _currentUsmBehaviour.Usm.ActiveTargets.Contains(item.gameObject))
                    .ToArray();
            }
            else
            {
                _currentUsmChildrenFiltered = _currentUsmChildren
                    .OrderByDescending(item => _currentUsmBehaviour.Usm.ActiveTargets.Contains(item.gameObject))
                    .ToArray();
            }
        }

        private bool IsUsmChangableInCurrentMode()
        {
            var prefabType = PrefabUtility.GetPrefabAssetType(_currentUsmBehaviour.gameObject);
            if (prefabType == PrefabAssetType.NotAPrefab)
                return true;

            // ������ ������Ʈ�� ������ ��忡�� �ֻ����϶��� ���� ����
            PrefabStage prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage == null)
                return false;

            if (_currentUsmBehaviour.gameObject != prefabStage.prefabContentsRoot)
                return false;

            return true;
        }

        private void EnsureUsmDataValidity()
        {
            // scene���� usm�� ����Ǿ����� ���� Ȯ��
            bool usmInSceneChanged =
                _usmBehavioursInScene.ToList().Exists(x => x == null) ||
                _selectedUsmBehaviourIndex >= _usmBehavioursInScene.Length ||
                _currentUsmBehaviour == null;

            if (usmInSceneChanged)
            {
                int prevSelectedIndex = _selectedUsmBehaviourIndex;
                InitializeUsmBehaviours();

                if (_usmBehavioursInScene.Length == 0)
                    return;

                int selectIndex = Math.Min(prevSelectedIndex, _usmBehavioursInScene.Length - 1);
                SelectUsmBehaviour(_usmBehavioursInScene[selectIndex]);
            }

            Debug.Assert(_currentUsmBehaviour != null);

            // usm���� ������Ʈ �����Ͱ� �ùٸ��� Ȯ��
            if (_currentUsmBehaviour.Usm.RemoveInvalidLinks())
            {
                RefreshUsmChildren();
            }
        }

        private void SelectObjectInInspector(GameObject go)
        {
            Debug.Assert(go != null);

            Selection.activeGameObject = go;
            EditorGUIUtility.PingObject(go);
        }

        private void OpenInPrefabMode()
        {
            Debug.Assert(_currentUsmBehaviour != null);
            var go = _currentUsmBehaviour.gameObject;

            var assetPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(go);
            if (!string.IsNullOrEmpty(assetPath))
            {
                PrefabStageUtility.OpenPrefab(assetPath, go, PrefabStage.Mode.InIsolation);
            }
            else
            {
                Debug.LogWarning("Selected object is not a prefab or not part of a prefab asset.");
            }
        }

        private string GetNewStateName(int count)
        {
            if (count == 0)
                return "state";
            else
                return $"state ({count})";
        }
    }
}