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


/// <summary>
/// 각 오브젝트 버튼을 눌렀을때 나오는 프로퍼티
/// </summary>
/// <param name="sender"></param>
/// <param name="e"></param>
void ObjectClik(object sender, RoutedEventArgs e)
{
    TreeViewItem item = sender as TreeViewItem;

    textBlock.VerticalAlignment = VerticalAlignment.Top;
    textBlock.FontSize = 20;
    textBlock.Height = 450;

    textBlock.Text = item.Header.ToString() + " \n프로퍼티 특성";

    Canvas.SetLeft(textBlock, btnW);
    Canvas.SetTop(textBlock, 20);
    if (ListCanvas.Children.Contains(textBlock))
    {
        return;
    }
    ListCanvas.Children.Add(textBlock);
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

            //버튼 기능 추가
            item.PreviewMouseLeftButtonDown += ObjectClik;
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
