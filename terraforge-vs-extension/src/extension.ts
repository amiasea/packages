import * as vscode from 'vscode';
import * as fs from "fs";
import * as path from "path";
import * as cp from "child_process";

export function activate(context: vscode.ExtensionContext) {
  console.log('Terraforge activated');

  const output = vscode.window.createOutputChannel("Terraforge");

  output.appendLine("Hello from Terraforge Activate!");
  output.show(true);

  // terraforge.run
  context.subscriptions.push(
    vscode.commands.registerCommand('terraforge.run', () => {
      vscode.window.showInformationMessage('Terraforge build triggered');
    })
  );

  // terraforge.compile
  context.subscriptions.push(
    vscode.commands.registerCommand('terraforge.compile', () => {
      // compile provider
    })
  );

  // terraforge.greenlight
  context.subscriptions.push(
    vscode.commands.registerCommand('terraforge.greenlight', () => {
      // check greenlight file
    })
  );

  // terraforge.panel
  context.subscriptions.push(
    vscode.commands.registerCommand("terraforge.panel", () => {
      const panel = vscode.window.createWebviewPanel(
        "terraforgePanel",
        "Terraforge",
        vscode.ViewColumn.One,
        {
          enableScripts: true,
          localResourceRoots: [
            vscode.Uri.joinPath(context.extensionUri, "src", "panel")
          ]
        }
      );

      panel.webview.html = getWebviewContent(panel.webview, context.extensionUri);
    })
  );
}

export function deactivate() {}

function getWebviewContent(panel: vscode.Webview, extensionUri: vscode.Uri) {
  const indexPath = vscode.Uri.joinPath(extensionUri, "src", "panel", "index.html");
  const indexHtml = fs.readFileSync(indexPath.fsPath, "utf8");

  return indexHtml
    .replace(/{{cspSource}}/g, panel.cspSource)
    .replace(/{{webviewUri}}/g, panel.asWebviewUri(extensionUri).toString());
}

async function fileExists(filePath: string): Promise<boolean> {
  try {
    await fs.promises.access(filePath);
    return true;
  } catch {
    return false;
  }
}

async function runTerraforgeCommand(
  output: vscode.OutputChannel,
  args: string[]
): Promise<void> {
  const root = await resolveTerraforgeRoot();
  if (!root) {
    vscode.window.showErrorMessage("No Terraforge project found.");
    return;
  }

  output.appendLine(`terraforge ${args.join(" ")}`);

  const proc = cp.spawn("terraforge", args, { cwd: root });

  proc.stdout.on("data", data => output.append(data.toString()));
  proc.stderr.on("data", data => output.append(data.toString()));

  proc.on("close", code => {
    if (code === 0) {
      vscode.window.showInformationMessage(`Terraforge ${args[0]} completed.`);
    } else {
      vscode.window.showErrorMessage(`Terraforge ${args[0]} failed.`);
    }
  });
}

async function resolveTerraforgeRoot(): Promise<string | null> {
  const folders = vscode.workspace.workspaceFolders;
  if (!folders || folders.length === 0) {
    return null;
  }

  for (const folder of folders) {
    const root = await getTerraforgeRoot(folder);
    if (root) {
      return root;
    }
  }

  return null;
}

async function getTerraforgeRoot(folder: vscode.WorkspaceFolder): Promise<string | null> {
  const configPath = path.join(folder.uri.fsPath, "terraforge.json");

  if (!await fileExists(configPath)) {
    return null;
  }

  const raw = await fs.promises.readFile(configPath, "utf8");
  const config = JSON.parse(raw);

  const root = path.resolve(folder.uri.fsPath, config.projectRoot ?? ".");
  return root;
}