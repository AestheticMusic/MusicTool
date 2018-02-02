using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DragNote : Note
{
	public int dir = 1;

	private SpriteRenderer spriteRenderer;



	private void Awake()
	{
		spriteRenderer = this.GetComponent<SpriteRenderer>();
	}

	public override void Start()
	{
		base.Start();
		dir = data.drag;
		spriteRenderer.flipX = (dir == -1) ? true : false;
	}

	protected override void Update()
	{
		base.Update();
	}
}
