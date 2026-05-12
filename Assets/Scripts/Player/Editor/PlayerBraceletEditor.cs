using UnityEditor;

namespace Assets.Scripts.Player
{
    [CustomEditor(typeof(PlayerBracelet))]
    [CanEditMultipleObjects]
    public class PlayerBraceletEditor : Editor
    {
        private PlayerBracelet subject;

        private SerializedProperty mainButton;
        private SerializedProperty watch;
        private SerializedProperty menu;
        private SerializedProperty braceletType;
        private SerializedProperty time;
        private SerializedProperty day;
        private SerializedProperty data;

        void OnEnable()
        {
            subject = target as PlayerBracelet;

            menu = serializedObject.FindProperty("_menu");
            braceletType = serializedObject.FindProperty("_braceletType");
            mainButton = serializedObject.FindProperty("MainButton");
            watch = serializedObject.FindProperty("_watch");
            time = serializedObject.FindProperty("_time");
            day = serializedObject.FindProperty("_day");
            data = serializedObject.FindProperty("_data");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(braceletType);

            if (subject.BraceletType == BraceletType.Menu)
            {
                EditorGUILayout.PropertyField(mainButton);
                EditorGUILayout.PropertyField(menu);
            }
            else
            {
                EditorGUILayout.PropertyField(watch);
                EditorGUILayout.PropertyField(time);
                EditorGUILayout.PropertyField(data);
                EditorGUILayout.PropertyField(day);
            }

            serializedObject.ApplyModifiedProperties();

            DrawDefaultInspector();
        }
    }
}
