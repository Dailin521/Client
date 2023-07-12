using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginView : MonoBehaviour
{
    GameObject register;

    //注册相关
    Button registerBtn;
    InputField usernameInput;
    InputField emailInput;
    InputField passwordInput;
    Button _registerBtn;
    Button _closeBtn;

    //登录相关
    Button loginBtn;
    InputField usernameInput_Login;
    InputField passwordInput_Login;

    // Start is called before the first frame update
    void Start()
    {
        register = transform.Find("Register").gameObject;

        //打开注册界面
        registerBtn = transform.Find("RegisterBtn").GetComponent<Button>();
        registerBtn.onClick.AddListener(OpenRegisterView);

        //关闭注册界面
        _closeBtn = transform.Find("Register/View/CloseBtn").GetComponent<Button>();
        _closeBtn.onClick.AddListener(CloseRegisterView);//alt+enter

        //注册界面输入框
        usernameInput = transform.Find("Register/View/UsernameInput").GetComponent<InputField>();
        emailInput = transform.Find("Register/View/EmailInput").GetComponent<InputField>();
        passwordInput = transform.Find("Register/View/PasswordInput").GetComponent<InputField>();

        //注册界面-注册按钮
        _registerBtn = transform.Find("Register/View/RegisterBtn").GetComponent<Button>();
        _registerBtn.onClick.AddListener(RegisterOnClick);



        //登录界面-登录按钮
        loginBtn = transform.Find("LoginBtn").GetComponent<Button>();
        loginBtn.onClick.AddListener(LoginOnClick);

        usernameInput_Login = transform.Find("UsernameInput").GetComponent<InputField>();
        passwordInput_Login = transform.Find("PasswordInput").GetComponent<InputField>();

        //处理登录结果
        MessageHelper.Instance().loginHandle += LoginHandle;
        MessageHelper.Instance().registerHandle += RegisterHandle;
    }

    private void RegisterHandle(RegisterMsgS2C obj)
    {
        if (obj.result == 0)
        {
            Debug.Log("注册成功");
            Tips("注册成功");
            register.gameObject.SetActive(false);
            usernameInput_Login.text = obj.account;
            passwordInput_Login.text = obj.password;
        }
        else
        {
            Tips($"注册失败,错误码:{obj.result}");
        }
    }

    private void LoginHandle(LoginMsgS2C obj)
    {
        if (obj.result == 0)
        {
            PlayerData.Instance.loginMsgS2C = obj;
            Debug.Log("登录成功");
            this.gameObject.SetActive(false);

            var chatView = Resources.Load<GameObject>("ChatView");
            var chatObj = GameObject.Instantiate<GameObject>(chatView);
            chatObj.AddComponent<ChatView>();
        }
        else
        {
            Tips($"登录失败,错误码:{obj.result}");
        }
    }

    private void LoginOnClick()
    {
        if (string.IsNullOrEmpty(usernameInput_Login.text))
        {
            Tips("请输入帐号!!!");
            return;
        }

        if (string.IsNullOrEmpty(passwordInput_Login.text))
        {
            Tips("请输入密码!!!");
            return;
        }

        //发送数据给服务器
        MessageHelper.Instance().SendLoginMsg(usernameInput_Login.text, passwordInput.text);
    }

    private void RegisterOnClick()
    {
        if (string.IsNullOrEmpty(usernameInput.text))
        {
            Tips("请输入帐号!!!");
            return;
        }

        if (string.IsNullOrEmpty(emailInput.text))
        {
            Tips("请输入邮箱!!!");
            return;
        }

        if (string.IsNullOrEmpty(passwordInput.text))
        {
            Tips("请输入密码!!!");
            return;
        }
        //发送数据给服务器
        MessageHelper.Instance().SendRegisterMsg(usernameInput.text, emailInput.text, passwordInput.text);
    }

    private void CloseRegisterView()
    {
        register.gameObject.SetActive(false);
    }

    private void OpenRegisterView()
    {
        register.gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {

    }

    GameObject _tipsObj;
    //提示
    public void Tips(string str)
    {
        if (_tipsObj != null)
        {
            GameObject.Destroy(_tipsObj);
        }
        var tipsItem = transform.Find("TipsItem").gameObject;
        _tipsObj = GameObject.Instantiate<GameObject>(tipsItem);
        _tipsObj.transform.SetParent(this.transform, false);
        var content = _tipsObj.transform.Find("Text").GetComponent<Text>();
        content.text = str;
        _tipsObj.gameObject.SetActive(true);
        GameObject.Destroy(_tipsObj, 1.5f);
    }

}
