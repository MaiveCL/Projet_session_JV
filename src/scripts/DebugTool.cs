using Godot;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

public static class DebugTool
{
	private static HashSet<string> _printed = new HashSet<string>();

	public static void LogOnce(string msg,
		[CallerMemberName] string method = "",
		[CallerFilePath] string file = "")
	{
		string className = System.IO.Path.GetFileNameWithoutExtension(file);
		string key = $"LOG:{className}.{method}:{msg}";

		if (_printed.Contains(key)) return;

		_printed.Add(key);
		GD.Print($"[DEBUG] {className}.{method}() → {msg}");
	}

	public static void NodeOnce(string label, Node node,
		[CallerMemberName] string method = "",
		[CallerFilePath] string file = "")
	{
		string className = System.IO.Path.GetFileNameWithoutExtension(file);
		string result = node == null ? "NULL" : $"{node.Name} ({node.GetType().Name})";

		string key = $"NODE:{className}.{method}:{label}={result}";

		if (_printed.Contains(key)) return;

		_printed.Add(key);

		if (node == null)
			GD.PrintErr($"[DEBUG] {className}.{method}() → {label} = NULL");
		else
			GD.Print($"[DEBUG] {className}.{method}() → {label} = {result}");
	}

	// Option: vider le cache si tu veux relancer un cycle propre
	public static void Reset()
	{
		_printed.Clear();
	}
}
