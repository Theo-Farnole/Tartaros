
namespace Lortedo.Utilities.Pattern
{
    public abstract class State
    {
        public abstract void Tick();
        public virtual void FixedTick() { }

        public virtual void OnStateEnter() { }
        public virtual void OnStateExit() { }
    }
}