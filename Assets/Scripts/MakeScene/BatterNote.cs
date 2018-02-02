using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BatterNote : Note
{
	public const float appearTime = 0.583f;
	public const float disappearTime = 0.667f;

	public float endTime
	{
		get
		{
			return _endTime;
		}
		set
		{
			_endTime = value;
			length = _endTime - time;
		}
	}

	private float _endTime = 0f;
	private float length = 0f;
	private float timeInterval = 0f;
	private Animator animator;
	private MakeManager g;



	private void Awake()
	{
		animator = this.GetComponent<Animator>();
		g = MakeManager.instance;
	}

	public override void Start()
	{
		base.Start();
		endTime = data.batterEndTime;
	}

	protected override void Update()
	{
		base.Update();
	}

	private void LateUpdate()
	{
		UpdateBatter();
    }

	private void UpdateBatter()
	{
		timeInterval = g.syncedTime - time;

		if (timeInterval <= appearTime)
		{
			animator.Play("Appear", -1, Mathf.Clamp01(timeInterval / appearTime));
		}
		else if (timeInterval >= length)
		{
			animator.Play("Disappear", -1, Mathf.Clamp01((timeInterval - length) / disappearTime));
		}
		else
		{
			animator.Play("Idle");
		}
	}
}