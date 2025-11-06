# Subscription Access API - Backend Implementation

## Overview

This document describes the backend implementation for subscription-based access control in the CyberCity Learning Platform.

## Business Logic

### Subscription Rules

- **Free Users (No Subscription)**: Can access only the **first 2 modules** (index 0 and 1) of any course
- **Paid Users (Active Subscription)**: Can access **all modules** in all courses

### Active Subscription Criteria

A user has an active subscription when:

1. There exists an `Order` with:
   - `user_uid` = current user
   - `payment_status` = `'paid'`
   - `approval_status` = `'approved'`
   - `end_at` >= NOW() OR `end_at IS NULL` (unlimited subscription)

2. There exists a related `Payment` with:
   - `order_uid` = matching order
   - `status` = `'paid'` OR `'completed'`

---

## API Endpoints

### 1. Check User Subscription Access

**Endpoint:** `GET /api/student/subscription/access`

**Authorization:** Bearer Token (Required)

**Description:** Returns the current user's subscription status and access level.

**Response Success (200 OK):**

```json
{
  "success": true,
  "data": {
    "hasAccess": true,
    "canViewAllModules": true,
    "maxFreeModules": 2,
    "subscriptionInfo": {
      "active": true,
      "orderUid": "order-123",
      "planUid": "pp1",
      "planName": "Premium Plan",
      "startAt": "2024-01-01T00:00:00Z",
      "endAt": "2025-01-01T00:00:00Z",
      "daysRemaining": 180
    }
  },
  "message": "User has active subscription"
}
```

**Response when no subscription (200 OK):**

```json
{
  "success": true,
  "data": {
    "hasAccess": false,
    "canViewAllModules": false,
    "maxFreeModules": 2,
    "subscriptionInfo": null
  },
  "message": "User has no active subscription"
}
```

---

### 2. Check Module Access

**Endpoint:** `GET /api/student/courses/{courseUid}/modules/{moduleIndex}/access`

**Authorization:** Bearer Token (Required)

**Parameters:**
- `courseUid` (path, string): Course UID
- `moduleIndex` (path, integer): Module index (0-based)

**Description:** Checks if the current user can access a specific module.

**Response Success - Can Access (200 OK):**

```json
{
  "success": true,
  "data": {
    "canAccess": true,
    "hasSubscription": true,
    "moduleIndex": 0,
    "maxFreeModules": 2,
    "reason": null
  }
}
```

**Response Success - Cannot Access (200 OK):**

```json
{
  "success": true,
  "data": {
    "canAccess": false,
    "hasSubscription": false,
    "moduleIndex": 2,
    "maxFreeModules": 2,
    "reason": "Module này chỉ dành cho học viên đã mua gói. Vui lòng mua gói để tiếp tục học."
  }
}
```

**Error Response (401 Unauthorized):**

```json
{
  "success": false,
  "message": "Unauthorized"
}
```

**Error Response (500 Internal Server Error):**

```json
{
  "success": false,
  "message": "Error message details"
}
```

---

## Implementation Details

### Files Created/Modified

1. **DTOs:**
   - `CyberCity.DTOs/Subscriptions/SubscriptionAccessDto.cs`
   - `CyberCity.DTOs/Subscriptions/ModuleAccessDto.cs`

2. **Service Interface:**
   - `CyberCity.Application/Interface/ISubscriptionService.cs`

3. **Service Implementation:**
   - `CyberCity.Application/Implement/SubscriptionService.cs`

4. **Controller:**
   - `CyberCity.Controller/Controllers/StudentApiController.cs` (updated)

5. **DI Registration:**
   - `CyberCity.Controller/Program.cs` (updated)

### Database Tables Used

- **orders**: Contains subscription order information
  - `uid`: Order unique identifier
  - `user_uid`: User identifier
  - `plan_uid`: Pricing plan identifier
  - `payment_status`: Payment status ('pending', 'paid', 'failed')
  - `approval_status`: Admin approval status ('pending', 'approved', 'rejected')
  - `start_at`: Subscription start date
  - `end_at`: Subscription end date (NULL = unlimited)
  - `created_at`: Order creation timestamp

- **payments**: Contains payment transaction information
  - `uid`: Payment unique identifier
  - `order_uid`: Related order identifier
  - `status`: Payment status ('pending', 'paid', 'completed', 'failed', 'cancelled')
  - `amount`: Payment amount
  - `paid_at`: Payment completion timestamp

- **pricing_plans**: Contains subscription plan details
  - `uid`: Plan unique identifier
  - `plan_name`: Plan name
  - `price`: Plan price
  - `duration_days`: Plan duration in days

---

## Testing Guide

### Prerequisites

1. Valid JWT token for a test user
2. At least one course with multiple modules in the database
3. Test orders and payments in various states

### Test Scenarios

#### Scenario 1: User Without Subscription

**Setup:**
- User has no paid orders OR orders are expired/rejected

**Test 1.1 - Check Subscription Access:**
```bash
GET https://localhost:7168/api/student/subscription/access
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected Result:**
```json
{
  "success": true,
  "data": {
    "hasAccess": false,
    "canViewAllModules": false,
    "maxFreeModules": 2,
    "subscriptionInfo": null
  },
  "message": "User has no active subscription"
}
```

**Test 1.2 - Check Module 0 Access (Should Allow):**
```bash
GET https://localhost:7168/api/student/courses/{courseUid}/modules/0/access
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected Result:**
```json
{
  "success": true,
  "data": {
    "canAccess": true,
    "hasSubscription": false,
    "moduleIndex": 0,
    "maxFreeModules": 2,
    "reason": null
  }
}
```

**Test 1.3 - Check Module 2 Access (Should Deny):**
```bash
GET https://localhost:7168/api/student/courses/{courseUid}/modules/2/access
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected Result:**
```json
{
  "success": true,
  "data": {
    "canAccess": false,
    "hasSubscription": false,
    "moduleIndex": 2,
    "maxFreeModules": 2,
    "reason": "Module này chỉ dành cho học viên đã mua gói. Vui lòng mua gói để tiếp tục học."
  }
}
```

---

#### Scenario 2: User With Active Subscription

**Setup:**
- Create a paid order:
  ```sql
  INSERT INTO orders (uid, user_uid, plan_uid, amount, payment_status, approval_status, start_at, end_at, created_at)
  VALUES ('test-order-1', 'user-123', 'plan-1', 100000, 'paid', 'approved', NOW(), NOW() + INTERVAL '30 days', NOW());
  ```
- Create a completed payment:
  ```sql
  INSERT INTO payments (uid, order_uid, payment_method, amount, currency, status, paid_at, created_at)
  VALUES ('test-payment-1', 'test-order-1', 'SEPAY', 100000, 'VND', 'completed', NOW(), NOW());
  ```

**Test 2.1 - Check Subscription Access:**
```bash
GET https://localhost:7168/api/student/subscription/access
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected Result:**
```json
{
  "success": true,
  "data": {
    "hasAccess": true,
    "canViewAllModules": true,
    "maxFreeModules": 2,
    "subscriptionInfo": {
      "active": true,
      "orderUid": "test-order-1",
      "planUid": "plan-1",
      "planName": "Premium Plan",
      "startAt": "2024-XX-XXT00:00:00Z",
      "endAt": "2024-XX-XXT00:00:00Z",
      "daysRemaining": 30
    }
  },
  "message": "User has active subscription"
}
```

**Test 2.2 - Check Any Module Access (Should Allow):**
```bash
GET https://localhost:7168/api/student/courses/{courseUid}/modules/5/access
Authorization: Bearer YOUR_JWT_TOKEN
```

**Expected Result:**
```json
{
  "success": true,
  "data": {
    "canAccess": true,
    "hasSubscription": true,
    "moduleIndex": 5,
    "maxFreeModules": 2,
    "reason": null
  }
}
```

---

#### Scenario 3: User With Expired Subscription

**Setup:**
- Create an expired order (end_at in the past)

**Expected Behavior:**
- Same as Scenario 1 (no subscription)

---

#### Scenario 4: Unlimited Subscription

**Setup:**
- Create an order with `end_at = NULL`

**Expected Behavior:**
- User has access to all modules
- `daysRemaining` = null in response

---

### Postman Collection Examples

**Collection Structure:**
```
CyberCity API
├── Authentication
│   └── Login
├── Subscription
│   ├── Get Subscription Access
│   ├── Check Module 0 Access
│   ├── Check Module 2 Access
│   └── Check Module 5 Access
```

**Environment Variables:**
- `baseUrl`: `https://localhost:7168`
- `token`: (Set after login)
- `courseUid`: (Your test course UID)

---

## Edge Cases Handled

1. **Multiple Active Orders:**
   - Returns the most recent order (ordered by `created_at DESC`)

2. **Order Without Payment:**
   - Returns no access even if order status is 'paid'

3. **Expired Subscription:**
   - Automatically excludes orders where `end_at < NOW()`

4. **Unlimited Subscription:**
   - Handles `end_at IS NULL` as permanent access

5. **Pending Approval:**
   - Only approved orders grant access (`approval_status = 'approved'`)

6. **Payment Status:**
   - Accepts both 'paid' and 'completed' payment statuses

---

## Security Considerations

1. **Authentication Required:**
   - All endpoints require valid JWT token
   - User can only check their own subscription

2. **Authorization:**
   - User UID extracted from JWT token claims
   - No ability to check other users' subscriptions

3. **Input Validation:**
   - Module index validated as integer
   - Course UID validated as string

---

## Integration with Frontend

### Example React Hook:

```typescript
import { useState, useEffect } from 'react';
import { subscriptionService } from '@/services/subscriptionService';

export const useSubscriptionAccess = () => {
  const [hasAccess, setHasAccess] = useState(false);
  const [canViewAllModules, setCanViewAllModules] = useState(false);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const checkAccess = async () => {
      try {
        const response = await subscriptionService.checkAccess();
        setHasAccess(response.data.hasAccess);
        setCanViewAllModules(response.data.canViewAllModules);
      } catch (error) {
        console.error('Failed to check subscription:', error);
      } finally {
        setLoading(false);
      }
    };

    checkAccess();
  }, []);

  return { hasAccess, canViewAllModules, loading };
};
```

### Module Access Check:

```typescript
const checkModuleAccess = async (courseUid: string, moduleIndex: number) => {
  const response = await subscriptionService.checkModuleAccess(courseUid, moduleIndex);
  if (!response.data.canAccess) {
    // Show upgrade prompt
    showUpgradeModal(response.data.reason);
    return false;
  }
  return true;
};
```

---

## Future Enhancements

1. **Caching:**
   - Add Redis caching for subscription status
   - Cache duration: 5-10 minutes

2. **Webhook Integration:**
   - Auto-update subscription when payment webhook received
   - Real-time access updates

3. **Grace Period:**
   - Allow X days grace period after subscription expires

4. **Multiple Plans:**
   - Different access levels for different plans
   - Custom module limits per plan

5. **Organization Subscriptions:**
   - Support organization-wide subscriptions
   - Bulk user access management

---

## Troubleshooting

### Issue: User has paid order but no access

**Check:**
1. Verify `payment_status = 'paid'` in orders table
2. Verify `approval_status = 'approved'` in orders table
3. Verify payment record exists with `status = 'paid'` or `'completed'`
4. Check if `end_at` is not in the past

**SQL Query:**
```sql
SELECT o.*, p.* 
FROM orders o
LEFT JOIN payments p ON p.order_uid = o.uid
WHERE o.user_uid = 'YOUR_USER_UID'
ORDER BY o.created_at DESC;
```

### Issue: Module index not working correctly

**Check:**
- Ensure frontend sends 0-based module index
- Module 0 and 1 are free (indices 0-1)
- Module 2+ require subscription (indices 2+)

### Issue: Subscription not updating after payment

**Check:**
1. Payment webhook processing
2. Order approval status
3. Database transaction completion

---

## Support

For issues or questions:
- Check logs in `ILogger<SubscriptionService>`
- Review payment and order records
- Contact backend team

---

**Last Updated:** November 6, 2025
**Version:** 1.0
**Author:** Backend Team
