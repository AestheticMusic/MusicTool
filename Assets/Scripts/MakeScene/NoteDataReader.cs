using System.Collections.Generic;
using UnityEngine;

public class NoteDataReader
{
    public bool dataReaded = false;
    public List<NoteData> datas;
    public float startBPM = 120f;
    public float noteSync = 0f;

    private int dataVersion = 1;

    public void ReadData(string _ntd)
    {
        MakeManager g = MakeManager.instance;

        string[] lines = _ntd.Split('\n');
        datas = new List<NoteData>();

        NoteData noteData = null;
        NoteMusicData musicData = null;
        NoteCameraData cameraData = null;

        foreach (string line in lines)
        {
            if (string.IsNullOrEmpty(line))
                break;

            string[] words = line.Split(':');
            switch (words[0])
            {
                case "NoteDataVersion":
                    dataVersion = int.Parse(words[1]);
                    break;
                case "StartBPM":
                    startBPM = float.Parse(words[1]);
                    break;
                case "NoteSync":
                    noteSync = int.Parse(words[1]) / 1000f;
                    break;
                case "NormalNote":
                    musicData = new NoteMusicData();
                    musicData.type = NoteType.Music;
                    musicData.lineNum = int.Parse(words[1]);
                    musicData.time = int.Parse(words[2]) / 1000f;
                    noteData = musicData;
                    break;
                case "LongNote":
                    musicData = new NoteMusicData();
                    musicData.type = NoteType.Music;
                    musicData.lineNum = int.Parse(words[1]);
                    musicData.time = int.Parse(words[2]) / 1000f;
                    musicData.longEndTime = int.Parse(words[3]) / 1000f;
                    noteData = musicData;
                    break;
                case "DragNote":
                    musicData = new NoteMusicData();
                    musicData.type = NoteType.Music;
                    musicData.lineNum = int.Parse(words[1]);
                    musicData.time = int.Parse(words[2]) / 1000f;
                    musicData.drag = int.Parse(words[3]);
                    noteData = musicData;
                    break;
                case "BatterNote":
                    musicData = new NoteMusicData();
                    musicData.type = NoteType.Music;
                    musicData.batterHit = int.Parse(words[1]);
                    musicData.time = int.Parse(words[2]) / 1000f;
                    musicData.batterEndTime = int.Parse(words[3]) / 1000f;
                    noteData = musicData;
                    break;
                case "POS":
                    cameraData = new NoteCameraData();
                    cameraData.noteType = NoteCamera.N_POS;
                    Debug.Log(words[1]);
                    string[] startPos = words[1].Replace("(","").Replace(")", "").Split(',');
                    cameraData.startPos = new Vector2(float.Parse(startPos[0]), float.Parse(startPos[1]));
                    string[] endPos = words[2].Replace("(", "").Replace(")", "").Split(',');
                    cameraData.endPos = new Vector2(float.Parse(endPos[0]), float.Parse(endPos[1]));
                    cameraData.curvePos = int.Parse(words[3]);
                    cameraData.time = int.Parse(words[4]) / 1000f;
                    cameraData.endTime = int.Parse(words[5]) / 1000f;
                    noteData = cameraData;
                    break;
                case "ROT":
                    cameraData = new NoteCameraData();
                    cameraData.noteType = NoteCamera.N_ROT;
                    cameraData.startRot = float.Parse(words[1]);
                    cameraData.endRot = float.Parse(words[2]);
                    cameraData.curveRot = int.Parse(words[3]);
                    cameraData.time = int.Parse(words[4]) / 1000f;
                    cameraData.endTime = int.Parse(words[5]) / 1000f;
                    noteData = cameraData;
                    break;
                case "ZOOM":
                    cameraData = new NoteCameraData();
                    cameraData.noteType = NoteCamera.N_ZOOM;
                    cameraData.startZoom = float.Parse(words[1]);
                    cameraData.endZoom = float.Parse(words[2]);
                    cameraData.curveZoom = int.Parse(words[3]);
                    cameraData.time = int.Parse(words[4]) / 1000f;
                    cameraData.endTime = int.Parse(words[5]) / 1000f;
                    noteData = cameraData;
                    break;
                default:
                    Debug.LogError("NoteDataReader::Type Unknown : " + words[0]);
                    break;
            }

            if (noteData != null)
            {
                datas.Add(noteData);
                noteData = null;
            }
        }

        dataReaded = true;
    }
}
