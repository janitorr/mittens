## Why

The project root contains 13 files, several of which are setup/onboarding artifacts meant for external users installing Mittens in their own environment. These files — `SETUP.md`, `AGENTS.template.md`, and `docker-compose.example.yml` — are not part of the runtime or development of the server itself. Moving them to a dedicated `setup/` directory reduces visual clutter in the root and makes the purpose of each file clearer.

## What Changes

- Move `SETUP.md` → `setup/SETUP.md`
- Move `AGENTS.template.md` → `setup/AGENTS.template.md`
- Move `docker-compose.example.yml` → `setup/docker-compose.example.yml`
- Update all internal markdown links, raw GitHub URLs, and command examples that reference these files at the root path
- Update OpenSpec main spec files that assert these files must be at the project root

**BREAKING**: Raw GitHub URLs change from `https://raw.githubusercontent.com/janitorr/mittens/main/{filename}` to `https://raw.githubusercontent.com/janitorr/mittens/main/setup/{filename}`. Any external scripts or bookmarks using the old URLs will break.

## Capabilities

### New Capabilities
<!-- None - this is a documentation reorganization -->

### Modified Capabilities
- `project-identity`: Spec asserts `SETUP.md` and `AGENTS.template.md` are at the project root; this constraint changes to `setup/` subdirectory
- `llm-setup-guide`: Spec asserts `SETUP.md` and `AGENTS.template.md` are at the project root; paths in curl commands and markdown links update to `setup/`
- `global-memory-setup`: Spec references to `SETUP.md` and `AGENTS.template.md` update to reflect new paths

## Impact

- `README.md` — 9 references (markdown links, raw GitHub URLs, command examples)
- `AGENTS.md` — 1 text reference to `SETUP.md`
- `SETUP.md` — 1 raw GitHub URL to `AGENTS.template.md`
- `AGENTS.template.md` — 1 raw GitHub URL to `docker-compose.example.yml`
- `openspec/specs/project-identity/spec.md` — path assertions
- `openspec/specs/llm-setup-guide/spec.md` — path assertions and curl examples
- `openspec/specs/global-memory-setup/spec.md` — path references
