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

**Planned**

* 🔲 Push notifications / real‑time alerts
* 🔲 Localization (multi‑language support)

---


## Getting started 

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

