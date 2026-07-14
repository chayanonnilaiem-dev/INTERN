# Multi-service monorepo แบบ yota (scope ปัจจุบัน: CORE + Biz)

ใช้รูปแบบ monorepo เหมือน yota เพราะต้องการ pattern, naming, DI/middleware conventions เดิมของทีม:
shared `DZ_MP.CORE` + service API แยก project + Dockerfile + docker-compose + slnx

ตัด Masterfile / System Mgmt / Log / Jenkins / Helm ออกจาก scaffold ตอนนี้ — โฟกัส Biz service
Biz ใช้ Models layout แบบ Building: `Models/{DTO,Entities,Request,Response,QueryResults}`
