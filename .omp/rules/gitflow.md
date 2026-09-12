# Gitflow Workflow Rules

Strictly adhere to the Atlassian Gitflow Workflow standard in this project:

## Branch Strategy & Roles

1. **Permanent Branches**:
   - `main`: Production release history only. Never commit directly to `main`. Every commit must be a merge commit from `release/*` or `hotfix/*` and tagged with `vX.Y.Z`.
   - `Develop`: Continuous integration branch. All features branch from `Develop` and merge back into `Develop`.

2. **Temporary Branches**:
   - `feature/<topic>`: Created from `Develop`. Merged back into `Develop` via PR or `--no-ff`. Deleted after merge.
   - `release/v<version>`: Created from `Develop` for release stabilization (only bugfixes, docs, metadata). Merged into BOTH `main` (with tag) and `Develop`. Deleted after release.
   - `hotfix/v<version>`: Created from `main` to patch critical production bugs. Merged into BOTH `main` (with tag) and `Develop`. Deleted after hotfix.

## Development Checklist

- Before starting work, ensure you are branching from the latest `Develop`:
  ```bash
  git checkout Develop && git pull origin Develop
  git checkout -b feature/<descriptive-name>
  ```
- Use Conventional Commits (`feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`).
- Always verify locally before opening a PR or merging:
  ```bash
  dotnet build CoPoject/CoPoject.slnx
  dotnet run --project CoPoject/BePal -- --screenshot
  ```
- Always use `--no-ff` (non-fast-forward) when merging feature, release, or hotfix branches to preserve branch history.
