# DZ-MP-API

โครง Backend API (.NET 10) — monorepo multi-service ตาม **pattern เต็มของ yota (BICM)** ชื่อ prefix `DZ_MP`

## โครงสร้าง (map กับ yota)

| yota | mp |
|---|---|
| `DZ_BICM.CORE` | `DZ_MP.CORE` |
| `DZ_BICM_MASTERFILE_SERVICE.API` (5002) | `DZ_MP_MASTERFILE_SERVICE.API` (5002) |
| `DZ_BICM_SYSTEM_MGMT_SERVICE.API` (5003) | `DZ_MP_SYSTEM_MGMT_SERVICE.API` (5003) |
| `DZ_BICM_BUILDING_SERVICE.API` (5004) | `DZ_MP_BIZ_SERVICE.API` (5004) — domain service (ยังไม่มีชื่อโดเมนจริง) |
| `DZ_BICM_LOG_SERVICE.API` (5005) | `DZ_MP_LOG_SERVICE.API` (5005) |
| `DZ_BICM_SERVICES.slnx` | `DZ_MP_SERVICES.slnx` |
| `docker-compose.yml` / Dockerfile ต่อ service | เหมือนกัน |
| Jenkins / Helm / charts | **ยังไม่ใส่** (ไม่ใช่ skeleton นี้) |

## Layer pattern

```
Controller  →  IService  →  IRepository  →  DbContext (PostgreSQL / Npgsql)
```

- Repository **return Entity เสมอ**
- Service **map Entity → Response DTO**
- Response ห่อด้วย `BaseResponse<T>` จาก CORE

## Models layout ต่อ service (ตามยota จริง)

| Service | Models layout |
|---|---|
| **System Mgmt** | `Models/Entities` + `Models/DTO/{Request,Response,Auth,Notification}` |
| **Masterfile** | `Models/Entities` + `Models/DTO/{Request,Response}` |
| **Biz** (≈ Building) | `Models/Entities` + `Models/DTO` + `Models/Request` + `Models/Response` + `Models/QueryResults` |
| **Log** | `Models/Entities` + `Models/Request` + `Models/Response` (ไม่มีโฟลเดอร์ `DTO`) |

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

# ตัวอย่าง System Mgmt
dotnet run --project DZ_MP_SYSTEM_MGMT_SERVICE.API
# http://localhost:5003/api/health
# http://localhost:5003/api/v1/ping
# http://localhost:5003/scalar
```

| Service | Port |
|---|---|
| Masterfile | 5002 |
| System Mgmt | 5003 |
| Biz | 5004 |
| Log | 5005 |

## Connection string

ลำดับ:
1. env `MP_DB_CONN`
2. `ConnectionStrings:DefaultConnection` ใน appsettings ของแต่ละ service (`Search Path` ต่างกันต่อ schema)

## Docker

```bash
# ต้องมี DB_URL / JWT_KEY
docker compose up --build
```

## ยังไม่ทำ (ตั้งใจ)

- domain ธุรกิจจริง / entity จริง
- Jenkinsfile / Helm charts / GitLab CI
- MinIO / Playwright PDF providers (ของ Building)
- Tests project
