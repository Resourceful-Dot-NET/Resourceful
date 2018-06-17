<p align="center">
  <img width="1174" src="https://github.com/Resourceful-Dot-NET/Resourceful/raw/master/Media/Logo.png">
</p>

# Resourceful
Resourceful is a .NET resource manager and live reloader 🙂
## Why Resourceful?
Resourceful offers a extendable base API for loading any resource embedded into the assembly in a uniform fashion. Additionally, plugins can support live reloading functionality of different types of resources.

# Current State
Resourceful is currently pre-release. It functions properly but is still considered an alpha as the API fluctuates.

# Getting Started
## Get The Library
The library is what actually interfaces with your code and handles plugins and updates to resources.


Grab the Nuget package: https://www.nuget.org/packages/Resourceful.Net/
## Get the CLI
The CLI is used to serve updated resource assets to your app while it runs. It is only needed for live reload/updates. If you're just using Resourceful as a resource manager, you can skip this step.


Use the following command to install the CLI tool globally:
```
dotnet tool install --global Resourceful.Net.CLI
```
If that command fails to run, you likely need version 2.1 of the .Net Core SDK. You can get that here: https://www.microsoft.com/net/download/

Note: Your project doesn't need to be a .Net Core app to use Resourceful. .Net Core is just needed to run the CLI, which is otherwise a stand alone program.

## Using Resourceful

Out of the box, Resourceful can load and live update "embedded resources". With plugins, Resourceful can potentially load and live update any type of resource that is embedded in the assembly or otherwise available to read at runtime. Resource can work with any type of .Net assembly, whether it's a console application or a Xamarin Form app. 

Here's an example that takes an embedded resource text file and uses the data to set a text label in WPF:
```
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
```

`ResourceManager` is a reference to the static class that handles all resource managers. `Default` is the default resource manager, which is the resource manager for the calling assembly (in this case - our WPF app). `BindToEmbeddedResource` is a method whose first argument should be the path to the embedded resource relative to the application; The second argument is a callback lambda that is executed initially when `BindToEmbeddedResource` is called and then everytime the resource updates.

To do this in Xamarin Forms, the code is almost exactly the same except for our callback:

```
      ResourceManager.Default
        .BindToEmbeddedResource("TestResources/test.txt",
          res => Device.BeginInvokeOnMainThread(() => Label.Text = res))
```

If you installed the CLI, we can test live updates.
Run the command `resourceful-cli` in the directory of your `.csproj` file. (It's always best to run this as close to the `.csproj` as possible because this command will recursively crawl up the directory heirarchy looking for `.csproj` files.)


Once the CLI is watching, try changing  the content of the text file. Your label should update in realtime after file save as the application is running. 😊

More documentation to come 

# Plugins
Currently no official plugins are available yet. The first official plugin will be for Xamarin Forms and will feature live reloading XAML pages.

# To Do
- [x] Write basic functioning library and server
- [x] Make Nuget packages
- [ ] Write unit tests (WIP)
- [ ] Write working plugin
- [x] Write WPF integration tests
- [ ] Write configuration code
