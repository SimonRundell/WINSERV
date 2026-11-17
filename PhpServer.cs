// PhpServer.cs
// ─────────────────────────────────────────────────────────────────────────────
// Manages the PHP built-in web server process.
//
// The PHP built-in server is launched with:
//   php.exe -S localhost:8080 -t C:\WINSRV\webroot C:\WINSRV\tools\router.php
//
// Flags explained:
//   -S host:port   Start the built-in web server on this address/port
//   -t path        Set the document root (the folder files are served from)
//   <script>       A router script run for every request. Returning false
//                   from it falls through to the normal -t docroot handling;
//                   WINSERV uses this to serve a fixed /adminer/adminer.php
//                   URL regardless of which project WebRoot points at. See
//                   EnsureRouterPhp() below.
//
// The built-in server is single-threaded: it handles one request at a time.
// For development tooling used by one developer this is perfectly adequate.
// If concurrency becomes a concern, the architecture can be extended to
// use Nginx + PHP-FPM, but that adds significant complexity.
//
// C# concept — inheritance (':' operator):
//   'class PhpServer : ProcessWrapper' means PhpServer inherits from
//   ProcessWrapper. PhpServer gets all of ProcessWrapper's code and must
//   provide concrete implementations for ProcessWrapper's abstract members.
//   The 'override' keyword on a method means "I am replacing the abstract
//   or virtual method defined in the base class."
// ─────────────────────────────────────────────────────────────────────────────

namespace WINSERV;

/// <summary>
/// Manages the PHP built-in web server process.
/// Inherits process lifecycle management from ProcessWrapper.
/// </summary>
public class PhpServer : ProcessWrapper
{
    // The current configuration (paths, port, webroot).
    // 'private readonly' = set once in the constructor, never changed.
    private readonly AppConfig _config;

    // ── Constructor ──────────────────────────────────────────────────────────

    // C# concept — constructors:
    //   A constructor has the same name as the class and no return type.
    //   It runs once when 'new PhpServer(config)' is called.
    //   'this._config = config' assigns the parameter to our private field.

    public PhpServer(AppConfig config)
    {
        _config = config;
    }

    // ── ProcessWrapper Overrides ─────────────────────────────────────────────

    // 'override' means we are providing the implementation for abstract
    // members declared in ProcessWrapper.

    /// <summary>Human-readable service name for log messages and UI labels.</summary>
    public override string ServiceName => "PHP";

    /// <summary>Full path to php.exe in the portable PHP distribution.</summary>
    protected override string ExecutablePath => _config.PhpExePath;

    /// <summary>
    /// Builds the command-line arguments for the PHP built-in server.
    /// Result example: -S localhost:8080 -t "C:\WINSRV\webroot" "C:\WINSRV\tools\router.php"
    ///
    /// All paths are quoted in case they contain spaces.
    /// </summary>
    protected override string BuildArguments()
    {
        // Ensure the webroot directory exists before PHP tries to serve from it.
        // Directory.CreateDirectory() is a no-op if the directory already exists,
        // so it is safe to call unconditionally.
        Directory.CreateDirectory(_config.WebRoot);

        // $"..." is a C# interpolated string — {expressions} are evaluated inline.
        // The router script is appended as PHP's built-in server's optional
        // third argument — see EnsureRouterPhp(), which must have written it
        // before Start() runs (ServiceManager calls both in sequence).
        return $"-S localhost:{_config.HttpPort} -t \"{_config.WebRoot}\" \"{_config.RouterPhpPath}\"";
    }

    // ── PHP-Specific Methods ─────────────────────────────────────────────────

    /// <summary>
    /// Writes a minimal php.ini to the PHP directory if one doesn't exist.
    /// php.ini controls PHP's behaviour: which extensions load, error reporting,
    /// upload limits, etc.
    ///
    /// The portable PHP zip ships with 'php.ini-development' and
    /// 'php.ini-production' as templates but no active php.ini.
    /// We generate a sensible development-oriented one.
    /// </summary>
    public void EnsurePhpIni()
    {
        string phpDir = Path.GetDirectoryName(_config.PhpExePath)!;
        string iniPath = _config.PhpIniPath;

        if (File.Exists(iniPath))
            return;  // Already exists — don't overwrite user's customisations

        OnOutputReceived("[PHP] Writing default php.ini...", isError: false);

        // The extension_dir must point to the 'ext' subdirectory of the PHP folder.
        // We use Path.Combine to build this correctly, then format it for INI syntax.
        // In php.ini, backslashes don't need escaping (it's not JSON).
        string extDir = Path.Combine(phpDir, "ext");

        // This is a multi-line C# verbatim string (@"...").
        // The @ prefix means backslashes are treated literally (no \n escape needed).
        // Everything between the quotes is the file content, including newlines.
        string phpIniContent = $@"; php.ini — generated by WINSERV
; Edit this file to customise PHP behaviour.
; Full documentation: https://www.php.net/manual/en/ini.php

[PHP]

; ── Error Reporting (development settings) ──────────────────────────────────
; Show all errors during development so problems are visible immediately.
; For production deployments, set error_reporting = E_ALL & ~E_DEPRECATED
error_reporting  = E_ALL
display_errors   = On
display_startup_errors = On
log_errors       = On
error_log        = ""{Path.Combine(_config.LogDirectory, "php_errors.log")}""

; ── File Uploads ─────────────────────────────────────────────────────────────
file_uploads     = On
upload_max_filesize = 64M
post_max_size    = 64M
max_file_uploads = 20

; ── Memory & Execution ───────────────────────────────────────────────────────
memory_limit     = 256M
max_execution_time = 60
max_input_time   = 60

; ── Extension Directory ───────────────────────────────────────────────────────
; This must point to the 'ext' folder inside your PHP installation.
extension_dir    = ""{extDir}""

; ── Extensions ───────────────────────────────────────────────────────────────
; Uncomment (remove the semicolon) to enable additional extensions.
; The .dll files must exist in extension_dir above.
extension=mysqli
extension=pdo_mysql
extension=mbstring
extension=openssl
extension=gd
extension=curl
extension=fileinfo
extension=zip
;extension=intl
;extension=soap

; ── SSL / CA Bundle ──────────────────────────────────────────────────────────
; Path to the CA certificate bundle for cURL and OpenSSL HTTPS requests.
; Download the latest bundle from: https://curl.se/ca/cacert.pem
; Place it next to php.ini as 'cacert.pem' — WINSERV sets the path automatically.
curl.cainfo   = ""{Path.Combine(phpDir, "cacert.pem")}""
openssl.cafile = ""{Path.Combine(phpDir, "cacert.pem")}""

; ── Session ───────────────────────────────────────────────────────────────────
session.save_path = ""{Path.Combine(_config.InstallPath, "sessions")}""
";

        // Ensure the sessions directory exists
        Directory.CreateDirectory(Path.Combine(_config.InstallPath, "sessions"));

        Directory.CreateDirectory(Path.GetDirectoryName(iniPath)!);
        File.WriteAllText(iniPath, phpIniContent);
        OnOutputReceived($"[PHP] php.ini written to {iniPath}", isError: false);
    }

    /// <summary>
    /// Writes the router script passed to PHP's built-in server (see
    /// BuildArguments() above). It intercepts /adminer, /adminer/ and
    /// /adminer/adminer.php so that URL always reaches the vendored Adminer
    /// build, no matter which project WebRoot currently points at. Every
    /// other request falls through ('return false') to PHP's normal
    /// -t docroot handling, unchanged from before the router existed.
    ///
    /// Unlike php.ini, this is always regenerated — it is WINSERV's own
    /// script, not something a student is meant to customise, so there is
    /// no "don't overwrite" guard here (compare EnsurePhpIni() above).
    /// </summary>
    public void EnsureRouterPhp()
    {
        Directory.CreateDirectory(_config.ToolsDirectory);

        // Forward slashes avoid having to escape backslashes inside the
        // single-quoted PHP string literals below.
        string adminerPhpPath = _config.AdminerPhpPath.Replace(@"\", "/");
        string adminerDir     = _config.AdminerDirectory.Replace(@"\", "/");

        string routerContent = $@"<?php
// router.php — generated by WINSERV. Do not hand-edit; it is overwritten
// on every PHP start. See PhpServer.EnsureRouterPhp() in the WINSERV source.
//
// PHP's built-in server runs this script for every request. Returning
// false falls through to the normal -t docroot handling — this only
// intercepts /adminer* so the permalink works regardless of WebRoot.

$uri = urldecode(parse_url($_SERVER['REQUEST_URI'], PHP_URL_PATH));

if ($uri === '/adminer' || $uri === '/adminer/' || $uri === '/adminer/adminer.php') {{
    if (!file_exists('{adminerPhpPath}')) {{
        http_response_code(404);
        echo 'Adminer is not deployed. In WINSERV, open Settings and click Deploy Adminer.';
        return true;
    }}
    chdir('{adminerDir}');
    require '{adminerPhpPath}';
    return true;
}}

return false;
";

        File.WriteAllText(_config.RouterPhpPath, routerContent);
        OnOutputReceived($"[PHP] Router script written to {_config.RouterPhpPath}", isError: false);
    }
}
