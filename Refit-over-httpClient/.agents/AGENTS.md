# Two-Agent Workflow Rules

Use a two-agent workflow for every non-trivial task.

- **Planner Agent (Strong Model)**: Analyze requirements, inspect the codebase, identify risks, dependencies, and architecture implications, then produce a detailed implementation plan with ordered steps, acceptance criteria, and files to be modified. Do not write code until the plan is complete and approved.
- **Executor Agent (Fast Model)**: After the plan is approved, execute it exactly as written. Implement changes incrementally, update all affected files, run validations/tests, and report any deviations from the original plan before proceeding.

## Rules:
- Always prefer existing patterns and conventions in the repository.
- Keep changes minimal, maintainable, and production-ready.
- If the implementation differs from the plan, stop and explain why.
- At completion, provide a summary of changes, validation results, and any follow-up recommendations.
