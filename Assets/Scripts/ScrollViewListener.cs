using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class ScrollViewListener : MonoBehaviour
{
    private Button[] btnList;
    private int gamePartNum = 0;
    // Start is called before the first frame update
    void Start()
    {
        gamePartNum = SceneManager.sceneCountInBuildSettings -1;
        btnList = GameObject.FindGameObjectsWithTag("ListButton").Select(go => go.GetComponent<Button>()).ToArray();
        for (int i = 0; i < btnList.Length; i++)
        {
            int index = i; // 创建一个局部变量来捕获循环变量的值
            btnList[i].onClick.AddListener(() => OnClick(btnList[index], index));

            if (index + 1 > gamePartNum)
            {
                btnList[i].interactable = false;
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
        if (index + 1 <= gamePartNum){
            Debug.Log("Button clicked: " + clickedObject.name + " index = " + index  + " gamePartNum = " + gamePartNum);
             // 在这里可以执行特定的操作，例如打开一个窗口、加载一个场景、改变游戏状态等。
        }
    }
}
