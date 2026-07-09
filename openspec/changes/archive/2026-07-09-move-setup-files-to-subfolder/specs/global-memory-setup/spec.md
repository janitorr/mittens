## MODIFIED Requirements

### Requirement: SETUP.md uses global OpenCode configuration for agent instructions
The SETUP.md guide SHALL instruct the LLM to write the memory server agent instructions to `~/.config/opencode/memory-server.md` and register it in the global `instructions` array, rather than writing to a project's `AGENTS.md`.

#### Scenario: Fetch and place instruction file
- **WHEN** the LLM follows the setup guide
- **THEN** it fetches `setup/AGENTS.template.md` from the repo and writes it to `~/.config/opencode/memory-server.md`
