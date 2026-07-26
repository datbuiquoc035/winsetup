mod installer;
mod ipc;
mod manifest;
mod winget;

use std::path::Path;

use ipc::{IpcRequest, IpcResponse};

fn send_response(resp: &IpcResponse) {
    if let Ok(json) = serde_json::to_string(resp) {
        println!("{json}");
    }
}

fn send_error(msg: &str) {
    send_response(&IpcResponse::Error { message: msg.to_string() });
}

#[tokio::main]
async fn main() {
    let stdin = tokio::io::BufReader::new(tokio::io::stdin());
    let mut lines = tokio::io::AsyncBufReadExt::lines(stdin);

    let catalog_path = Path::new("catalog.json");
    let catalog = match manifest::load_catalog(catalog_path) {
        Ok(c) => c,
        Err(e) => {
            send_error(&format!("Failed to load catalog: {e}"));
            return;
        }
    };

    let (tx, mut rx) = tokio::sync::mpsc::unbounded_channel::<IpcResponse>();

    loop {
        tokio::select! {
            line = lines.next_line() => {
                match line {
                    Ok(Some(line)) => {
                        let request: IpcRequest = match serde_json::from_str(&line) {
                            Ok(req) => req,
                            Err(e) => {
                                send_error(&format!("Invalid request: {e}"));
                                continue;
                            }
                        };

                        match request {
                            IpcRequest::GetCatalog => {
                                send_response(&IpcResponse::Catalog { items: catalog.items.clone() });
                            }
                            IpcRequest::Install { ids } => {
                                let selected: Vec<_> = catalog.items.iter()
                                    .filter(|item| ids.contains(&item.id))
                                    .cloned()
                                    .collect();
                                let tx_clone = tx.clone();
                                tokio::spawn(async move {
                                    installer::Installer::run(&selected, tx_clone).await;
                                });
                            }
                            IpcRequest::Cancel => {
                                // In a full implementation, cancellation would
                                // be wired through the installer.
                                break;
                            }
                        }
                    }
                    Ok(None) => break,
                    Err(e) => {
                        send_error(&format!("IO error: {e}"));
                        break;
                    }
                }
            }
            resp = rx.recv() => {
                match resp {
                    Some(response) => send_response(&response),
                    None => break,
                }
            }
        }
    }
}
