    public void SelectioView()
    {
        Camera.main.gameObject.transform.localRotation = Quaternion.Euler(90, 0, 0);
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
        Camera.main.gameObject.transform.position = new Vector3(bounds.center.x, bounds.center.y+80, bounds.center.z);
    }
}
