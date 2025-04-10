<Grid>
    <TreeView x:Name="_TreeView" Margin="0,37,205,84"/>
</Grid>

private void Window_Loaded(object sender, RoutedEventArgs e)
{
    // foreach (string str in Directory.GetDirectories(""))   // 특정폴더
    foreach (string str in Directory.GetLogicalDrives())   // 루트폴더
    {
        try
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = str;
            item.Tag = str;
            item.Expanded += new RoutedEventHandler(item_Expanded);   // 노드 확장시 추가
 
            _TreeView.Items.Add(item);
            GetSubDirectories(item);
        }
 
        catch (Exception except)
        {
            // MessageBox.Show(except.Message);   // 접근 거부 폴더로 인해 주석처리
        }
    }
}
 
// 서브 디렉토리
private void GetSubDirectories(TreeViewItem itemParent)
{
    if (itemParent == null) return;
    if (itemParent.Items.Count != 0) return;
 
    try
    {
        string strPath = itemParent.Tag as string;
        DirectoryInfo dInfoParent = new DirectoryInfo(strPath);
        foreach (DirectoryInfo dInfo in dInfoParent.GetDirectories())
        {
            TreeViewItem item = new TreeViewItem();
            item.Header = dInfo.Name;
            item.Tag = dInfo.FullName;
            item.Expanded += new RoutedEventHandler(item_Expanded);
            itemParent.Items.Add(item);
        }
    }
 
    catch (Exception except)
    {
        // MessageBox.Show(except.Message);   // 접근 거부 폴더로 인해 주석처리
    }
}
 
// 트리확장시 내용 추가
void item_Expanded(object sender, RoutedEventArgs e)
{
    TreeViewItem itemParent = sender as TreeViewItem;
    if (itemParent == null) return;
    if (itemParent.Items.Count == 0) return;
    foreach (TreeViewItem item in itemParent.Items)
    {
        GetSubDirectories(item);
    }
}
