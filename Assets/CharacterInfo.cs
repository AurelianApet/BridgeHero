using UnityEngine;
using System.Collections;

public class CharacterInfo : MonoBehaviour {
    public GameObject CharacterPrefab;
    public string DisplayName = "Name me";
    public int PriceInCoins = 10;

    [Multiline]
    public string InappID = "Type Apple/Google ProductID here.\nIt should be same for iOS and Android.\nAlso used as Unique ID";
}
