using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CreateLobbyManager : MonoBehaviour
{
    [SerializeField] private TMP_Text maxPlayersText;
    [SerializeField] private Slider maxPlayersSlider;
    [SerializeField] private TMP_Dropdown regionDropdown;
    [SerializeField] private TMP_Dropdown lobbyTypeDropdown;

    public void UpdateText()
    {
        maxPlayersText.text = "Max Players: " + (int)maxPlayersSlider.value;
    }

    public void HostLobby()
    {
        LobbyManager.instance.HostLobby(lobbyTypeDropdown.value, (int)maxPlayersSlider.value, regionDropdown.value);
    }
}
