namespace LawnCareSim.Interaction
{
    public interface IInteractable
    {
        void Initialize();

        bool CanInteract();

        void Interact();
    }
}
