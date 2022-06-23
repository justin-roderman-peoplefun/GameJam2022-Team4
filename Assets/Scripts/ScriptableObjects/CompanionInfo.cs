using System.Collections.Generic;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CompanionInfo", order = 1)]
    public class CompanionInfo : ScriptableObject
    {
        public string key;
        public string companionName;
        public string companionQuote;
        public Sprite image;
        public Sprite angryImage;
        public Sprite sadImage;
        public Sprite happyImage;
        public Sprite blushImage;
        public List<TextAsset> dialogueTexts;
    }
}