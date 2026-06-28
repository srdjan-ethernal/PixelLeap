# Publish PixelLeap to the web (local build → GitHub Pages)

GameCI's free cloud build needs the legacy Unity `.ulf` license, which **Unity 6's
new Personal license format does not provide** — so building Unity *in* CI is a dead
end on a free plan. Since you have the editor locally, we do it the simple way:

> **You build WebGL on your machine → push the output → GitHub serves it on Pages.**
> No Unity, no license, no secrets in CI.

- Repo: https://github.com/srdjan-ethernal/PixelLeap
- Pages is already enabled (Source: GitHub Actions).
- Live URL: **https://srdjan-ethernal.github.io/PixelLeap/**

---

## One-time: the build settings are already handled

`Assets/Editor/WebGLBuildConfig.cs` disables WebGL compression automatically, which
is what makes the build work on GitHub Pages (Pages can't set the headers that
gzip/brotli builds need). You don't have to touch anything.

## Each time you want to publish

1. In Unity: **File → Build Profiles** (Unity 6) → select **Web / WebGL** →
   **Switch Platform** if needed → **Build**.
2. In the folder picker, choose the repo's **`WebGLBuild`** folder:
   `C:\Users\Srdjan\PixelLeap\WebGLBuild`
   (Unity writes `index.html`, `Build/`, `TemplateData/` there.)
3. Commit & push the build:
   ```powershell
   git -C C:\Users\Srdjan\PixelLeap add WebGLBuild
   git -C C:\Users\Srdjan\PixelLeap commit -m "Publish WebGL build"
   git -C C:\Users\Srdjan\PixelLeap push
   ```
4. The **Deploy WebGL to Pages** workflow runs automatically (~30–60 s). When it's
   green, play at **https://srdjan-ethernal.github.io/PixelLeap/**.

That's it — repeat steps 1–4 whenever you want to update the live version.

---

## Troubleshooting

| Symptom | Fix |
| --- | --- |
| Page is blank / "failed to download" | Confirm `Assets/Editor/WebGLBuildConfig.cs` is in the project (it disables compression). Rebuild. |
| Deploy workflow didn't trigger | It only runs when files under `WebGLBuild/**` change. Make sure the build output was committed. Or run it manually: `gh workflow run "Deploy WebGL to Pages" --repo srdjan-ethernal/PixelLeap`. |
| 404 at the URL | First deploy can take a minute after the workflow goes green; also check Settings → Pages shows the site as built. |
| Want a smaller download | In the build, Unity → Player Settings → Publishing Settings, but keep **Compression Format = Disabled** for Pages. |
