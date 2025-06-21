using UnityEngine;

public class InputFieldController : MonoBehaviour
{
    [SerializeField] private HobbyCreator hobbyCreator;

    public void SetHobbyName(string hobbyName)
    {
        hobbyCreator.SetName(hobbyName);
    }
}
