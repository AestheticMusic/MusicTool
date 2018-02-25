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
    public Text modeCategoryToggleText;


    private MusicManager musicManager;
    private MakeManager makeManager;
    private NoteManager noteManager;
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

    public UIMusicMaker musicMaker;
    public UICameraMaker camMaker;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        makeManager = MakeManager.instance;
        musicManager = MusicManager.instance;
        noteManager = NoteManager.instance;

        musicBar.minValue = 0f;
        OnNoteModeChanged(MakeManager.NMODE_SELECT);

        noteSyncText.text = makeManager.noteSync.ToString();
        bpmText.text = makeManager.bpm.ToString();
    }

    private void Update()
    {
        if (makeManager.playMode)
        {
            ChangeMusicBarIgnored(makeManager.time);
        }
        timeText.text = ((int)makeManager.time / 60).ToString("D02") + ":" +
                        ((int)makeManager.time % 60).ToString("D02");
    }

    public void OnMusicPlay()
    {
        if (makeManager.playMode)
            return;

        musicManager.musicTime = makeManager.time;
        musicManager.Play();
        makeManager.playMode = true;
        makeManager.makerCursorEnabled = false;
    }

    public void OnMusicPause()
    {
        if (!makeManager.playMode)
            return;

        musicManager.Pause();
        makeManager.playMode = false;
        makeManager.makerCursorEnabled = true;
    }

    public void OnMusicStop()
    {
        if (!makeManager.playMode)
            return;

        musicManager.Stop();
        musicManager.musicTime = 0f;
        makeManager.time = 0f;
        makeManager.playMode = false;
        ChangeMusicBarIgnored(0f);
        makeManager.makerCursorEnabled = true;
    }

    public void OnMusicBarChanged()
    {
        if (musicBarIgnoreChange)
            return;

        musicManager.musicTime = musicBar.value;
        makeManager.time = musicBar.value;
    }

    public void OnPitchBarChanged()
    {
        float pitch = pitchBar.value * 0.1f;
        musicManager.ChangePitch(pitch);
        pitchText.text = "Pitch : " + pitch.ToString("F01");
    }

    public void OnSpeedBarChanged()
    {
        float speed = speedBar.value * 0.5f;
        makeManager.speed = speed;
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

    public void OnNoteCategoryModeChanged(bool _isCameraMode)
    {
        makeManager.isCameraNoteMode = _isCameraMode;

        if (_isCameraMode)
        {
            modeCategoryToggleText.text = "Music";
            camMaker.makerControl.SetActive(true);
            musicMaker.makerControl.SetActive(false);
        }
        else
        {
            modeCategoryToggleText.text = "Camera";
            camMaker.makerControl.SetActive(false);
            musicMaker.makerControl.SetActive(true);
        }

        OnNoteModeChanged(MakeManager.NMODE_SELECT);
    }

    public void OnNoteModeChanged(int _mode)
    {
        makeManager.noteMode = _mode;
        makeManager.makerCursor.SetNormalCursor();
        makerMessageText.text = makerMessages[_mode + 2];
    }

    public void OnSaveButton()
    {
        noteManager.SaveNoteData();
    }

    public void OnGoStartSceneButton()
    {
        makeManager.GoStartScene();
    }

    public void OnNoteSyncChanged(string _sync)
    {
        makeManager.noteSync = float.Parse(_sync);
    }

    public void OnBPMChanged(string _bpm)
    {
        makeManager.bpm = float.Parse(_bpm);
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
                makeManager.scrollSpeed = 1f / 1f;
                break;
            case 1:
                makeManager.scrollSpeed = 1f / 2f;
                break;
            case 2:
                makeManager.scrollSpeed = 1f / 3f;
                break;
            case 3:
                makeManager.scrollSpeed = 1f / 4f;
                break;
            case 4:
                makeManager.scrollSpeed = 1f / 6f;
                break;
            case 5:
                makeManager.scrollSpeed = 1f / 8f;
                break;
            case 6:
                makeManager.scrollSpeed = 1f / 12f;
                break;
            case 7:
                makeManager.scrollSpeed = 1f / 16f;
                break;
            case 8:
                makeManager.scrollSpeed = 1f / 24f;
                break;
            case 9:
                makeManager.scrollSpeed = 1f / 32f;
                break;
        }
    }

    public void OnGridChanged(int _value)
    {
        switch (_value)
        {
            case 0:
                makeManager.beatRate = 1f / 1f;
                break;
            case 1:
                makeManager.beatRate = 1f / 2f;
                break;
            case 2:
                makeManager.beatRate = 1f / 3f;
                break;
            case 3:
                makeManager.beatRate = 1f / 4f;
                break;
            case 4:
                makeManager.beatRate = 1f / 6f;
                break;
            case 5:
                makeManager.beatRate = 1f / 8f;
                break;
            case 6:
                makeManager.beatRate = 1f / 12f;
                break;
            case 7:
                makeManager.beatRate = 1f / 16f;
                break;
            case 8:
                makeManager.beatRate = 1f / 24f;
                break;
            case 9:
                makeManager.beatRate = 1f / 32f;
                break;
        }
        makeManager.ChangeGridSprite(_value);
    }
}
