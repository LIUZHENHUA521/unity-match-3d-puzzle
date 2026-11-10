# Compound item collider implementation

1. Implement editor-only primitive fitting with triangle clipping, bounded complexity and conservative coverage.
2. Integrate it into missing-body generation; add an explicit adapter-body migration preserving unrelated catalog/prefab data.
3. Run migration in the isolated validation project and inspect exported silhouettes, adjusting only where geometric evidence warrants it.
4. Validate profile/prefab round trips, restoration, geometry coverage, physical drops and pile behavior; run existing picking regressions.
5. Copy reviewed generated prefabs/catalog back, verify the diff and document results.
