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
## 📸 Screenshots

|                                                                                                                    |                                                                                                                    |                                                                                                                    |
| :----------------------------------------------------------------------------------------------------------------: | :----------------------------------------------------------------------------------------------------------------: | :----------------------------------------------------------------------------------------------------------------: | :----------------------------------------------------------------------------------------------------------------: |
| ![1](https://github.com/user-attachments/assets/0b27f336-b8b0-4afc-9d76-c2314d755409) | ![2](https://github.com/user-attachments/assets/8b2e2b52-215e-493c-ba63-bc4116c7ec94) | ![3](https://github.com/user-attachments/assets/74f5e0d0-10de-4e5a-a2dd-9dfe2f02558f) | ![4](https://github.com/user-attachments/assets/f5b3b384-0a20-40d0-ade6-14d0d4872b3c) |
| ![5](https://github.com/user-attachments/assets/2f638665-4c1b-4b9a-84ca-0a63453903e9) | ![6](https://github.com/user-attachments/assets/1dbebb36-1cc5-4544-8a6b-a41e5e572a42) | ![7](https://github.com/user-attachments/assets/eadc79ae-63ca-495a-8d35-a6eab0fb8a60) | ![8](https://github.com/user-attachments/assets/ae77d557-1e43-4a0b-81c1-7b5dff22db02) |
| ![9](https://github.com/user-attachments/assets/fc1843a7-af61-4473-a5e6-651d6f092317) | ![10](https://github.com/user-attachments/assets/b3b57273-5686-4984-b6d7-6b0cda82fdde) | ![11](https://github.com/user-attachments/assets/386ac27d-42d6-4dad-a0b8-bfcfe47d3b8c) | ![12](https://github.com/user-attachments/assets/5c3644be-96e3-4b04-82d1-9fff083c9512) |
| ![13](https://github.com/user-attachments/assets/d9990413-190a-4f9a-b152-4ec2335b88af) | ![14](https://github.com/user-attachments/assets/2bcca720-14c5-4706-ac75-668282fba011) | ![15](https://github.com/user-attachments/assets/4e3c1aeb-13d8-4fda-b17a-c6950d925cb9) | ![16](https://github.com/user-attachments/assets/113ad106-cc68-49b1-a51f-908c84074777) |
| ![17](https://github.com/user-attachments/assets/dbbe15c8-d548-42ce-8927-021c7adc0f86) |  |  |  |

🚀 *NovaIMS: Smart inventory management on the go.*
