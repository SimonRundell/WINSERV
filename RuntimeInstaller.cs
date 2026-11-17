// RuntimeInstaller.cs
// ─────────────────────────────────────────────────────────────────────────────
// Handles extracting the PHP and MariaDB portable distributions from
// WINSERV-runtime.zip on first run (or when the runtime is missing).
//
// Deployment model:
//   WINSERV.exe          ← the launcher/GUI (this project)
//   WINSERV-runtime.zip  ← PHP + MariaDB + Node portable binaries, shipped separately
//
// Both files should be in the same folder. On first run (or if the install
// path's runtime folders are absent), WINSERV extracts the zip to the
// configured InstallPath.
//
// The runtime zip should have this internal structure:
//   php\
//     php.exe
//     php.ini-development   (we generate our own php.ini)
//     ext\
//       *.dll
//   mariadb\
//     bin\
//       mysqld.exe
//       mysql.exe
//       mysqladmin.exe
//       ...
//     share\
//       ...
//   node\
//     node.exe
//     npm.cmd
//     npx.cmd
//     node_modules\
//       ...
//   adminer.php   (optional — see DeployAdminer() below)
//
// Unlike PHP and MariaDB, Node is not a background service WINSERV starts or
// stops — it's portable tooling students invoke themselves (node/npm/npx)
// from the VS Code integrated terminal. Since it's not under Program Files,
// npm install and npm install -g both work without elevation, writing into
// {InstallPath}\node\node_modules by default.
//
// adminer.php, if present at the zip root, is moved by DeployAdminer() into
// {InstallPath}\tools\adminer\adminer.php, where PHP's router script expects
// to find it so a fixed /adminer/adminer.php URL always reaches it.
//
// C# concept — progress reporting (IProgress<T>):
//   IProgress<T> is a standard .NET interface for reporting progress from a
//   background operation to the UI. The UI creates a Progress<T> object,
//   which captures the UI's SynchronizationContext so that the callback
//   automatically marshals back to the UI thread — you don't need to
//   call Invoke() yourself. You pass it to the background method, which
//   calls progress.Report(value) as work proceeds.
// ─────────────────────────────────────────────────────────────────────────────

using System.IO.Compression;

namespace WINSERV;

/// <summary>
/// Checks for and extracts the WINSERV runtime (PHP + MariaDB).
/// </summary>
public static class RuntimeInstaller
{
    // Expected filename of the runtime zip, placed next to WINSERV.exe.
    public const string RuntimeZipName = "WINSERV-runtime.zip";

    // ── Public API ───────────────────────────────────────────────────────────

    /// <summary>
    /// Returns the expected path of the runtime zip file.
    /// </summary>
    public static string GetRuntimeZipPath()
    {
        return Path.Combine(AppContext.BaseDirectory, RuntimeZipName);
    }

    /// <summary>
    /// Returns true if both the PHP executable and MariaDB executable
    /// exist at the paths defined in config. Existence of the executables
    /// is our signal that extraction has already been done.
    /// </summary>
    public static bool IsRuntimeExtracted(AppConfig config)
    {
        return File.Exists(config.PhpExePath) &&
               File.Exists(config.MariaDbExePath);
    }

    /// <summary>
    /// Returns true if node.exe exists at the path defined in config.
    /// Checked separately from IsRuntimeExtracted because Node is optional
    /// tooling, not a service WINSERV manages — an older runtime zip built
    /// before Node support was added would otherwise never satisfy this.
    /// </summary>
    public static bool IsNodeExtracted(AppConfig config)
    {
        return File.Exists(config.NodeExePath);
    }

    /// <summary>
    /// Returns true if the runtime zip file is present next to WINSERV.exe.
    /// </summary>
    public static bool IsRuntimeZipPresent()
    {
        return File.Exists(GetRuntimeZipPath());
    }

    /// <summary>
    /// Extracts the runtime zip to config.InstallPath asynchronously.
    ///
    /// Parameters:
    ///   config    — provides the target InstallPath
    ///   progress  — receives (0-100) percentage updates for a progress bar
    ///   log       — receives string messages for a log display
    ///
    /// Returns true on success.
    ///
    /// C# concept — async Task<bool>:
    ///   This method is async (it uses await internally) and returns a bool
    ///   wrapped in a Task. The caller awaits it: 'bool ok = await ExtractAsync(...)'
    /// </summary>
    public static async Task<bool> ExtractAsync(
        AppConfig config,
        IProgress<int>? progress = null,
        IProgress<string>? log = null)
    {
        string zipPath = GetRuntimeZipPath();

        if (!File.Exists(zipPath))
        {
            log?.Report($"ERROR: Runtime zip not found at {zipPath}");
            log?.Report($"Please place {RuntimeZipName} in the same folder as WINSERV.exe.");
            return false;
        }

        // Ensure the install path exists. Directory.CreateDirectory is safe
        // to call even if the directory already exists.
        Directory.CreateDirectory(config.InstallPath);

        log?.Report($"Extracting runtime to {config.InstallPath}...");
        log?.Report($"Source: {zipPath}");

        try
        {
            // Run the extraction on a background thread so the UI stays responsive.
            // Task.Run() executes the lambda on the ThreadPool.
            await Task.Run(() => ExtractZip(zipPath, config.InstallPath, progress, log));

            // Ensure all required subdirectories exist even if they were empty
            // or absent in the zip (e.g. empty webroot, logs).
            Directory.CreateDirectory(config.WebRoot);
            Directory.CreateDirectory(config.DataDirectory);
            Directory.CreateDirectory(config.LogDirectory);
            Directory.CreateDirectory(Path.Combine(config.InstallPath, "sessions"));

            WriteDefaultIndexPage(config.WebRoot, log);
            WriteTestPage(config.WebRoot, log);
            WriteVsCodeSettings(config, log);
            DeployAdminer(config, log);

            await DownloadCaBundleAsync(config.InstallPath, log);

            log?.Report("Extraction complete.");
            progress?.Report(100);
            return true;
        }
        catch (Exception ex)
        {
            log?.Report($"Extraction failed: {ex.Message}");
            return false;
        }
    }

    // ── Private ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Downloads the Mozilla/curl CA certificate bundle (cacert.pem) from
    /// curl.se and saves it to the PHP directory. This enables cURL and
    /// OpenSSL to verify HTTPS certificates on Windows, which ships without
    /// a system CA store accessible to PHP.
    /// Silently skips if the file already exists or the download fails.
    /// </summary>
    private static async Task DownloadCaBundleAsync(string installPath, IProgress<string>? log)
    {
        string phpDir   = Path.Combine(installPath, "php");
        string destPath = Path.Combine(phpDir, "cacert.pem");

        if (File.Exists(destPath))
        {
            log?.Report("cacert.pem already present — skipping download.");
            return;
        }

        if (!Directory.Exists(phpDir))
        {
            log?.Report("PHP directory not found — skipping cacert.pem download.");
            return;
        }

        log?.Report("Downloading CA certificate bundle (cacert.pem) from curl.se...");

        try
        {
            using var http     = new System.Net.Http.HttpClient();
            http.Timeout       = TimeSpan.FromSeconds(30);
            var bytes          = await http.GetByteArrayAsync(
                "https://curl.se/ca/cacert.pem");
            await Task.Run(() => File.WriteAllBytes(destPath, bytes));
            log?.Report($"cacert.pem downloaded ({bytes.Length / 1024} KB) — HTTPS/SSL ready.");
        }
        catch (Exception ex)
        {
            log?.Report($"Warning: could not download cacert.pem: {ex.Message}");
            log?.Report("HTTPS cURL requests may fail. Download manually from https://curl.se/ca/cacert.pem");
        }
    }

    /// <summary>
    /// Extracts a zip file to a destination folder, reporting progress.
    /// This is a synchronous method intended to run on a background thread.
    /// </summary>
    private static void ExtractZip(
        string zipPath,
        string destFolder,
        IProgress<int>? progress,
        IProgress<string>? log)
    {
        using ZipArchive archive = ZipFile.OpenRead(zipPath);

        // Detect a common top-level folder in the zip (e.g. "WINSERV-runtime/")
        // so we can strip it and extract php\ and mariadb\ directly into destFolder.
        // We find the prefix by taking the first path segment that all entries share.
        string stripPrefix = DetectCommonPrefix(archive);
        if (!string.IsNullOrEmpty(stripPrefix))
            log?.Report($"  Stripping zip prefix: {stripPrefix}");

        int total   = archive.Entries.Count;
        int current = 0;

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            current++;

            int percent = (int)((double)current / total * 100);
            progress?.Report(percent);

            // Remove the common prefix so entries like "WINSERV-runtime/php/php.exe"
            // become "php/php.exe" and land directly under destFolder.
            string relativeName = entry.FullName;
            if (!string.IsNullOrEmpty(stripPrefix) &&
                relativeName.StartsWith(stripPrefix, StringComparison.OrdinalIgnoreCase))
            {
                relativeName = relativeName.Substring(stripPrefix.Length);
            }

            // Skip the prefix directory entry itself (now an empty string).
            if (string.IsNullOrEmpty(relativeName))
                continue;

            string destPath = Path.Combine(destFolder, relativeName);

            // Directory entries — just create, don't extract.
            if (relativeName.EndsWith("/") || relativeName.EndsWith("\\"))
            {
                Directory.CreateDirectory(destPath);
                continue;
            }

            string? parentDir = Path.GetDirectoryName(destPath);
            if (parentDir != null)
                Directory.CreateDirectory(parentDir);

            entry.ExtractToFile(destPath, overwrite: true);

            if (current % 50 == 0 || current == total)
                log?.Report($"  Extracted {current}/{total}: {relativeName}");
        }
    }

    /// <summary>
    /// If every entry in the archive shares the same first path segment
    /// (e.g. "WINSERV-runtime/"), returns that segment with its trailing slash
    /// so callers can strip it. Returns an empty string when there is no
    /// single common root folder.
    /// </summary>
    private static string DetectCommonPrefix(ZipArchive archive)
    {
        string? prefix = null;

        foreach (ZipArchiveEntry entry in archive.Entries)
        {
            int slash = entry.FullName.IndexOfAny(new char[] { '/', '\\' });
            if (slash < 0)
                return string.Empty;  // An entry lives at the root — no prefix to strip.

            string segment = entry.FullName.Substring(0, slash + 1);  // e.g. "WINSERV-runtime/"

            if (prefix == null)
                prefix = segment;
            else if (!string.Equals(prefix, segment, StringComparison.OrdinalIgnoreCase))
                return string.Empty;  // Entries have different roots.
        }

        return prefix ?? string.Empty;
    }

    /// <summary>
    /// Writes a default index.php to the webroot if one does not already exist,
    /// so the user can immediately verify that both PHP and the web server are working.
    /// </summary>
    private static void WriteDefaultIndexPage(string webRoot, IProgress<string>? log)
    {
        string indexPath = Path.Combine(webRoot, "index.php");

        if (File.Exists(indexPath))
            return;

        const string content = """
            <!DOCTYPE html>
            <html lang="en">
            <head>
                <meta charset="UTF-8">
                <title>WINSERV</title>
                <style>
                    body { font-family: Segoe UI, sans-serif; max-width: 860px; margin: 60px auto; color: #333; }
                    h1   { color: #0078d4; }
                    .ok  { display: inline-block; background: #d4edda; color: #155724;
                           border: 1px solid #c3e6cb; border-radius: 4px; padding: 4px 12px; }
                    table.info { width: 100%; border-collapse: collapse; margin-top: 24px; font-size: 0.9em; }
                    table.info th, table.info td { text-align: left; padding: 6px 10px; border-bottom: 1px solid #dee2e6; }
                    table.info th { background: #f8f9fa; width: 200px; color: #000000; }
                </style>
            </head>
            <body>
                <h1>WINSERV is running</h1>
                <p><span class="ok">&#10003; PHP is active</span></p>

                <table class="info">
                    <tr><th>PHP version</th>    <td><?= phpversion() ?></td></tr>
                    <tr><th>Server software</th><td><?= $_SERVER['SERVER_SOFTWARE'] ?? 'PHP built-in server' ?></td></tr>
                    <tr><th>Document root</th>  <td><?= $_SERVER['DOCUMENT_ROOT'] ?></td></tr>
                    <tr><th>Date / time</th>    <td><?= date('Y-m-d H:i:s') ?></td></tr>
                </table>

                <details style="margin-top:32px">
                    <summary style="cursor:pointer;color:#0078d4">Full phpinfo()</summary>
                    <?php phpinfo(); ?>
                </details>
            </body>
            </html>
            """;

        File.WriteAllText(indexPath, content);
        log?.Report($"  Created default index.php in {webRoot}");
    }

    /// <summary>
    /// Writes .vscode/settings.json into the webroot so the VS Code integrated
    /// terminal automatically picks up node/npm/npx on PATH, without touching
    /// the machine's or user's system PATH (which lab machines may reset on
    /// re-image, or students may not have permission to edit at all).
    /// Skips if the file already exists so a customised copy is not overwritten
    /// — if InstallPath is changed later via Settings, delete the file to have
    /// it regenerated on the next extraction.
    /// </summary>
    private static void WriteVsCodeSettings(AppConfig config, IProgress<string>? log)
    {
        string vscodeDir    = Path.Combine(config.WebRoot, ".vscode");
        string settingsPath = Path.Combine(vscodeDir, "settings.json");

        if (File.Exists(settingsPath))
            return;

        // Escaped for JSON: backslashes doubled.
        string nodeDirJson = config.NodeDirectory.Replace("\\", "\\\\");

        string content = $$"""
            {
                "terminal.integrated.env.windows": {
                    "PATH": "{{nodeDirJson}};${env:PATH}"
                }
            }
            """;

        Directory.CreateDirectory(vscodeDir);
        File.WriteAllText(settingsPath, content);
        log?.Report($"  Created .vscode/settings.json in {config.WebRoot} (node/npm/npx on terminal PATH)");
    }

    /// <summary>
    /// Writes test.php to the webroot if it does not already exist.
    /// Skips if the file is present so a customised copy is not overwritten.
    /// </summary>
    private static void WriteTestPage(string webRoot, IProgress<string>? log)
    {
        string testPath = Path.Combine(webRoot, "test.php");

        if (File.Exists(testPath))
            return;

        // Read the content from the embedded resource bundled at build time.
        // The resource name matches the project namespace + filename.
        var assembly   = System.Reflection.Assembly.GetExecutingAssembly();
        using var stream = assembly.GetManifestResourceStream("WINSERV.test.php");

        if (stream == null)
        {
            log?.Report("  Warning: test.php resource not found in assembly — skipping.");
            return;
        }

        using var reader = new System.IO.StreamReader(stream);
        File.WriteAllText(testPath, reader.ReadToEnd());
        log?.Report($"  Created test.php in {webRoot}");
    }

    // ── Adminer ──────────────────────────────────────────────────────────────

    /// <summary>Result of <see cref="DeployAdminer"/>.</summary>
    public enum AdminerDeployResult
    {
        /// <summary>adminer.php was already at config.AdminerPhpPath — nothing to do.</summary>
        AlreadyPresent,

        /// <summary>A source copy was found and copied into place.</summary>
        Deployed,

        /// <summary>No source copy of adminer.php could be found anywhere.</summary>
        SourceNotFound,
    }

    /// <summary>
    /// Deploys the vendored Adminer single-file build to
    /// config.AdminerPhpPath, where PHP's router script (see
    /// PhpServer.EnsureRouterPhp()) expects to find it so that
    /// /adminer/adminer.php works regardless of WebRoot.
    ///
    /// Adminer is not bundled as an embedded resource like test.php, because
    /// unlike test.php it is third-party code — it is vendored into the
    /// deployment package the same way PHP/MariaDB/Node are: dropped at the
    /// root of WINSERV-runtime.zip (see BUILD.md), which lands it at
    /// {InstallPath}\adminer.php after extraction. As a manual fallback (e.g.
    /// re-running this from Settings after adding Adminer to an existing
    /// install without re-running the full wizard), a copy placed next to
    /// WINSERV.exe is also accepted.
    ///
    /// Called automatically during ExtractAsync(), and exposed publicly so
    /// the Settings dialog's "Deploy Adminer" button can re-run it on demand.
    /// </summary>
    public static AdminerDeployResult DeployAdminer(AppConfig config, IProgress<string>? log = null)
    {
        if (File.Exists(config.AdminerPhpPath))
        {
            log?.Report("[Adminer] adminer.php already deployed — skipping.");
            return AdminerDeployResult.AlreadyPresent;
        }

        string[] candidates =
        {
            Path.Combine(config.InstallPath, "adminer.php"),
            Path.Combine(AppContext.BaseDirectory, "adminer.php"),
        };

        string? source = null;
        foreach (string candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                source = candidate;
                break;
            }
        }

        if (source == null)
        {
            log?.Report("[Adminer] adminer.php not found. Add it to the root of " +
                        $"{RuntimeZipName} (see BUILD.md) or place a copy next to " +
                        "WINSERV.exe, then try again.");
            return AdminerDeployResult.SourceNotFound;
        }

        Directory.CreateDirectory(config.AdminerDirectory);
        File.Copy(source, config.AdminerPhpPath, overwrite: true);
        log?.Report($"[Adminer] Deployed adminer.php to {config.AdminerPhpPath}");
        return AdminerDeployResult.Deployed;
    }
}
