using System;
using System.Collections.Generic;
using System.Linq;
using Vintagestory.API.Common;
using Vintagestory.API.Server;
using Vintagestory.API.Datastructures;

namespace conclass
{
    public class CharacterSystem : ModSystem
    {
        public List<CharacterClass> characterClasses = new List<CharacterClass>();
        private ICoreAPI api;

        public override void Start(ICoreAPI api)
        {
            base.Start(api);
            this.api = api;
        }

        public override void AssetsFinalize(ICoreAPI api)
        {
            base.AssetsFinalize(api);
            
            // Load character classes from the JSON configuration file
            LoadCharacterClasses(api);
        }

        private void LoadCharacterClasses(ICoreAPI api)
        {
            try
            {
                // Load the character classes from the JSON file
                var asset = api.Assets.TryGet(new AssetLocation("conclass", "config/characterclasses.json"));
                
                if (asset != null)
                {
                    var classesData = asset.ToObject<List<CharacterClassData>>();
                    
                    foreach (var classData in classesData)
                    {
                        var charClass = new CharacterClass
                        {
                            Code = classData.code,
                            Name = classData.code, // You can add a name field to the JSON if needed
                            Traits = classData.traits ?? new List<string>()
                        };
                        
                        characterClasses.Add(charClass);
                        api.Logger.Notification($"Loaded character class: {charClass.Code} with {charClass.Traits.Count} traits");
                    }
                    
                    api.Logger.Notification($"CharacterSystem initialized with {characterClasses.Count} character classes");
                }
                else
                {
                    api.Logger.Error("Could not find characterclasses.json in assets/conclass/config/");
                }
            }
            catch (Exception ex)
            {
                api.Logger.Error($"Error loading character classes: {ex.Message}");
                api.Logger.Error(ex.StackTrace);
            }
        }

        public void RegisterCharacterClass(CharacterClass charClass)
        {
            if (!characterClasses.Any(c => c.Code == charClass.Code))
            {
                characterClasses.Add(charClass);
            }
        }

        public CharacterClass GetCharacterClass(string code)
        {
            return characterClasses.FirstOrDefault(c => c.Code == code);
        }
    }

    // Helper class for deserializing the JSON
    public class CharacterClassData
    {
        public string code { get; set; }
        public List<string> traits { get; set; }
        public List<object> gear { get; set; }
    }

    public class CharacterClass
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public List<string> Traits { get; set; } = new List<string>();
        
        public CharacterClass()
        {
        }

        public CharacterClass(string code, string name, List<string> traits = null)
        {
            Code = code;
            Name = name;
            Traits = traits ?? new List<string>();
        }
    }
}
