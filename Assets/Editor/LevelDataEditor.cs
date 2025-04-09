// using GameData;
// using UnityEditor;
//
//
// [CustomEditor(typeof(LevelData))]
// public class LevelDataEditor: Editor
// {
//     public override void OnInspectorGUI()
//     {
//         LevelData data = (LevelData)target;
//         EditorGUILayout.LabelField("关卡数据","123");
//         data.index = EditorGUILayout.IntField("奖励类型", data.index);
//         data.winAwardType = (WinAwardType)EditorGUILayout.EnumPopup("奖励类型", data.winAwardType);
//     }
// }