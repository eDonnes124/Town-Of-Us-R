using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace TownOfUs.Interfaces
{
    public interface IGuesser
    {
        public Dictionary<byte, (GameObject, GameObject, GameObject, TMP_Text)> Buttons { get; set; }
    }
}