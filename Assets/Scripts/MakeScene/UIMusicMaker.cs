using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMusicMaker : MonoBehaviour {
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

    private MakeManager makeManager;
    private MusicMakeManager musicMaker;

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

    public UIManager uiManager;
    
    private void Start()
    {
        musicMaker = MusicMakeManager.instance;
        uiManager = UIManager.instance;
        makeManager = MakeManager.instance;
        OnNoteModeChanged(MakeManager.NMODE_SELECT);
    }
    
    public void OnNoteModeChanged(int _mode)
    {
        musicMaker.makerCursor.SetNormalCursor();
        makerMessageText.text = makerMessages[_mode + 2];
        makeManager.noteMode = _mode;
        switch (_mode)
        {
            case MusicMakeManager.NMODE_NORMAL:
                makeManager.makerCursor.SetColor(MakerMusicNote.C_NORMAL);
                break;
            case MusicMakeManager.NMODE_LONG:
                makeManager.makerCursor.SetColor(MakerMusicNote.C_LONG);
                makeManager.makerCursor.SetBodyColor(MakerNoteLong.C_LONG_BODY);
                break;
            case MusicMakeManager.NMODE_DRAG:
                makeManager.makerCursor.SetColor(MakerMusicNote.C_DRAG);
                break;
            case MusicMakeManager.NMODE_BATTER:
                makeManager.makerCursor.SetColor(MakerMusicNote.C_BATTER);
                makeManager.makerCursor.SetBodyColor(MakerNoteLong.C_BATTER_BODY);
                break;
        }
    }
    
    public void OnSetBatterHit()
    {
        int hit = int.Parse(batterHitText.text);
        if (hit < 2 || hit > musicMaker.selectedBatter.GetMaxBatterHit())
        {
            batterHitDesc.color = Color.red;
            return;
        }

        musicMaker.selectedBatter.batterHit = hit;
        CloseBatterHitSet();
    }

    public void OpenBatterHitSet()
    {
        makeManager.hotkeyEnabled = false;
        batterSetGroup.SetActive(true);
        batterHitDesc.color = Color.black;
        batterHitDesc.text = "2 ~ " + musicMaker.selectedBatter.GetMaxBatterHit();
        batterHitText.text = musicMaker.selectedBatter.batterHit.ToString();
    }

    public void CloseBatterHitSet()
    {
        makeManager.hotkeyEnabled = true;
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

    
}
