# 📱 NovaIMS (IMS‑Mobile)

A modern mobile client for Inventory Management, built with **.NET MAUI** and backed by **Supabase**. NovaIMS provides powerful inventory tools, analytics, and reporting capabilities while maintaining a clean mobile‑friendly interface.

---

## ✨ Features

### 🔑 Core

* 👤 **User Authentication** (Login / Logout with Supabase Auth)
* 📊 **Dashboard Overview** with KPIs & quick actions
* 📦 **Inventory Management**: Add / Edit / Delete / View items
* 🔍 **Search & Filtering** across items
* 🔄 **Transaction Recording** (Sales, Purchases, Stock updates)
* 📱 **Responsive UI** with bottom navigation
* 📴 **Offline Support** with SQLite caching
* 🔗 **Sync with Supabase** backend when online
* 🛠 **MVVM Architecture** with data binding
* 🎨 **Static resources & themes** for consistent design

### 📑 Advanced

* 📈 **Analytics** with charts and metrics
* 🧾 **Reports** for stock and transactions
* 📤 **Export Reports as PDF** (locally and shareable)
* 🌐 **Supabase Backend Integration** (Database + Auth + API)
* ⚠️ **Error Handling & Validation**

### 🚀 Planned

* 🔔 Push Notifications (via Supabase Functions / FCM)
* 🎨 Dark Mode & custom theming
* 🌍 Localization (multi‑language support)
* 📊 Advanced reporting (CSV, XLSX)
* 💰 Monetization (Ads: Banner / Interstitial)

---

## 🛠 Tech Stack

* **Framework**: .NET MAUI (C# + XAML)
* **Architecture**: MVVM (CommunityToolkit.MVVM)
* **Backend**: Supabase (Auth + Database)
* **Local Storage**: SQLite (offline support)
* **UI**: XAML layouts, styles, resources
* **Exporting**: Report generation to PDF

---

## ⚡ Getting Started

### Prerequisites

* Visual Studio with .NET MAUI workload
* .NET 6/7 SDK
* Android SDK (and Xcode for iOS)

### Setup

```bash
git clone https://github.com/Bhbored/IMS-Mobile.git
cd IMS-Mobile
dotnet restore
```

### Run

```bash
# Android
dotnet build -t:Run -f net7.0-android

# iOS
dotnet build -t:Run -f net7.0-ios
```

### Configuration

* Update Supabase project URL and API key in config/service files.
* Do not commit secrets to GitHub (use environment variables).

---

## 📈 Roadmap

1. Push notifications (real‑time IMS updates)
2. Dark mode + theme customization
3. Localization (multi‑language)
4. CSV/XLSX report exports
5. Ads integration (monetization)

---

## 🤝 Contributing

1. Fork the repo
2. Create a feature branch (`feature/your-feature`)
3. Commit changes with clear messages
4. Open a Pull Request

---


## 👨‍💻 Author

* Developer: **Bhbored**
* GitHub: [@Bhbored](https://github.com/Bhbored)

---

🚀 *NovaIMS: Smart inventory management on the go.*
