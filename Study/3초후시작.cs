void Awake()
{
    StartCoroutine(ShowStartText());
}

//3초뒤 시작
IEnumerator ShowStartText()
{
    _startTimeText.text = "";
    yield return new WaitForSecondsRealtime(0.5f);
    _startTimeText.text = "3";
    yield return new WaitForSecondsRealtime(1f);
    _startTimeText.text = "2";
    yield return new WaitForSecondsRealtime(1f);
    _startTimeText.text = "1";
    yield return new WaitForSecondsRealtime(1f);
    _startTimeText.text = "Start!!";
    yield return new WaitForSecondsRealtime(1f);
    GameManager.Instance.IsPlayGame = true;//이때 게임이 시작해야 함
    SoundManager._sound.PlayBGM(1);
    _startTimeText.text = "";
}
