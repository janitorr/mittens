## ADDED Requirements

### Requirement: SETUP.md uses global OpenCode configuration for MCP registration
The SETUP.md guide SHALL instruct the LLM to register the memory server MCP endpoint in the global OpenCode configuration (`~/.config/opencode/opencode.json`) rather than in a project-level `opencode.json`.

#### Scenario: Global config does not exist
- **WHEN** the LLM checks for `~/.config/opencode/opencode.json` and it does not exist
- **THEN** the LLM creates the file with the `mcp.memory` section and `$schema` field

#### Scenario: Global config exists without MCP section
- **WHEN** the global config exists but has no `mcp` key
- **THEN** the LLM adds the `mcp` section with the `memory` entry, preserving all existing config

#### Scenario: Global config exists with other MCP servers
- **WHEN** the global config has an `mcp` section with other servers
- **THEN** the LLM adds the `memory` entry alongside existing servers without modifying them

#### Scenario: Memory MCP already registered
- **WHEN** the global config already has `mcp.memory` configured
- **THEN** the LLM skips this step and informs the user

### Requirement: SETUP.md uses global OpenCode configuration for agent instructions
The SETUP.md guide SHALL instruct the LLM to write the memory server agent instructions to `~/.config/opencode/memory-server.md` and register it in the global `instructions` array, rather than writing to a project's `AGENTS.md`.

#### Scenario: Fetch and place instruction file
- **WHEN** the LLM follows the setup guide
- **THEN** it fetches `setup/AGENTS.template.md` from the repo and writes it to `~/.config/opencode/memory-server.md`

#### Scenario: Global config has no instructions array
- **WHEN** the global config exists but has no `instructions` key
- **THEN** the LLM adds `"instructions": ["memory-server.md"]` to the config

#### Scenario: Global config has existing instructions
- **WHEN** the global config has an `instructions` array with other entries
- **THEN** the LLM appends `memory-server.md` if not already present, preserving existing entries

#### Scenario: Instructions already registered
- **WHEN** `memory-server.md` is already in the global `instructions` array
- **THEN** the LLM skips this step and informs the user

### Requirement: SETUP.md includes uninstall instructions
The SETUP.md guide SHALL include a standalone Uninstall section with step-by-step instructions to remove all changes made during setup.

#### Scenario: Docker teardown
- **WHEN** the user follows the uninstall instructions
- **THEN** the memory server container is stopped and its data volume is removed via `docker compose -f docker-compose.memory.yml down -v`

#### Scenario: Instruction file removal
- **WHEN** the user follows the uninstall instructions
- **THEN** `~/.config/opencode/memory-server.md` is deleted

#### Scenario: Global config cleanup
- **WHEN** the user follows the uninstall instructions
- **THEN** the `memory` entry is removed from the `mcp` section and `memory-server.md` is removed from the `instructions` array in the global config, with other entries preserved

#### Scenario: Optional Docker image removal
- **WHEN** the user follows the optional cleanup steps
- **THEN** the `janitorr/aot-memory-server:latest` Docker image is removed

### Requirement: README.md reflects global setup approach
The README.md "Using with opencode" section SHALL be updated to describe the global configuration approach and remove per-project `curl -o AGENTS.md` instructions.

#### Scenario: Human reader sees global instructions
- **WHEN** a human reads the README.md "Using with opencode" section
- **THEN** it describes setting up the memory server via global OpenCode configuration

#### Scenario: No AGENTS.md download instruction
- **WHEN** a human reads the README.md "Using with opencode" section
- **THEN** it does not instruct downloading `AGENTS.template.md` to the project root as `AGENTS.md`

#### Scenario: LLM referral to SETUP.md
- **WHEN** an LLM reads the README.md
- **THEN** it is directed to SETUP.md for the complete step-by-step installation guide
