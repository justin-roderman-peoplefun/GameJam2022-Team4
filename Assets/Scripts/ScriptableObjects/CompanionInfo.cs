using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ScriptableObjects
{
    [CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/CompanionInfo", order = 1)]
    public class CompanionInfo : ScriptableObject
    {
        public string key;
        public bool complete;
        public string companionName;
        public string companionQuote;
        public float zoomInXOffset;
        public Sprite image;
        public Sprite angryImage;
        public Sprite sadImage;
        public Sprite happyImage;
        public Sprite blushImage;
        public Sprite bigMadImage;
        public Sprite backgroundImage;
        public TextAsset introDialogue;
        public List<TextAsset> stageDialogues;
        public TextAsset goodEndingDialogue;
        public TextAsset badEndingDialogue;
        public AudioClip introSoundClip;
        [SerializeField]
        private List<AudioClip> soundClips;

        public IDictionary<string, AudioClip> SoundClips => soundClips.ToDictionary(clip => clip.name.Substring(
            (key + "_").Length).Replace("_", ""), clip => clip);
    }
}