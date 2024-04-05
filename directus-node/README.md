# Directus with the npx CLI

This project contains files generated from the `npx directus init` command, the configuration for directus is the .env file.

The snapshot.yaml file contains the data models that have been created in directus and then exported to yaml format. This is very useful so we can save different versions of the applications and apply the configurations in a CI/CD pipeline.

## Running directus

With these commands, the database can be installed and the snapshots of the data models can be applied. The configuration of the directus server is in the .env file.

Before you get started, make sure you have the following prerequisites:

- Node v18.17 or higher.
- A running PostgreSQL server. (can be run with `docker compose up`)

Bootstrap the project to install the database and apply migrations.

```bash
npx directus bootstrap
```

Apply the snapshot to insert the data models into directus. This applies the latest snapshot.

```bash
npx directus schema apply --yes ./snapshot.yaml
```

Start the directus server

```bash
npx directus start
```

## Snapshot the Data Model

Directus can automatically generate a snapshot of your current data model in YAML or JSON format. This includes all collections, fields, and relations, and their configuration. This snapshot can be checked in version control and shared with your team. To generate the snapshot, run

```bash
npx directus schema snapshot ./snapshot.yaml
```

To run non-interactively (e.g. when running in a CI/CD workflow), run
bash

```bash
npx directus schema snapshot --yes ./snapshot.yaml
```

Note, that this will force overwrite existing snapshot files with the same name.

The most recent snapshot will always be snapshot.yaml in the root directory, so when making a new snapshot you should first rename the current snapshot and put it in the snapshots directory. Then you can use the command above to create a snapshot of the current state of the directus configuration.
