using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Linq;

public class CharacterShopLogic : MonoBehaviour {
    CharacterInfo[] Infos;
    int ShowedCharacterIndex;

    public Text CoinsPrice;
    public GameObject CoinsGroup;

    public Text CharacterName;
    public Text ActionTitle;
    public Text Wallet;

    public MenuLogic menuLogic;

    public GameObject CharacterPlaceHolder;

    int Coins = 0;

    void Awake()
    {
        Infos = GetComponentsInChildren<CharacterInfo>().OrderBy(go => go.PriceInCoins).ToArray();
    }

    void OnEnable()
    {
        UpdateCoins();

        string SelectedCharacterUID = PlayerPrefs.GetString("CurrentCharacterCode");
        for (int i=0;i<Infos.Length;++i)
            if (Infos[i].InappID.Equals(SelectedCharacterUID))
                ShowedCharacterIndex = i;
        ShowCharacter(ShowedCharacterIndex);
    }

    void ShowCharacter(int index)
    {
        if (index < 0) index = Infos.Length - 1;
        ShowedCharacterIndex = index % Infos.Length;

        for (int i = 0; i < CharacterPlaceHolder.transform.childCount; ++i) Destroy(CharacterPlaceHolder.transform.GetChild(i).gameObject);

        CoinsPrice.text = Infos[ShowedCharacterIndex].PriceInCoins.ToString();
        CharacterName.text = Infos[ShowedCharacterIndex].DisplayName;
        GameObject prefab = (GameObject)Instantiate(Infos[ShowedCharacterIndex].CharacterPrefab);
        prefab.transform.parent = CharacterPlaceHolder.transform;
        prefab.transform.localPosition = Vector3.zero;
        prefab.transform.localRotation = Quaternion.identity;
        prefab.transform.localScale = Vector3.one;
        bool unlocked = IsUnlocked();

        CoinsGroup.SetActive(!unlocked);
        CoinsPrice.gameObject.SetActive(!unlocked);
        ActionTitle.text = unlocked ? "SELECT" : "UNLOCK";
    }

    public void OnNextCharacter()
    {
        ShowCharacter(ShowedCharacterIndex + 1);

        MenuLogic.PlaySFX();
    }
    public void OnPreviousCharacter() 
    {
        ShowCharacter(ShowedCharacterIndex - 1);
        MenuLogic.PlaySFX();
    }

    bool IsUnlocked()
    {
        string[] unlockedCharacters = PlayerPrefs.GetString("UnlockedCharacters").Split(',');
        int index = unlockedCharacters.ToList<string>().IndexOf(Infos[ShowedCharacterIndex].InappID);
            return index >= 0;
    }

    public void OnAction() 
    {
        if (IsUnlocked())
        {
            // Character is Unlocked
            PlayerPrefs.SetString("CurrentCharacterCode",Infos[ShowedCharacterIndex].InappID);
            PlayerPrefs.Save();

            menuLogic.popMenu();
        }
        else
        {
            if (Coins >= Infos[ShowedCharacterIndex].PriceInCoins)
            {
                Coins -= Infos[ShowedCharacterIndex].PriceInCoins;
                string unlockedCharacters = PlayerPrefs.GetString("UnlockedCharacters");
                unlockedCharacters += Infos[ShowedCharacterIndex].InappID + ",";

                Wallet.text = Coins.ToString();

                PlayerPrefs.SetInt("Coins", Coins);
                PlayerPrefs.SetString("UnlockedCharacters",unlockedCharacters);

                PlayerPrefs.Save();

                ShowCharacter(ShowedCharacterIndex);

                MenuLogic.PlaySFX();
                // Play Unlock Sound ? 
            }
            else
            {
                MenuLogic.PlaySFX(false);
                // Suggest to buy coins ? 
                // Will be done in next update
            }
        }
    }


    void StartUnlockForCoins()
    {

    }

    void StartUnlockForCash()
    {

    }

    void SelectCharacter()
    {

    }

    public void UpdateCoins()
    {
        Coins = PlayerPrefs.GetInt("Coins");
        Wallet.text = Coins.ToString();
    }
}
