using System;

namespace Netherite.Texts
{
    [Flags]
    public enum TextFormatFlag
    {
        Bold = 1,
        Italic = 2,
        Obfuscated = 4,
        Strikethrough = 8,
        Underline = 16,
        Reset = 32
    }
}
