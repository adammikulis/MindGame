using Godot;
using Godot.Collections;

namespace MindGame
{
    [Tool]
    public partial class Character : Resource
    {
        [Export]
        public Dictionary<string, Variant> CharacterData { get; set; } = new Dictionary<string, Variant>();
    }
}
