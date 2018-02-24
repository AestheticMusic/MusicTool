using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
	public static CameraManager instance;

	public Transform limitLT;
	public Transform limitRB;
	public AnimationCurve[] curves;

	private Camera cam;
	private Vector3 camPos;
	private float camZoom;
	private Vector3 camRot = Vector3.zero;
	private float screenRatio = 1f;
	public List<NoteCameraData> datas;
	private MakeManager g;



	private void Awake()
	{
		instance = this;		
	}

	private void Start()
	{
		g = MakeManager.instance;

		cam = Camera.main;
		screenRatio = Screen.width / Screen.height;
	}

	private void Update()
	{
		GetCameraInfo(g.syncedTime, out camPos, out camZoom, out camRot.z);

		float camXSize = camZoom * screenRatio * 0.5f;
		float camYSize = camZoom * 0.5f;
		camPos.x = Mathf.Clamp(camPos.x, limitLT.position.x + camXSize, limitRB.position.x - camXSize);
		camPos.y = Mathf.Clamp(camPos.y, limitRB.position.y + camYSize, limitLT.position.y - camYSize);
		camZoom = Mathf.Clamp(camZoom, 1f, 5.5f);

		cam.transform.position = camPos;
		cam.transform.eulerAngles = camRot;
		cam.orthographicSize = camZoom;
	}

	public void GetCameraInfo(float _time, out Vector3 _pos, out float _zoom, out float _rot)
	{
		NoteCameraData data = GetCameraData(_time);
		if (data == null)
		{
			_pos = Vector3.zero;
			_pos.z = -10f;
			_zoom = 3f;
			_rot = 0f;
			return;
		}

		float t = Mathf.Clamp01((_time - data.time) / data.length);

		_pos = Vector3.Lerp(data.startPos, data.endPos, curves[data.curvePos].Evaluate(t));
		_zoom = Mathf.Lerp(data.startZoom, data.endZoom, curves[data.curveZoom].Evaluate(t));
		_rot = Mathf.Lerp(data.startRot, data.endRot, curves[data.curveRot].Evaluate(t));
		_pos.z = -10f;
	}

	private NoteCameraData GetCameraData(float _time)
	{
		NoteCameraData res = null;
		for (int i = 0; i < datas.Count; ++i)
		{
			NoteCameraData data = datas[i];
			if (data.time <= _time)
			{
				if (res != null && res.time > data.time)
					res = datas[i];
				else if (res == null)
					res = datas[i];
			}
        }
		return res;
	}
}
