# Gitflow Workflow Specification

This document establishes the official **Gitflow Workflow** for the **BePal** repository, modeled after the canonical [Atlassian Gitflow Workflow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) standard (originally formulated by Vincent Driessen) and adapted for our .NET / MonoGame simulation engine lifecycle.

---

## 1. Overview & Branching Architecture

Gitflow enforces a strict branching model designed around project releases. It assigns explicit responsibilities to branches and governs when and how they interact.

```
(main)       ──────────────────────────────● [v0.1.0] ───────────────────────● [v0.1.1] ───
                                           ▲                                 ▲
                                          ╱                                 ╱
(release)                       ╭────────●────────╮                        ╱
                               ╱                  ▼                       ╱
(Develop)    ──●─────●────────●───────────────────●──────●─────●─────────●───────────────
                ╲   ▲            ╲                      ▲       ▲       ▲
                 ▼ ╱              ▼                    ╱       ╱         ╲
(feature)         ● (feature/A)    ● (feature/B) ─────╯       ╱           ▼
                                                             ╱      (hotfix/v0.1.1)
(hotfix)                                    ────────────────╯
```

---

## 2. Core Branches

The repository maintains two permanent branches with infinite lifetimes:

### `main` (Production Branch)
- **Role**: Contains official production-ready release history.
- **Rules**:
  - Direct commits to `main` are strictly forbidden.
  - Every commit on `main` must represent an official release or hotfix.
  - Every merge into `main` must be tagged with a semantic version (e.g. `v0.1.0`, `v0.1.1`).
  - `main` must compile and pass all automated tests and screenshot regressions at all times.

### `Develop` (Integration Branch)
- **Role**: Serves as the continuous integration hub for completed features.
- **Rules**:
  - Serves as the source branch for all new features and releases.
  - Receives merged feature branches, release bugfix back-merges, and hotfix back-merges.
  - Must remain in a stable, buildable state (`dotnet build CoPoject/CoPoject.slnx` must pass).

---

## 3. Supporting Branches

Supporting branches have limited lifespans and are deleted after being merged back into the core branches.

| Branch Type | Branch From | Merge Into | Naming Convention | Lifecycle |
|---|---|---|---|---|
| **Feature** | `Develop` | `Develop` | `feature/<topic>` or `feature/<issue-id>-<name>` | Deleted after merge |
| **Release** | `Develop` | `main` **AND** `Develop` | `release/v<version>` (e.g. `release/v0.2.0`) | Deleted after release |
| **Hotfix** | `main` | `main` **AND** `Develop` | `hotfix/v<version>` (e.g. `hotfix/v0.1.1`) | Deleted after hotfix |

---

## 4. Operational Workflows

### 4.1. Feature Branches (`feature/*`)

Used to develop new gameplay mechanics, subsystems, or screens (e.g., screen decomposition, new pet behaviors, UI components).

1. **Create branch from `Develop`**:
   ```bash
   git checkout Develop
   git pull origin Develop
   git checkout -b feature/care-wheel-decay
   ```

2. **Develop & Commit**:
   - Write domain logic and tests first or alongside engine integration.
   - Commit frequently with conventional commit messages:
     ```bash
     git commit -m "feat(gameplay): implement satisfaction decay per care cycle"
     ```

3. **Verify Locally**:
   ```bash
   dotnet build CoPoject/CoPoject.slnx
   dotnet run --project CoPoject/BePal -- --screenshot
   ```

4. **Merge Back into `Develop`**:
   - Push feature branch to remote:
     ```bash
     git push -u origin feature/care-wheel-decay
     ```
   - Open a Pull Request targeting `Develop`.
   - After review and CI green light, merge using `--no-ff` (non-fast-forward) to preserve feature history:
     ```bash
     git checkout Develop
     git merge --no-ff feature/care-wheel-decay
     git push origin Develop
     git branch -d feature/care-wheel-decay
     git push origin --delete feature/care-wheel-decay
     ```

---

### 4.2. Release Branches (`release/*`)

Used when `Develop` has accumulated all features for an upcoming milestone (e.g., Sprint completion, public demo).

1. **Create branch from `Develop`**:
   ```bash
   git checkout Develop
   git pull origin Develop
   git checkout -b release/v0.1.0
   ```

2. **Polish & Release Preparation**:
   - **Allowed**: Bug fixes, documentation updates, asset polish, release version metadata.
   - **Forbidden**: Adding new features or significant refactoring.

3. **Merge into `main` and Tag**:
   ```bash
   git checkout main
   git pull origin main
   git merge --no-ff release/v0.1.0
   git tag -a v0.1.0 -m "Release v0.1.0: 5-Day abnormal pet daycare simulation prototype"
   git push origin main --tags
   ```

4. **Back-Merge into `Develop`**:
   - Ensures any bug fixes applied on the release branch propagate to ongoing development:
     ```bash
     git checkout Develop
     git pull origin Develop
     git merge --no-ff release/v0.1.0
     git push origin Develop
     ```

5. **Clean Up**:
   ```bash
   git branch -d release/v0.1.0
   git push origin --delete release/v0.1.0
   ```

---

### 4.3. Hotfix Branches (`hotfix/*`)

Used to quickly fix critical defects in production releases without waiting for or disrupting ongoing feature development on `Develop`.

1. **Create branch from `main`**:
   ```bash
   git checkout main
   git pull origin main
   git checkout -b hotfix/v0.1.1
   ```

2. **Fix & Verify**:
   - Implement minimal, surgical fix.
   - Verify build and regression suite:
     ```bash
     dotnet build CoPoject/CoPoject.slnx
     dotnet run --project CoPoject/BePal -- --screenshot
     ```

3. **Merge into `main` and Tag**:
   ```bash
   git checkout main
   git merge --no-ff hotfix/v0.1.1
   git tag -a v0.1.1 -m "Hotfix v0.1.1: resolve crash on needle teleport wrap-around"
   git push origin main --tags
   ```

4. **Back-Merge into `Develop`**:
   - If an active `release/*` branch exists, merge into that release branch instead (it will eventually merge into `Develop`). Otherwise, merge into `Develop`:
     ```bash
     git checkout Develop
     git merge --no-ff hotfix/v0.1.1
     git push origin Develop
     ```

5. **Clean Up**:
   ```bash
   git branch -d hotfix/v0.1.1
   git push origin --delete hotfix/v0.1.1
   ```

---

## 5. Commit Message Conventions

All commits across all branches must follow Conventional Commits format:

```
<type>(<scope>): <short summary>

[optional body describing motivation, domain context, or design rationale]

[optional footer: Closes #123, Refs #456]
```

### Allowed Types
- **`feat`**: New user-facing gameplay mechanic, screen, or domain state model.
- **`fix`**: Bug fix in simulation math, QTE logic, graphics, or asset pipeline.
- **`docs`**: Documentation updates (GDD, ADR, CONTEXT.md, workflow guidelines).
- **`style`**: Code formatting, whitespace, naming consistency (no logic changes).
- **`refactor`**: Code restructuring without changing observable gameplay behavior.
- **`test`**: Automated unit tests, playtest harness scripts, regression assertions.
- **`chore`**: Build pipeline, NuGet packages, MGCB configs, repo tooling.

### Canonical Scopes
- `(gameplay)`: PrototypeRun, day loop, satisfaction, HP, pet taxonomy.
- `(qte)`: Needle movement, radial sectors, dodge window, input confirmation.
- `(screens)`: IScreen architecture, MainMenu, Home, Care, Dodge, SurvivalLog.
- `(content)`: Sprites, fonts, shaders, Content.mgcb pipeline.
- `(qa)`: Headless screenshot runner, unit tests, automated visual tests.

---

## 6. Pre-Merge Verification Gates

Before any branch may be merged into `Develop` or `main`:

1. **Compilation**:
   ```powershell
   dotnet build CoPoject/CoPoject.slnx
   ```
   Must produce 0 errors and 0 build warnings.

2. **Automated Visual Regression Harness**:
   ```powershell
   dotnet run --project CoPoject/BePal -- --screenshot
   ```
   Must complete all 18 test frames and successfully write screenshots to `screenshots/`.

3. **Domain Unit Tests** (when test project is present):
   ```powershell
   dotnet test CoPoject/CoPoject.slnx
   ```
   All tests must pass.

4. **PR Review**:
   - Every merge into `Develop` or `main` must occur via a Pull Request reviewed by a team member or approved agent.
