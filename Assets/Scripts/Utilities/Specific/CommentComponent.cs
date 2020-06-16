namespace Game.Utils
{
    using Sirenix.OdinInspector;
    using UnityEngine;

    public class CommentComponent : MonoBehaviour
    {
        [TextArea]
        [DrawWithUnity]
        [SerializeField] private string _notes = "Comment Here.";                                              
    }
}