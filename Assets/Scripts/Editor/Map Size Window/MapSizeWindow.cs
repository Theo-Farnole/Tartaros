namespace Game.MapSizeEditor
{
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    public class MapSizeWindow : EditorWindow
    {
        #region Fields
        private int _mapSize = -1;

        private Camera _cameraMinimapFrustum = null;
        private Camera _cameraMinimap = null;
        private Camera _cameraFogRevealed = null;
        private Camera _cameraFogVisible = null;

        private Projector _fogProjector = null;

        private SnapGridDatabase _constructionGrid = null;
        private SnapGridDatabase _fogGrid = null;
        #endregion

        [MenuItem("Tartaros/Advanced - DON'T TOUCH/Resize map")]
        public static void OpenWindow()
        {
            MapSizeWindow window = GetWindow<MapSizeWindow>();
            window.Show();
            window.titleContent = new GUIContent("Tartaros - Map resizer");

            window.InitalizeValues();
        }

        #region Methods
        #region Editor Window Callbacks
        void OnGUI()
        {
            DrawMapSizeProperty();

            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);

            DrawProperty("Minimap Frustum Camera", ref _cameraMinimapFrustum);
            DrawProperty("Minimap Camera", ref _cameraMinimap);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            DrawProperty("Construction Grid", ref _constructionGrid);
            DrawProperty("Fog Grid", ref _fogGrid);
            EditorGUILayout.Space(EditorGUIUtility.singleLineHeight);
            DrawProperty("Camera Fog Revealed", ref _cameraFogRevealed);
            DrawProperty("Camera Fog Visible", ref _cameraFogVisible);
            DrawProperty("Fog Projector", ref _fogProjector);


            if (GUILayout.Button("Set size"))
            {
                SetNewMapSize(_mapSize);
            }

            if (GUILayout.Button("Find objects"))
            {
                InitalizeValues();
            }
        }
        #endregion

        void SetNewMapSize(float length)
        {
            SetCameraLength(_cameraMinimap, length);
            SetCameraLength(_cameraMinimapFrustum, length);

            SetCameraLength(_cameraFogRevealed, length);
            SetCameraLength(_cameraFogVisible, length);

            SetProjectorLength(_fogProjector, length);

            _constructionGrid.SetGridLength(length);
            _fogGrid.SetGridLength(length);
        }

        #region Initialize Values
        void InitalizeValues()
        {
            Object[] assets = AssetDatabase.FindAssets("t:SnapGridDatabase")
                .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
                .Select(path => AssetDatabase.LoadAssetAtPath(path, typeof(SnapGridDatabase)))
                .ToArray();

            _constructionGrid = (SnapGridDatabase)assets.Where(x => !x.name.Contains("FogOfWar")).FirstOrDefault();
            _fogGrid = (SnapGridDatabase)assets.Where(x => x.name.Contains("FogOfWar")).FirstOrDefault();

            _fogProjector = FindObjectOfType<Projector>();

            Camera[] cameras = FindObjectsOfType<Camera>();

            _cameraMinimap = cameras.Where(x => x.name.Contains("MiniMap") && !x.name.Contains("Frustum")).FirstOrDefault();
            _cameraMinimapFrustum = cameras.Where(x => x.name.Contains("Frustum")).FirstOrDefault();
            _cameraFogRevealed = cameras.Where(x => x.name.Contains("Revealed")).FirstOrDefault();
            _cameraFogVisible = cameras.Where(x => x.name.Contains("Visible")).FirstOrDefault();
        }
        #endregion

        #region Properties Setter
        void SetCameraLength(Camera camera, float length)
        {
            camera.orthographicSize = length / 2;

            Vector3 position = camera.transform.position;

            position.x = length / 2;
            // keep Y component
            position.z = length / 2;

            camera.transform.position = position;
        }

        void SetProjectorLength(Projector projector, float length)
        {
            projector.orthographicSize = length / 2;

            Vector3 position = projector.transform.position;

            position.x = length / 2;
            // keep Y component
            position.z = length / 2;

            projector.transform.position = position;
        }
        #endregion

        #region Drawing
        void DrawProperty<T>(string prefix, ref T field) where T : Object
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(prefix);
                field = (T)EditorGUILayout.ObjectField(field, typeof(T), true);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawMapSizeProperty()
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel("Map Length");
                _mapSize = EditorGUILayout.IntField(_mapSize);
            }
            EditorGUILayout.EndHorizontal();
        }
        #endregion
        #endregion
    }
}
