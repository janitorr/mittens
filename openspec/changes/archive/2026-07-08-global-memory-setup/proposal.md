## Why

The current SETUP.md instructs the LLM to write 139 lines of memory server guidance into every target project's `AGENTS.md` via `curl -o AGENTS.md`. This pollutes project AGENTS.md files with infrastructure documentation that doesn't belong there — AGENTS.md should contain project conventions, build commands, and tech stack details, not MCP server usage guides. Additionally, per-project copies become stale when the guidance is updated, and merging with existing AGENTS.md content creates friction.

## What Changes

- **Rewrite SETUP.md Step 3**: MCP config targets global `~/.config/opencode/opencode.json` instead of project-level `opencode.json`
- **Rewrite SETUP.md Step 4**: Agent instructions written to `~/.config/opencode/memory-server.md` and merged into global `instructions` array, instead of `curl -o AGENTS.md` in project root
- **Add SETUP.md Uninstall section**: Instructions to remove the Docker container, delete the instruction file, and clean up global config entries
- **Update README.md "Using with opencode" section**: Reflect global config approach for humans, remove per-project `curl -o AGENTS.md` instruction
- **Claude Desktop MCP config**: Retained as-is with a note that tool guidance is deferred; OpenCode is the primary supported client

## Capabilities

### New Capabilities

- `global-memory-setup`: Install and uninstall the AOT Memory Server using global OpenCode configuration (`~/.config/opencode/`) without modifying any files in target project repositories. Covers MCP server registration, instruction file placement, and clean removal.

### Modified Capabilities

- `llm-setup-guide`: The setup approach changes from per-project file writes to global configuration. The SETUP.md and README.md artifacts are updated to reflect the global approach. (Note: `add-llm-setup-guide` change is complete but not yet archived; this change supersedes its approach.)

## Impact

- **Modified files**: `SETUP.md`, `README.md` (documentation only)
- **No code changes**: No server-side code, API changes, or breaking changes
- **No Docker image changes**: The Docker image and compose file are unchanged
- **AGENTS.template.md**: Unchanged — remains the source content fetched during setup, just placed in a different location
