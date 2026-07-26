use std::path::Path;

use crate::ipc::Catalog;

pub fn load_catalog(path: &Path) -> Result<Catalog, String> {
    let data = std::fs::read_to_string(path)
        .map_err(|e| format!("Failed to read catalog: {e}"))?;
    serde_json::from_str(&data)
        .map_err(|e| format!("Failed to parse catalog: {e}"))
}
