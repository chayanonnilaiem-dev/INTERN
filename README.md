# DZ-MP-API

โครง Backend API (.NET 10) — monorepo ตาม pattern yota ชื่อ prefix `DZ_MP`

## โครงสร้างปัจจุบัน

```
mp/
├── DZ_MP.CORE                         # Shared library
├── DZ_MP_BIZ_SERVICE.API              # Biz service (port 5004)
├── DZ_MP_SERVICES.slnx
├── docker-compose.yml
└── README.md
```

> เหลือเฉพาะ **CORE + Biz** (ตัด Masterfile / System Mgmt / Log ออกแล้ว)

## Layer pattern

```
Controller  →  IService  →  IRepository  →  DbContext (PostgreSQL / Npgsql)
```

- Repository **return Entity เสมอ**
- Service **map Entity → Response DTO**
- Response ห่อด้วย `BaseResponse<T>` จาก CORE

## Models layout (Biz ≈ Building ของ yota)

```
Models/
├── Entities/
├── DTO/
├── Request/
├── Response/
└── QueryResults/
```

## CORE (`DZ_MP.CORE`)

```
Authorization/   PermissionAction, IPermissionService, MenuIds
Commons/         ErrorCode
Exceptions/      BusinessException, InternalErrorException
Extensions/      Serilog, JWT, Scalar OpenAPI, ClaimsPrincipal, routing
Helpers/         DateHelper, EncryptionHelper, CachedClientBase
Middlewares/     CorrelationId, ExceptionHandling, Logging, AuthTokenPropagation
Models/DTO/      BaseResponse, PaginatedRequest/Response
Utilities/       ITokenService, IHashService, IPasswordService, ISymmetricEncryptionService (+ Impl)
```

## รัน (local)

```bash
dotnet restore DZ_MP_SERVICES.slnx
dotnet build DZ_MP_SERVICES.slnx
dotnet run --project DZ_MP_BIZ_SERVICE.API
```

- Health: `http://localhost:5004/api/health`
- Ping: `http://localhost:5004/api/v1/ping`
- Scalar: `http://localhost:5004/scalar`

## Connection string

1. env `MP_DB_CONN`
2. `ConnectionStrings:DefaultConnection` ใน appsettings

## Docker

```bash
# ต้องมี DB_URL / JWT_KEY
docker compose up --build
```

## ยังไม่ทำ (ตั้งใจ)

- domain ธุรกิจจริง / entity จริง
- Jenkins / Helm / multi-service อื่น
- MinIO / Playwright PDF / Tests
