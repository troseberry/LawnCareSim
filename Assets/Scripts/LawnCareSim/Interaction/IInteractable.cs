namespace LawnCareSim.Interaction
{
    public interface IInteractable
    {
        string Prompt { get; }

        void Initialize();

        bool CanInteract();

        void Interact();
    }
}
