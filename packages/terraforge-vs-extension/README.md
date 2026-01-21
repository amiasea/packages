#️⃣ Terraforge VS Code Extension — Capability Catalog
This is the authoritative list of what the extension can and will do.
Think of it as the semantic surface area of the tool.
## 1. Core Commands
- terraforge.run
Trigger a Terraforge semantic build.
- terraforge.compile
Compile provider code (Go, schema, resources).
- terraforge.greenlight
Validate the presence and correctness of the Greenlight file.
- terraforge.panel
Open the Terraforge Webview Panel (UI surface).

## 2. Webview Panel
- Render a semantic compiler dashboard.
- Display build status, logs, and diagnostics.
- Provide interactive UI for:
- triggering builds
- viewing provider metadata
- showing schema diffs
- showing resource graphs

## 3. Problem Matcher Integration
- Use the esbuild-problem-matchers extension to parse build output.
- Highlight errors and warnings in the editor.
- Support watch mode for continuous feedback.

## 4. File System Integration
- Detect changes in:
- provider schema files
- Greenlight file
- resource definitions
- Trigger incremental rebuilds.
- Provide status notifications.

## 5. Terraforge Engine Integration
- Invoke the semantic compiler.
- Generate provider code.
- Emit diagnostics.
- Produce structured output for the panel.

## 6. Workspace Awareness
- Detect whether the workspace is a Terraforge project.
- Validate required files:
- greenlight.json
- provider.tf.json
- schema/ directory
- Provide onboarding hints if missing.

## 7. Developer Experience Enhancements
- Status bar item for build state.
- Output channel for Terraforge logs.
- QuickPick menus for common actions.
- Notifications for success/failure.

#️⃣ Linear Design Plan
This is the step‑by‑step execution plan, in the correct order, with no ambiguity.
Follow this linearly and the extension will grow cleanly.

Step 1 — Establish the Modern Scaffold
- Create src/, dist/, package.json, tsconfig.json.
- Implement minimal activate() and deactivate().
- Add one working command (terraforge.run).
- Verify extension loads in the Dev Host.

Step 2 — Register All Commands
Inside activate():
- terraforge.run
- terraforge.compile
- terraforge.greenlight
- terraforge.panel
Push each into context.subscriptions.

Step 3 — Implement the Webview Panel
- Create terraforge.panel.
- Add getWebviewContent().
- Load static HTML.
- Verify panel opens in Dev Host.

Step 4 — Integrate the esbuild Problem Matcher
- Install the esbuild matcher extension.
- Add tasks.json entry referencing $esbuild-watch.
- Verify schema accepts it.
- Trigger a build and confirm diagnostics appear.

Step 5 — Add Output Channel + Logging
- Create const output = vscode.window.createOutputChannel("Terraforge").
- Pipe build logs into it.
- Pipe semantic compiler output into it.

Step 6 — Implement Terraforge Engine Hooks
- Wire terraforge.run → semantic compiler.
- Wire terraforge.compile → provider codegen.
- Wire terraforge.greenlight → validation logic.

Step 7 — Add Workspace Awareness
- Detect if workspace contains Terraforge project files.
- If not, show onboarding message.
- If yes, enable full functionality.

Step 8 — Add File Watchers
Watch for:
- greenlight.json
- schema/**/*.json
- provider/**/*.tf.json
Trigger incremental rebuilds.

Step 9 — Enhance the Webview Panel
Add:
- build status
- logs
- schema diffs
- resource graphs
- interactive controls
Use panel.webview.postMessage() for communication.

Step 10 — Polish the Developer Experience
- Add status bar item.
- Add QuickPick menus.
- Add notifications.
- Add command palette entries.
- Add icons and branding.
