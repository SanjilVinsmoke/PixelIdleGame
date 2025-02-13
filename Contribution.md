# 🚀 Contributing Guidelines

Welcome to the project! To ensure smooth collaboration, we follow the **Git Flow workflow** with specific branch naming conventions. Please read and follow these guidelines when contributing.

---

## 🌳 **Branch Structure**

### 🔹 **Main Branches:**
- **`main`**: Production-ready code only.
- **`develop`**: Active development branch where features are integrated.

### 🌱 **Supporting Branches:**
- **Features:** `feat/<feature-name>`
    - For new features.
    - **Base branch:** `develop`

- **Bugfixes:** `bug/<bug-description>`
    - For fixing non-critical bugs.
    - **Base branch:** `develop`

- **Releases:** `release/<version>`
    - Prepares code for production deployment.
    - Merges into `main` and `develop`.

- **Hotfixes:** `hotfix/<critical-fix>`
    - For urgent fixes in production.
    - Merges into `main` and `develop`.

- **Support:** `support/<support-branch>`
    - For legacy or long-term support maintenance.

- **Version Tags:** Prefixed with `v` (e.g., `v1.0.0`).

---

## ⚡ **Git Flow Commands**

### ✅ **Starting Work:**
- Start a feature:
  ```bash
  git flow feature start <feature-name>
  # Example:
  git flow feature start login-page
  ```

- Start a bugfix:
  ```bash
  git flow bugfix start <bug-description>
  # Example:
  git flow bugfix start fix-login-error
  ```

- Start a release:
  ```bash
  git flow release start <version>
  # Example:
  git flow release start 1.2.0
  ```

- Start a hotfix:
  ```bash
  git flow hotfix start <critical-fix>
  # Example:
  git flow hotfix start urgent-security-patch
  ```

### 📥 **Finishing Work:**
- Finish a feature:
  ```bash
  git flow feature finish <feature-name>
  ```

- Finish a bugfix:
  ```bash
  git flow bugfix finish <bug-description>
  ```

- Finish a release or hotfix:
  ```bash
  git flow release finish <version>
  git flow hotfix finish <critical-fix>
  ```

---

## 🔑 **Commit Message Guidelines**
- Use clear, concise messages:
    - `feat: add user authentication`
    - `bug: fix null pointer exception`
    - `hotfix: resolve payment gateway issue`

---

## ⚠️ **Important Notes:**
- Always pull the latest changes before starting work:
  ```bash
  git pull origin develop
  ```
- Test your code thoroughly before creating a pull request.
- Follow code review feedback promptly.

Happy coding! 🚀

 