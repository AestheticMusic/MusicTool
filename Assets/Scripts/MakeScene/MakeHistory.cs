using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeHistory
{
    public const int A_ADDNOTE = 0;
    public const int A_REMOVENOTE = 1;
    public const int A_MODIFYNOTE = 2;

    public int action = A_ADDNOTE;
    public NoteData noteData = null;
    public NoteData beforeData = null;

    public MakeHistory(int _action, NoteData _noteData)
    {
        action = _action;
        noteData = _noteData;
    }

    public MakeHistory(int _action, NoteData _noteData, NoteData _beforeData)
    {
        action = _action;
        noteData = _noteData;
        beforeData = _beforeData;
    }
}

public class MakeHistoryStack
{
    private const int maxStackSize = 30;

    private Stack<MakeHistory> history;
    private NoteManager n;

    public void Initialize()
    {
        history = new Stack<MakeHistory>(maxStackSize);
        n = NoteManager.instance;
    }

    public void PushHistory(int _action, NoteData _noteData)
    {
        MakeHistory h = new MakeHistory(_action, _noteData);
        history.Push(h);
    }

    public void PushHistory(int _action, NoteData _noteData, NoteData _beforeData)
    {
        MakeHistory h = new MakeHistory(_action, _noteData, _beforeData);
        history.Push(h);
    }

    public void UndoHistory()
    {
        if (history.Count == 0)
            return;

        MakeHistory h = history.Pop();

        switch (h.action)
        {
            case MakeHistory.A_ADDNOTE:
                n.RemoveNoteData(h.noteData);
                break;
            case MakeHistory.A_REMOVENOTE:
                n.AddNoteData(h.noteData);
                break;
            case MakeHistory.A_MODIFYNOTE:
                if (h.noteData.type == NoteType.Music)
                {
                    ((NoteMusicData)(h.noteData)).Set((NoteMusicData)h.beforeData);
                }
                else {
                    ((NoteCameraData)(h.noteData)).Set((NoteCameraData)h.beforeData);
                }
                break;
        }
    }
}

