using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraData
{
	public Vector2 startPos = Vector2.zero;
	public Vector2 endPos = Vector2.zero;
	public int curvePos = 0;
	public float startZoom = 3f;
	public float endZoom = 3f;
	public int curveZoom = 0;
	public float startRot = 0f;
	public float endRot = 0f;
	public int curveRot = 0;

	public float time = 0f;
	public float endTime = 0f;
	public float length
	{
		get
		{
			return endTime - time;
		}
	}
}
