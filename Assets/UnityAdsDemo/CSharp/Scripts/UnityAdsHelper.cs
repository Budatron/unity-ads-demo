﻿// UnityAdsHelper.cs - Written for Unity Ads Asset Store v1.0.4 (SDK 1.3.10)
//  by Nikkolai Davenport <nikkolai@unity3d.com>
//
// Setup Instructions:
// 1. Attach this script to a new game object.
// 2. Enter game IDs into the fields provided.
// 3. Enable Development Build in Build Settings to 
//     enable test mode and show SDK debug levels.
// 
// Usage Guide:
//  Write a script and call UnityAdsHelper.ShowAd() to show an ad. 
//  Customize the HandleShowResults method to perform actions based 
//  on whether an ad was succesfully shown or not.
//
// Notes:
//  - Game IDs by platform are required to initialize Unity Ads.
//  - Test game IDs are optional. If not set while in test mode, 
//     test game IDs will default to platform game IDs.
//  - The various debug levels and test mode are only used when
//     Development Build is enabled in Build Settings.
//  - Test mode can be disabled while Development Build is set
//     by checking the option to disable it in the inspector.
//
// HACK Notes:
//  - Enable usePauseOverride if your game fails to unpause 
//     or gets stuck on a black screen after an ad is closed.

using UnityEngine;
using System.Collections;
#if UNITY_IOS || UNITY_ANDROID
using UnityEngine.Advertisements;
#endif

public class UnityAdsHelper : MonoBehaviour 
{
	[System.Serializable]
	public struct GameInfo
	{  
		[SerializeField]
		private string _gameID;
		[SerializeField]
		private string _testGameID;
		
		public string GetGameID ()
		{
			return Debug.isDebugBuild && !string.IsNullOrEmpty(_testGameID) ? _testGameID : _gameID;
		}
	}
	public GameInfo iOS;
	public GameInfo android;
	public bool disableTestMode;
	public bool usePauseOverride; // HACK: Workaround for pause/resume bug.
	public bool showInfoLogs;
	public bool showDebugLogs;
	public bool showWarningLogs = true;
	public bool showErrorLogs = true;

	protected static bool _isPaused; // HACK: Workaround for pause/resume bug.

#if UNITY_IOS || UNITY_ANDROID
	protected void Awake() 
	{
		string gameID = null;

#if UNITY_IOS
		gameID = iOS.GetGameID();
#elif UNITY_ANDROID
		gameID = android.GetGameID();
#endif

		if (!Advertisement.isSupported) 
		{
			Debug.LogWarning("Unity Ads is not supported on the current platform.");
		}
		else if (string.IsNullOrEmpty(gameID))
		{
			Debug.LogError("A valid game ID is required to initialize Unity Ads.");
		}
		else
		{
			Advertisement.allowPrecache = true;
			
			Advertisement.debugLevel = Advertisement.DebugLevel.NONE;	
			if (showInfoLogs) Advertisement.debugLevel |= Advertisement.DebugLevel.INFO;
			if (showDebugLogs) Advertisement.debugLevel |= Advertisement.DebugLevel.DEBUG;
			if (showWarningLogs) Advertisement.debugLevel |= Advertisement.DebugLevel.WARNING;
			if (showErrorLogs) Advertisement.debugLevel |= Advertisement.DebugLevel.ERROR;
			
			bool enableTestMode = Debug.isDebugBuild && !disableTestMode; 
			Debug.Log(string.Format("Initializing Unity Ads for game ID {0} with test mode {1}...",
			                        gameID, enableTestMode ? "enabled" : "disabled"));
			
			Advertisement.Initialize(gameID,enableTestMode);
		}
	}

	// HACK: Workaround for pause/resume bug. See Hack Notes above for details.
	protected void OnApplicationPause (bool isPaused)
	{
		if (!usePauseOverride || isPaused == _isPaused) return;

		if (isPaused) Debug.Log ("App was paused.");
		else Debug.Log("App was resumed.");

		if (usePauseOverride) PauseOverride(isPaused);
	}

	public static bool isReady (string zone = null)
	{
		if (string.IsNullOrEmpty(zone)) zone = null;
		return Advertisement.isReady(zone);
	}

	public static void ShowAd (string zone = null, bool pauseGameDuringAd = true)
	{
		if (string.IsNullOrEmpty(zone)) zone = null;

		ShowOptions options = new ShowOptions();
		options.pause = pauseGameDuringAd;
		options.resultCallback = HandleShowResult;
		
		Advertisement.Show(zone,options);
	}
	
	public static void HandleShowResult (ShowResult result)
	{
		switch (result)
		{
		case ShowResult.Finished:
			Debug.Log("The ad was successfully shown.");
			break;
		case ShowResult.Skipped:
			Debug.Log("The ad was skipped before reaching the end.");
			break;
		case ShowResult.Failed:
			Debug.LogError("The ad failed to be shown.");
			break;
		}
	}

	// HACK: Workaround for pause/resume bug. See Hack Notes above for details.
	public static void PauseOverride (bool pause)
	{
		if (pause) Debug.Log("Pause game while ad is shown.");
		else Debug.Log("Resume game after ad is closed.");
		
		AudioListener.volume = pause ? 0f : 1f;
		Time.timeScale = pause ? 0f : 1f;

		_isPaused = pause;
	}
#else
	protected void Awake ()
	{
		Debug.Log("Unity Ads is not supported on the current platform.");
	}
#endif
}
