# Commit Message Convention — DZ-MP-API

รูปแบบ commit message มาตรฐานของโปรเจกต์นี้

---

## รูปแบบ

```
<type>(<scope>): <description>
```

- **type** — ประเภทของ commit (บังคับ)
- **scope** — ชื่อ module/feature ที่แก้ไข (ไม่บังคับ แต่แนะนำ)
- **description** — อธิบายสิ่งที่ทำ เป็นภาษาไทย (บังคับ)

---

## Type ที่ใช้

| Type | ความหมาย |
|---|---|
| `feat` | ฟีเจอร์ใหม่ / เพิ่ม API / เพิ่มฟังก์ชัน |
| `fix` | แก้บัก |
| `refactor` | ปรับโครงสร้างโค้ด ไม่เปลี่ยน behavior |
| `chore` | งาน config, dependency, CI/CD |
| `docs` | แก้ไข documentation เท่านั้น |

---

## Scope ที่ใช้บ่อย

| Scope | หมายถึง |
|---|---|
| `system-mgmt` | System Management service |
| `masterfile` | Masterfile service |
| `biz` | Biz / domain service |
| `log` | Log service |
| `core` | Shared CORE library |
