using System.Collections.Generic;

namespace Dialogue
{
    [System.Serializable]
    class DialogueOptionModel
    {
        public string Option { get; set; }
        public string LinkPath { get; set; }
    }

    [System.Serializable]
    public class DialogueContentModel
    {
        public List<object> Content { get; set; }
    }

    [System.Serializable]
    public class DialogueDataModel
    {
        public Dictionary<string, DialogueContentModel> Stitches { get; set; }
        public string Initial { get; set; }
    }

    [System.Serializable]
    public class DialogueRootModel
    {
        public string Title { get; set; }
        public DialogueDataModel Data { get; set; }
    }
}
