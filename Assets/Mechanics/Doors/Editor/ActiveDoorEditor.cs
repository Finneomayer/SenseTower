using Assets.Scripts.Space;
using UnityEditor;

namespace Assets.Mechanics.Doors
{
    [CustomEditor(typeof(ActiveDoor))]
    [CanEditMultipleObjects]
    public class ActiveDoorEditor : Editor
    {
        private ActiveDoor subject;

        private SerializedProperty isPrivate;
        private SerializedProperty spaceType;
        private SerializedProperty doorImageTexture;
        private SerializedProperty doorImage;

        void OnEnable()
        {
            subject = target as ActiveDoor;

            isPrivate = serializedObject.FindProperty("_isPrivate");
            spaceType = serializedObject.FindProperty("_spaceType");
            doorImageTexture = serializedObject.FindProperty("_emptyDoorImageTexture");
            doorImage = serializedObject.FindProperty("_doorImage");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(isPrivate);
            EditorGUILayout.PropertyField(spaceType);

            if (subject.IsPrivate)
            {
                EditorGUILayout.PropertyField(doorImageTexture);
                EditorGUILayout.PropertyField(doorImage);
            }

            serializedObject.ApplyModifiedProperties();

            DrawDefaultInspector();
        }
    }
}
