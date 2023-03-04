using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Steamworks;

public class PlayerListItem : MonoBehaviour
{
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private RawImage avatar;

    public Transform leaderIconNew;
    public Transform leaderIconOld;
    public GameObject leaderIcon;
    public GameObject addFriendButton;

    [HideInInspector] public bool avatarRecieved;
    [HideInInspector] public ulong steamId;
    [HideInInspector] public int connectionID;
    [HideInInspector] public string username;

    protected Callback<AvatarImageLoaded_t> avatarImageLoaded;

    private void Awake()
    {
        avatarImageLoaded = Callback<AvatarImageLoaded_t>.Create(OnAvatarImageLoaded);
    }

    public void SetPlayerListItemValues()
    {
        nameText.text = username;

        if (!avatarRecieved)
            GetPlayerAvatar();
    }

    public void AddFriend()
    {
        SteamFriends.ActivateGameOverlayToUser("friendAdd", (CSteamID)steamId);
    }

    //Avatar
    private void GetPlayerAvatar()
    {
        int imageId = SteamFriends.GetLargeFriendAvatar((CSteamID)steamId);

        if (imageId == -1) { return; }

        avatar.texture = GetSteamImageAsTexture(imageId);
    }

    private void OnAvatarImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == steamId)
        {
            avatar.texture = GetSteamImageAsTexture(callback.m_iImage);
        }
        else
        {
            return;
        }
    }

    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);

        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if (isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }

        avatarRecieved = true;

        return texture;
    }
}
