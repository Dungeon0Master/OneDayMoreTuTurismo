public interface IInteractable
{
    void OnPlayerEnter(PlayerInteractor player);
    void OnPlayerExit(PlayerInteractor player);
    void Interact(PlayerInteractor player);
}