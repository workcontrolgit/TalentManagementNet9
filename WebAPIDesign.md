# Position Description Web API Design

## Overview
This document outlines the Web API design for the Position Description system, supporting form display, workflow management, ACRS/GEMS integration, and email notifications.

## System Architecture

### Core Components
- **Position Description Forms** - J and K form data entry and management
- **Workflow Engine** - Approval process management via PdTrackHistory
- **Interface Layer** - Data staging to AcrsGemsPd for external system integration
- **Notification System** - Email notifications for workflow events

### Data Flow
```
Form Display (with External WebAPI) → Create/Edit PD → Submit → Workflow → Approval → Interface Staging → External Export → Email Notification
```

### Integration Architecture
```
┌─────────────────┐    ┌──────────────────┐    ┌─────────────────┐
│   Frontend      │    │   Local          │    │   External      │
│   Form          │◄───┤   Web API        │◄───┤   WebAPI        │
└─────────────────┘    └──────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌──────────────────┐    ┌─────────────────┐
                       │   Workflow       │    │   ACRS/GEMS     │
                       │  (PdTrackHistory)│───►│   (AcrsGemsPd)  │
                       └──────────────────┘    └─────────────────┘
                              │
                              ▼
                       ┌──────────────────┐
                       │   Email Service  │
                       │   (Notifications)│
                       └──────────────────┘
```

## API Endpoints

### 1. Form Display APIs (Lookup/Reference Data)

#### Get PD Types
```http
GET /api/v1/lookups/pd-types
```
**Description:** Returns available Position Description types for dropdown selection.

**Response:**
```json
{
  "data": [
    { "code": "STANDARD", "name": "Standard Position", "description": "Regular full-time position", "isActive": true },
    { "code": "TEMPORARY", "name": "Temporary Position", "description": "Temporary or contract position", "isActive": true },
    { "code": "INTERN", "name": "Internship Position", "description": "Student internship position", "isActive": true }
  ],
  "success": true,
  "message": "Retrieved PD types successfully"
}
```

#### Get Acquisition Position Types
```http
GET /api/v1/lookups/acquisition-positions
```
**Description:** Returns acquisition position indicators for compliance tracking.

**Response:**
```json
{
  "data": [
    { "code": "YES", "name": "Yes", "description": "Position requires acquisition certification", "isActive": true },
    { "code": "NO", "name": "No", "description": "Position does not require acquisition certification", "isActive": true },
    { "code": "PARTIAL", "name": "Partial", "description": "Position has some acquisition responsibilities", "isActive": true }
  ],
  "success": true,
  "message": "Retrieved acquisition position types successfully"
}
```

#### Get Financial Statement Requirements
```http
GET /api/v1/lookups/financial-statement-requirements
```
**Description:** Returns financial disclosure requirements for positions.

**Response:**
```json
{
  "data": [
    { "code": "REQUIRED", "name": "Required", "description": "Financial disclosure required", "isActive": true },
    { "code": "NOT_REQUIRED", "name": "Not Required", "description": "No financial disclosure needed", "isActive": true },
    { "code": "CONDITIONAL", "name": "Conditional", "description": "Required under certain conditions", "isActive": true }
  ],
  "success": true,
  "message": "Retrieved financial statement requirements successfully"
}
```

#### Get All Form Data (Combined)
```http
GET /api/v1/lookups/form-data
```
**Description:** Returns all lookup data needed for form display in a single call, combining internal lookups with External WebAPI data.

**Response:**
```json
{
  "data": {
    "pdTypes": [...],
    "acquisitionPositions": [...],
    "financialStatementRequirements": [...],
    "occupationalSeries": [
      {
        "code": "2210",
        "value": "Information Technology Management",
        "parentSeries": "2200",
        "isActive": true
      }
    ],
    "payPlans": [
      {
        "code": "GS",
        "value": "General Schedule",
        "description": "General Schedule pay plan",
        "isActive": true
      }
    ],
    "hiringPaths": [
      {
        "code": "public",
        "value": "Open to the public",
        "description": "All US citizens",
        "isActive": true
      }
    ],
    "positionScheduleTypes": [
      {
        "code": "1",
        "value": "Full-time",
        "description": "Full-time permanent position",
        "isActive": true
      }
    ],
    "securityClearances": [...],
    "countries": [...],
    "postalCodes": [...]
  },
  "success": true,
  "message": "Retrieved all form lookup data successfully"
}
```

#### Get Occupational Series
```http
GET /api/v1/lookups/occupational-series?search=software
```
**Description:** Returns occupational series data from External WebAPI with optional search filtering.

**Query Parameters:**
- `search` - Optional search term to filter occupational series

**Response:**
```json
{
  "data": [
    {
      "code": "2210",
      "value": "Information Technology Management",
      "parentSeries": "2200",
      "isActive": true,
      "jobFamily": "Information Technology"
    },
    {
      "code": "0854",
      "value": "Computer Engineering",
      "parentSeries": "0800",
      "isActive": true,
      "jobFamily": "Engineering"
    }
  ],
  "success": true,
  "message": "Retrieved occupational series successfully"
}
```

#### Get Pay Plans
```http
GET /api/v1/lookups/pay-plans
```
**Description:** Returns pay plan data from External WebAPI for salary structure selection.

**Response:**
```json
{
  "data": [
    {
      "code": "GS",
      "value": "General Schedule",
      "description": "General Schedule pay plan for most federal employees",
      "isActive": true
    },
    {
      "code": "WG",
      "value": "Wage Grade",
      "description": "Federal Wage System for trades and crafts",
      "isActive": true
    },
    {
      "code": "SES",
      "value": "Senior Executive Service",
      "description": "Senior Executive Service positions",
      "isActive": true
    }
  ],
  "success": true,
  "message": "Retrieved pay plans successfully"
}
```

#### Get Hiring Paths
```http
GET /api/v1/lookups/hiring-paths
```
**Description:** Returns hiring path data from External WebAPI for applicant eligibility.

**Response:**
```json
{
  "data": [
    {
      "code": "public",
      "value": "Open to the public",
      "description": "All qualified US citizens",
      "isActive": true
    },
    {
      "code": "fed-employees",
      "value": "Federal employees",
      "description": "Current federal employees only",
      "isActive": true
    },
    {
      "code": "veterans",
      "value": "Veterans",
      "description": "Military veterans with preference",
      "isActive": true
    }
  ],
  "success": true,
  "message": "Retrieved hiring paths successfully"
}
```

#### Get Position Schedule Types
```http
GET /api/v1/lookups/position-schedule-types
```
**Description:** Returns position schedule types from External WebAPI for work arrangements.

**Response:**
```json
{
  "data": [
    {
      "code": "1",
      "value": "Full-time",
      "description": "Full-time permanent position",
      "isActive": true
    },
    {
      "code": "2",
      "value": "Part-time",
      "description": "Part-time permanent position",
      "isActive": true
    },
    {
      "code": "3",
      "value": "Shift work",
      "description": "Shift work schedule",
      "isActive": true
    }
  ],
  "success": true,
  "message": "Retrieved position schedule types successfully"
}
```

#### Get Security Clearances
```http
GET /api/v1/lookups/security-clearances
```
**Description:** Returns security clearance levels from External WebAPI.

**Response:**
```json
{
  "data": [
    {
      "code": "PUBLIC_TRUST",
      "value": "Public Trust",
      "description": "Public trust background investigation required",
      "isActive": true
    },
    {
      "code": "SECRET",
      "value": "Secret",
      "description": "Secret security clearance required",
      "isActive": true
    },
    {
      "code": "TOP_SECRET",
      "value": "Top Secret",
      "description": "Top Secret security clearance required",
      "isActive": true
    }
  ],
  "success": true,
  "message": "Retrieved security clearances successfully"
}
```

### 2. Position Description CRUD APIs

#### Create Position Description
```http
POST /api/v1/position-descriptions
```
**Description:** Creates a new Position Description in draft status.

**Request Body:**
```json
{
  "pdNumber": "PD-2025-001",
  "positionTitle": "Senior Software Engineer",
  "organizationalTitle": "Lead Developer",
  "occupationalSeries": "2210",
  "payPlan": "GS",
  "grade": "13",
  "pdType": "STANDARD",
  "acquisitionPosition": "NO",
  "financialStatementRequired": "NOT_REQUIRED",
  "duties": "Primary duties include...",
  "qualifications": "Required qualifications...",
  "supervisoryResponsibilities": false,
  "managementLevel": "NON_SUPERVISORY",
  "createdBy": "user@agency.gov"
}
```

**Response:**
```json
{
  "data": {
    "pdSeqNum": 12345,
    "pdNumber": "PD-2025-001",
    "status": "DRAFT",
    "createdDate": "2025-01-06T10:00:00Z"
  },
  "success": true,
  "message": "Position Description created successfully"
}
```

#### Get Position Description by ID
```http
GET /api/v1/position-descriptions/{pdSeqNum}
```
**Description:** Retrieves a specific Position Description by its sequence number.

**Response:**
```json
{
  "data": {
    "pdSeqNum": 12345,
    "pdNumber": "PD-2025-001",
    "positionTitle": "Senior Software Engineer",
    "organizationalTitle": "Lead Developer",
    "occupationalSeries": "2210",
    "payPlan": "GS",
    "grade": "13",
    "pdType": "STANDARD",
    "acquisitionPosition": "NO",
    "financialStatementRequired": "NOT_REQUIRED",
    "duties": "Primary duties include...",
    "qualifications": "Required qualifications...",
    "supervisoryResponsibilities": false,
    "managementLevel": "NON_SUPERVISORY",
    "currentStatus": "DRAFT",
    "createdBy": "user@agency.gov",
    "createdDate": "2025-01-06T10:00:00Z",
    "lastModifiedBy": "user@agency.gov",
    "lastModifiedDate": "2025-01-06T10:00:00Z"
  },
  "success": true,
  "message": "Position Description retrieved successfully"
}
```

#### Update Position Description
```http
PUT /api/v1/position-descriptions/{pdSeqNum}
```
**Description:** Updates an existing Position Description. Only allowed in DRAFT or CHANGES_REQUESTED status.

**Request Body:** (Same structure as Create)

**Response:**
```json
{
  "data": {
    "pdSeqNum": 12345,
    "lastModifiedBy": "user@agency.gov",
    "lastModifiedDate": "2025-01-06T14:30:00Z"
  },
  "success": true,
  "message": "Position Description updated successfully"
}
```

#### List Position Descriptions
```http
GET /api/v1/position-descriptions?status=DRAFT&createdBy=user@agency.gov&pageNumber=1&pageSize=10
```
**Description:** Retrieves a paginated list of Position Descriptions with filtering options.

**Query Parameters:**
- `status` - Filter by workflow status
- `createdBy` - Filter by creator
- `pdType` - Filter by PD type
- `pageNumber` - Page number (default: 1)
- `pageSize` - Items per page (default: 10)

**Response:**
```json
{
  "data": [
    {
      "pdSeqNum": 12345,
      "pdNumber": "PD-2025-001",
      "positionTitle": "Senior Software Engineer",
      "currentStatus": "DRAFT",
      "createdBy": "user@agency.gov",
      "createdDate": "2025-01-06T10:00:00Z"
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 10,
    "totalCount": 25,
    "totalPages": 3
  },
  "success": true,
  "message": "Position Descriptions retrieved successfully"
}
```

#### Submit Position Description
```http
POST /api/v1/position-descriptions/{pdSeqNum}/submit
```
**Description:** Submits a Position Description to the workflow for approval.

**Request Body:**
```json
{
  "submitComments": "Ready for management review",
  "submittedBy": "user@agency.gov"
}
```

**Response:**
```json
{
  "data": {
    "pdSeqNum": 12345,
    "newStatus": "SUBMITTED",
    "submittedDate": "2025-01-06T15:00:00Z",
    "trackingId": 67890
  },
  "success": true,
  "message": "Position Description submitted for approval"
}
```

#### Delete Position Description
```http
DELETE /api/v1/position-descriptions/{pdSeqNum}
```
**Description:** Deletes a Position Description. Only allowed in DRAFT status.

**Response:**
```json
{
  "success": true,
  "message": "Position Description deleted successfully"
}
```

### 3. Workflow APIs (PdTrackHistory Integration)

#### Get Pending Approvals
```http
GET /api/v1/workflow/pending?assignedTo=approver@agency.gov
```
**Description:** Retrieves Position Descriptions pending approval for the current user.

**Response:**
```json
{
  "data": [
    {
      "pdSeqNum": 12345,
      "pdNumber": "PD-2025-001",
      "positionTitle": "Senior Software Engineer",
      "currentStatus": "UNDER_REVIEW",
      "submittedBy": "user@agency.gov",
      "submittedDate": "2025-01-06T15:00:00Z",
      "daysPending": 2,
      "priority": "NORMAL"
    }
  ],
  "success": true,
  "message": "Retrieved pending approvals successfully"
}
```

#### Approve Position Description
```http
POST /api/v1/workflow/{pdSeqNum}/approve
```
**Description:** Approves a Position Description and updates PdTrackHistory with PdStateCd = 'APPROVED'.

**Request Body:**
```json
{
  "approvalComments": "Position description meets all requirements",
  "approvedBy": "approver@agency.gov",
  "autoStageToInterface": true
}
```

**Response:**
```json
{
  "data": {
    "pdSeqNum": 12345,
    "newStatus": "APPROVED",
    "approvedDate": "2025-01-06T16:00:00Z",
    "trackingId": 67891,
    "stagedToInterface": true
  },
  "success": true,
  "message": "Position Description approved successfully"
}
```

#### Reject Position Description
```http
POST /api/v1/workflow/{pdSeqNum}/reject
```
**Description:** Rejects a Position Description and updates PdTrackHistory with PdStateCd = 'REJECTED'.

**Request Body:**
```json
{
  "rejectionComments": "Missing required qualifications section",
  "rejectedBy": "approver@agency.gov",
  "rejectionReason": "INCOMPLETE_INFORMATION"
}
```

**Response:**
```json
{
  "data": {
    "pdSeqNum": 12345,
    "newStatus": "REJECTED",
    "rejectedDate": "2025-01-06T16:00:00Z",
    "trackingId": 67892
  },
  "success": true,
  "message": "Position Description rejected"
}
```

#### Request Changes
```http
POST /api/v1/workflow/{pdSeqNum}/request-changes
```
**Description:** Requests changes to a Position Description and updates PdTrackHistory with PdStateCd = 'CHANGES_REQUESTED'.

**Request Body:**
```json
{
  "changeComments": "Please clarify supervisory responsibilities",
  "requestedBy": "approver@agency.gov",
  "changeCategory": "CLARIFICATION_NEEDED"
}
```

**Response:**
```json
{
  "data": {
    "pdSeqNum": 12345,
    "newStatus": "CHANGES_REQUESTED",
    "requestedDate": "2025-01-06T16:00:00Z",
    "trackingId": 67893
  },
  "success": true,
  "message": "Changes requested for Position Description"
}
```

#### Get Workflow History
```http
GET /api/v1/workflow/{pdSeqNum}/history
```
**Description:** Retrieves complete workflow history from PdTrackHistory for a specific Position Description.

**Response:**
```json
{
  "data": [
    {
      "trackingId": 67890,
      "pdSeqNum": 12345,
      "pdStateCd": "SUBMITTED",
      "statusDate": "2025-01-06T15:00:00Z",
      "userId": "user@agency.gov",
      "comments": "Ready for management review",
      "actionType": "SUBMIT"
    },
    {
      "trackingId": 67891,
      "pdSeqNum": 12345,
      "pdStateCd": "APPROVED",
      "statusDate": "2025-01-06T16:00:00Z",
      "userId": "approver@agency.gov",
      "comments": "Position description meets all requirements",
      "actionType": "APPROVE"
    }
  ],
  "success": true,
  "message": "Workflow history retrieved successfully"
}
```

#### Get Current Workflow Status
```http
GET /api/v1/workflow/{pdSeqNum}/status
```
**Description:** Retrieves the current workflow status from the latest PdTrackHistory entry.

**Response:**
```json
{
  "data": {
    "pdSeqNum": 12345,
    "currentStatus": "APPROVED",
    "statusDate": "2025-01-06T16:00:00Z",
    "assignedTo": "approver@agency.gov",
    "comments": "Position description meets all requirements",
    "canEdit": false,
    "canSubmit": false,
    "canApprove": false,
    "nextActions": ["STAGE_TO_INTERFACE"]
  },
  "success": true,
  "message": "Current workflow status retrieved successfully"
}
```

### 4. Interface APIs (AcrsGemsPd Integration)

#### Stage to Interface
```http
POST /api/v1/interface/{pdSeqNum}/stage
```
**Description:** Stages approved Position Description data to AcrsGemsPd table for external system pickup.

**Request Body:**
```json
{
  "forceStaging": false,
  "stagingComments": "Approved PD ready for GEMS integration",
  "stagedBy": "system@agency.gov"
}
```

**Response:**
```json
{
  "data": {
    "gemsPdId": 98765,
    "pdSeqNum": 12345,
    "stagedDate": "2025-01-06T16:30:00Z",
    "status": "READY_FOR_EXPORT",
    "exportPriority": "NORMAL"
  },
  "success": true,
  "message": "Position Description staged to interface successfully"
}
```

#### Get Pending Export Records
```http
GET /api/v1/interface/pending-export?status=READY_FOR_EXPORT&pageNumber=1&pageSize=20
```
**Description:** Retrieves AcrsGemsPd records that are ready for external system pickup.

**Response:**
```json
{
  "data": [
    {
      "gemsPdId": 98765,
      "pdSeqNum": 12345,
      "pdNumber": "PD-2025-001",
      "positionTitle": "Senior Software Engineer",
      "stagedDate": "2025-01-06T16:30:00Z",
      "status": "READY_FOR_EXPORT",
      "priority": "NORMAL",
      "retryCount": 0
    }
  ],
  "pagination": {
    "currentPage": 1,
    "pageSize": 20,
    "totalCount": 5,
    "totalPages": 1
  },
  "success": true,
  "message": "Pending export records retrieved successfully"
}
```

#### Mark as Exported
```http
POST /api/v1/interface/{gemsPdId}/mark-exported
```
**Description:** Marks an AcrsGemsPd record as successfully exported by external system.

**Request Body:**
```json
{
  "exportedDate": "2025-01-06T18:00:00Z",
  "externalSystemId": "GEMS-REF-12345",
  "exportedBy": "gems-service@agency.gov",
  "exportComments": "Successfully processed by GEMS"
}
```

**Response:**
```json
{
  "data": {
    "gemsPdId": 98765,
    "pdSeqNum": 12345,
    "status": "EXPORTED",
    "exportedDate": "2025-01-06T18:00:00Z",
    "externalSystemId": "GEMS-REF-12345"
  },
  "success": true,
  "message": "Record marked as exported successfully"
}
```

#### Get Export Status
```http
GET /api/v1/interface/export-status?dateFrom=2025-01-01&dateTo=2025-01-06
```
**Description:** Provides export status report for specified date range.

**Response:**
```json
{
  "data": {
    "reportPeriod": {
      "from": "2025-01-01T00:00:00Z",
      "to": "2025-01-06T23:59:59Z"
    },
    "summary": {
      "totalStaged": 25,
      "readyForExport": 3,
      "exported": 20,
      "failed": 2,
      "successRate": 80.0
    },
    "statusBreakdown": [
      { "status": "READY_FOR_EXPORT", "count": 3 },
      { "status": "EXPORTED", "count": 20 },
      { "status": "EXPORT_FAILED", "count": 2 }
    ]
  },
  "success": true,
  "message": "Export status report generated successfully"
}
```

#### Get Staging Status
```http
GET /api/v1/interface/{pdSeqNum}/staging-status
```
**Description:** Checks if a Position Description has been staged to AcrsGemsPd.

**Response:**
```json
{
  "data": {
    "pdSeqNum": 12345,
    "isStaged": true,
    "gemsPdId": 98765,
    "stagedDate": "2025-01-06T16:30:00Z",
    "currentStatus": "EXPORTED",
    "exportHistory": [
      {
        "attemptDate": "2025-01-06T18:00:00Z",
        "status": "EXPORTED",
        "externalSystemId": "GEMS-REF-12345"
      }
    ]
  },
  "success": true,
  "message": "Staging status retrieved successfully"
}
```

### 5. Notification APIs

#### Send Notification
```http
POST /api/v1/notifications/send
```
**Description:** Sends email notification for workflow events.

**Request Body:**
```json
{
  "pdSeqNum": 12345,
  "notificationType": "APPROVAL",
  "recipients": ["user@agency.gov", "supervisor@agency.gov"],
  "subject": "Position Description PD-2025-001 Approved",
  "templateData": {
    "pdNumber": "PD-2025-001",
    "positionTitle": "Senior Software Engineer",
    "approvedBy": "approver@agency.gov",
    "approvedDate": "2025-01-06T16:00:00Z",
    "comments": "Position description meets all requirements"
  },
  "priority": "NORMAL"
}
```

**Response:**
```json
{
  "data": {
    "notificationId": "NOTIF-12345-001",
    "pdSeqNum": 12345,
    "sentDate": "2025-01-06T16:05:00Z",
    "recipients": ["user@agency.gov", "supervisor@agency.gov"],
    "status": "SENT"
  },
  "success": true,
  "message": "Notification sent successfully"
}
```

#### Get Notification History
```http
GET /api/v1/notifications/{pdSeqNum}/history
```
**Description:** Retrieves email notification history for a Position Description.

**Response:**
```json
{
  "data": [
    {
      "notificationId": "NOTIF-12345-001",
      "pdSeqNum": 12345,
      "notificationType": "SUBMISSION",
      "recipients": ["approver@agency.gov"],
      "subject": "New Position Description Submitted for Review",
      "sentDate": "2025-01-06T15:05:00Z",
      "status": "SENT"
    },
    {
      "notificationId": "NOTIF-12345-002",
      "pdSeqNum": 12345,
      "notificationType": "APPROVAL",
      "recipients": ["user@agency.gov"],
      "subject": "Position Description PD-2025-001 Approved",
      "sentDate": "2025-01-06T16:05:00Z",
      "status": "SENT"
    }
  ],
  "success": true,
  "message": "Notification history retrieved successfully"
}
```

#### Test Email Configuration
```http
POST /api/v1/notifications/test
```
**Description:** Tests email configuration and connectivity.

**Request Body:**
```json
{
  "testRecipient": "test@agency.gov",
  "includeTemplateTest": true
}
```

**Response:**
```json
{
  "data": {
    "emailServerConnected": true,
    "testEmailSent": true,
    "templateRendering": true,
    "testDate": "2025-01-06T17:00:00Z"
  },
  "success": true,
  "message": "Email configuration test completed successfully"
}
```

## Data Models

### Position Description Entity
```json
{
  "pdSeqNum": "decimal (Primary Key)",
  "pdNumber": "string (50) - Unique identifier",
  "positionTitle": "string (200) - Official position title",
  "organizationalTitle": "string (200) - Internal org title",
  "occupationalSeries": "string (10) - OPM occupational series",
  "payPlan": "string (10) - Pay plan (GS, WG, etc.)",
  "grade": "string (10) - Grade level",
  "pdType": "string - Position description type",
  "acquisitionPosition": "string - Acquisition position indicator",
  "financialStatementRequired": "string - Financial disclosure requirement",
  "duties": "text - Position duties and responsibilities",
  "qualifications": "text - Required qualifications",
  "supervisoryResponsibilities": "boolean - Has supervisory duties",
  "managementLevel": "string - Management level indicator",
  "createdBy": "string (100) - Creator email",
  "createdDate": "datetime - Creation timestamp",
  "lastModifiedBy": "string (100) - Last modifier email",
  "lastModifiedDate": "datetime - Last modification timestamp"
}
```

### PdTrackHistory Entity
```json
{
  "trackingId": "decimal (Primary Key)",
  "pdSeqNum": "decimal (Foreign Key) - References Position Description",
  "pdStateCd": "string (10) - Workflow status code",
  "statusDate": "datetime - Status change timestamp",
  "userId": "string (100) - User who made the change",
  "comments": "text - Status change comments",
  "actionType": "string - Type of action taken",
  "assignedTo": "string (100) - Assigned approver email",
  "previousStatus": "string (10) - Previous status code"
}
```

### AcrsGemsPd Entity
```json
{
  "gemsPdId": "decimal (Primary Key)",
  "pdSeqNum": "decimal (Foreign Key) - References Position Description",
  "pdNumber": "string (50) - Position description number",
  "positionTitle": "string (200) - Position title",
  "occupationalSeries": "string (10) - OPM occupational series",
  "payPlan": "string (10) - Pay plan",
  "grade": "string (10) - Grade level",
  "stagedDate": "datetime - Date staged for export",
  "exportStatus": "string - Export status (READY_FOR_EXPORT, EXPORTED, FAILED)",
  "exportedDate": "datetime - Date successfully exported",
  "externalSystemId": "string (50) - External system reference ID",
  "retryCount": "int - Number of export attempts",
  "lastRetryDate": "datetime - Last export attempt date",
  "exportComments": "text - Export-related comments"
}
```

## Workflow States

### PdStateCd Values
- **DRAFT** - Position Description is being created/edited
- **SUBMITTED** - Submitted for workflow approval
- **UNDER_REVIEW** - Currently being reviewed by approver
- **APPROVED** - Approved and ready for staging
- **REJECTED** - Rejected, needs revision
- **CHANGES_REQUESTED** - Changes requested by approver
- **STAGED** - Staged to AcrsGemsPd for export
- **EXPORTED** - Successfully exported to external system
- **CANCELLED** - Cancelled by user or system
- **EXPIRED** - Approval request expired

### State Transition Rules
```
DRAFT → SUBMITTED (user submits)
SUBMITTED → UNDER_REVIEW (assigned to approver)
UNDER_REVIEW → APPROVED (approver approves)
UNDER_REVIEW → REJECTED (approver rejects)
UNDER_REVIEW → CHANGES_REQUESTED (approver requests changes)
REJECTED → DRAFT (user can edit and resubmit)
CHANGES_REQUESTED → DRAFT (user can edit and resubmit)
APPROVED → STAGED (system or manual staging)
STAGED → EXPORTED (external system confirms pickup)
```

## Security and Authorization

### Role-Based Access Control
- **PD_CREATOR** - Can create, edit (draft), and submit Position Descriptions
- **PD_APPROVER** - Can approve, reject, or request changes to submitted PDs
- **PD_ADMINISTRATOR** - Full access to all PDs and workflow management
- **SYSTEM_INTEGRATION** - Can stage to interface and mark as exported
- **READ_ONLY** - Can view PDs and workflow status

### API Security
- All endpoints require authentication via JWT tokens
- Role-based authorization enforced at controller level
- Audit logging for all data modifications
- Rate limiting on public endpoints
- Input validation and sanitization

## Error Handling

### Standard Error Response
```json
{
  "success": false,
  "message": "Error description",
  "errors": [
    {
      "field": "positionTitle",
      "code": "REQUIRED_FIELD",
      "message": "Position title is required"
    }
  ],
  "timestamp": "2025-01-06T12:00:00Z",
  "traceId": "12345-67890-abcde"
}
```

### HTTP Status Codes
- **200 OK** - Successful operation
- **201 Created** - Resource created successfully
- **400 Bad Request** - Invalid request data
- **401 Unauthorized** - Authentication required
- **403 Forbidden** - Insufficient permissions
- **404 Not Found** - Resource not found
- **409 Conflict** - Business rule violation
- **422 Unprocessable Entity** - Validation errors
- **500 Internal Server Error** - Server error

## Performance Considerations

### Caching Strategy
- Lookup data cached for 4 hours
- Position Description list queries cached for 15 minutes
- Workflow status cached for 5 minutes
- Interface export status cached for 30 minutes

### Database Optimization
- Indexes on frequently queried fields (PdSeqNum, PdStateCd, CreatedBy)
- Pagination for all list endpoints
- Efficient joins between related tables
- Archive strategy for old workflow records

## Integration Points

### External WebAPI Integration
- **Existing Service**: Leverages existing `IExternalCodeListService`
- **Caching Strategy**: 4-hour cache for codelist data (already implemented)
- **Available Endpoints**:
  - Occupational Series (`/occupationalseries`)
  - Pay Plans (`/payplans`) 
  - Hiring Paths (`/hiringpaths`)
  - Position Schedule Types (`/positionscheduletypes`)
  - Security Clearances (`/securityclearances`)
  - Countries (`/countries`)
  - Postal Codes (`/postalcodes`)
- **Search Capability**: Existing search functionality for occupational series
- **Error Handling**: Built-in fallback and retry mechanisms

### ACRS/GEMS System
- Pull-based integration model
- External system queries AcrsGemsPd table
- Retry mechanism for failed exports
- Status callbacks for successful processing

### Email System
- SMTP configuration for notifications
- Template-based email generation
- Async processing for bulk notifications
- Delivery status tracking

## Implementation Strategy

### Phase 1: Leverage Existing External WebAPI Integration
The Position Description form APIs will utilize the existing External WebAPI CodeList infrastructure:

```csharp
// Example: Form Data Controller Implementation
[ApiController]
[Route("api/v1/lookups")]
public class FormLookupsController : BaseApiController
{
    private readonly IExternalCodeListService _externalApiService;
    
    [HttpGet("form-data")]
    public async Task<IActionResult> GetFormData()
    {
        var occupationalSeries = await _externalApiService.GetOccupationalSeriesAsync();
        var payPlans = await _externalApiService.GetPayPlansAsync();
        var hiringPaths = await _externalApiService.GetHiringPathsAsync();
        
        return Ok(new Response<object>(new {
            occupationalSeries,
            payPlans,
            hiringPaths,
            // ... other lookup data
        }));
    }
    
    [HttpGet("occupational-series")]
    public async Task<IActionResult> GetOccupationalSeries([FromQuery] string search = null)
    {
        var result = string.IsNullOrEmpty(search) 
            ? await _externalApiService.GetOccupationalSeriesAsync()
            : await _externalApiService.SearchOccupationalSeriesAsync(search);
            
        return Ok(new Response<List<OccupationalSeriesItem>>(result));
    }
}
```

### Phase 2: Position Description Entity Enhancement
Extend the existing entity structure to support form requirements:

```csharp
// Enhanced Position Description entity
public class PositionDescription : AuditableBaseEntity
{
    public decimal PdSeqNum { get; set; }
    public string PdNumber { get; set; }
    public string PositionTitle { get; set; }
    public string OrganizationalTitle { get; set; }
    
    // External WebAPI Integration Fields
    public string OccupationalSeries { get; set; }  // From External WebAPI
    public string PayPlan { get; set; }             // From External WebAPI
    public string Grade { get; set; }
    public string HiringPath { get; set; }          // From External WebAPI
    public string ScheduleType { get; set; }        // From External WebAPI
    public string SecurityClearance { get; set; }   // From External WebAPI
    
    // Internal Lookup Fields
    public string PdType { get; set; }
    public string AcquisitionPosition { get; set; }
    public string FinancialStatementRequired { get; set; }
    
    // Form Content
    public string Duties { get; set; }
    public string Qualifications { get; set; }
    public bool SupervisoryResponsibilities { get; set; }
    public string ManagementLevel { get; set; }
    
    // Navigation Properties
    public virtual ICollection<PdTrackHistory> TrackHistory { get; set; }
    public virtual ICollection<AcrsGemsPd> InterfaceRecords { get; set; }
}
```

### Phase 3: Workflow Integration with Existing Entities
Utilize existing `PdTrackHistory` entity for workflow management:

```csharp
// Workflow Service Implementation
public class PositionDescriptionWorkflowService
{
    public async Task<bool> SubmitForApproval(decimal pdSeqNum, string comments, string userId)
    {
        var trackEntry = new PdTrackHistory
        {
            PdSeqNum = pdSeqNum,
            PdStateCd = "SUBMITTED",
            StatusDate = DateTime.UtcNow,
            UserId = userId,
            Comments = comments,
            ActionType = "SUBMIT"
        };
        
        await _trackHistoryRepository.AddAsync(trackEntry);
        await SendNotification(pdSeqNum, "SUBMISSION", userId);
        
        return true;
    }
}
```

## Monitoring and Logging

### Key Metrics
- API response times
- Workflow completion rates
- Export success rates
- Email delivery rates
- Error rates by endpoint

### Audit Trail
- All Position Description changes logged
- Workflow state transitions tracked
- User actions recorded with timestamps
- Data export events logged

## Future Enhancements

### Planned Features
- Batch Position Description operations
- Advanced workflow routing rules
- Real-time notifications via WebSocket
- Integration with document management systems
- Mobile-responsive form interface
- Advanced reporting and analytics

### API Versioning
- Current version: v1
- Backward compatibility maintained
- Deprecation notices for breaking changes
- Migration guides for version updates