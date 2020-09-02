using Android.App;
using Android.Content;
using Microsoft.Identity.Client;

namespace Pictua.XFUI.Droid
{
    [Activity]
    [IntentFilter(new[] { Intent.ActionView },
        Categories = new[] { Intent.CategoryBrowsable, Intent.CategoryDefault },
        DataHost = "auth",
        DataScheme = "msalfd564916-e1a7-41aa-9e2e-5867cac60129")]
    public class MsalActivity : BrowserTabActivity
    {
    }
}