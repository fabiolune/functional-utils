using LanguageExt;

namespace Fl.Functional.Utils;

public static partial class Functional
{
    public static T OrElse<T>(this Option<T> @this, T defaultValue) => @this.IfNone(defaultValue);
}
