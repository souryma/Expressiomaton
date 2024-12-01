using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu]
    public class EmailData : ScriptableObject
    {
        public LocalizedString emailSubject;
        public LocalizedString emailText;
    }
