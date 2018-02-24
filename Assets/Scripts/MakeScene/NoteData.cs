using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteData {

    // Common
    public float time = 0f;
    public int lineNum = 0;

    public NoteType type;

    public virtual string GetNoteDataString() {
        return "";
    }
}


public enum NoteType {
    Music,
    Camera
}