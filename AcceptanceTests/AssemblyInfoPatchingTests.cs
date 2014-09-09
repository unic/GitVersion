
namespace AcceptanceTests
{
    using System;
    using System.IO;
    using System.Linq;
    using GitVersion;
    using Helpers;
    using LibGit2Sharp;
    using Xunit;

    public class AssemblyInfoPatchingTests
    {
        [Fact]
        public void SimpleAssemblyInfoPatching()
        {
            using (var fixture = new EmptyRepositoryFixture())
            {
                fixture.Repository.MakeATaggedCommit("1.3.0");
                fixture.Repository.CreateBranch("develop").Checkout();
                var path = Path.Combine(fixture.RepositoryPath, "AssemblyInfo.cs");
                const string content = @"[assembly: AssemblyVersion(""1.0.0.0"")]
[assembly: AssemblyFileVersion(""1.0.0.0"")]
[assembly: AssemblyInformationalVersion(""1.0.0.0"")]";
                File.WriteAllLines(path, content.Split('\n'));
                fixture.Repository.Index.Stage(path);
                fixture.Repository.Commit("assemblyinfo");
                var result = GitVersionHelper.ExecuteIn(fixture.RepositoryPath, updateAssemblyInfo: true);
                Assert.Equal(result.OutputVariables[VariableProvider.AssemblyFileVersion], "1.4.0.0");
                var afterContent = File.ReadAllLines(path);
                afterContent.ToList().ForEach(Console.WriteLine);
                Assert.Contains("[assembly: AssemblyVersion(\"1.4.0.0\")]", afterContent);
            }
        }
    }
}
