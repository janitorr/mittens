## 1. Move files to setup/

- [x] 1.1 Create `setup/` directory
- [x] 1.2 Move `SETUP.md` → `setup/SETUP.md`
- [x] 1.3 Move `AGENTS.template.md` → `setup/AGENTS.template.md`
- [x] 1.4 Move `docker-compose.example.yml` → `setup/docker-compose.example.yml`

## 2. Update README.md references

- [x] 2.1 Update markdown link `[SETUP.md](SETUP.md)` → `[SETUP.md](setup/SETUP.md)` (2 occurrences)
- [x] 2.2 Update markdown link `[docker-compose.example.yml](docker-compose.example.yml)` → `[docker-compose.example.yml](setup/docker-compose.example.yml)`
- [x] 2.3 Update command `docker compose -f docker-compose.example.yml` → `docker compose -f setup/docker-compose.example.yml` (2 occurrences)
- [x] 2.4 Update raw GitHub URL for `docker-compose.example.yml` → `setup/docker-compose.example.yml`
- [x] 2.5 Update markdown link `[AGENTS.template.md](AGENTS.template.md)` → `[AGENTS.template.md](setup/AGENTS.template.md)`
- [x] 2.6 Update raw GitHub URL for `AGENTS.template.md` → `setup/AGENTS.template.md`

## 3. Update AGENTS.md reference

- [x] 3.1 Update text reference `See SETUP.md` → `See setup/SETUP.md`

## 4. Update SETUP.md self-references

- [x] 4.1 Update raw GitHub URL `.../main/AGENTS.template.md` → `.../main/setup/AGENTS.template.md`

## 5. Update AGENTS.template.md self-references

- [x] 5.1 Update raw GitHub URL `.../main/docker-compose.example.yml` → `.../main/setup/docker-compose.example.yml`

## 6. Verify

- [x] 6.1 Grep root for any remaining references to the old paths
- [x] 6.2 Verify all markdown links resolve correctly
- [x] 6.3 Verify raw GitHub URLs match the new file locations on GitHub
