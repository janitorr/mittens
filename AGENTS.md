# Memory Server

This project has an AOT memory server at `http://localhost:5070`. Use it to store
and retrieve persistent context across coding sessions.

## When to use

- Before starting a complex task, check memory for relevant facts
- After discovering important info (bugs, decisions, patterns, conventions), store it
- When unsure about project setup or past decisions, query memory first
- Share context between agents by saving facts under shared categories and scopes

## API reference

Base URL: `http://localhost:5070/api/memory`

### Store a fact

```
POST /api/memory
Content-Type: application/json

{ "category": "...", "key": "...", "value": "...", "scope": "...", "confidence": 0.9 }
```

### List facts

```
GET /api/memory?category=&scope=&key=&page=&pageSize=
```

### Get a fact by ID

```
GET /api/memory/{id}
```

### Update a fact

```
PUT /api/memory/{id}
Content-Type: application/json

{ "key": "...", "value": "..." }
```

### Delete a fact

```
DELETE /api/memory/{id}
```

## Categories

Use one of: `preference`, `fact`, `concept`, `rule`, `plan`, `goal`, `task`, `note`

## Scope convention

Use the feature or area name (e.g. `auth`, `api`, `db`, `frontend`, `project`).

## Examples

Save a project convention:

```bash
curl -s -X POST http://localhost:5070/api/memory \
  -H "Content-Type: application/json" \
  -d '{"category":"rule","key":"naming","value":"Use PascalCase for public API and camelCase for internal fields","scope":"project","confidence":0.9}'
```

Check what's stored before a task:

```bash
curl -s "http://localhost:5070/api/memory?category=rule&scope=project"
```
