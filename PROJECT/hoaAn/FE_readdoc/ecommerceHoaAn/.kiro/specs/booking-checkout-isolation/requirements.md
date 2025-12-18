## 1. Core Principles (Non-negotiable Rules)

1. Booking flow và Product checkout flow **PHẢI tách biệt hoàn toàn**
2. **Không được phép tồn tại checkout context hỗn hợp**
3. Checkout **CHỈ được trigger bởi user action**, không auto-run
4. Cart là **single source of truth cho pricing**
5. BookingInfo chỉ là **metadata**, không được override cart data

---

## 2. Definitions

* **BookingInfo**: Temporary session state (eventId, serviceProductId, customization, notes)
* **BookingCart**: Cart chỉ chứa booking service
* **ProductCart**: Cart chỉ chứa product thông thường
* **CheckoutContext**: `{ cartType: booking | product, cartId, bookingInfo? }`
* **Invalid Context**: bookingInfo + product items (hoặc ngược lại)

---

## 3. State Lifecycle Rules

### 3.1 Booking Completion (Clean Exit)

**WHEN** booking checkout hoàn tất
**THEN**

* bookingInfo MUST be removed from sessionStorage
* booking cart MUST be cleared/closed
* new empty product cart MUST be created

---

### 3.2 Normal Product Checkout

**WHEN** bookingInfo does NOT exist
**AND** cart contains only product items
**THEN**

* checkout is treated as normal product purchase
* booking logic MUST NOT execute

---

### 3.3 State Contamination Protection

**WHEN**

* bookingInfo exists
* AND cart contains product items

**THEN**

* checkout MUST be blocked
* user MUST see error: *“Booking và mua sản phẩm không thể thực hiện chung”*
* NO checkout API call is allowed

---

## 4. Checkout Page Guard Rules

### 4.1 On Page Mount

**WHEN** CheckoutPage mounts
**THEN**

* system MUST validate CheckoutContext
* IF context is invalid → show error & stop flow
* checkout process MUST NOT auto-trigger

---

### 4.2 Checkout Trigger

* Checkout API **CHỈ được gọi khi user click Confirm**
* `useEffect` / auto-process checkout is **STRICTLY FORBIDDEN**

---

## 5. Payload Construction Rules

### 5.1 Booking Checkout Payload

**ONLY IF**

* bookingInfo exists
* AND cartType === booking
* AND cart contains only booking items

→ booking notes & metadata MAY be attached

---

### 5.2 Product Checkout Payload

**WHEN**

* bookingInfo does NOT exist
* AND cartType === product

→ booking fields MUST NOT appear in payload

---

## 6. Pricing Integrity

1. Booking order pricing MUST come from booking service pricing
2. Product order pricing MUST come from cart items
3. Booking pricing and product pricing MUST NEVER be mixed
4. FE MUST validate:
   `checkoutTotal === cartTotal` before submit

---

## 7. Cancellation & Recovery

* Cancel booking → bookingInfo MUST be removed immediately
* Navigate away → cart preserved, context revalidated on return
* Session expired → all temporary checkout state cleared
* Reused cartId → backend MUST validate ownership & contents

---

## 8. Backend Validation (Hard Stop)

Backend MUST reject checkout request IF:

* bookingInfo + product items detected
* payload contains conflicting pricing sources
* cart contents do not match checkout context

Backend SHOULD log:

* checkoutType (booking | product)
* cartId
* bookingInfo presence (true/false)

---

## 9. Failure Handling

**IF** order creation fails
**THEN**

* session state MUST be preserved
* user MAY retry without data loss
