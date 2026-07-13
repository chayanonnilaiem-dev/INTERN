# Multi-service monorepo แบบ yota

ใช้รูปแบบ monorepo multi-service เหมือน yota เพราะต้องการ pattern, naming, DI/middleware conventions เดิมของทีม:
shared `DZ_MP.CORE` + service แยก project (Masterfile / System Mgmt / Biz / Log) + Dockerfile ต่อ service + docker-compose + slnx

ตัดเฉพาะ CI/CD (Jenkins) และ Helm ออกจาก skeleton — ยังกลับมาเติมได้โดยไม่กระทบโครงโค้ด

## DTO layout ต่อ service

ยึด layout จริงของ yota ไม่ทำแบบเดียวกันทุก service:
- System Mgmt → `Models/DTO/{Request,Response,Auth,...}` + Entities
- Masterfile → `Models/DTO/{Request,Response}` + Entities
- Biz (≈ Building) → `Models/{DTO,Entities,Request,Response,QueryResults}`
- Log → `Models/{Entities,Request,Response}` ไม่มีโฟลเดอร์ DTO
