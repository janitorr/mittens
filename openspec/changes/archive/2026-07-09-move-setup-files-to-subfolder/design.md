## Context

The project root currently contains 13 files, including 3 setup/onboarding artifacts (`SETUP.md`, `AGENTS.template.md`, `docker-compose.example.yml`) that are meant for external users installing Mittens in their own environment. These files are not part of the server's runtime or development workflow. The root also contains 7 files that are locked to the root by tooling conventions (`.gitignore`, `.dockerignore`, `opencode.json`, `AGENTS.md`, `README.md`, `LICENSE`, `Mittens.slnx`), plus `Dockerfile` and `docker-compose.yml` which are conventionally at root.

These three files reference each other via raw GitHub URLs and markdown links, and are referenced by `README.md`, `AGENTS.md`, and three OpenSpec main spec files.

## Goals / Non-Goals

**Goals:**
- Move `SETUP.md`, `AGENTS.template.md`, and `docker-compose.example.yml` to `setup/` subdirectory
- Update all internal references (markdown links, raw GitHub URLs, command examples)
- Update OpenSpec main specs to reflect new paths
- Reduce root directory from 13 files to 10 (plus 2 `.db` artifacts to delete separately)

**Non-Goals:**
- Moving `Dockerfile` or `docker-compose.yml` (they stay at root by convention)
- Moving `AGENTS.md` or `README.md` (locked to root by tooling)
- Changing the content of the moved files beyond path references
- Creating a `docs/` directory (these are setup files, not general documentation)

## Decisions

### Decision 1: Directory name is `setup/`

**Rationale:** The primary file is `SETUP.md`, and all three files serve the same purpose — helping users set up Mittens in a new environment. `setup/` is short, obvious, and matches the existing file name. Alternatives like `onboarding/` or `install/` are more verbose or too narrow.

### Decision 2: Update raw GitHub URLs immediately

**Rationale:** The user is the only consumer so far. Breaking raw URLs now is better than accumulating stale bookmarks. The new URLs will be `https://raw.githubusercontent.com/janitorr/mittens/main/setup/{filename}`.

### Decision 3: Update OpenSpec main specs via delta specs

**Rationale:** Three main spec files (`project-identity`, `llm-setup-guide`, `global-memory-setup`) contain requirements that assert these files are at the project root. These are requirement-level changes, so delta spec files are the correct mechanism.

### Decision 4: Atomic update — move files and update references in one commit

**Rationale:** Moving files without updating references would break the repo. Updating references before moving files would point to non-existent paths. Both must happen together.

## Risks / Trade-offs

| Risk | Mitigation |
|---|---|
| External scripts/bookmarks using old raw URLs break | User is the only consumer; acceptable breakage |
| Raw GitHub URL caching (GitHub may serve stale content briefly) | No mitigation needed; cache expires quickly |
| Spec archive may have conflicting path assertions | Delta specs use MODIFIED with full updated content; archive will reconcile |

## Migration Plan

1. Create `setup/` directory
2. Move three files into `setup/`
3. Update all references in `README.md`, `AGENTS.md`, `SETUP.md`, `AGENTS.template.md`
4. Update OpenSpec main specs via delta spec files
5. Commit atomically
6. Delete stale `memory.db` files (separate cleanup, not part of this change)

## Open Questions

None.
