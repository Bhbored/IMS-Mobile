# NovaIMS (IMS‑Mobile)

A lightweight mobile client for an Inventory Management System — designed for quick lookups, offline edits, and smooth inventory workflows on the go.

---

## Features

**Core (Completed)**

* ✅ User authentication (Login / Logout)
* ✅ Dashboard overview (high‑level KPIs and quick actions)
* ✅ Inventory listing with search and pagination
* ✅ Add / Edit / Delete inventory items
* ✅ Transaction recording (sales / stock changes)
* ✅ Offline caching (view & edit while offline; sync when online)
* ✅ Manual and automatic sync with backend when connectivity returns
* ✅ Reports page with stock and transaction summaries
* ✅ Responsive mobile UI with tab/bottom navigation
* ✅ Static resources & centralized styles (themes)
* ✅ Basic validation and error handling
* ✅ Signed Android build (.aab) produced for testing

**Planned / Optional**

* 🔲 Push notifications / real‑time alerts
* 🔲 Ads / monetization (banner / interstitial) — planned for later
* 🔲 Dark mode and additional theming options
* 🔲 Localization (multi‑language support)
* 🔲 Advanced reporting and export (CSV, PDF)
* 🔲 Analytics / user behavior tracking

---

## Other stuff (important notes)

* **Project name:** NovaIMS (use this name in store listing and screenshots)
* **Architecture:** MVVM (Models, Views, ViewModels) using .NET MAUI / XAML
* **Local storage:** SQLite or equivalent for offline support
* **Networking:** HTTP(s) client to communicate with IMS backend API
* **App signing:** You already have a signed AAB for testing; keep the keystore and passwords backed up securely.
* **Resources:** Fix any missing StaticResource references before producing release builds — missing resources can break XAML at runtime.
* **Permissions:** Review AndroidManifest for only the permissions actually needed to avoid Play Store policy issues.

---

## Getting started (developer)

### Prerequisites

* Visual Studio (Windows or Mac) with .NET MAUI mobile workload
* .NET SDK matching the project (check `global.json` or project targets)
* Android SDK (and Xcode if targeting iOS)

### Build & run (example)

```bash
# restore
dotnet restore
# build for android
dotnet build -f net6.0-android -c Release
# run on device/emulator (or use Visual Studio)
```


---

## Contact

* GitHub: `@Bhbored`
* Project: NovaIMS

---

