using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNote : Note
{
	private const float bodyLength = 0.475f;

	public Transform body;
	public Transform end;
	public float endTime
	{
		get
		{
			return _endTime;
		}
		set
		{
			_endTime = value;
			length = endTime - time;
		}
	}

	private float _endTime = 0f;
	private float length = 0f;
	private float endTimeInterval = 0f;
	private float longProgress = 0f;
	private Vector3 bodyScale = Vector3.one;
	private Vector3 endPos = Vector3.zero;
	private float noteScale = 1.5f;
	private MakeManager g;
	private NoteManager n;



	private void Awake()
	{
		g = MakeManager.instance;
		n = NoteManager.instance;
		noteScale = this.transform.localScale.x;
    }

	public override void Start()
	{
		base.Start();
		endTime = data.longEndTime;
	}

	protected override void Update()
	{
		UpdateLong();
		base.Update();
    }

	private void UpdateLong()
	{
		endTimeInterval = _endTime - g.syncedTime;
		longProgress = 1f - endTimeInterval / length;

		if (g.syncedTime >= time)
			notePos.x = 0f;

		endPos.x = (n.TimeToLineLength(endTimeInterval) - notePos.x) / noteScale;
		end.localPosition = endPos;

		bodyScale.x = endPos.x / -bodyLength;
		body.localScale = bodyScale;
    }
}
