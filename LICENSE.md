# License

## WINSERV source code

WINSERV's own source code (everything in this repository except the
third-party components listed below) is released under:

**Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International
(CC BY-NC-SA 4.0)**

In brief:

- **Attribution** — give credit to the original author when you use, copy,
  or build on this work.
- **NonCommercial** — you may not use this work for commercial purposes.
- **ShareAlike** — if you remix, adapt, or build on this work, you must
  distribute your contributions under the same license as the original.

This is a plain-English summary only, not a substitute for the license
itself. Full legal text: <https://creativecommons.org/licenses/by-nc-sa/4.0/>

## Third-party components

WINSERV packages, but does not modify or statically link against, several
third-party projects. Each keeps its own original license, independently of
the CC BY-NC-SA 4.0 terms above. In each case WINSERV launches the
component as a separate child process (PHP, MariaDB) or serves its
unmodified file as-is (Adminer) — it is not combined into a single program
with WINSERV's own source.

### PHP

Bundled as the portable PHP distribution (`php.exe` and its extensions).

- **License:** PHP License, version 3.01 (OSI-approved, BSD-style).
- **Key terms:** free to redistribute and modify; the name "PHP" may not be
  used in a product's name or for endorsement without permission; a required
  acknowledgement is retained: *"This product includes PHP software, freely
  available from http://www.php.net/software/"*.
- Full text: <https://www.php.net/license/3_01.txt>

### MariaDB

Bundled as the portable MariaDB Server distribution (`mysqld.exe` and
related binaries).

- **License:** GNU General Public License, version 2 (GPLv2).
- WINSERV runs `mysqld.exe` as an independent subprocess rather than linking
  against MariaDB's code, which is the generally accepted basis (the same
  used by other bundled-stack tools such as XAMPP and Laragon) for keeping
  the two programs under separate licenses. GPLv2 itself permits commercial
  and non-commercial use freely; its copyleft terms apply to MariaDB's own
  source, not to software that merely runs it as a separate process.
- Full text: <https://www.gnu.org/licenses/old-licenses/gpl-2.0.html>

### Adminer

Optionally vendored into `{InstallPath}\tools\adminer\adminer.php` and
served via WINSERV's PHP router script (see BUILD.md). Not required for
WINSERV to run.

- **License:** dual-licensed — your choice of the Apache License 2.0 or the
  GNU General Public License version 2.
- Apache 2.0 text: <https://www.apache.org/licenses/LICENSE-2.0.html>
- GPLv2 text: <https://www.gnu.org/licenses/old-licenses/gpl-2.0.html>

---

None of the above third-party licenses carry a non-commercial restriction;
the NC term in CC BY-NC-SA 4.0 applies only to WINSERV's own source code,
not to PHP, MariaDB, or Adminer.
