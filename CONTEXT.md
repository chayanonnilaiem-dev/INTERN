# MP

โครง .NET 10 backend multi-service ตาม pattern monorepo ของ yota (BICM) — scaffold โครงสร้าง พร้อม multi-service shells ยังไม่ผูกโดเมนธุรกิจ

## Language

**DZ_MP**:
ชื่อ prefix ของ solution/project ใน monorepo นี้ (แทน `DZ_BICM` ของ yota)
_Avoid_: MP alone, DzMp

**CORE**:
shared library (`DZ_MP.CORE`) ที่ทุก API project อ้างอิง — middlewares, JWT, response wrapper, utilities
_Avoid_: shared kit, common lib (เมื่อหมายถึง project นี้)

**System Mgmt Service**:
service จัดการผู้ใช้/สิทธิ์/การตั้งค่าระบบ บนพอร์ต 5003
_Avoid_: auth service อย่างเดียว, identity

**Masterfile Service**:
service ข้อมูล master / reference บนพอร์ต 5002
_Avoid_: lookup service

**Biz Service**:
service โดเมนธุรกิจหลัก บนพอร์ต 5004 (เทียบ Building ของ yota — ชื่อ generic จนกว่าจะมีโดเมนจริง)
_Avoid_: Building Service (จนกว่าโดเมนจะชัดว่าใช่ building)

**Log Service**:
service บันทึก audit/error log บนพอร์ต 5005
_Avoid_: logging framework

**Entity**:
โมเดลที่ map กับตาราง DB อยู่ที่ `Models/Entities/` — Repository คืนค่า Entity เสมอ
_Avoid_: DTO, model (เมื่อหมายถึง DB row)

**DTO**:
ชั้นถ่ายโอนข้อมูล API — วางตาม layout ของแต่ละ service (System Mgmt/Masterfile ใช้ `Models/DTO/*`, Log ใช้ `Models/Request|Response` ตรง root แบบ yota)
_Avoid_: model (เมื่อหมายถึง request/response)

**Request**:
DTO ฝั่งรับ input จาก client
_Avoid_: input DTO, command (ในชั้น API)

**Response**:
DTO ฝั่งส่ง output กลับ client — Service เป็นคน map จาก Entity
_Avoid_: output DTO, view model

**BaseResponse**:
response wrapper กลางใน CORE (`statusCode`, `message`, `errorCode`, `data`, `traceId`, `errors`)
_Avoid_: ApiResponse, Result envelope
