using System.IO;
using System.Reflection;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Resourceful.Xamarin.TestApp {
  public partial class TestAppPage : ContentPage {
    public TestAppPage() {
      InitializeComponent();

      ResourceManager.Default
        .BindToEmbeddedResource("TestResources/test.txt",
          res => Device.BeginInvokeOnMainThread(() => Label.Text = res))
        .BindToEmbeddedResource("TestResources/font-size.txt",
          res => Device.BeginInvokeOnMainThread(() => Label.FontSize = double.Parse(res)));


    }
  }
}