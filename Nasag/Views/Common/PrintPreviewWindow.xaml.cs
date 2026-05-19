using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Nasag.Services.Printing;

namespace Nasag.Views.Common;

public partial class PrintPreviewWindow : Window
{
    public PrintPreviewWindow(FlowDocument document, string description)
    {
        InitializeComponent();
        Title = description;
        TitleLabel.Text = description;

        // A4 at 96 DPI
        document.PageWidth = 793;
        document.PageHeight = 1122;
        document.PagePadding = new Thickness(48);
        document.ColumnGap = 0;
        document.ColumnWidth = 793;
        Viewer.Document = document;

        // Allow dragging the window from the top bar area since we removed system chrome.
        MouseLeftButtonDown += (_, _) =>
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                try { DragMove(); } catch { /* DragMove can throw if not in the right state — ignore. */ }
            }
        };
    }

    private void OnPrintClick(object sender, RoutedEventArgs e)
    {
        if (Viewer.Document is FlowDocument doc)
            PrintService.Print(doc, Title ?? "Document");
    }

    private void OnCloseClick(object sender, RoutedEventArgs e) => Close();
}
