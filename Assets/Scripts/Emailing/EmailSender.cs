using UnityEngine;
using UnityEngine.Serialization;
[CreateAssetMenu]
public class EmailSender: ScriptableObject
{
    
        public string address;
        public string nameSender;
        public string password;
        public string service;
        public int port;
}
