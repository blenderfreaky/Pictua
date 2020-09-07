using DLToolkit.Forms.Controls;
using DLToolkitControlsSamples.iOS;
using UIKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(FlowListViewInternalCell), typeof(FlowListViewInternalCellRenderer))]

namespace DLToolkitControlsSamples.iOS
{
    // DISABLES FLOWLISTVIEW ROW HIGHLIGHT
    public class FlowListViewInternalCellRenderer : ViewCellRenderer
    {
        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            tv.AllowsSelection = false;
            var cell = base.GetCell(item, reusableCell, tv);
            cell.SelectionStyle = UITableViewCellSelectionStyle.None;

            return cell;
        }
    }
}