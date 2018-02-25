using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICameraMaker : MonoBehaviour
{

    public Text makerMessageText;

    public GameObject posSetGroup;
    public InputField[] startPosText, endPosText;

    public GameObject rotSetGroup;
    public InputField startRotText, endRotText;

    public GameObject zoomSetGroup;
    public InputField startZoomText, endZoomText;

    public GameObject hotkeyHelp;
    public GameObject makerControl;

    private MakeManager makeManager;
    private CameraMakeManager camMaker;

    private bool musicBarIgnoreChange = false;
    private string[] makerMessages =
    {
        "Click :\n노트 선택\n선택된 노트를 삭제",
        "Click :\n노트 선택",
        "Click + Drag :\n이동노트 배치\n\n(노트 위에서)\nShift + Click :\n범위 설정",
        "Click + Drag :\n회전노트 배치\n\n(노트 위에서)\nShift + Click :\n범위 설정",
        "Click + Drag :\n줌노트 배치\n\n(노트 위에서)\nShift + Click :\n범위 설정"
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

    public void OnSetPos()
    {
        float startPosX = float.Parse(startPosText[0].text);
        float startPosY = float.Parse(startPosText[1].text);

        float endPosX = float.Parse(endPosText[0].text);
        float endPosY = float.Parse(endPosText[1].text);

        camMaker.selectedData.startPos = new Vector2(startPosX, startPosY);
        camMaker.selectedData.endPos = new Vector2(endPosX, endPosY);

        ClosePos();
    }

    public void OpenPos()
    {
        makeManager.hotkeyEnabled = false;
        camMaker.hotkeyEnabled = false;

        posSetGroup.SetActive(true);

        startPosText[0].text = camMaker.selectedData.startPos.x.ToString();
        startPosText[1].text = camMaker.selectedData.startPos.y.ToString();

        endPosText[0].text = camMaker.selectedData.endPos.x.ToString();
        endPosText[1].text = camMaker.selectedData.endPos.y.ToString();
    }

    public void ClosePos()
    {
        makeManager.hotkeyEnabled = true;
        camMaker.hotkeyEnabled = true;

        posSetGroup.SetActive(false);
    }

    public void OnSetRot()
    {
        float startRot = float.Parse(startRotText.text);

        float endRot = float.Parse(endRotText.text);

        camMaker.selectedData.startRot = startRot;
        camMaker.selectedData.endRot = endRot;

        CloseRot();
    }

    public void OpenRot()
    {
        makeManager.hotkeyEnabled = false;
        camMaker.hotkeyEnabled = false;

        rotSetGroup.SetActive(true);

        startRotText.text = camMaker.selectedData.startRot.ToString();

        endRotText.text = camMaker.selectedData.endRot.ToString();
    }

    public void CloseRot()
    {
        makeManager.hotkeyEnabled = true;
        camMaker.hotkeyEnabled = true;

        rotSetGroup.SetActive(false);
    }

    public void OnSetZoom()
    {
        float startZoom = float.Parse(startZoomText.text);

        float endZoom = float.Parse(endZoomText.text);

        camMaker.selectedData.startZoom = startZoom;
        camMaker.selectedData.endZoom = endZoom;

        CloseZoom();
    }

    public void OpenZoom()
    {
        makeManager.hotkeyEnabled = false;
        camMaker.hotkeyEnabled = false;

        zoomSetGroup.SetActive(true);

        startZoomText.text = camMaker.selectedData.startZoom.ToString();

        endZoomText.text = camMaker.selectedData.endZoom.ToString();
    }

    public void CloseZoom()
    {
        makeManager.hotkeyEnabled = true;
        camMaker.hotkeyEnabled = true;

        zoomSetGroup.SetActive(false);
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
