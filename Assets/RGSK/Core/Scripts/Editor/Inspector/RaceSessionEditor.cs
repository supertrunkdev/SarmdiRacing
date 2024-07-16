using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using RGSK.Helpers;

namespace RGSK.Editor
{
    [CustomEditor(typeof(RaceSession))]
    public class RaceSessionEditor : UnityEditor.Editor
    {
        RaceSession _target;

        SerializedProperty sessionType;
        SerializedProperty raceType;
        SerializedProperty startMode;
        SerializedProperty lapCount;
        SerializedProperty opponentCount;
        SerializedProperty playerGridStartMode;
        SerializedProperty playerStartPosition;
        SerializedProperty sessionTimeLimit;
        SerializedProperty enableGhost;
        SerializedProperty disableCollision;
        SerializedProperty targetScores;
        SerializedProperty entrants;
        SerializedProperty opponentVehicleClass;
        SerializedProperty autoPopulateOpponents;
        SerializedProperty opponentClass;
        SerializedProperty raceRewards;
        SerializedProperty track;

        SerializedProperty id;
        SerializedProperty objectName;
        SerializedProperty description;
        SerializedProperty icon;
        SerializedProperty previewPhoto;
        SerializedProperty unlockedByDefault;

        SerializedProperty saveRecords;

        List<string> aiDifficultyList = new List<string>();

        string[] tabs = new string[]
        {
            "Settings",
            "Info"
        };

        static int tabIndex;

        void OnEnable()
        {
            _target = (RaceSession)target;

            sessionType = serializedObject.FindProperty(nameof(sessionType));
            raceType = serializedObject.FindProperty(nameof(raceType));
            startMode = serializedObject.FindProperty(nameof(startMode));
            lapCount = serializedObject.FindProperty(nameof(lapCount));
            opponentCount = serializedObject.FindProperty(nameof(opponentCount));
            playerGridStartMode = serializedObject.FindProperty(nameof(playerGridStartMode));
            playerStartPosition = serializedObject.FindProperty(nameof(playerStartPosition));
            sessionTimeLimit = serializedObject.FindProperty(nameof(sessionTimeLimit));
            enableGhost = serializedObject.FindProperty(nameof(enableGhost));
            disableCollision = serializedObject.FindProperty(nameof(disableCollision));
            targetScores = serializedObject.FindProperty(nameof(targetScores));
            entrants = serializedObject.FindProperty(nameof(entrants));
            opponentVehicleClass = serializedObject.FindProperty(nameof(opponentVehicleClass));
            autoPopulateOpponents = serializedObject.FindProperty(nameof(autoPopulateOpponents));
            opponentClass = serializedObject.FindProperty(nameof(opponentClass));
            raceRewards = serializedObject.FindProperty(nameof(raceRewards));
            track = serializedObject.FindProperty(nameof(track));

            description = serializedObject.FindProperty(nameof(description));
            objectName = serializedObject.FindProperty(nameof(objectName));
            icon = serializedObject.FindProperty(nameof(icon));
            previewPhoto = serializedObject.FindProperty(nameof(previewPhoto));
            saveRecords = serializedObject.FindProperty(nameof(saveRecords));
            id = serializedObject.FindProperty(nameof(id));
            unlockedByDefault = serializedObject.FindProperty(nameof(unlockedByDefault));

            RGSKCore.Instance.AISettings.difficulties.ForEach(x => aiDifficultyList.Add(string.IsNullOrWhiteSpace(x.displayName) ? x.name : x.displayName));
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            tabIndex = GUILayout.Toolbar(tabIndex, tabs);

            switch (tabs[tabIndex].ToLower())
            {
                case "settings":
                    {
                        GUILayout.Label("Entrants", EditorStyles.boldLabel);
                        using (new GUILayout.VerticalScope("Box"))
                        {
                            if (_target.entrants.Count(x => x.isPlayer) > 1)
                            {
                                EditorGUILayout.HelpBox("More than 1 player is assigned! This is will result in undesired behavior.", MessageType.Error);
                            }

                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(entrants, true);
                            EditorGUI.indentLevel--;

                            EditorGUILayout.PropertyField(autoPopulateOpponents);

                            if (_target.autoPopulateOpponents)
                            {
                                EditorGUILayout.PropertyField(opponentCount);
                                EditorGUILayout.PropertyField(opponentClass);

                                if (_target.opponentClass == OpponentClassOptions.Selected)
                                {
                                    EditorGUILayout.PropertyField(opponentVehicleClass);
                                }
                            }
                        }

                        using (new GUILayout.VerticalScope("Box"))
                        {
                            GUILayout.Label("Race Settings", EditorStyles.boldLabel);
                            EditorGUILayout.PropertyField(raceType);
                            EditorGUILayout.PropertyField(sessionType);
                            EditorGUILayout.PropertyField(startMode);
                            _target.opponentDifficulty = EditorGUILayout.Popup("AI Difficulty", _target.opponentDifficulty, aiDifficultyList.ToArray());

                            EditorGUILayout.PropertyField(playerGridStartMode);
                            EditorGUI.BeginDisabledGroup(_target.playerGridStartMode != SelectionMode.Selected);
                            {
                                EditorGUILayout.PropertyField(playerStartPosition);
                                EditorGUI.EndDisabledGroup();
                            }

                            var hideLaps = false;

                            if (_target.raceType != null)
                            {
                                hideLaps = _target.IsInfiniteLaps() || !EnumFlags.GetSelectedIndexes(_target.raceType.allowedTrackLayouts).
                                Contains((int)TrackLayoutType.Circuit);
                            }

                            EditorGUI.BeginDisabledGroup(hideLaps);
                            {
                                EditorGUILayout.PropertyField(lapCount);
                                EditorGUI.EndDisabledGroup();
                            }

                            EditorGUI.BeginDisabledGroup(!_target.IsTimedSession());
                            {
                                EditorGUILayout.PropertyField(sessionTimeLimit);
                                EditorGUI.EndDisabledGroup();
                            }

                            EditorGUI.BeginDisabledGroup(!_target.UseGhostVehicle());
                            {
                                EditorGUILayout.PropertyField(enableGhost);
                                EditorGUI.EndDisabledGroup();
                            }

                            EditorGUILayout.PropertyField(disableCollision);
                        }

                        using (new GUILayout.VerticalScope("Box"))
                        {
                            GUILayout.Label("Targets", EditorStyles.boldLabel);

                            if (!_target.UseTargetScores())
                            {
                                EditorGUILayout.HelpBox("Target scores only apply for the \"Target Score\" session type.", MessageType.Info);
                            }

                            EditorGUI.BeginDisabledGroup(!_target.UseTargetScores());
                            {
                                EditorGUI.indentLevel++;
                                EditorGUILayout.PropertyField(targetScores, true);
                                EditorGUI.indentLevel--;
                                EditorGUI.EndDisabledGroup();
                            }
                        }

                        using (new GUILayout.VerticalScope("Box"))
                        {
                            GUILayout.Label("Rewards", EditorStyles.boldLabel);
                            EditorGUI.indentLevel++;
                            EditorGUILayout.PropertyField(raceRewards);
                            EditorGUI.indentLevel--;
                        }

                        break;
                    }

                case "info":
                    {
                        using (new GUILayout.VerticalScope("Box"))
                        {
                            EditorGUILayout.PropertyField(id);
                            EditorGUILayout.PropertyField(objectName);
                            EditorGUILayout.PropertyField(description);
                            EditorGUILayout.PropertyField(icon);
                            EditorGUILayout.PropertyField(previewPhoto);
                            EditorGUILayout.PropertyField(unlockedByDefault);
                        }

                        using (new GUILayout.VerticalScope("Box"))
                        {
                            EditorGUILayout.HelpBox("Only assign if this session is used to load a race event.", MessageType.Info);
                            EditorGUILayout.PropertyField(track);
                        }

                        using (new GUILayout.VerticalScope("Box"))
                        {
                            EditorGUILayout.PropertyField(saveRecords);
                            EditorGUILayout.LabelField($"Best: {UIHelper.FormatOrdinalText(_target.LoadBestPosition())}");
                        }
                        break;
                    }
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}