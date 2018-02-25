using UnityEngine;

[System.Serializable]
public class NoteMusicData : NoteData
{
	private const float batterMinHitInterval = 0.125f;
    
	// Long
	public float longEndTime = 0f;

	// Drag
	public int drag = 0;

	// Batter
	public float batterEndTime = 0f;
	public int batterHit = 0;
	
	public int noteType
	{
		get
		{
            if (batterHit > 0)
				return NoteMusic.N_BATTER;
			if (drag != 0)
				return NoteMusic.N_DRAG;
			if (longEndTime > 0f)
				return NoteMusic.N_LONG;
			return NoteMusic.N_NORMAL;
		}
	}

    public NoteMusicData()
    {
        type = NoteType.Music;
    }

    public void Set(NoteMusicData _data)
	{
		this.time = _data.time;
		this.lineNum = _data.lineNum;
		this.longEndTime = _data.longEndTime;
		this.drag = _data.drag;
		this.batterEndTime = _data.batterEndTime;
		this.batterHit = _data.batterHit;
    }

	public override string GetNoteDataString()
	{
		switch (noteType)
		{
			case NoteMusic.N_NORMAL:
				return "NormalNote:" + lineNum + ":" + Mathf.Floor(time * 1000f);
			case NoteMusic.N_LONG:
				return "LongNote:" + lineNum + ":" + Mathf.Floor(time * 1000f) + ":" + Mathf.Floor(longEndTime * 1000f);
			case NoteMusic.N_DRAG:
				return "DragNote:" + lineNum + ":" + Mathf.Floor(time * 1000f) + ":" + drag;
			case NoteMusic.N_BATTER:
				return "BatterNote:" + batterHit + ":" + Mathf.Floor(time * 1000f) + ":" + Mathf.Floor(batterEndTime * 1000f);
		}
		return "";
	}

	public int GetMaxBatterHit()
	{
		return Mathf.Clamp((int)((batterEndTime - time - BatterNote.appearTime) / batterMinHitInterval), 2, 9999);
    }

	public NoteMusicData GetClone()
	{
		return (NoteMusicData)this.MemberwiseClone();
	}
}