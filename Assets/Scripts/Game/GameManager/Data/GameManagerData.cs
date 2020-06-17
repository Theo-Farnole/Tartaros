namespace Game.GameManagers
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Tartaros/System/Game Manager")]
    public class GameManagerData : ScriptableObject
    {
        [SerializeField] private ResourcesWrapper _startingResources = new ResourcesWrapper(10, 5, 0);
        [SerializeField] private int _startMaxPopulationCount = 10;

        [Header("CONSTRUCTION")]
        [SerializeField] private string[] _IDInPanelConstruction = new string[0];

        [Header("LOSE CONDITION")]
        [SerializeField] private string _loseOnDestroyedEntityID = "building_temple";

        public ResourcesWrapper StartingResources { get => _startingResources; }
        public int StartMaxPopulationCount { get => _startMaxPopulationCount; }
        public string[] IDsInPanelConstruction { get => _IDInPanelConstruction; }
        public string LoseOnDestroyedEntityID { get => _loseOnDestroyedEntityID; }

        public OrderContent[] GetConstructionOrders()
        {
            OrderContent[] output = new OrderContent[_IDInPanelConstruction.Length];

            for (int i = 0; i < _IDInPanelConstruction.Length; i++)
            {
                string entityID = _IDInPanelConstruction[i];

                var entityData = MainRegister.Instance.GetEntityData(entityID);

                var order = new OrderContent(
                    entityData.Hotkey,
                    entityData.Portrait,
                    entityData.HoverPopupData,
                    () => GameManager.Instance.StartBuilding(entityID),
                    1
                );

                output[i] = order;
            }

            return output;
        }
    }
}
