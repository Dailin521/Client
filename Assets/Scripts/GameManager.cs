using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Client.Instance().Start();
        var loginPrefab = Resources.Load<GameObject>("LoginView");
        var loginView = GameObject.Instantiate(loginPrefab);
        loginView.AddComponent<LoginView>();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.A))
        {
            Client.Instance().Send(Encoding.UTF8.GetBytes("login...."));
        }
    }


}
