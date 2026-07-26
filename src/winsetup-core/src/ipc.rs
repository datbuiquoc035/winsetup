use serde::{Deserialize, Serialize};

#[derive(Serialize, Deserialize)]
pub struct CatalogItem {
    pub id: String,
    pub name: String,
    pub category: String,
    pub description: String,
    #[serde(default)]
    pub winget_id: Option<String>,
    #[serde(default)]
    pub download_url: Option<String>,
    #[serde(default)]
    pub silent_args: Option<String>,
}

#[derive(Serialize, Deserialize)]
pub struct Catalog {
    pub items: Vec<CatalogItem>,
}

#[derive(Serialize, Deserialize)]
#[serde(tag = "type")]
pub enum IpcRequest {
    GetCatalog,
    Install { ids: Vec<String> },
    Cancel,
}

#[derive(Serialize, Deserialize)]
#[serde(tag = "type")]
pub enum IpcResponse {
    Catalog { items: Vec<CatalogItem> },
    Progress { id: String, status: ProgressStatus, message: String },
    Summary { succeeded: Vec<String>, failed: Vec<(String, String)> },
    Error { message: String },
}

#[derive(Serialize, Deserialize)]
pub enum ProgressStatus {
    Queued,
    Downloading,
    Installing,
    Done,
    Failed,
}
