using System.Collections.Generic;

[System.Serializable]
public class Chapter
{
    public int id;
    public string title;
    public int childTotal;
    public string bossName;
    public string bossDesc;
}

public class ChapterConfig
{
    private readonly string _path = "config/chapter";
    private List<Chapter> _chapter = null;
    public void init()
    {
        this._chapter = JsonMgr.Instance.readObject<List<Chapter>>(this._path);
    }

    public List<Chapter> getChapter() {
        return this._chapter;
    }

    public Chapter getChapter(int id) { 
        return this._chapter.Find(p=> p.id==id);
    }
}
