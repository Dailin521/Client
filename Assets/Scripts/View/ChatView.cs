using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatView : MonoBehaviour
{
    GameObject chatItem;
    Transform chatItemParent;
    Scrollbar scrollbar;

    Button sendBtn;
    InputField sendInput;

    // Start is called before the first frame update
    [Obsolete]
    void Start()
    {

        chatItem = Resources.Load<GameObject>("ChatItem");
        chatItemParent = this.transform.Find("ScrollView/Viewport/Content");
        scrollbar = this.transform.Find("ScrollView/Vertical").GetComponent<Scrollbar>();

        sendInput = this.transform.Find("SendInput").GetComponent<InputField>();//聊天输入框

        sendBtn = this.transform.Find("SendBtn").GetComponent<Button>();//发送按钮
        sendBtn.onClick.AddListener(SendChatMsg);//发送聊天数据

        //收到聊天数据 进行处理
        MessageHelper.Instance().chatMsgHandle += ChatHandle;
    }

    private void ChatHandle(ChatMsgS2C obj)
    {
        AddMessage(obj.player, obj.msg);
    }

    //显示消息
    public void AddMessage(string title, string content)
    {

        var go = GameObject.Instantiate<GameObject>(chatItem);
        go.transform.SetParent(chatItemParent, false);
        var titleText = go.transform.Find("Background/Title").GetComponent<Text>();
        titleText.text = title;

        var chat = go.transform.Find("Background/Text").GetComponent<Text>();
        chat.text = content;

        StartCoroutine(ReSetScrollbar());
        //scrollbar.value = -0.01f;
    }

    public IEnumerator ReSetScrollbar()
    {
        yield return new WaitForEndOfFrame();
        scrollbar.value = 0;
    }

    private void SendChatMsg()
    {
        if (string.IsNullOrEmpty(sendInput.text))
        {
            return;
        }
        MessageHelper.Instance().SendChatMsg(PlayerData.Instance.loginMsgS2C.account, sendInput.text);
    }

    // Update is called once per frame
    void Update()
    {
        //if (Input.GetKeyDown(KeyCode.A))
        //{
        //    AddMessage("xxxx", "yyysksljaklfjkldjsaklrejkl rjekwljrklewmn,fdma,. eklr;klw;");
        //}
    }

}
