## Context

The `add-llm-setup-guide` change (complete, not archived) established a per-project installation approach: SETUP.md instructs the LLM to write `AGENTS.template.md` content into the target project's `AGENTS.md` and configure MCP in the project's `opencode.json`. This pollutes project repositories with 139 lines of memory server infrastructure documentation that doesn't belong in AGENTS.md (which should contain project-specific conventions, build commands, and tech stack details).

OpenCode supports a global `instructions` configuration field in `~/.config/opencode/opencode.json` that loads instruction files into the system prompt. This provides a clean mechanism to inject memory guidance at the user level, without touching any project files.

## Goals / Non-Goals

**Goals:**
- Rewrite SETUP.md to use global OpenCode configuration (`~/.config/opencode/`) for both MCP registration and agent instructions
- Add uninstall instructions to SETUP.md for clean removal of all changes
- Update README.md "Using with opencode" section to reflect the global approach
- Zero files written to target project repositories (except `docker-compose.memory.yml` for server infrastructure)
- Retain AGENTS.template.md as the source content fetched during setup (just placed in a different location)

**Non-Goals:**
- No changes to the server code, API, or Docker image
- No changes to AGENTS.template.md content (only where it gets placed)
- No changes to `docker-compose.example.yml` (backward compatibility)
- No Claude Desktop tool guidance support (deferred — MCP config retained, guidance note added)
- No changes to this repo's own `AGENTS.md`

## Decisions

### Decision 1: Global instructions via OpenCode `instructions` field

**Choice:** Write memory guidance to `~/.config/opencode/memory-server.md` and add `"instructions": ["memory-server.md"]` to the global `~/.config/opencode/opencode.json`.

**Rationale:** OpenCode's `instructions` field resolves paths relative to the config file directory, so `"memory-server.md"` resolves to `~/.config/opencode/memory-server.md`. This is a native OpenCode feature — no custom tooling needed. One file, all projects benefit.

**Alternatives considered:**
- Per-project `.opencode/memory-server.md` — still pollutes project repos, just with a different file
- MCP resource (`memory://instructions`) — protocol-clean but requires server changes and client support
- Global AGENTS.md (`~/.config/opencode/AGENTS.md`) — OpenCode doesn't auto-load this; must use `instructions` field

### Decision 2: Global MCP config alongside global instructions

**Choice:** MCP server registration also goes into `~/.config/opencode/opencode.json` instead of project-level `opencode.json`.

**Rationale:** The memory server always runs at `localhost:5070`. A single global MCP registration works for all projects. Projects that don't need the memory server simply don't start the Docker container — the MCP config is inert. This keeps the zero-project-files goal.

**Alternatives considered:**
- Per-project MCP config — requires writing to project `opencode.json`, violates the zero-files goal
- User manually configures MCP — unreliable, defeats the purpose of SETUP.md automation

### Decision 3: Config merge via LLM editing tools, not scripts

**Choice:** SETUP.md instructs the LLM to read the existing global config and surgically merge new entries using its file editing capabilities, rather than running a bash script with `jq` or `python`.

**Rationale:** The LLM has built-in file read/edit tools that handle JSONC natively. Bash-based JSON merge is fragile (jq doesn't handle JSONC comments, python may not be installed). The LLM can read the file, understand its structure, and make targeted edits.

**Alternatives considered:**
- `jq` script — doesn't handle JSONC comments, loses formatting
- `python -c` script — may not be available on all systems, complex one-liners
- `node -e` script — may not be available on all systems

### Decision 4: Uninstall section as a standalone section in SETUP.md

**Choice:** Add an "Uninstall" section at the end of SETUP.md (not a separate file) with step-by-step removal instructions.

**Rationale:** SETUP.md is the single source of truth for installation. Keeping uninstall instructions in the same file ensures they stay in sync with the install steps. A separate UNINSTALL.md would drift out of sync.

**Alternatives considered:**
- Separate UNINSTALL.md file — harder to keep in sync, another file to maintain
- No uninstall instructions — users would need to manually figure out what to clean up

### Decision 5: AGENTS.template.md filename unchanged

**Choice:** Keep the file named `AGENTS.template.md` in the repo. SETUP.md fetches it and writes it as `~/.config/opencode/memory-server.md`.

**Rationale:** The filename in the repo is an internal detail. The curl URL and destination filename are what matter. Renaming would break any existing references and provides no real benefit.

**Alternatives considered:**
- Rename to `memory-server-instructions.md` — more accurate name but breaks existing links
- Inline the content in SETUP.md — loses single-source-of-truth benefit

## Risks / Trade-offs

| Risk | Mitigation |
|------|-----------|
| User already has `instructions` in global config with other entries | SETUP.md instructs LLM to merge, not overwrite — append `memory-server.md` to existing array |
| User already has other MCP servers in global config | SETUP.md instructs LLM to merge `memory` entry alongside existing servers |
| Global config doesn't exist yet | SETUP.md instructs LLM to create it with the required sections |
| Claude Desktop users get MCP config but no tool guidance | Retain Claude Desktop MCP config option in SETUP.md with a note that guidance is deferred; rely on tool descriptions |
| User wants project-specific memory guidance overrides | User can still add project-level `instructions` in project `opencode.json` — global and project configs merge |
| `memory-server.md` path resolution fails | OpenCode resolves `instructions` paths relative to config file directory; this is documented behavior |
| Uninstall leaves orphaned config entries | SETUP.md uninstall instructions explicitly cover removing both `memory` from `mcp` and `memory-server.md` from `instructions` |
