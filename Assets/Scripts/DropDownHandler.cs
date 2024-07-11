using System.Collections.Generic;
using Console;
using TMPro;
using UnityEngine;

public class DropDownHandler : MonoBehaviour
{
    GameManager gameManager;
    public TMP_Dropdown dropdown;
    private List<DropDownItem> items;

    void Start()
    {
        Store.changeDecks += LoadItems;
        gameManager = GetComponent<GameManager>();
        LoadItems();
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void LoadItems()
    {
        items = new List<DropDownItem>();
        foreach (var deck in Store.decks)
        {
            items.Add(new DropDownItem(deck.id, deck.name));
            Debug.Log($"{deck.id}");
        }
        Debug.Log($"{Store.decks.Count}");
        PopulateDropdown();
    }

    void PopulateDropdown()
    {
        List<string> options = new List<string>();
        Debug.Log($"{items[0].id}");
        foreach (DropDownItem item in items)
        {
            options.Add(item.label);
        }

        dropdown.ClearOptions();
        dropdown.AddOptions(options);
    }

    void OnDropdownValueChanged(int index)
    {
        DropDownItem selectedItem = items[index];
        Debug.Log($"{selectedItem.id}");
        gameManager.ChangeSelectedDeck(selectedItem.id);
    }

}
