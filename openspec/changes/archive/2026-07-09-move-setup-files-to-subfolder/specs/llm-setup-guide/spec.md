## MODIFIED Requirements

### Requirement: LLM setup guide document (SETUP.md)
The repository SHALL contain a `setup/SETUP.md` file that serves as a self-contained installation playbook for LLMs. The guide SHALL enable an LLM to install and configure the memory server for any target project without requiring additional context.

#### Scenario: LLM fetches and reads SETUP.md
- **WHEN** an LLM fetches `setup/SETUP.md` from the repository
- **THEN** it contains all information needed to perform a complete installation, including prerequisites, step-by-step instructions, verification, and troubleshooting

#### Scenario: SETUP.md is written as instructions to the LLM
- **WHEN** reading SETUP.md
- **THEN** the content uses imperative, second-person language directed at the LLM (e.g., "write this file", "run this command"), not at the human user

### Requirement: Agent instructions template (AGENTS.template.md)
The repository SHALL contain a `setup/AGENTS.template.md` file that provides tool-usage-only guidance for target projects. It SHALL NOT contain Docker startup instructions, release instructions, CI/CD details, or any repo-specific maintenance information.

#### Scenario: AGENTS.template.md contains only tool usage
- **WHEN** reading `setup/AGENTS.template.md`
- **THEN** it includes: available tools table, when to use memory, categories enum, scope convention, and data model schema

#### Scenario: AGENTS.template.md excludes repo-specific content
- **WHEN** reading `setup/AGENTS.template.md`
- **THEN** it does NOT include: Docker compose instructions, release/semver instructions, CI/CD pipeline details, or project-specific startup commands

### Requirement: AGENTS.md installation
SETUP.md SHALL instruct the LLM to fetch `setup/AGENTS.template.md` from the repository and save it as `AGENTS.md` in the target project root.

#### Scenario: LLM fetches and installs AGENTS.template.md
- **WHEN** the LLM reaches the agent instructions step
- **THEN** it fetches `AGENTS.template.md` from `https://raw.githubusercontent.com/janitorr/mittens/main/setup/AGENTS.template.md` and saves it as `AGENTS.md` in the target project root

#### Scenario: Existing AGENTS.md is detected
- **WHEN** the target project already has an `AGENTS.md` file
- **THEN** the LLM asks the user whether to overwrite, merge, or skip
