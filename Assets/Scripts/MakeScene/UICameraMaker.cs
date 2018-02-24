using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICameraMaker : MonoBehaviour
{
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
    private CameraMakeManager camMaker;

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
        camMaker = CameraMakeManager.instance;
        uiManager = UIManager.instance;
        makeManager = MakeManager.instance;
        OnNoteModeChanged(MakeManager.NMODE_SELECT);
    }

    public void OnNoteModeChanged(int _mode)
    {
        camMaker.makerCursor.SetNormalCursor();
        makerMessageText.text = makerMessages[_mode + 2];
        makeManager.noteMode = _mode;
        switch (_mode)
        {
            case CameraMakeManager.NMODE_POS:
            case CameraMakeManager.NMODE_ROT:
            case CameraMakeManager.NMODE_ZOOM:
                makeManager.makerCursor.SetColor(MakerMusicNote.C_LONG);
                makeManager.makerCursor.SetBodyColor(MakerNoteLong.C_LONG_BODY);
                break;
        }
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
