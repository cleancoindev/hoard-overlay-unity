using Hoard.MVC;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using Nethereum;

namespace Hoard.MVC.Utilities
{
    public class HoardConfigLoader : IHoardConfigProvider<HoardServiceConfig>
    {
        public TextAsset configAsset;

        public HoardConfigLoader(TextAsset configAsset)
        {
            this.configAsset = configAsset != null ? configAsset : throw new System.NotImplementedException(nameof(configAsset));
        }

        public HoardServiceConfig GetHoardServiceOptions()
        {
            return JsonConvert.DeserializeObject<HoardServiceConfig>(configAsset.text);
        }
    }
}