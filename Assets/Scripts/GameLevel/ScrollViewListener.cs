using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class ScrollViewListener : MonoBehaviour
{
    [Header("隐藏未开发的关卡显示")]
    public bool HiddenOtherPart = true;
    private Button[] btnList;
    private int gamePartNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        btnList = GameObject.FindGameObjectsWithTag("ListButton")
       .Select(go => go.GetComponent<Button>())
       .OrderBy(btn => btn.name)
       .ToArray();

        // 对排序后的按钮进行处理
        // foreach (Button button in btnList)
        // {
        //     // 在这里进行对排序后的按钮的处理
        //     Debug.Log(button.name);
        // }
        gamePartNum = SceneManager.sceneCountInBuildSettings - 1;
        // btnList = GameObject.FindGameObjectsWithTag("ListButton").Select(btn => btn.GetComponent<Button>()).ToArray();
        // Debug.Log($"gamePartNum = {gamePartNum} , btnList.Length={btnList.Length}");
        for (int i = 0; i < btnList.Length; i++)
        {
            int index = i; // 创建一个局部变量来捕获循环变量的值
            btnList[index].onClick.AddListener(() => OnClick(btnList[index], index));
            Debug.Log($"index= {index} ,name = {btnList[index].name}, position.x = {btnList[index].transform.position}");
            if (index + 1 > gamePartNum)
            {
                btnList[i].interactable = false;
                if (HiddenOtherPart) btnList[index].gameObject.SetActive(false);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    void OnClick(Button clickedObject, int index)
    {
        // 第一个场景为选择关卡，后续的场景为关卡
        if (index + 1 <= gamePartNum)
        {
            Debug.Log("Button clicked: " + clickedObject.name + " index = " + index + " gamePartNum = " + gamePartNum);
            // 在这里可以执行特定的操作，例如打开一个窗口、加载一个场景、改变游戏状态等。
            SceneManager.LoadScene(index + 1);
        }
    }
}
