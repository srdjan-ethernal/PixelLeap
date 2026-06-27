# Cloud build setup (GameCI → GitHub Pages)

This builds PixelLeap in the cloud on every push and publishes a **playable WebGL
version** to a GitHub Pages URL. No local Unity install needed.

You only do steps 1–6 once. After that, every `git push` rebuilds and redeploys.

---

## Step 1 — Put the project on GitHub

Create a repo and push this `PixelLeap` folder so that `Assets/`, `ProjectSettings/`,
`Packages/` and `.github/` are at the **repository root**.

```bash
cd C:\Users\Srdjan\PixelLeap
git init
git add .
git commit -m "PixelLeap: Unity 2D platformer + GameCI"
git branch -M main
git remote add origin https://github.com/<you>/PixelLeap.git
git push -u origin main
```

## Step 2 — Get a Unity activation file (.alf)

1. On GitHub: **Actions** tab → in the left list pick **"1 · Acquire Unity activation file"** → **Run workflow**.
2. When it finishes (~1 min), open the run → **Artifacts** → download **"Manual Activation File"**.
3. Unzip it — inside is `Unity_v2022.x.alf`.

## Step 3 — Convert .alf → .ulf (free Personal license)

1. Go to **https://license.unity3d.com/manual**.
2. Upload the `.alf` file.
3. Choose **Unity Personal** → *"I don't use Unity in a professional capacity"* (or whichever fits).
4. Download the resulting **`Unity_v2022.x.ulf`** file.

## Step 4 — Add the license as a repo secret

1. Repo → **Settings** → **Secrets and variables** → **Actions** → **New repository secret**.
2. Name: `UNITY_LICENSE`
3. Value: open the `.ulf` file in a text editor and paste its **entire contents** (it's XML).
4. Save.

> Optional (only for Unity Plus/Pro seats): also add `UNITY_EMAIL` and `UNITY_PASSWORD`
> secrets. For a free Personal license, `UNITY_LICENSE` alone is enough.

## Step 5 — Turn on GitHub Pages

Repo → **Settings** → **Pages** → **Build and deployment** → **Source: GitHub Actions**.

## Step 6 — Build it

Either push any commit, or **Actions** → **"2 · Build WebGL & deploy to Pages"** → **Run workflow**.

- First build takes ~10–20 min (Unity image download + compile). Later builds are faster (Library is cached).
- When the **deploy** job finishes, your game is live at:
  `https://<you>.github.io/PixelLeap/`
  (the exact URL is shown on the deploy job and under Settings → Pages.)

---

## Troubleshooting

| Symptom | Fix |
| --- | --- |
| Build job fails at "Build project" with a license error | Re-check `UNITY_LICENSE` secret holds the **full** `.ulf` XML, and that you converted the `.alf` for the **same** Unity version as `ProjectSettings/ProjectVersion.txt`. |
| Game page is blank / "failed to download" | Compression is disabled by `Assets/Editor/WebGLBuildConfig.cs`, which fixes this on Pages. Make sure that file got committed. |
| Deploy job skipped | Pages source must be **GitHub Actions** (Step 5). |
| Want a different Unity version | Edit `ProjectSettings/ProjectVersion.txt`; GameCI auto-selects the matching cloud image. |

## What just compiles, no WebGL?

If you only want a pass/fail compile check (faster, no Pages), you can later add a
`game-ci/unity-test-runner@v4` job — say the word and I'll add it.
