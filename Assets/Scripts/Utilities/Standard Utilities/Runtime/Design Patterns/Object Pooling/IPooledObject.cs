namespace Lortedo.Utilities.Pattern
{
    public interface IPooledObject
    {
        void OnObjectSpawn();
        string ObjectTag { get; set; }
    }
}
