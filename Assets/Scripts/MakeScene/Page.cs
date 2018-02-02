using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Page : MonoBehaviour
{
	public TextMesh pageNumberText;

	[HideInInspector]
	public SpriteRenderer spriteRenderer;



	private void Awake()
	{
		spriteRenderer = this.GetComponent<SpriteRenderer>();
    }

	private void Start()
	{

	}

	private void Update()
	{
		if (this.transform.localPosition.y < -0.1f)
			pageNumberText.gameObject.SetActive(false);
		else
			pageNumberText.gameObject.SetActive(true);
	}

	public void SetPageNumber(int _num)
	{
		pageNumberText.text = _num.ToString();
	}
}
