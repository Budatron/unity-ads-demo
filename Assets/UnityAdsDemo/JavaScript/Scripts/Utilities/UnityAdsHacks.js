﻿// UnityAdsHacks.js - Written for Unity Ads Asset Store v1.0.4 (SDK 1.3.10)
//  by Nikkolai Davenport <nikkolai@unity3d.com>
//
// HACK 1: Unity Ads fails to call ResumeGame when an ad is closed, resulting in the 
//          game remaining indefinitely in a pause state or being stuck on a black screen.
//       - The UnityAds.PauseGame method sets Time.timeScale and AudioListener.volume to 0.
//       - Set these values back to 1 manually within the OnApplicaitonPause method.
//       - Enable usePauseOverride in the inspector to use this workaround.

#pragma strict

public class UnityAdsHacks extends MonoBehaviour 
{
	public var usePauseOverride : boolean;

	protected static var _isPaused : boolean;
	
	protected function OnApplicationPause (isPaused : boolean) : void
	{
		if (!usePauseOverride || isPaused == _isPaused) return;
		
		if (isPaused) Debug.Log ("App was paused.");
		else Debug.Log("App was resumed.");
		
		if (usePauseOverride) PauseOverride(isPaused);
	}

	public static function PauseOverride (pause : boolean) : void
	{
		if (pause) Debug.Log("Pause game while ad is shown.");
		else Debug.Log("Resume game after ad is closed.");
		
		AudioListener.volume = pause ? 0.0 : 1.0;
		Time.timeScale = pause ? 0.0 : 1.0;
		
		_isPaused = pause;
	}
}
