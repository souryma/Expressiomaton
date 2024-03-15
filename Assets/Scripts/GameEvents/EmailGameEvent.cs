using System;
using System.Net.Mail;
using System.Text.RegularExpressions;
using UnityEngine;


    [CreateAssetMenu]
    public class EmailGameEvent : TypedGameEvent<string>
    {
            

        public new void RaiseEvent(string item)
        {
            if (IsValid(item))
            {
               base.RaiseEvent(item);
            }
            else
            {
                Debug.Log("Bad email");
            }
           
        }
        public bool IsValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);

                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
