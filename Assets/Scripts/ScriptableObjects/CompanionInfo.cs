using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CompanionInfo", order = 1)]
    public class CompanionInfo : ScriptableObject
    {
        public string companionName;
        public Sprite image;
        public List<TextAsset> dateDialogueTexts;
    }
}