 public void PanelView()
 {
     Camera.main.gameObject.transform.localRotation = Quaternion.Euler(90, 0, 90);
     Scene sc = SceneManager.GetActiveScene();

     Bounds bounds = default;
     foreach (GameObject go in sc.GetRootGameObjects())
     {
         if (go.GetComponent<RTComponent>() is var component && component != null)
         {
             Bounds gobounds = component.GetBounds(false);
             if (bounds == default) bounds = gobounds;
             else bounds.Encapsulate(gobounds);
         }
     }

     bounds.size *= 1.5f;
     RTG.RTFocusCamera.Get.Focus(new RTG.AABB(bounds));
     //Camera.main.gameObject.transform.position = new Vector3(bounds.center.x, bounds.center.y + 50, bounds.center.z);
 }
 public void SelectioView()
 {
     Camera.main.gameObject.transform.localRotation = Quaternion.Euler(0, -90, 0);
     Scene sc = SceneManager.GetActiveScene();

     Bounds bounds = default;
     foreach (GameObject go in sc.GetRootGameObjects())
     {
         if (go.GetComponent<RTComponent>() is var component && component != null)
         {
             if (bounds == default) bounds = component.GetBounds(false);
             else bounds.Encapsulate(go.GetComponent<Bounds>());
         }
     }
     Camera.main.gameObject.transform.position = new Vector3(bounds.center.x + 80, bounds.center.y, bounds.center.z);
 }
