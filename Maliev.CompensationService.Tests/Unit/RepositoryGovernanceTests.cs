namespace Maliev.CompensationService.Tests.Unit;

/// <summary>
/// Protects the validation-only repository and immutable public dependency boundaries.
/// </summary>
public sealed class RepositoryGovernanceTests
{
    private static readonly string[] WorkflowNames =
    [
        "_validate.yml",
        "ci-develop.yml",
        "ci-main.yml",
        "ci-staging.yml",
        "pr-validation.yml"
    ];

    [Fact]
    public void Workflows_AreValidationOnlyAndCredentialFree()
    {
        foreach (var workflowName in WorkflowNames)
        {
            var source = ReadRepositoryFile(".github", "workflows", workflowName);

            Assert.DoesNotContain("google-github-actions/auth", source, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("gcloud", source, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("docker push", source, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("push: true", source, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("maliev-gitops", source, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("GCP_SA_KEY", source, StringComparison.Ordinal);
            Assert.DoesNotContain("GITOPS_PAT", source, StringComparison.Ordinal);
            Assert.DoesNotContain("secrets.", source, StringComparison.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void ReusableValidation_ContainsRequiredGatesAndImmutablePins()
    {
        var source = ReadRepositoryFile(".github", "workflows", "_validate.yml");

        Assert.Contains("--locked-mode", source, StringComparison.Ordinal);
        Assert.Contains("--vulnerable --include-transitive", source, StringComparison.Ordinal);
        Assert.Contains("dotnet format", source, StringComparison.Ordinal);
        Assert.Contains("dotnet build", source, StringComparison.Ordinal);
        Assert.Contains("dotnet test", source, StringComparison.Ordinal);
        Assert.Contains("gitleaks_8.30.1_linux_x64.tar.gz", source, StringComparison.Ordinal);
        Assert.Contains("551f6fc83ea457d62a0d98237cbad105af8d557003051f41f3e7ca7b3f2470eb", source, StringComparison.Ordinal);
        Assert.Contains("./gitleaks dir CompensationService --no-banner --redact", source, StringComparison.Ordinal);
        Assert.DoesNotContain("gitleaks/gitleaks-action", source, StringComparison.Ordinal);
        Assert.Contains("aquasecurity/trivy-action@ed142fd0673e97e23eac54620cfb913e5ce36c25", source, StringComparison.Ordinal);
        Assert.Contains("repository: MALIEV-Co-Ltd/Maliev.Aspire", source, StringComparison.Ordinal);
        Assert.Contains("ref: 25a5c3b2d3d6b5ce8ed485d2d44a28f4dc4c9b51", source, StringComparison.Ordinal);
        Assert.Contains("repository: MALIEV-Co-Ltd/Maliev.MessagingContracts", source, StringComparison.Ordinal);
        Assert.Contains("ref: 559a00db0c7920a5247fdff60d4476ad23a9a501", source, StringComparison.Ordinal);
        Assert.DoesNotContain("uses: actions/checkout@v", source, StringComparison.Ordinal);
        Assert.DoesNotContain("uses: actions/setup-dotnet@v", source, StringComparison.Ordinal);
    }

    [Fact]
    public void Projects_UsePublicSourceReferencesWithoutCiPackageSwitches()
    {
        var api = ReadRepositoryFile("Maliev.CompensationService.Api", "Maliev.CompensationService.Api.csproj");
        var application = ReadRepositoryFile("Maliev.CompensationService.Application", "Maliev.CompensationService.Application.csproj");
        var infrastructure = ReadRepositoryFile("Maliev.CompensationService.Infrastructure", "Maliev.CompensationService.Infrastructure.csproj");
        var tests = ReadRepositoryFile("Maliev.CompensationService.Tests", "Maliev.CompensationService.Tests.csproj");
        var combined = string.Join(Environment.NewLine, api, application, infrastructure, tests);

        Assert.Contains("Maliev.Aspire.ServiceDefaults.csproj", combined, StringComparison.Ordinal);
        Assert.Contains("Maliev.MessagingContracts.csproj", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("GITHUB_ACTIONS", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("PackageReference Include=\"Maliev.Aspire.ServiceDefaults\"", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("PackageReference Include=\"Maliev.MessagingContracts\"", combined, StringComparison.Ordinal);
        Assert.DoesNotContain("1.0.*", combined, StringComparison.Ordinal);
    }

    [Fact]
    public void NuGetAndDockerBuild_AreCredentialFreeAndLocked()
    {
        var nuget = ReadRepositoryFile("nuget.config");
        var dockerfile = ReadRepositoryFile("Maliev.CompensationService.Api", "Dockerfile");
        var dockerIgnore = ReadRepositoryFile(".dockerignore");

        Assert.Contains("https://api.nuget.org/v3/index.json", nuget, StringComparison.Ordinal);
        Assert.DoesNotContain("nuget.pkg.github.com", nuget, StringComparison.OrdinalIgnoreCase);
        Assert.DoesNotContain("packageSourceCredentials", nuget, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("COPY --from=aspire . /Maliev.Aspire/", dockerfile, StringComparison.Ordinal);
        Assert.Contains("COPY --from=messaging . /Maliev.MessagingContracts/", dockerfile, StringComparison.Ordinal);
        Assert.Contains("--locked-mode", dockerfile, StringComparison.Ordinal);
        Assert.DoesNotContain("nuget_password", dockerfile, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("**/bin/", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains("**/obj/", dockerIgnore, StringComparison.Ordinal);
        Assert.Contains(".git/", dockerIgnore, StringComparison.Ordinal);
    }

    private static string ReadRepositoryFile(params string[] segments)
    {
        var path = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,
            "..",
            "..",
            "..",
            "..",
            Path.Combine(segments)));

        Assert.True(File.Exists(path), $"Could not find source file: {path}");
        return File.ReadAllText(path);
    }
}
