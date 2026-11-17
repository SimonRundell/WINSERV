<?php
/**
 * WINSERV PHP Functionality Test Page
 * Tests the key extensions and capabilities needed for web dev backends.
 * Access at: http://localhost:8080/test.php
 */

// session_start() must be called before ANY output (including HTML).
session_start();

// ── Helpers ───────────────────────────────────────────────────────────────────

/**
 * Renders a pass/fail result row.
 */
function result(string $label, bool $ok, string $detail = ''): void
{
    $icon   = $ok ? '✅' : '❌';
    $colour = $ok ? '#2d6a2d' : '#8b0000';
    $bg     = $ok ? '#f0fff0' : '#fff0f0';
    echo "<tr style='background:{$bg}'>"
       . "<td style='padding:6px 12px;font-size:1.1em'>{$icon}</td>"
       . "<td style='padding:6px 12px;font-weight:bold;color:{$colour}'>{$label}</td>"
       . "<td style='padding:6px 12px;color:#555'>" . htmlspecialchars($detail) . "</td>"
       . "</tr>\n";
}

/**
 * Renders a section heading row.
 */
function section(string $title): void
{
    echo "<tr><td colspan='3' style='padding:10px 12px 4px;font-weight:bold;"
       . "font-size:1.05em;background:#e8eef7;border-top:2px solid #b0c0d8'>"
       . htmlspecialchars($title) . "</td></tr>\n";
}

?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>WINSERV — PHP Test</title>
    <style>
        body { font-family: Segoe UI, Arial, sans-serif; margin: 0; padding: 20px; background: #f4f6fa; }
        h1   { color: #1a3a6b; }
        h2   { color: #333; font-size: 1em; font-weight: normal; margin-top: 0; }
        table { border-collapse: collapse; width: 100%; max-width: 860px;
                background: #fff; box-shadow: 0 1px 4px rgba(0,0,0,.12); border-radius: 6px; overflow: hidden; }
        td   { border-bottom: 1px solid #e8e8e8; }
        .summary { max-width: 860px; margin-top: 16px; padding: 12px 16px;
                   border-radius: 6px; font-weight: bold; font-size: 1.05em; }
        .pass { background: #d4edda; color: #155724; }
        .fail { background: #f8d7da; color: #721c24; }
    </style>
</head>
<body>

<h1>🐘 WINSERV PHP Test Page</h1>
<h2>PHP <?= PHP_VERSION ?> &nbsp;|&nbsp; <?= php_uname('s') . ' ' . php_uname('r') ?></h2>

<table>

<?php

$pass = 0;
$fail = 0;

// ── PHP Core ──────────────────────────────────────────────────────────────────
section('PHP Core');

$v = PHP_MAJOR_VERSION >= 8;
result('PHP 8+', $v, 'Version ' . PHP_VERSION);
$v ? $pass++ : $fail++;

$v = ini_get('display_errors') === '1' || ini_get('display_errors') === 'On';
result('display_errors = On', $v, $v ? 'Development mode active' : 'Errors may be hidden');
$v ? $pass++ : $fail++;

$v = (int)ini_get('memory_limit') >= 128 || ini_get('memory_limit') === '-1';
result('memory_limit ≥ 128M', $v, ini_get('memory_limit'));
$v ? $pass++ : $fail++;

// ── Database ──────────────────────────────────────────────────────────────────
section('Database');

$v = extension_loaded('mysqli');
result('mysqli extension', $v, $v ? 'Loaded' : 'Not loaded — check php.ini');
$v ? $pass++ : $fail++;

$v = extension_loaded('pdo') && extension_loaded('pdo_mysql');
result('PDO + pdo_mysql', $v, $v ? 'Loaded' : 'Not loaded — check php.ini');
$v ? $pass++ : $fail++;

// Live connection test.
// Edit $dbPass to match the password you entered during WINSERV setup.
$dbHost = '127.0.0.1';
$dbUser = 'root';
$dbPass = '161787401'; // ← change this if you used a different password

$connected = false;
$dbDetail  = '';
if (extension_loaded('mysqli')) {
    mysqli_report(MYSQLI_REPORT_OFF); // prevent fatal — we handle errors below
    try {
        $mysqli = new mysqli($dbHost, $dbUser, $dbPass, '', 3307);
        if ($mysqli->connect_errno === 0) {
            $connected = true;
            $row       = $mysqli->query('SELECT VERSION() AS v')->fetch_assoc();
            $dbDetail  = 'MariaDB ' . ($row['v'] ?? '?');
            $mysqli->close();
        } else {
            $dbDetail = 'Error ' . $mysqli->connect_errno . ': ' . $mysqli->connect_error;
        }
    } catch (Exception $e) {
        $dbDetail = $e->getMessage();
    }
}
result('MariaDB connection', $connected, $dbDetail);
$connected ? $pass++ : $fail++;

// ── String & Encoding ─────────────────────────────────────────────────────────
section('String & Encoding');

$v = extension_loaded('mbstring');
result('mbstring', $v, $v ? 'mb_strlen, mb_strtolower etc. available' : 'Not loaded');
$v ? $pass++ : $fail++;

if ($v) {
    $test   = mb_strtolower('HÉLLO WÖRLD', 'UTF-8');
    $detail = ($test === 'héllo wörld') ? "mb_strtolower UTF-8: '{$test}' ✓" : "Unexpected: '{$test}'";
    result('mbstring UTF-8 ops', $test === 'héllo wörld', $detail);
    ($test === 'héllo wörld') ? $pass++ : $fail++;
}

$v = extension_loaded('openssl');
result('openssl', $v, $v ? OpenSSL_get_cert_locations()['default_cert_file'] ?? 'Loaded' : 'Not loaded — HTTPS/JWT will fail');
$v ? $pass++ : $fail++;

// ── HTTP & Network ────────────────────────────────────────────────────────────
section('HTTP & Network (cURL)');

$v = extension_loaded('curl');
result('curl extension', $v, $v ? 'Version ' . (curl_version()['version'] ?? '?') : 'Not loaded — check php.ini');
$v ? $pass++ : $fail++;

if ($v) {
    // PHP on Windows has no CA bundle by default. Look for cacert.pem next
    // to php.ini, or fall back to the openssl default, or disable peer verify
    // (noted in result detail so the user knows to add a CA bundle).
    $phpDir  = dirname(php_ini_loaded_file() ?: PHP_BINARY);
    $caBundle = $phpDir . '/cacert.pem';
    $hasCA   = file_exists($caBundle);

    // example.com is IANA-maintained — far more reliable than httpbin.org for uptime tests.
    $ch = curl_init('https://example.com');
    curl_setopt_array($ch, [
        CURLOPT_RETURNTRANSFER => true,
        CURLOPT_TIMEOUT        => 8,
        CURLOPT_SSL_VERIFYPEER => $hasCA,
        CURLOPT_SSL_VERIFYHOST => $hasCA ? 2 : 0,
        ...($hasCA ? [CURLOPT_CAINFO => $caBundle] : []),
    ]);
    $resp     = curl_exec($ch);
    $httpCode = curl_getinfo($ch, CURLINFO_HTTP_CODE);
    $err      = curl_error($ch);
    // curl_close() is deprecated since PHP 8.0 — omitted intentionally.

    $ok     = $httpCode === 200 && $resp !== false;
    $detail = $ok
        ? "HTTP {$httpCode} — " . ($hasCA ? 'SSL verified via cacert.pem' : 'SSL verify disabled (add cacert.pem to fix)')
        : ($err ?: "HTTP {$httpCode}");
    result('cURL HTTPS request', $ok, $detail);
    $ok ? $pass++ : $fail++;

    if (!$hasCA) {
        result('cURL CA bundle', false, 'cacert.pem not found in ' . $phpDir . ' — download from https://curl.se/ca/cacert.pem');
        $fail++;
    }
}

// ── File Handling ─────────────────────────────────────────────────────────────
section('File Handling');

$v = extension_loaded('fileinfo');
result('fileinfo extension', $v, $v ? 'mime_content_type() available' : 'Not loaded — file upload MIME checks will fail');
$v ? $pass++ : $fail++;

if ($v) {
    $tmpFile = tempnam(sys_get_temp_dir(), 'winsrv_');
    file_put_contents($tmpFile, '<html><body>test</body></html>');
    $mime   = mime_content_type($tmpFile);
    $ok     = str_contains($mime, 'html') || str_contains($mime, 'text');
    result('MIME detection', $ok, "Detected: {$mime}");
    $ok ? $pass++ : $fail++;
    unlink($tmpFile);
}

$v = extension_loaded('zip');
result('zip extension', $v, $v ? 'ZipArchive available' : 'Not loaded — zip creation/extraction unavailable');
$v ? $pass++ : $fail++;

if ($v) {
    $zipPath = sys_get_temp_dir() . '/winsrv_test.zip';
    $zip     = new ZipArchive();
    $ok      = $zip->open($zipPath, ZipArchive::CREATE | ZipArchive::OVERWRITE) === true;
    if ($ok) {
        $zip->addFromString('hello.txt', 'Hello from WINSERV');
        $zip->close();
        $ok = file_exists($zipPath) && filesize($zipPath) > 0;
        unlink($zipPath);
    }
    result('ZipArchive create', $ok, $ok ? 'Created and verified' : 'Failed to create zip');
    $ok ? $pass++ : $fail++;
}

// ── Image Processing ──────────────────────────────────────────────────────────
section('Image Processing');

$v = extension_loaded('gd');
result('GD extension', $v, $v ? 'GD ' . (GD_VERSION ?? '?') : 'Not loaded — image resizing unavailable');
$v ? $pass++ : $fail++;

if ($v) {
    $img = imagecreatetruecolor(10, 10);
    $ok  = $img !== false;
    // imagedestroy() deprecated since PHP 8.0 — GD resources are garbage collected automatically.
    result('GD imagecreatetruecolor', $ok, $ok ? '10×10 image created OK' : 'Failed');
    $ok ? $pass++ : $fail++;
}

// ── Sessions ──────────────────────────────────────────────────────────────────
section('Sessions');

$savePath = ini_get('session.save_path');
$v        = !empty($savePath) && is_dir($savePath) && is_writable($savePath);
result('session.save_path writable', $v, $savePath ?: '(not set)');
$v ? $pass++ : $fail++;

// session_start() was called at top of file before any output.
$_SESSION['winserv_test'] = 'ok';
$v = isset($_SESSION['winserv_test']) && $_SESSION['winserv_test'] === 'ok';
result('$_SESSION read/write', $v, $v ? 'Session ID: ' . session_id() : 'Failed');
$v ? $pass++ : $fail++;
session_destroy();

// ── JSON ──────────────────────────────────────────────────────────────────────
section('JSON');

$data   = ['winserv' => true, 'emoji' => '🐘', 'nested' => ['a' => 1]];
$enc    = json_encode($data, JSON_UNESCAPED_UNICODE);
$dec    = json_decode($enc, true);
$v      = $dec['emoji'] === '🐘' && $dec['nested']['a'] === 1;
result('json_encode / json_decode', $v, $v ? $enc : 'Mismatch');
$v ? $pass++ : $fail++;

?>
</table>

<div class="summary <?= $fail === 0 ? 'pass' : 'fail' ?>">
    <?= $fail === 0
        ? "✅ All {$pass} tests passed — stack is ready for development."
        : "⚠️ {$pass} passed, {$fail} failed — see red rows above." ?>
</div>

<p style="color:#888;font-size:.85em;max-width:860px">
    <strong>MariaDB connection test:</strong> uses root with no password (WINSERV default).
    Edit <code>$dbPass</code> near the top of this file if you set a custom password during setup.
</p>

</body>
</html>
