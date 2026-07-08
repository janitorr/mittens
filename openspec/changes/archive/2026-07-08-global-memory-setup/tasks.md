## 1. Rewrite SETUP.md — MCP config step (Step 3)

- [x] 1.1 Update Step 3 title to reference global OpenCode configuration
- [x] 1.2 Replace per-project `opencode.json` MCP config instructions with global `~/.config/opencode/opencode.json` merge instructions
- [x] 1.3 Add merge guidance for existing global config (no MCP key, existing MCP servers, already registered)
- [x] 1.4 Retain Claude Desktop MCP config option with a note that tool guidance is deferred

## 2. Rewrite SETUP.md — Agent instructions step (Step 4)

- [x] 2.1 Replace `curl -o AGENTS.md` with `curl -o ~/.config/opencode/memory-server.md`
- [x] 2.2 Add instructions to merge `"instructions": ["memory-server.md"]` into global config
- [x] 2.3 Add merge guidance for existing global config (no instructions key, existing instructions array, already registered)
- [x] 2.4 Remove the "If project already has AGENTS.md" merge guidance (no longer relevant)

## 3. Add SETUP.md — Uninstall section

- [x] 3.1 Add Docker teardown instructions (`docker compose down -v`, optional image removal)
- [x] 3.2 Add instruction file removal (`rm ~/.config/opencode/memory-server.md`)
- [x] 3.3 Add global config cleanup instructions (remove `memory` from `mcp`, remove `memory-server.md` from `instructions`)
- [x] 3.4 Add warning about preserving other MCP servers and instruction entries during config cleanup

## 4. Update SETUP.md — Step renumbering and verification

- [x] 4.1 Renumber existing Step 5 (Verify the Setup) to Step 6
- [x] 4.2 Update verification step to note that MCP tools should be available globally
- [x] 4.3 Review remaining SETUP.md sections (Prerequisites, Troubleshooting, Management Commands) for any per-project references that need updating

## 5. Update README.md — "Using with opencode" section

- [x] 5.1 Update "Configure opencode" subsection to reference global `~/.config/opencode/opencode.json`
- [x] 5.2 Update "Add agent instructions" subsection to reference `~/.config/opencode/memory-server.md` and global `instructions` field
- [x] 5.3 Remove the `curl -o AGENTS.md` command from README.md
- [x] 5.4 Verify the "Agent setup instructions" referral to SETUP.md remains intact

## 6. Verification

- [x] 6.1 Run `openspec validate global-memory-setup` to confirm all artifacts pass
- [x] 6.2 Review SETUP.md end-to-end to ensure install/uninstall flow is coherent
- [x] 6.3 Review README.md to ensure human-facing instructions are accurate and consistent with SETUP.md
