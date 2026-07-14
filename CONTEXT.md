# MP

โครง .NET 10 backend ตาม pattern monorepo ของ yota (BICM) — ตอนนี้เหลือ `DZ_MP.CORE` + `DZ_MP_BIZ_SERVICE.API` ยังไม่ผูกโดเมนธุรกิจ

## Language

**DZ_MP**:
ชื่อ prefix ของ solution/project ใน monorepo นี้ (แทน `DZ_BICM` ของ yota)
_Avoid_: MP alone, DzMp

**CORE**:
shared library (`DZ_MP.CORE`) ที่ API project อ้างอิง — middlewares, JWT, response wrapper, utilities
_Avoid_: shared kit, common lib (เมื่อหมายถึง project นี้)

**Biz Service**:
service โดเมนธุรกิจหลัก บนพอร์ต 5004 (เทียบ Building ของ yota — ชื่อ generic จนกว่าจะมีโดเมนจริง)
_Avoid_: Building Service (จนกว่าโดเมนจะชัดว่าใช่ building)

**Entity**:
โมเดลที่ map กับตาราง DB อยู่ที่ `Models/Entities/` — Repository คืนค่า Entity เสมอ
_Avoid_: DTO, model (เมื่อหมายถึง DB row)

**DTO**:
ชั้นถ่ายโอนข้อมูล API (Biz ใช้ layout แบบ Building: `Models/DTO` + `Models/Request` + `Models/Response` + `Models/QueryResults`)
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
