# Snapshot the Data Model

Directus can automatically generate a snapshot of your current data model in YAML or JSON format. This includes all collections, fields, and relations, and their configuration. This snapshot can be checked in version control and shared with your team. To generate the snapshot, run

```bash
npx directus schema snapshot ./snapshot.yaml
```

To run non-interactively (e.g. when running in a CI/CD workflow), run
bash

```bash
npx directus schema snapshot --yes ./snapshot.yaml
```

Note, that this will force overwrite existing snapshot files.
