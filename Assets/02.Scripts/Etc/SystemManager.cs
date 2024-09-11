using UnityEngine;

public class SystemManager : MonoBehaviour
{
    #region private variable

    private static SystemManager _instance;

    private ChapterType _chapter = 0;
    private StageType _stage = 0;

    #endregion // private variable





    #region properties

    public static SystemManager Instance { get { return _instance; } }

    public ChapterType CurrChapter { get { return _chapter; } }
    public StageType CurrStage { get { return _stage; } }

    #endregion // properties





    #region mono funcs

    void Start() {
        _instance = this;
    }

    #endregion // mono funcs





    #region public funcs

    public void SetChapter(ChapterType chapterType) {
        _chapter = chapterType;
    }

    public void SetStage(StageType stageType) {
        _stage = stageType;
    }

    #endregion // private funcs
}
