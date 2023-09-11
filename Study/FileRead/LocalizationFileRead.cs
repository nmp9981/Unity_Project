TextAsset textFile = Resources.Load("Localization_sqgolf") as TextAsset;//텍스트 에셋에 메모장 파일을 불러온 뒤
            StringReader reader = new StringReader(textFile.text);//그 안에 있는 글자들을 저장한다.

            string languageArray = "";
            string[] textAllFile = new string[272];//전체 파일
            Debug.LogError(textFile.text.Length);
            for (int i = 0; i < textFile.text.Length; i++)//한줄씩 탐색
            {
                if (i == 271) break;
                if (i == 0)
                {
                    languageArray = reader.ReadLine();
                }
                else textAllFile[i] = reader.ReadLine();
            }
            string[] languages = languageArray.Split(",");//사용하는 언어를 쉼표로 구분
                                                          //각 언어별 사전 생성
            for (int j = 1; j < languages.Length; j++)
            {
                if (!LanguageDic.ContainsKey(languages[j]))
                {
                    LanguageDic.Add(languages[j], new Dictionary<string, string>());
                }
            }

            for (int j = 1; j < languages.Length; j++)//1 : English, 2:korean,...
            {
                for (int i = 1; i < textAllFile.Length - 3; i++)//한줄씩 탐색
                {
                    string line = textAllFile[i];

                    if (line == null) break;//빈 줄
                    if (line == ",,,") continue;//다음 줄로

                    string[] words = line.Split(",");//쉼표로 구분

                    var key = words[0];//키 값

                    if (LanguageDic[languages[j]].ContainsKey(key))
                    {
                        IVDebug.LogWarning("Already Key exist:" + key);
                        LanguageDic[languages[j]][key] = words[j];
                    }
                    else
                    {
                        LanguageDic[languages[j]].Add(key, words[j]);

                    }
                }
            }
