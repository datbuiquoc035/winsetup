use crate::ipc::{CatalogItem, IpcResponse, ProgressStatus};
use crate::winget;

pub struct Installer;

impl Installer {
    pub async fn run(items: &[CatalogItem], tx: tokio::sync::mpsc::UnboundedSender<IpcResponse>) {
        let mut succeeded = Vec::new();
        let mut failed = Vec::new();

        for item in items {
            let _ = tx.send(IpcResponse::Progress {
                id: item.id.clone(),
                status: ProgressStatus::Installing,
                message: format!("Installing {}...", item.name),
            });

            let result = if let Some(winget_id) = &item.winget_id {
                winget::install_winget(winget_id).await
            } else if let (Some(url), Some(args)) = (&item.download_url, &item.silent_args) {
                winget::install_direct(url, args).await
            } else {
                Err("No install method configured".to_string())
            };

            match result {
                Ok(_) => {
                    let _ = tx.send(IpcResponse::Progress {
                        id: item.id.clone(),
                        status: ProgressStatus::Done,
                        message: format!("{} installed successfully", item.name),
                    });
                    succeeded.push(item.id.clone());
                }
                Err(e) => {
                    let _ = tx.send(IpcResponse::Progress {
                        id: item.id.clone(),
                        status: ProgressStatus::Failed,
                        message: format!("{} failed: {e}", item.name),
                    });
                    failed.push((item.id.clone(), e));
                }
            }
        }

        let _ = tx.send(IpcResponse::Summary { succeeded, failed });
    }
}
