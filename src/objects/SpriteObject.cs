using Integrity.Assets;
using Integrity.Components;

namespace Integrity.Objects;
public class SpriteObject : GameObject
{
    public SpriteComponent Sprite { get; }

    public SpriteObject(string name, GLTexture glTexture) : base(name)
    {
        Sprite = new SpriteComponent(glTexture);
        AddComponent(Sprite);
    }
}