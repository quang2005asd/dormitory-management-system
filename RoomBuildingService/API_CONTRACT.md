# RoomBuildingService API Contract

Ngay cap nhat: 2026-05-18

## Tong quan
- Base path: `/api`
- Dinh dang: JSON
- Xac thuc: chua ghi nhan trong service (neu co, se cap nhat sau)

## Quy uoc kieu du lieu
- `Guid`: chuoi UUID
- `DateTime`: ISO 8601
- Enum (string): xem phan ghi chu cho tung endpoint

## Error response (chung)
Service tra ve JSON theo `ProblemDetails` (middleware `GlobalExceptionHandler`).

**400 Bad Request** (validation / business rule)
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "Du lieu khong hop le",
  "status": 400,
  "detail": "Vui long nhap ly do khi chuyen phong sang trang thai bao tri."
}
```

**404 Not Found**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.5",
  "title": "Khong tim thay",
  "status": 404,
  "detail": "Khong tim thay du lieu."
}
```

**500 Internal Server Error**
```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.6.1",
  "title": "Loi he thong",
  "status": 500,
  "detail": "Loi he thong, vui long thu lai sau."
}
```

## Buildings
### GET /api/buildings
Tra ve danh sach toa nha.

**200 OK**
```json
[
  {
    "id": "2b2c4a9f-2b5a-4b48-8e5a-7c7b7c8db0a1",
    "name": "KTX A",
    "totalFloors": 6,
    "genderType": "MALE",
    "status": "ACTIVE",
    "description": "Khu A nam",
    "createdAt": "2026-05-18T08:00:00Z",
    "updatedAt": null
  }
]
```

### GET /api/buildings/{id}
Tra ve thong tin toa nha theo `id`.

**200 OK**
```json
{
  "id": "2b2c4a9f-2b5a-4b48-8e5a-7c7b7c8db0a1",
  "name": "KTX A",
  "totalFloors": 6,
  "genderType": "MALE",
  "status": "ACTIVE",
  "description": "Khu A nam",
  "createdAt": "2026-05-18T08:00:00Z",
  "updatedAt": null
}
```

### POST /api/buildings
Tao toa nha.

**Request body**
```json
{
  "name": "KTX A",
  "totalFloors": 6,
  "genderType": "MALE",
  "description": "Khu A nam"
}
```

**Rang buoc**
- `name`: bat buoc, toi da 50 ky tu
- `totalFloors`: bat buoc, 1-100
- `genderType`: bat buoc, MALE | FEMALE | MIXED

**201 Created**
Tra ve object `Building` vua tao.

### PUT /api/buildings/{id}
Cap nhat toa nha.

**Request body**
```json
{
  "name": "KTX A",
  "totalFloors": 7,
  "genderType": "MALE",
  "status": "ACTIVE",
  "description": "Cap nhat so tang"
}
```

**Rang buoc**
- `status`: bat buoc, ACTIVE | INACTIVE | UNDER_MAINTENANCE

**200 OK**
Tra ve object `Building` da cap nhat.

### DELETE /api/buildings/{id}
Xoa toa nha.

**204 No Content**

## RoomTypes
### GET /api/roomtypes
Tra ve danh sach loai phong.

**200 OK**
```json
[
  {
    "id": "c9fbdd80-0f79-4a8c-9d5c-5c0c2d5c3c9a",
    "typeName": "Phong 6",
    "capacity": 6,
    "basePrice": 1200000,
    "description": "Phong 6 nguoi",
    "amenities": ["May lanh", "WC rieng"],
    "createdAt": "2026-05-18T08:00:00Z",
    "updatedAt": null
  }
]
```

### GET /api/roomtypes/{id}
Tra ve loai phong theo `id`.

**200 OK**
Tra ve object `RoomType` (xem mau o GET /api/roomtypes).

### POST /api/roomtypes
Tao loai phong.

**Request body**
```json
{
  "typeName": "Phong 6",
  "capacity": 6,
  "basePrice": 1200000,
  "description": "Phong 6 nguoi",
  "amenities": ["May lanh", "WC rieng"]
}
```

**Rang buoc**
- `typeName`: bat buoc, toi da 50 ky tu
- `capacity`: bat buoc, 1-20
- `basePrice`: bat buoc, so duong

**201 Created**
Tra ve object `RoomType` vua tao.

### PUT /api/roomtypes/{id}
Cap nhat loai phong.

**Request body**
Giong `POST /api/roomtypes`.

**200 OK**
Tra ve object `RoomType` da cap nhat.

### DELETE /api/roomtypes/{id}
**204 No Content**

## Rooms
### GET /api/rooms
Loc theo `buildingId`, `floor`, `status`.

**Query params**
- `buildingId` (Guid, optional)
- `floor` (int, optional)
- `status` (string, optional)

**200 OK**
Tra ve danh sach phong (kieu `RoomResponse`).

### GET /api/rooms/{id}
Tra ve phong theo `id`.

### GET /api/rooms/floormap?buildingId=...&floor=...
Tra ve danh sach phong theo toa va tang (dung cho so do tang).

### POST /api/rooms
Tao phong.

**Request body**
```json
{
  "buildingId": "2b2c4a9f-2b5a-4b48-8e5a-7c7b7c8db0a1",
  "roomTypeId": "c9fbdd80-0f79-4a8c-9d5c-5c0c2d5c3c9a",
  "roomNumber": "A101",
  "floorNumber": 1
}
```

**Rang buoc**
- `roomNumber`: bat buoc, toi da 20 ky tu
- `floorNumber`: bat buoc, 1-100

**201 Created**
Tra ve object `Room` vua tao.

### PUT /api/rooms/{id}
Cap nhat phong.

**Request body**
```json
{
  "roomTypeId": "c9fbdd80-0f79-4a8c-9d5c-5c0c2d5c3c9a",
  "roomNumber": "A101",
  "floorNumber": 1
}
```

**200 OK**
Tra ve object `Room` da cap nhat.

### PATCH /api/rooms/{id}/status
Cap nhat trang thai phong.

**Request body**
```json
{
  "status": "UNDER_MAINTENANCE",
  "maintenanceReason": "Sua dieu hoa"
}
```

**Rang buoc**
- `status`: AVAILABLE | FULL | UNDER_MAINTENANCE | INACTIVE
- Neu `status` = UNDER_MAINTENANCE thi bat buoc co `maintenanceReason`

**204 No Content**

### DELETE /api/rooms/{id}
**204 No Content**

### RoomResponse (mau)
```json
{
  "id": "6ec4a1a2-7b42-4a4b-8b73-21b3e7cfcd11",
  "buildingId": "2b2c4a9f-2b5a-4b48-8e5a-7c7b7c8db0a1",
  "roomNumber": "A101",
  "floorNumber": 1,
  "currentOccupancy": 4,
  "status": "AVAILABLE",
  "maintenanceReason": null,
  "roomType": {
    "id": "c9fbdd80-0f79-4a8c-9d5c-5c0c2d5c3c9a",
    "typeName": "Phong 6",
    "capacity": 6,
    "basePrice": 1200000,
    "description": "Phong 6 nguoi",
    "amenities": ["May lanh", "WC rieng"],
    "createdAt": "2026-05-18T08:00:00Z",
    "updatedAt": null
  },
  "beds": [
    {
      "id": "20e0b7a6-8d89-46c2-8a2d-9c4fdb5c621f",
      "roomId": "6ec4a1a2-7b42-4a4b-8b73-21b3e7cfcd11",
      "bedNumber": "A101-01",
      "status": "AVAILABLE",
      "createdAt": "2026-05-18T08:00:00Z",
      "updatedAt": null
    }
  ],
  "equipments": [
    {
      "id": "d07f5d1b-69c7-4d1b-bdb4-3a6bc33b7a1c",
      "roomId": "6ec4a1a2-7b42-4a4b-8b73-21b3e7cfcd11",
      "equipmentName": "May lanh",
      "equipmentIndex": 1,
      "status": "ACTIVE",
      "createdAt": "2026-05-18T08:00:00Z",
      "updatedAt": null
    }
  ],
  "createdAt": "2026-05-18T08:00:00Z",
  "updatedAt": null
}
```

## Beds
### GET /api/beds?roomId=...
Tra ve danh sach giuong theo phong.

**Query params**
- `roomId` (Guid, bat buoc)

### GET /api/beds/{id}
Tra ve giuong theo `id`.

### POST /api/beds
Tao giuong.

**Request body**
```json
{
  "roomId": "6ec4a1a2-7b42-4a4b-8b73-21b3e7cfcd11",
  "bedNumber": "A101-01"
}
```

**Rang buoc**
- `bedNumber`: bat buoc, toi da 20 ky tu

**201 Created**
Tra ve object `Bed` vua tao.

### PUT /api/beds/{id}
Cap nhat giuong.

**Request body**
```json
{
  "bedNumber": "A101-01",
  "status": "AVAILABLE"
}
```

**200 OK**
Tra ve object `Bed` da cap nhat.

### DELETE /api/beds/{id}
**204 No Content**

## Equipments
### GET /api/equipments?roomId=...
Tra ve danh sach thiet bi theo phong.

**Query params**
- `roomId` (Guid, bat buoc)

### GET /api/equipments/{id}
Tra ve thiet bi theo `id`.

### POST /api/equipments
Tao thiet bi.

**Request body**
```json
{
  "roomId": "6ec4a1a2-7b42-4a4b-8b73-21b3e7cfcd11",
  "equipmentName": "May lanh"
}
```

**Rang buoc**
- `equipmentName`: bat buoc, toi da 100 ky tu

**201 Created**
Tra ve object `Equipment` vua tao.

### PATCH /api/equipments/{id}/status
Cap nhat trang thai thiet bi (nhom Maintenance su dung).

**Request body**
```json
{
  "status": "UNDER_MAINTENANCE"
}
```

**Rang buoc**
- `status`: ACTIVE | UNDER_MAINTENANCE | BROKEN | RETIRED

**204 No Content**

### DELETE /api/equipments/{id}
**204 No Content**

## Events (RabbitMQ)

Exchange: `ktx.events` (type: topic)

### room.status.changed
Publish khi phong thay doi trang thai.

**Routing key:** `room.status.changed`

**Payload:**
```json
{
  "roomId": "6ec4a1a2-7b42-4a4b-8b73-21b3e7cfcd11",
  "roomNumber": "A101",
  "buildingId": "2b2c4a9f-2b5a-4b48-8e5a-7c7b7c8db0a1",
  "oldStatus": "AVAILABLE",
  "newStatus": "UNDER_MAINTENANCE",
  "maintenanceReason": "Sua dieu hoa",
  "changedAt": "2026-05-18T08:00:00Z"
}
```

**Cac truong hop trigger:**
- AVAILABLE -> FULL        (phong day)
- FULL -> AVAILABLE        (co nguoi roi di)
- * -> UNDER_MAINTENANCE   (chuyen bao tri)
- * -> INACTIVE            (dong phong)
