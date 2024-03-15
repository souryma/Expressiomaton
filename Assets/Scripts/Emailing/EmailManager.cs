using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(EmailGameEventListener))]
public class EmailManager : MonoBehaviour
{
    [SerializeField] private EmailSender m_sender;
    [SerializeField] private EmailData m_data;

    public void SendPicture(string p_emailreceiver)
    {
        GameScreenShotManager l_gameScreenShotManager = FindObjectOfType<GameScreenShotManager>();
        Emailer.SendEmail(m_sender, m_data, p_emailreceiver, l_gameScreenShotManager.LastScreenShotName);
    }
}
