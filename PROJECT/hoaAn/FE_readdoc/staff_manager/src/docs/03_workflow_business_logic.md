# Workflow & Business Logic Documentation

## Overview
This document outlines the core business workflows and logic for the multi-page role-based POS system. Each workflow includes step-by-step processes, role requirements, and system interactions.

## Core Business Workflows

### 1. Staff Shift Management

#### Opening a Shift

```mermaid
flowchart TD
    A[Staff Arrives] --> B[Login to System]
    B --> C{Active Shift Exists?}
    C -->|Yes| D[Continue Existing Shift]
    C -->|No| E[Create New Shift]
    E --> F[Count Opening Cash]
    F --> G[Record Opening Amount]
    G --> H[System Creates Shift Record]
    H --> I[Shift Status: OPEN]
    I --> J[Enable POS Functions]
    
    D --> J
```

**Business Rules:**
- Staff can only have one active shift at a time
- Opening cash must be recorded and validated
- Shift start time is automatically recorded
- POS functions are disabled until shift is opened

**Input/Output:**
- **Input**: Staff credentials, opening cash amount, optional notes
- **Output**: Shift ID, start timestamp, opening cash balance
- **Role**: Staff, Manager

#### Closing a Shift

```mermaid
flowchart TD
    A[Staff Requests Shift Close] --> B[Count Physical Cash]
    B --> C[Record Closing Amount]
    C --> D[System Calculates Expected Cash]
    D --> E{Cash Matches?}
    E -->|Yes| F[Mark Shift as CLOSED]
    E -->|No| G[Record Cash Variance]
    G --> H[Manager Notification]
    H --> F
    F --> I[Generate Shift Report]
    I --> J[Archive Shift Data]
    J --> K[Disable POS for User]
```

**Business Rules:**
- All pending orders must be completed or voided
- Cash variance over threshold requires manager approval
- Shift end time is automatically recorded
- System generates shift summary report

**Expected Cash Calculation:**
```
Expected Cash = Opening Cash + Cash Sales - Cash Refunds + Tips
```

### 2. POS Transaction Flow

#### Creating a Sale

```mermaid
flowchart TD
    A[Start New Sale] --> B[Scan/Add Products]
    B --> C[Update Cart]
    C --> D{More Items?}
    D -->|Yes| B
    D -->|No| E[Calculate Totals]
    E --> F[Apply Discounts/Coupons]
    F --> G[Add Tips if Applicable]
    G --> H[Display Final Total]
    H --> I[Select Payment Method]
    
    I --> J{Payment Method?}
    J -->|Cash| K[Process Cash Payment]
    J -->|Card| L[Process Card Payment]
    J -->|Mixed| M[Process Mixed Payment]
    
    K --> N[Calculate Change]
    L --> O[Card Authorization]
    M --> P[Process Multiple Payments]
    
    N --> Q[Print Receipt]
    O --> Q
    P --> Q
    
    Q --> R[Update Inventory]
    R --> S[Record Sale in Database]
    S --> T[Update Customer History]
    T --> U[End Transaction]
```

**Business Rules:**
- Minimum transaction amount: $0.01
- Maximum transaction amount: $9,999.99
- Tax calculation based on store settings
- Inventory deduction happens after payment confirmation
- Receipt must be offered for all transactions

**Inventory Update Logic:**
```
For each item in sale:
  IF product.track_inventory = true:
    inventory.quantity -= item.quantity
    inventory.reserved_quantity -= item.quantity (if previously reserved)
    
  IF inventory.quantity <= inventory.min_stock:
    CREATE low_stock_notification
```

#### Barcode Scanning Workflow

```mermaid
flowchart TD
    A[Staff Initiates Scan] --> B{Scanning Method?}
    B -->|Camera| C[Activate Camera]
    B -->|Hardware Scanner| D[Listen for Barcode Input]
    
    C --> E[Capture Barcode from Video]
    D --> F[Receive Barcode Data]
    
    E --> G[Decode Barcode]
    F --> G
    
    G --> H[Search Product Database]
    H --> I{Product Found?}
    
    I -->|Yes| J[Check Inventory]
    I -->|No| K[Show Product Not Found]
    
    J --> L{In Stock?}
    L -->|Yes| M[Add to Cart]
    L -->|No| N[Show Out of Stock Warning]
    
    M --> O[Update Cart Display]
    K --> P[Allow Manual Entry]
    N --> Q[Allow Override if Authorized]
    
    P --> R[Manual Product Selection]
    Q --> S{Manager Override?}
    S -->|Yes| M
    S -->|No| T[Cancel Addition]
```

### 3. Order Management Workflow

#### Order Processing for Staff

```mermaid
flowchart TD
    A[Staff Views Assigned Orders] --> B[Select Order]
    B --> C{Order Status?}
    C -->|Pending| D[Start Processing]
    C -->|Processing| E[Continue Processing]
    C -->|Completed| F[View Order Details]
    
    D --> G[Gather Items]
    G --> H[Mark Items as Picked]
    H --> I[Update Order Status to Processing]
    I --> J[Package Items]
    J --> K[Print Labels/Receipts]
    K --> L[Mark Order as Ready]
    L --> M[Notify Customer]
    M --> N[Complete Order]
    N --> O[Update Order Status to Completed]
```

#### Order Assignment (Manager)

```mermaid
flowchart TD
    A[Manager Views All Orders] --> B[Filter Unassigned Orders]
    B --> C[Select Order]
    C --> D[View Staff Availability]
    D --> E[Select Staff Member]
    E --> F[Assign Order]
    F --> G[Send Notification to Staff]
    G --> H[Update Order Record]
    H --> I[Log Assignment in Audit]
```

**Assignment Logic:**
- Consider staff workload and current assignments
- Match order requirements with staff skills
- Priority orders get assigned first
- Staff availability and shift status

### 4. Inventory Management

#### Stock Level Monitoring

```mermaid
flowchart TD
    A[System Checks Inventory] --> B{Stock Level?}
    B -->|Below Min| C[Create Low Stock Alert]
    B -->|Zero| D[Create Out of Stock Alert]
    B -->|Normal| E[Continue Monitoring]
    
    C --> F[Notify Manager]
    D --> G[Notify Manager + Staff]
    
    F --> H[Generate Reorder Suggestion]
    G --> I[Block Sales for Product]
    
    H --> J[Add to Reorder Report]
    I --> K[Update Product Status]
```

**Stock Check Triggers:**
- After each sale transaction
- During inventory adjustments
- Scheduled daily checks (automated)
- Manual checks by staff

#### Inventory Adjustment Process

```mermaid
flowchart TD
    A[Staff/Manager Initiates Adjustment] --> B[Select Product]
    B --> C[Current Stock Display]
    C --> D[Enter New Quantity]
    D --> E[Select Adjustment Reason]
    E --> F[Add Notes]
    F --> G{Requires Approval?}
    
    G -->|Yes| H[Send to Manager]
    G -->|No| I[Apply Adjustment]
    
    H --> J[Manager Reviews]
    J --> K{Approved?}
    K -->|Yes| I
    K -->|No| L[Reject with Reason]
    
    I --> M[Update Inventory Record]
    M --> N[Create Audit Log]
    N --> O[Update Stock Levels]
    O --> P[Recalculate Metrics]
```

**Approval Required When:**
- Adjustment amount exceeds threshold ($500)
- Reason is "Loss" or "Damage"
- Staff member (non-manager) making adjustment
- High-value product adjustment

### 5. Task Management Workflow

#### Task Creation (Manager)

```mermaid
flowchart TD
    A[Manager Creates Task] --> B[Fill Task Details]
    B --> C[Set Priority Level]
    C --> D[Select Assignee]
    D --> E[Set Due Date]
    E --> F[Add Instructions/Notes]
    F --> G[Create Task Record]
    G --> H[Send Notification]
    H --> I[Add to Staff Dashboard]
```

#### Task Execution (Staff)

```mermaid
flowchart TD
    A[Staff Views Tasks] --> B[Select Task]
    B --> C[Read Instructions]
    C --> D[Start Task]
    D --> E[Update Status to In Progress]
    E --> F[Perform Task Activities]
    F --> G[Add Progress Notes]
    G --> H{Task Complete?}
    
    H -->|No| I[Save Progress]
    H -->|Yes| J[Mark as Completed]
    
    I --> K[Continue Later]
    J --> L[Add Completion Notes]
    L --> M[Submit for Review]
    M --> N[Notify Manager]
    N --> O[Update Task Status]
```

### 6. Customer Management

#### Customer Registration

```mermaid
flowchart TD
    A[Customer Provides Info] --> B[Validate Email/Phone]
    B --> C{Already Exists?}
    C -->|Yes| D[Link to Existing]
    C -->|No| E[Create New Customer]
    
    E --> F[Generate Customer ID]
    F --> G[Initialize Purchase History]
    G --> H[Set Loyalty Status]
    H --> I[Send Welcome Message]
    
    D --> J[Update Contact Info]
    J --> K[Link Current Purchase]
```

#### Purchase History Tracking

```mermaid
flowchart TD
    A[Customer Makes Purchase] --> B[Update Purchase History]
    B --> C[Calculate Total Spent]
    C --> D[Update Visit Count]
    D --> E[Check Loyalty Tier]
    E --> F{Tier Upgrade?}
    
    F -->|Yes| G[Upgrade Customer Tier]
    F -->|No| H[Apply Current Benefits]
    
    G --> I[Send Upgrade Notification]
    I --> J[Apply New Tier Benefits]
    J --> H
    
    H --> K[Complete Transaction]
```

### 7. Reporting & Analytics

#### Daily Sales Report Generation

```mermaid
flowchart TD
    A[End of Day Trigger] --> B[Collect Sales Data]
    B --> C[Calculate Metrics]
    C --> D[Generate Report]
    D --> E[Format for Distribution]
    E --> F[Send to Recipients]
    F --> G[Archive Report]
    
    C --> H[Revenue Totals]
    C --> I[Transaction Counts]
    C --> J[Top Products]
    C --> K[Staff Performance]
    C --> L[Customer Analytics]
```

**Report Metrics Calculation:**
```
Daily Revenue = SUM(order.total_amount) WHERE DATE(created_at) = TODAY
Average Transaction = Daily Revenue / Transaction Count
Items Sold = SUM(order_items.quantity)
Top Staff = MAX(SUM(order.total_amount) GROUP BY staff_id)
```

### 8. Security & Audit Workflows

#### Permission Checking

```mermaid
flowchart TD
    A[User Action Request] --> B[Extract User Permissions]
    B --> C[Check Required Permission]
    C --> D{Has Permission?}
    
    D -->|Yes| E[Allow Action]
    D -->|No| F[Check Role-Based Access]
    
    F --> G{Role Allows?}
    G -->|Yes| E
    G -->|No| H[Log Access Attempt]
    
    H --> I[Return Access Denied]
    E --> J[Log Successful Action]
    J --> K[Execute Action]
```

#### Audit Log Creation

```mermaid
flowchart TD
    A[System Action Occurs] --> B[Capture Action Details]
    B --> C[Extract User Context]
    C --> D[Record Before/After State]
    D --> E[Create Audit Entry]
    E --> F[Store in Audit Log]
    F --> G{Sensitive Action?}
    
    G -->|Yes| H[Send Security Alert]
    G -->|No| I[Continue Processing]
    
    H --> J[Notify Security Team]
    J --> I
```

**Audited Actions:**
- User login/logout
- Permission changes
- Order modifications
- Inventory adjustments
- Price changes
- Staff management actions
- System configuration changes

## Business Rules Summary

### General Rules
1. **Single Store Context**: Users operate within one store at a time
2. **Shift Requirement**: POS functions require active shift
3. **Permission-Based Access**: All actions validated against user permissions
4. **Audit Trail**: All significant actions logged for compliance
5. **Data Retention**: Customer data retained per GDPR requirements

### Financial Rules
1. **Cash Handling**: All cash transactions tracked and reconciled
2. **Tax Calculation**: Applied per store configuration
3. **Refund Policy**: Refunds require manager approval over threshold
4. **Discount Limits**: Staff discount limits enforced
5. **Payment Processing**: Card payments require external gateway confirmation

### Inventory Rules
1. **Stock Reservation**: Items reserved during cart creation
2. **Negative Stock**: Controlled by product configuration
3. **Auto-Reorder**: Triggered when stock falls below minimum
4. **Cycle Counting**: Regular inventory verification required
5. **Adjustment Approval**: Large adjustments require manager approval

### Security Rules  
1. **Password Policy**: Strong passwords enforced
2. **Session Management**: Automatic timeout after inactivity
3. **Access Logging**: All access attempts logged
4. **Data Encryption**: Sensitive data encrypted at rest and in transit
5. **Backup Requirements**: Regular automated backups maintained