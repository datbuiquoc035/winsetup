use std::process::Stdio;
use tokio::process::Command;

pub async fn install_winget(id: &str) -> Result<String, String> {
    let output = Command::new("winget")
        .args(["install", "--silent", "--accept-package-agreements", id])
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .output()
        .await
        .map_err(|e| format!("Failed to execute winget: {e}"))?;

    let stdout = String::from_utf8_lossy(&output.stdout).to_string();
    let stderr = String::from_utf8_lossy(&output.stderr).to_string();

    if output.status.success() {
        Ok(stdout)
    } else {
        Err(stderr)
    }
}

pub async fn install_direct(url: &str, args: &str) -> Result<String, String> {
    let exe_path = download_file(url).await?;

    let output = Command::new(&exe_path)
        .args(args.split_whitespace())
        .stdout(Stdio::piped())
        .stderr(Stdio::piped())
        .output()
        .await
        .map_err(|e| format!("Failed to run installer: {e}"))?;

    let stderr = String::from_utf8_lossy(&output.stderr).to_string();

    if output.status.success() {
        Ok(String::from_utf8_lossy(&output.stdout).to_string())
    } else {
        Err(stderr)
    }
}

async fn download_file(url: &str) -> Result<String, String> {
    let tmp = std::env::temp_dir().join("winsetup_installer.exe");
    let response = reqwest::get(url)
        .await
        .map_err(|e| format!("Download failed: {e}"))?;

    let bytes = response
        .bytes()
        .await
        .map_err(|e| format!("Failed to read response: {e}"))?;

    std::fs::write(&tmp, &bytes)
        .map_err(|e| format!("Failed to write installer: {e}"))?;

    Ok(tmp.to_string_lossy().to_string())
}
