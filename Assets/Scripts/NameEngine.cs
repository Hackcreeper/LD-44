using System.Collections.Generic;
using UnityEngine;

public static class NameEngine
{
    private static List<string> Names = new List<string>(new string[]
    {
        "The Starkster",
        "Jordan",
        "Markus",
        "The Weird One",
        "Rabbit Chicken",
        "Andreas Haltstop",
        "Techno Chicken",
        "Black Swordsman",
        "Pink Swordsman",
        "Walter Wales",
        "Neo",
        "Sora",
        "Firewallcreeper",
        "Hacktier",
        "Blank",
        "[ ]",
        "Godsuya",
        "Daisy Skye",
        "Coul Son",
        "Maka Albahn",
        "Zuko",
        "Mikasa",
        "Levi",
        "Hawk",
        "Kenny",
        "Sir Isaac",
        "Quillinator",
        "Steel Man",
        "Baummann",
        "Thursday",
        "Saturday",
        "Otto",
        "Mr Peppboy"
    });
    
    public static string[] GetNames(int amount)
    {
        var uniqueNames = new List<string>(Names);
        var selected = new List<string>();

        for (var i = 0; i < amount; i++)
        {
            var index = Random.Range(0, uniqueNames.Count);
            selected.Add(uniqueNames[index]);
            uniqueNames.RemoveAt(index);
        }

        return selected.ToArray();
    }
}