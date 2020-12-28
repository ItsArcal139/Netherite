using System.Collections.Generic;

namespace Netherite.Texts
{
    public interface IText
    {
        ICollection<IText> Extra { get; }
        IText Parent { get; }
        TextColor Color { get; }
        bool Bold { get; }
        bool Italic { get; }
        bool Obfuscated { get; }
        bool Underline { get; }
        bool Strikethrough { get; }
        bool Reset { get; }

        TextColor ParentColor { get; }
        string ToPlainText();
    }

    public interface IText<S> : IText
    {
        S AddExtra(params IText[] texts);
        S SetColor(TextColor color);
        S Format(TextFormatFlag flags);
    }
}
