//자식의 자식오브젝트까지 모두 구하기
foreach (var folderName in FolderNames)
        {
            string[] prefabNames = GetResourceFilePaths(folderName);

            foreach (var prefabName in prefabNames)
            {
                var p = folderName + "/" + prefabName;
                var prefab = Resources.Load<GameObject>(p);
                Prefabs.Add(prefab);
            }
        }

        foreach (var prefab in Prefabs)
        {
            //bfs로 모든 자식 오브젝트 구하기
            Transform[] allChild = prefab.GetComponentsInChildren<Transform>();

            Queue<Transform> q = new Queue<Transform>();
            foreach (Transform child in allChild) q.Enqueue(child);

            while (q.Count > 0)
            {
                Transform curChild = q.Dequeue();
                for (int i = 0; i < curChild.transform.childCount; i++)
                {
                    Transform nextChild = curChild.transform.GetChild(i);
                    TMP_Text childText = nextChild.GetComponent<TMP_Text>();
                    if (childText != null) childText.font = fontAsset;//폰트 변경

                    q.Enqueue(nextChild);
                }
            }
        }
