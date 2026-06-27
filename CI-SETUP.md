# Cloud build setup (GameCI → GitHub Pages)

This builds PixelLeap in the cloud on every push and publishes a **playable WebGL
version** to a GitHub Pages URL. The heavy Unity editor build runs in the cloud —
you only need **Unity Hub** locally (a small ~150 MB download, *not* the multi-GB
editor) to generate a free license file once.

You do steps 1–4 once. After that, every `git push` rebuilds and redeploys.

> The repo is already created and pushed: https://github.com/srdjan-ethernal/PixelLeap
> Pages is already enabled (Source: GitHub Actions). Future game URL:
> **https://srdjan-ethernal.github.io/PixelLeap/**

---

## Step 1 — Get a free Unity Personal license file (.ulf)

GameCI's old "request activation file" action is deprecated. The current way:

1. Install **Unity Hub**: https://unity.com/download (just the Hub — you can skip
   installing an Editor).
2. Open Unity Hub and **sign in** with a free Unity account (create one if needed).
3. Go to **Unity Hub → Preferences (⚙) → Licenses → Add → Get a free personal license**.
4. This writes the license file to:
   **`C:\ProgramData\Unity\Unity_lic.ulf`** (Windows).

## Step 2 — Add the license + account as repo secrets

Repo → **Settings → Secrets and variables → Actions → New repository secret**, add three:

| Secret name | Value |
| --- | --- |
| `UNITY_LICENSE` | The **entire contents** of `C:\ProgramData\Unity\Unity_lic.ulf` (open in a text editor, copy all — it's XML) |
| `UNITY_EMAIL` | Your Unity account email |
| `UNITY_PASSWORD` | Your Unity account password |

> GameCI uses these only to activate Unity during the build; it doesn't store them.
> `UNITY_SERIAL` is only for paid Plus/Pro seats — not needed here.

Quick way to copy the .ulf contents to your clipboard (PowerShell):

```powershell
Get-Content C:\ProgramData\Unity\Unity_lic.ulf -Raw | Set-Clipboard
```

Or add the secret straight from the CLI:

```powershell
gh secret set UNITY_LICENSE  --repo srdjan-ethernal/PixelLeap < C:\ProgramData\Unity\Unity_lic.ulf
gh secret set UNITY_EMAIL    --repo srdjan-ethernal/PixelLeap --body "you@example.com"
gh secret set UNITY_PASSWORD --repo srdjan-ethernal/PixelLeap --body "your-password"
```

## Step 3 — Build it

Either push any commit, or trigger manually:

```powershell
gh workflow run "Build WebGL & deploy to Pages" --repo srdjan-ethernal/PixelLeap
```

- First build takes ~10–20 min (Unity image download + compile). Later builds are
  faster (the `Library/` folder is cached).
- Watch progress: `gh run watch --repo srdjan-ethernal/PixelLeap`

## Step 4 — Play

When the **deploy** job finishes, your game is live at:
**https://srdjan-ethernal.github.io/PixelLeap/**

---

## Troubleshooting

| Symptom | Fix |
| --- | --- |
| Build fails with a license error | Re-check `UNITY_LICENSE` holds the **full** `.ulf` XML, and that `UNITY_EMAIL`/`UNITY_PASSWORD` match the account that generated it. |
| Game page is blank / "failed to download" | Compression is disabled by `Assets/Editor/WebGLBuildConfig.cs`, which fixes this on Pages — make sure that file is committed. |
| Deploy job skipped | Pages source must be **GitHub Actions** (already set for this repo). |
| Different Unity version | Edit `ProjectSettings/ProjectVersion.txt`; GameCI auto-selects the matching cloud image. Regenerate the `.ulf` with the same major version. |

## Just a compile check, no WebGL?

Want a faster pass/fail compile (no Pages)? I can add a `game-ci/unity-test-runner@v4`
job — just ask.
