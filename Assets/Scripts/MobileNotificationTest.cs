using System;
using System.Text;
using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.UI;

public class MobileNotificationTest : MonoBehaviour
{
    [SerializeField] private Text txtLog;
    
    private readonly StringBuilder _logString = new StringBuilder();
    private int _notificationTrace = -1;

    private void Awake()
    {
        if (AndroidNotificationCenter.Initialize() == false)
        {
            Log("[MobileNotificationTest::Awake] Failed Initialize");
            return;
        }
        AndroidNotificationCenter.OnNotificationReceived += OnNotificationReceived;
    }

    private void OnNotificationReceived(AndroidNotificationIntentData data)
    {
        var msg = "Notification received : " + data.Id + "\n";
        msg += "\n Notification received: ";
        msg += "\n .Title: " + data.Notification.Title;
        msg += "\n .Body: " + data.Notification.Text;
        msg += "\n .Channel: " + data.Channel;
        Log(msg);
        Debug.Log(msg);
        
        // NOTE(JJO): 알림 온 것을 눌러서 게임을 실행해도 알림이 지워지지 않을 것이므로 Callback에서 지워야 함.
        AndroidNotificationCenter.CancelNotification(data.Id);
    }

    public void OnClickRegister()
    {
        var c = new AndroidNotificationChannel()
        {
            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.High,
            Description = "Generic notifications",
        };
        try
        {
            AndroidNotificationCenter.RegisterNotificationChannel(c);

            Log("[MobileNotificationTest::OnClickRegister] Register!");
        }
        catch (Exception e)
        {
            Log($"[MobileNotificationTest::OnClickRegister] Failed Register! - {e}");
        }
    }

    public void OnClickUnregister()
    {
        AndroidNotificationCenter.DeleteNotificationChannel("channel_id");
        Log("[MobileNotificationTest::OnClickUnregister] Do unregister notification!");
    }

    public void OnClickSendNotification()
    {
        var notification = new AndroidNotification();
        notification.Title = "TestTitle";
        notification.Text = "TestText";
        notification.FireTime = System.DateTime.Now.AddSeconds(5.0);
        
        if ((_notificationTrace = AndroidNotificationCenter.SendNotification(notification, "channel_id")) == -1)
        {
            Log("[MobileNotificationTest::OnClickSendNotification] Initialize First!");
            Debug.LogError("[MobileNotificationTest::OnClickSendNotification] Initialize First!");
        }
        else
        {
            Log($"[MobileNotificationTest::OnClickSendNotification] {_notificationTrace}");
        }
    }

    private void Log(string msg)
    {
        _logString.Insert(0, $"[{System.DateTime.Now.ToString()}] {msg}\n");
        txtLog.text = _logString.ToString();
    }
}
