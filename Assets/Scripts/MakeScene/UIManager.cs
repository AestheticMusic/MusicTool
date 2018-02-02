using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
	public static UIManager instance;

	public Slider musicBar;
	public Text timeText;
	public Slider pitchBar;
	public Text pitchText;
	public Slider speedBar;
	public Text speedText;
	public Text makerMessageText;
	public InputField noteSyncText;
	public InputField bpmText;
	public GameObject batterSetGroup;
	public InputField batterHitText;
	public Text batterHitDesc;
	public GameObject hotkeyHelp;
	public GameObject makerControl;

	private MusicManager m;
	private MakeManager g;
	private NoteManager n;
	private bool musicBarIgnoreChange = false;
	private string[] makerMessages =
	{
		"Click :\n노트 선택\n선택된 노트를 삭제",
		"Click :\n노트 선택",
		"Click :\n노말노트 배치",
		"Click + Drag :\n롱노트 배치",
		"Click :\n드래그노트 배치\nShift + Click :\n드래그노트 반전 배치",
		"Click + Drag :\n연타노트 배치\n\n(연타노트 위에서)\nShift + Click :\n연타횟수 설정",
	};



	private void Awake()
	{
		instance = this;
	}

	private void Start()
	{
		g = MakeManager.instance;
		m = MusicManager.instance;
		n = NoteManager.instance;

		musicBar.minValue = 0f;
		OnNoteModeChanged(MakeManager.NMODE_SELECT);

		noteSyncText.text = g.noteSync.ToString();
		bpmText.text = g.bpm.ToString();
    }

	private void Update()
	{
		if (g.playMode)
		{
			ChangeMusicBarIgnored(g.time);
        }
		timeText.text = ((int)g.time / 60).ToString("D02") + ":" + 
						((int)g.time % 60).ToString("D02");
	}

	public void OnMusicPlay()
	{
		if (g.playMode)
			return;

		m.musicTime = g.time;
		m.Play();
		g.playMode = true;
		g.makerCursorEnabled = false;
	}

	public void OnMusicPause()
	{
		if (!g.playMode)
			return;

		m.Pause();
		g.playMode = false;
		g.makerCursorEnabled = true;
	}

	public void OnMusicStop()
	{
		if (!g.playMode)
			return;

		m.Stop();
		m.musicTime = 0f;
		g.time = 0f;
		g.playMode = false;
		ChangeMusicBarIgnored(0f);
		g.makerCursorEnabled = true;
	}

	public void OnMusicBarChanged()
	{
		if (musicBarIgnoreChange)
			return;
		
		m.musicTime = musicBar.value;
		g.time = musicBar.value;
	}

	public void OnPitchBarChanged()
	{
		float pitch = pitchBar.value * 0.1f;
		m.ChangePitch(pitch);
		pitchText.text = "Pitch : " + pitch.ToString("F01");
	}

	public void OnSpeedBarChanged()
	{
		float speed = speedBar.value * 0.5f;
		g.speed = speed;
		speedText.text = "GameSpeed : " + speed.ToString("F01");
	}

	public void ChangeMusicBar(float _time)
	{
		musicBar.value = _time;
	}

	public void ChangeMusicBarIgnored(float _time)
	{
		musicBarIgnoreChange = true;
		musicBar.value = _time;
		musicBarIgnoreChange = false;
	}

	public void OnNoteModeChanged(int _mode)
	{
		g.noteMode = _mode;
		g.makerCursor.SetNormalCursor();
		makerMessageText.text = makerMessages[_mode + 2];
        switch (_mode)
		{
			case MakeManager.NMODE_NORMAL:
				g.makerCursor.SetColor(MakerNote.C_NORMAL);
				break;
			case MakeManager.NMODE_LONG:
				g.makerCursor.SetColor(MakerNote.C_LONG);
				g.makerCursor.SetBodyColor(MakerNoteLong.C_LONG_BODY);
				break;
			case MakeManager.NMODE_DRAG:
				g.makerCursor.SetColor(MakerNote.C_DRAG);
				break;
			case MakeManager.NMODE_BATTER:
				g.makerCursor.SetColor(MakerNote.C_BATTER);
				g.makerCursor.SetBodyColor(MakerNoteLong.C_BATTER_BODY);
				break;
		}
	}

	public void OnSaveButton()
	{
		n.SaveNoteData();
	}

	public void OnGoStartSceneButton()
	{
		g.GoStartScene();
	}

	public void OnNoteSyncChanged(string _sync)
	{
		g.noteSync = float.Parse(_sync);
	}

	public void OnBPMChanged(string _bpm)
	{
		g.bpm = float.Parse(_bpm);
	}

	public void OnSetBatterHit()
	{
		int hit = int.Parse(batterHitText.text);
		if (hit < 2 || hit > g.selectedBatter.GetMaxBatterHit())
		{
			batterHitDesc.color = Color.red;
			return;
		}

		g.selectedBatter.batterHit = hit;
		CloseBatterHitSet();
    }

	public void OpenBatterHitSet()
	{
		g.hotkeyEnabled = false;
		batterSetGroup.SetActive(true);
		batterHitDesc.color = Color.black;
		batterHitDesc.text = "2 ~ " + g.selectedBatter.GetMaxBatterHit();
		batterHitText.text = g.selectedBatter.batterHit.ToString();
	}

	public void CloseBatterHitSet()
	{
		g.hotkeyEnabled = true;
		batterSetGroup.SetActive(false);
	}

	public void OnToggleHotkeyHelp()
	{
		hotkeyHelp.SetActive(!hotkeyHelp.activeSelf);
	}

	public void OnToggleMakerControl()
	{
		makerControl.SetActive(!makerControl.activeSelf);
    }

	public void OnScrollSpeedChanged(int _value)
	{
		switch (_value)
		{
			case 0:
				g.scrollSpeed = 1f / 1f;
				break;
			case 1:
				g.scrollSpeed = 1f / 2f;
				break;
			case 2:
				g.scrollSpeed = 1f / 3f;
				break;
			case 3:
				g.scrollSpeed = 1f / 4f;
				break;
			case 4:
				g.scrollSpeed = 1f / 6f;
				break;
			case 5:
				g.scrollSpeed = 1f / 8f;
				break;
			case 6:
				g.scrollSpeed = 1f / 12f;
				break;
			case 7:
				g.scrollSpeed = 1f / 16f;
				break;
			case 8:
				g.scrollSpeed = 1f / 24f;
				break;
			case 9:
				g.scrollSpeed = 1f / 32f;
				break;
		}
	}

	public void OnGridChanged(int _value)
	{
		switch(_value)
        {
			case 0:
				g.beatRate = 1f / 1f;
			break;
			case 1:
				g.beatRate = 1f / 2f;
			break;
			case 2:
				g.beatRate = 1f / 3f;
			break;
			case 3:
				g.beatRate = 1f / 4f;
			break;
			case 4:
				g.beatRate = 1f / 6f;
			break;
			case 5:
				g.beatRate = 1f / 8f;
			break;
			case 6:
				g.beatRate = 1f / 12f;
			break;
			case 7:
				g.beatRate = 1f / 16f;
			break;
			case 8:
				g.beatRate = 1f / 24f;
			break;
			case 9:
				g.beatRate = 1f / 32f;
			break;
		}
		g.ChangeGridSprite(_value);
	}
}
