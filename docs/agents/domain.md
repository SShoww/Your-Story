# Domain Docs

How engineering skills should consume this repo's domain documentation when exploring the codebase.

## Before exploring, read these

- **`AGENTS.md`** at the repo root — primary repository architectural rules, domain invariants, and verification commands.
- **`docs/adr/`** — read ADRs that touch the area you're about to work in (ADR 0003 for BePalV2 production).
- **`BEPAL/Docs/NewGDD/`** — active Game Design Document suite (v2.0).

## File structure

```
/
├── AGENTS.md
├── docs/adr/
├── BEPAL/Docs/NewGDD/
└── CoPoject/
    ├── BePalV2/          # PRIMARY PRODUCTION
    └── BePalV2.Tests/    # PRIMARY TESTS
```

## Use the glossary's vocabulary

When naming a domain concept in an issue title, refactor proposal, hypothesis, or test name, use the term as defined in `AGENTS.md` and `BEPAL/Docs/NewGDD/`. Don't drift to synonyms the glossary explicitly avoids.

## Flag ADR conflicts

If output contradicts an existing ADR, surface it explicitly rather than silently overriding it. Note that ADR 0003 supersedes ADR 0002.
