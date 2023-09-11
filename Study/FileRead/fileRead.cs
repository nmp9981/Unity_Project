using System.IO;

TextAsset textFile = Resources.Load("Localization_sqgolf_text") as TextAsset;//텍스트 에셋에 메모장 파일을 불러온 뒤
            StringReader reader = new StringReader(textFile.text);//그 안에 있는 글자들을 저장한다.
            
            for(int i=0;i< textFile.text.Length;i++)
            {
                string line = reader.ReadLine();

                if (line == null) break;
                string[] arr = line.Split(" ");//공백으로 구분
                
            }
