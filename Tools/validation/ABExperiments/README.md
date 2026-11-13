# AB runtime validation

Run `python3 Tools/validation/ABExperiments/run.py`. It creates an isolated Unity project under `work/ab-integration/runtime`, copies the production AB component, and supplies a local HTTP service and account fixture. The open game project is untouched. Checks: default behavior, login readiness, assignment lookup, immediate identity isolation, stopped experiments, expiry, transient failure, late old-account response, and authentication refresh. Results are written under the runtime directory.
