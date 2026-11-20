
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;
using System.Text;

public class AnimatorControllerExporter : EditorWindow
{
    private AnimatorController controller;

    [MenuItem("Tools/Animator Controller Exporter")]
    public static void OpenWindow()
    {
        GetWindow<AnimatorControllerExporter>("Animator Exporter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Animator Controller Exporter",
            EditorStyles.boldLabel);

        controller = (AnimatorController)EditorGUILayout.ObjectField(
            "Animator Controller",
            controller,
            typeof(AnimatorController),
            false
        );

        if (GUILayout.Button("Export C# Report") && controller != null)
        {
            ExportReport();
        }
    }

    private void ExportReport()
    {
        StringBuilder report = new StringBuilder();

        report.AppendLine("// ANIMATOR CONTROLLER REPORT");
        report.AppendLine("// Generated from: " + controller.name);

        report.AppendLine("\n// PARAMETERS");

        foreach (var parameter in controller.parameters)
        {
            report.AppendLine(
                parameter.name + " : " + parameter.type
            );
        }

        foreach (var layer in controller.layers)
        {
            report.AppendLine("\n// LAYER: " + layer.name);

            ExportStateMachine(layer.stateMachine, report, "");
        }

        string path = Path.Combine(
            Application.dataPath,
            "AnimatorControllerReport.cs"
        );

        File.WriteAllText(path, report.ToString());

        AssetDatabase.Refresh();

        EditorUtility.RevealInFinder(path);

        Debug.Log("Animator report exported: " + path);
    }

    private void ExportStateMachine(
        AnimatorStateMachine machine,
        StringBuilder report,
        string indent)
    {
        foreach (var state in machine.states)
        {
            report.AppendLine(
                indent + "STATE: " + state.state.name
            );

            foreach (var transition in state.state.transitions)
            {
                string destination = transition.destinationState != null
                    ? transition.destinationState.name
                    : transition.destinationStateMachine != null
                        ? transition.destinationStateMachine.name
                        : "Exit";

                report.AppendLine(
                    indent + "  TRANSITION TO: " + destination
                );

                report.AppendLine(
                    indent + "  Duration: " + transition.duration
                );

                report.AppendLine(
                    indent + "  Has Exit Time: " + transition.hasExitTime
                );

                foreach (var condition in transition.conditions)
                {
                    report.AppendLine(
                        indent + "  CONDITION: " +
                        condition.parameter + " " +
                        condition.mode + " " +
                        condition.threshold
                    );
                }
            }
        }

        foreach (var child in machine.stateMachines)
        {
            report.AppendLine(
                indent + "SUB STATE MACHINE: " + child.stateMachine.name
            );

            ExportStateMachine(
                child.stateMachine,
                report,
                indent + "    "
            );
        }
    }
}
