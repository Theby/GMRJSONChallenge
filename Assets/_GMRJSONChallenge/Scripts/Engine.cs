using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class Engine : MonoBehaviour
{
	[Header("Elements")]
	[SerializeField] private PanelView panelView;
	[SerializeField] private Button closeButton;

	[Header("Configuration")]
	[SerializeField] private string fileNameAndPath;

	private FileSystemWatcher fileSystemWatcher;
	private bool hasFileChanged;
	private Coroutine loadJSONCoroutine;

	private void Start()
	{
		fileSystemWatcher = null;
		hasFileChanged = false;
		loadJSONCoroutine = null;

		closeButton.onClick.AddListener(CloseApplicationHandler);

		panelView.CleanPanel();

		WatchJSONFile();
		StartLoadJSON();
	}

	private void WatchJSONFile()
	{
		fileSystemWatcher = new FileSystemWatcher();
		fileSystemWatcher.Path = Application.streamingAssetsPath;
		fileSystemWatcher.Filter = fileNameAndPath;
		fileSystemWatcher.Created += NotifyFileUpdatedHandler;
		fileSystemWatcher.Deleted += NotifyFileUpdatedHandler;
		fileSystemWatcher.Changed += NotifyFileUpdatedHandler;
		fileSystemWatcher.Renamed += NotifyFileUpdatedHandler;

		fileSystemWatcher.EnableRaisingEvents = true;
	}

	private void Update()
	{
		if (!hasFileChanged)
			return;

		hasFileChanged = false;

		StopCoroutine(loadJSONCoroutine);
		StartLoadJSON();
	}

	private void StartLoadJSON()
	{
		loadJSONCoroutine = StartCoroutine(LoadJSON());
	}

	private IEnumerator LoadJSON()
	{
		using (var unityWebRequest = UnityWebRequest.Get("file://" + Path.Combine(Application.streamingAssetsPath, fileNameAndPath)))
		{
			yield return unityWebRequest.SendWebRequest();

			PanelData panelData;

			if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
			{
				Debug.Log(unityWebRequest.error);
				panelData = new PanelData("ERROR: File Not Found", new List<string>());
			}
			else
			{
				try
				{
					panelData = PanelData.ParseJSON(unityWebRequest.downloadHandler.text);
				}
				catch (Exception exception)
				{
					Debug.Log(exception.Message);
					panelData = new PanelData("ERROR: Wrong JSON Format", new List<string>());
				}
			}

			panelView.UpdatePanelContent(panelData);
		}
	}

	private void NotifyFileUpdatedHandler(object sender, FileSystemEventArgs eventArgs)
	{
		hasFileChanged = true;
	}

	private void CloseApplicationHandler()
	{
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif
	}
}