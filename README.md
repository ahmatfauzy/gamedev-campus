# Unity Praktikum

Project Unity 3D untuk praktikum pengembangan game.

## 📦 Setup & Clone

### 1. Clone Repository

Clone project ini ke local perangkat Anda, dan buka folder project di Unity Hub.

### 2. Install Assets (Required)

Project ini menggunakan asset packages dari Unity Asset Store yang **tidak di-commit** ke repository untuk menghemat ukuran repo.

**Semua asset wajib diinstall sebelum menjalankan project!**

📋 **Lihat panduan lengkap di [`Assets.md`](Assets.md)** untuk:
- Daftar 6 asset packages yang diperlukan
- Link download ke Unity Asset Store
- Instruksi import asset

**Langkah install assets:**
1. Buka Unity Asset Store
2. Download semua packages (link tersedia di `Assets.md`)
3. Import ke folder `Assets/AssetsOnline/`
4. Restart Unity Editor

### 3. Buka Project di Unity

1. Buka **Unity Hub**
2. Klik **Add** → Pilih folder project ini
3. Pastikan Unity Editor versi **6.4** terinstall
4. Buka project dan tunggu import assets selesai

## 📁 Struktur Folder

```
praktikumGameDev/
├── Assets/
│   ├── AssetsOnline/          # Asset packages (tidak di-commit)
│   ├── Scenes/                # Scene files
│   ├── Scripts/               # Source code
│   ├── Animation/             # Animation files
│   ├── Settings/              # Project settings
│   └── TextMesh Pro/          # TextMesh Pro package
├── ProjectSettings/           # Unity project settings
├── Packages/                  # Package Manager config
├── README.md                  # Dokumentasi project
└── Assets.md                  # List assets
```

## 🚀 First Time Setup

- [ ] Clone repository
- [ ] Download semua 6 asset packages (lihat `Assets.md`)
- [ ] Import assets ke `Assets/AssetsOnline/`
- [ ] Buka project di Unity Hub
- [ ] Build & Run untuk testing

## 🛠 Requirements

- **Unity 6.4**
- **Git**

## 📝 Notes

- Folder `Assets/AssetsOnline/` diabaikan dari Git (ada di `.gitignore`)
- Setiap kali clone baru, wajib install assets manual
- Pastikan semua asset terimport dengan benar sebelum menjalankan project
