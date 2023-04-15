using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class LogPreferencesConfigs
{
	const string FIRST_TIME = "ENABLE_LOGGING_FIRST_TIME";
	const string LOG_DEFINE_SYMBOL = "USE_LOG";

	static bool initialized;
	public static bool enableLogging = true;

	public static readonly string[] symbols = new string[]
	{
		LOG_DEFINE_SYMBOL
	};

	[InitializeOnLoadMethod]
	static void OnProjectLoadedInEditorFirstTime()
	{
		if (EditorPrefs.GetBool(FIRST_TIME, true))
		{
			AddDefineSymbols(symbols);
			EditorPrefs.SetBool(FIRST_TIME, false);
			Debug.Log("Log is enable for debug, you can turn off in \"Preferences\"");
		}

		Log.SetUseDarkTheme(EditorGUIUtility.isProSkin);
	}

	[PreferenceItem("Logging")]
	static void PreferencesGUI()
	{
		// Load the preferences
		if (initialized == false)
		{
			ReadInitialSettings();
			initialized = true;
		}

		GUILayout.Space(10);
		GUILayout.Label("To remove debug logging when profiling development\nbuilds turn off \"Enable Logging\".");
		//		GUILayout.Label ("Debugging is automatically turned off in\nnon-development builds");
		GUILayout.Label("Debugging is not use for development builds");

		GUILayout.Space(10);
		GUI.enabled = !EditorApplication.isCompiling;

		enableLogging = EditorGUILayout.Toggle("Enable Logging", enableLogging);

		GUI.enabled = true;

		if (EditorApplication.isCompiling == true)
		{
			GUILayout.Space(10);
			GUILayout.Label("Please wait... Editor is currently setting define.");
		}

		if (GUI.changed == true)
		{
			UpdateDebugSettings();
		}
	}

	static bool HasDefineSymbol()
	{
		// Get defines.
		BuildTargetGroup buildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
		string define = PlayerSettings.GetScriptingDefineSymbolsForGroup(buildTargetGroup);

		return define.Contains(LOG_DEFINE_SYMBOL);
	}

	static void ReadInitialSettings()
	{
		if (HasDefineSymbol())
			enableLogging = true;
		else
			enableLogging = false;
	}

	static void UpdateDebugSettings()
	{
		if (enableLogging)
		{
			AddDefineSymbols(symbols);
		}
		else
		{
			RemoveDefineSymbols(symbols);
		}
	}

	/// <summary>
	/// Add define symbols as soon as Unity gets done compiling.
	/// </summary>
	static void AddDefineSymbols(string[] defineSymbols)
	{
		string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
		List<string> allDefines = definesString.Split(';').ToList();
		allDefines.AddRange(defineSymbols.Except(allDefines));
		PlayerSettings.SetScriptingDefineSymbolsForGroup(
			EditorUserBuildSettings.selectedBuildTargetGroup,
			string.Join(";", allDefines.ToArray()));
	}

	static void RemoveDefineSymbols(string[] defineSymbols)
	{
		string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);
		List<string> allDefines = definesString.Split(';').ToList();

		for (int i = allDefines.Count - 1; i >= 0; i--)
		{
			for (int j = 0; j < defineSymbols.Length; j++)
			{
				if (allDefines[i] == defineSymbols[j])
					allDefines.RemoveAt(i);
			}
		}

		PlayerSettings.SetScriptingDefineSymbolsForGroup(
			EditorUserBuildSettings.selectedBuildTargetGroup,
			string.Join(";", allDefines.ToArray()));
	}
}