using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    #region Fields
    [SerializeField] private GameManagerData _data;
    [SerializeField] private SnapGrid _grid;

    private OwnerState<GameManager> _state = null;
    private ResourcesWrapper _resources = new ResourcesWrapper();
    #endregion

    #region Properties
    public SnapGrid Grid { get => _grid; }
    public ResourcesWrapper Resources
    {
        get
        {
            return _resources;
        }
        set
        {
            _resources = value;
            UIManager.Instance.UpdateResourcesLabel(_resources);
        }
    }
    public OwnerState<GameManager> State
    {
        get
        {
            return _state;
        }

        set
        {
            _state?.OnStateExit();
            _state = value;
            _state?.OnStateEnter();
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks.
    void Start()
    {
        Resources = _data.StartingResources;
    }

    void Update()
    {
        CheckForStateChangement();

        _state?.Tick();
    }
    #endregion

    void CheckForStateChangement()
    {
        if (_state == null)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                State = new BuildingState(this);
            }
        }
    }
    #endregion
}
