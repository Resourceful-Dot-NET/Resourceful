using System.IO;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Resourceful.Xamarin.TestApp {
  public partial class TestAppPage : ContentPage {
    public TestAppPage() {
      InitializeComponent();

      ResourceManager.Default.BindToEmbeddedResource("TestResources/test.txt", res => Label.Text = res);
    }
  }
}