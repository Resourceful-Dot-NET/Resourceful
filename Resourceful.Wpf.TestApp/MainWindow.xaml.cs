using System.Windows;

namespace Resourceful.Wpf.TestApp {
  /// <summary>
  /// Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow {
    public MainWindow() {
      InitializeComponent();

      ResourceManager.Default
        .BindToEmbeddedResource("TestResources/test.txt",
          res => Application.Current.Dispatcher.Invoke(() => Label.Text = res));
    }
  }
}
