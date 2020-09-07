using DLToolkit.Forms.Controls;
using Pictua.XFUI.UWP;
using Xamarin.Forms;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(FlowListView), typeof(CustomListViewRenderer))]

namespace Pictua.XFUI.UWP
{
    public class CustomListViewRenderer : ListViewRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (List != null)
            {
                List.SelectionMode = Windows.UI.Xaml.Controls.ListViewSelectionMode.None;
            }
        }
    }
}