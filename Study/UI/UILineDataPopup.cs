using MAR.UI;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using System;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

public class UILineDataPopup : MonoBehaviour
{
    public static List<string[]> lineDataList_public = new List<string[]>();
    public static Dictionary<string[], IRecycleData> lineDataDic_static = new();
    private string[] fileList;
    /// <summary>
    /// 기능 : 라인 데이터 팝업 창
    /// 1) 팝업창 제목, 들어갈 파일 형식, 다중 선택 여부
    /// 2) csv, excel형식을 가져오게함
    /// </summary>
    public void LineDataPopup()
    {
        string title = "Linedata Excel File";
        string file_Filter = "Excel 통합 문서 (*.xlsx)|*.xlsx" +
            "|CSV (쉼표로 분리) (*.csv)|*.csv" +
            "|CSV UTF-8(쉼표로 분리)(*.csv)|*.csv";
        int filter_index = 1;
        bool restore_dictionary = true;
        bool multiSelect = true;

        fileList = UIManager.OpenFileDialog(title,file_Filter,filter_index,restore_dictionary,multiSelect);
        lineDataList_public.Add(fileList);

    }
    /// <summary>
    /// 기능 : 창닫기
    /// </summary>
    public void UIWondowClose()
    {
        if (this.gameObject.activeSelf)
        {
            this.gameObject.SetActive(false);
        }
    }
    /// <summary>
    /// 기능 : 창열기
    /// </summary>
    public void UIWondowOpen()
    {
        if (this.gameObject.activeSelf==false)
        {
            this.gameObject.SetActive(true);
        }
    }
    /// <summary>
    /// 기능 : 라인데이터 로드 과정
    /// 1) 프로그래스바가 진행
    /// 2) 라인데이터를 화면에 띄움
    /// 3) 완료 팝업창 보임
    /// </summary>
    public async void LoadLineDataFlow()
    {
        UIProgressModal progress = UIManager.Instance.GetInterfacePage<UIProgressModal>(UIManager.EInterfacePage.ProgressBar);
        try
        {
            //예외 처리
            if (fileList == null) throw new Exception();
            if (fileList.Length == 0) throw new Exception();

            //프로그래스 바를 통한 진행도 시각화
            progress.SetData(message: "라인 데이터 로드 중...", processCount: 1);
            progress.SetActive(true);
            foreach (string file in fileList)
            {
                Debug.Log("결과 : " + file);
            }
            //라인 생성
            await LoadLineData();
            await Task.Delay(3000);
        }
        //로드 실패
        catch(Exception e)
        {
            progress.SetActive(false);
            var errorPopup = UIManager.Instance.GetInterfacePage<UIMessagePopUp>(UIManager.EInterfacePage.NotificationError);
            errorPopup.SetMessage("로드 실패", "로드에 실패했습니다.");
            errorPopup.ViewPopup();
            return;
        }

        //로드 성공
        progress.SetActive(false);
        var successPopup = UIManager.Instance.GetInterfacePage<UIMessagePopUp>(UIManager.EInterfacePage.NotificationSuccess);
        successPopup.SetMessage("로드 성공", "로드에 성공했습니다.");
        successPopup.ViewPopup();
    }
    /// <summary>
    /// 기능 : to, from 노즐을 파싱해서 astar로 그린다
    /// </summary>
    /// <returns></returns>
    public async UniTask LoadLineData()
    {

    }
}
