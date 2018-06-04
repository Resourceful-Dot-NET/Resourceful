using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using ResourcefulServer;
using Xunit;

namespace ResourcefulServerTests {
  public class FileWatcherTests {

    private static string ValidWatchablePath { get; } =
      Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "../../"));
    private static string TestTxtPath { get; } =
      Path.GetFullPath(Path.Combine(ValidWatchablePath, "./Test.txt"));

    public static IEnumerable<object[]> FileWatcher_IsPassedValidData_CreatesWatcher_Data() {
      yield return new object[] { null, false };
      yield return new object[] { null, true };
      yield return new object[] { "", null };
      yield return new object[] { null, null };
      yield return new object[] { "", false };
      yield return new object[] { "", true };
      yield return new object[] { ValidWatchablePath, true };
      yield return new object[] { ValidWatchablePath, false };
      yield return new object[] { ValidWatchablePath, null };
    }

    public static IEnumerable<object[]> CurrentWatchingPath_IsPassedValidPath_DoesNotThrowException_Data() {
      yield return new object[] { ValidWatchablePath };
    }

    private FileWatcher constructFileWatcher(string path, bool? startWatching) {
      FileWatcher fileWatcher;

      if (path == null && startWatching == null) {
        fileWatcher = new FileWatcher();
      }
      else if (path == null) {
        fileWatcher = new FileWatcher(startWatching: (bool)startWatching);
      }
      else if (startWatching == null) {
        fileWatcher = new FileWatcher(path: path);
      }
      else {
        fileWatcher = new FileWatcher(path: path, startWatching: (bool)startWatching);
      }

      return fileWatcher;
    }

    [Theory]
    [MemberData(nameof(FileWatcher_IsPassedValidData_CreatesWatcher_Data))]
    public void FileWatcher_IsPassedValidData_CreatesWatcher(string path, bool? startWatching) {
      var fileWatcher = constructFileWatcher(path, startWatching);

      fileWatcher.CurrentWatchingPath.Should().NotBeNullOrWhiteSpace();
      fileWatcher.Watcher.Should().NotBeNull();
      fileWatcher.Watcher.EnableRaisingEvents.Should().Be(startWatching ?? true);
      Directory.Exists(fileWatcher.CurrentWatchingPath).Should().BeTrue();
    }

    [Theory]
    [InlineData("/This/Is/A/Bad/Path")]
    [InlineData("asdasd")]
    [InlineData("!@#$!")]
    [InlineData("http://test.com/")]
    public void FileWatcher_IsPassedInvalidPath_ThrowsException(string path) {
      Action constructAction = () => new FileWatcher(path);

      constructAction.Should().Throw<IOException>();
    }

    [Theory]
    [InlineData("/This/Is/A/Bad/Path")]
    [InlineData("asdasd")]
    [InlineData("!@#$!")]
    [InlineData("http://test.com/")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void CurrentWatchingPath_IsPassedInvalidPath_ThrowsException(string path) {
      var fileWatcher = new FileWatcher();

      fileWatcher.Invoking(fw => fw.CurrentWatchingPath = path).Should().Throw<IOException>();
    }

    [Theory]
    [MemberData(nameof(CurrentWatchingPath_IsPassedValidPath_DoesNotThrowException_Data))]
    public void CurrentWatchingPath_IsPassedValidPath_DoesNotThrowException(string path) {
      var fileWatcher = new FileWatcher();

      fileWatcher.Invoking(fw => fw.CurrentWatchingPath = path).Should().NotThrow();

      fileWatcher.CurrentWatchingPath.Should().NotBeNullOrWhiteSpace();
      Directory.Exists(fileWatcher.CurrentWatchingPath).Should().BeTrue();
    }

    [Fact]
    public async Task ResourceModified_WhenFileChanges_IsFired() {
      var fileWatcher = new FileWatcher(ValidWatchablePath);

      using (var monitoredFileWatcher = fileWatcher.Monitor()) {
        File.WriteAllText(TestTxtPath, File.ReadAllText(TestTxtPath) + "s");
        await Task.Delay(1000);
        monitoredFileWatcher.Should().Raise(nameof(fileWatcher.ResourceModified));
      }
    }

    [Fact]
    public async Task Stops_IsCalled_StopsRaisingEvents() {
      ResetTestFileData();

      try {
        var fileWatcher = new FileWatcher(ValidWatchablePath);

        fileWatcher.Stop();

        using (var monitoredFileWatcher = fileWatcher.Monitor()) {
          File.WriteAllText(TestTxtPath, File.ReadAllText(TestTxtPath) + "s");
          await Task.Delay(1000);
          monitoredFileWatcher.Should().NotRaise(nameof(fileWatcher.ResourceModified));
        }
      }
      finally {
        ResetTestFileData();
      }
    }

    [Fact]
    public async Task Resume_IsCalled_ContinuesRaisingEvents() {
      ResetTestFileData();

      try {
        var fileWatcher = new FileWatcher(ValidWatchablePath);

        fileWatcher.Stop();
        fileWatcher.Resume();

        using (var monitoredFileWatcher = fileWatcher.Monitor()) {
          File.WriteAllText(TestTxtPath, File.ReadAllText(TestTxtPath) + "s");
          await Task.Delay(1000);
          monitoredFileWatcher.Should().Raise(nameof(fileWatcher.ResourceModified));
        }
      }
      finally {
        ResetTestFileData();
      }
    }

    private static void ResetTestFileData() {
      File.WriteAllText(TestTxtPath, "Test Content");
    }
  }
}
