# Developer Guide: Ritual Pattern Definition

## Overview

This guide explains how to add new Vietnamese cultural rituals to the Sequential Ritual Recommendation System. The system uses a manifest-based approach where ritual patterns are defined in a JSON file and loaded at startup. This guide covers:

- Understanding the ritual manifest structure
- Defining action sequences
- Specifying required items
- Tuning confidence thresholds
- Validating your ritual definitions
- Frontend integration requirements

---

## Table of Contents

1. [Ritual Manifest Structure](#ritual-manifest-structure)
2. [Adding New Rituals](#adding-new-rituals)
3. [Action Sequence Syntax](#action-sequence-syntax)
4. [Required Items Specification](#required-items-specification)
5. [Confidence Threshold Tuning](#confidence-threshold-tuning)
6. [Validation and Testing](#validation-and-testing)
7. [Frontend Integration](#frontend-integration)
8. [Examples](#examples)

---

## Ritual Manifest Structure

The ritual manifest is stored in `VietCommerce.Api/wwwroot/data/ritual-manifest.json`. It contains a JSON object with a single `rituals` array:

```json
{
  "rituals": [
    {
      "id": "ritual-id",
      "name": "Ritual Name",
      "actionSequencePattern": [...],
      "requiredItems": [...],
      "confidenceThreshold": 0.7,
      "culturalSignificance": "Description of the ritual",
      "sources": ["Source 1", "Source 2"]
    }
  ]
}
```

### Ritual Object Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `id` | string | Yes | Unique identifier for the ritual (kebab-case, e.g., "day-thang") |
| `name` | string | Yes | Display name of the ritual in Vietnamese (e.g., "Đầy Tháng") |
| `actionSequencePattern` | array | Yes | Array of ActionType objects defining the pattern |
| `requiredItems` | array | Yes | Array of required product categories and items |
| `confidenceThreshold` | number | Yes | Confidence score threshold (0.0 to 1.0) for pattern matching |
| `culturalSignificance` | string | Yes | Description of the ritual's cultural importance |
| `sources` | array | Yes | List of sources or references for the ritual |

---

## Adding New Rituals

### Step 1: Identify the Ritual

Choose a Vietnamese cultural ritual you want to add. Examples include:
- Đầy Tháng (1-month baby celebration)
- Tết Nguyên Đán (Lunar New Year)
- Lễ Cúng Tổ Tiên (Ancestor worship)
- Lễ Cúng Thần Tài (Wealth god worship)
- Tết Trung Thu (Mid-Autumn Festival)

### Step 2: Define the Ritual ID

Create a unique kebab-case identifier:
- Use lowercase letters and hyphens only
- Be descriptive but concise
- Examples: `day-thang`, `tet-nguyen-dan`, `le-cung-to-tien`

### Step 3: Add to Manifest

Open `VietCommerce.Api/wwwroot/data/ritual-manifest.json` and add your ritual object to the `rituals` array:

```json
{
  "rituals": [
    // ... existing rituals ...
    {
      "id": "your-ritual-id",
      "name": "Your Ritual Name",
      "actionSequencePattern": [...],
      "requiredItems": [...],
      "confidenceThreshold": 0.7,
      "culturalSignificance": "Your description",
      "sources": ["Source 1"]
    }
  ]
}
```

### Step 4: Validate JSON

Ensure the JSON is valid:
- Use a JSON validator (e.g., jsonlint.com)
- Check for proper comma placement
- Verify all required fields are present

---

## Action Sequence Syntax

The `actionSequencePattern` defines the sequence of user actions that indicate this ritual. It's an array of ActionType objects.

### ActionType Object Structure

```json
{
  "type": "ActionTypeName",
  "productCategoryId": "00000000-0000-0000-0000-000000000001",
  "metadata": null
}
```

### ActionType Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `type` | string | Yes | Type of action (see Action Types below) |
| `productCategoryId` | UUID | No | Category ID if action is category-specific |
| `metadata` | object | No | Additional metadata (usually null) |

### Supported Action Types

| Action Type | Description | Example |
|------------|-------------|---------|
| `ViewProduct` | User views a product detail page | Viewing a mâm cúng (offering tray) |
| `AddToCart` | User adds a product to cart | Adding ritual items to cart |
| `BrowseCategory` | User browses a product category | Browsing "Ritual Items" category |
| `RemoveFromCart` | User removes item from cart | Removing an item |
| `UpdateCart` | User updates cart quantity | Changing quantity of items |

### Example Action Sequences

**Đầy Tháng (1-month celebration):**
```json
"actionSequencePattern": [
  {
    "type": "ViewProduct",
    "productCategoryId": "00000000-0000-0000-0000-000000000001",
    "metadata": null
  },
  {
    "type": "AddToCart",
    "productCategoryId": null,
    "metadata": null
  },
  {
    "type": "BrowseCategory",
    "productCategoryId": "00000000-0000-0000-0000-000000000002",
    "metadata": null
  }
]
```

**Tết Nguyên Đán (Lunar New Year):**
```json
"actionSequencePattern": [
  {
    "type": "BrowseCategory",
    "productCategoryId": "00000000-0000-0000-0000-000000000003",
    "metadata": null
  },
  {
    "type": "ViewProduct",
    "productCategoryId": "00000000-0000-0000-0000-000000000003",
    "metadata": null
  },
  {
    "type": "AddToCart",
    "productCategoryId": null,
    "metadata": null
  },
  {
    "type": "ViewProduct",
    "productCategoryId": "00000000-0000-0000-0000-000000000004",
    "metadata": null
  }
]
```

### Guidelines for Action Sequences

1. **Sequence Length**: Keep sequences between 2-5 actions for optimal matching
2. **Specificity**: More specific sequences reduce false positives
3. **Order Matters**: Actions must occur in the specified order
4. **Category IDs**: Use actual category IDs from your product catalog
5. **Flexibility**: The system uses PrefixSpan-inspired matching, so partial matches are allowed

---

## Required Items Specification

The `requiredItems` array specifies which products are traditionally required for the ritual.

### RitualRequiredItem Object Structure

```json
{
  "categoryId": "00000000-0000-0000-0000-000000000001",
  "productIds": [
    "00000000-0000-0000-0000-000000000101",
    "00000000-0000-0000-0000-000000000102"
  ]
}
```

### Properties

| Property | Type | Required | Description |
|----------|------|----------|-------------|
| `categoryId` | UUID | Yes | Product category ID |
| `productIds` | array | Yes | Array of specific product IDs in this category |

### Guidelines for Required Items

1. **Catalog Validation**: All product IDs must exist in the product catalog
2. **Multiple Options**: Include multiple product IDs if there are traditional alternatives
3. **Category Organization**: Group items by category for better organization
4. **Completeness**: Include all traditionally required items for the ritual

### Example Required Items

**Đầy Tháng (1-month celebration):**
```json
"requiredItems": [
  {
    "categoryId": "00000000-0000-0000-0000-000000000001",
    "productIds": [
      "00000000-0000-0000-0000-000000000101",
      "00000000-0000-0000-0000-000000000102"
    ]
  },
  {
    "categoryId": "00000000-0000-0000-0000-000000000002",
    "productIds": [
      "00000000-0000-0000-0000-000000000201",
      "00000000-0000-0000-0000-000000000202"
    ]
  }
]
```

### How to Find Product IDs

1. Query the product catalog API: `GET /api/products`
2. Search for ritual items: `GET /api/products?search=mâm cúng`
3. Check the database directly
4. Use the admin panel to browse products

---

## Confidence Threshold Tuning

The `confidenceThreshold` is a decimal value (0.0 to 1.0) that determines how confident the system must be before recommending this ritual.

### Understanding Confidence Scores

- **0.0 - 0.3**: Very loose matching (many false positives)
- **0.3 - 0.5**: Loose matching (some false positives)
- **0.5 - 0.7**: Moderate matching (balanced)
- **0.7 - 0.85**: Strict matching (few false positives)
- **0.85 - 1.0**: Very strict matching (only exact matches)

### Recommended Thresholds by Ritual Type

| Ritual Type | Threshold | Reasoning |
|------------|-----------|-----------|
| Common rituals (Tết, Đầy Tháng) | 0.70 - 0.75 | Should be detected frequently |
| Seasonal rituals (Tết Trung Thu) | 0.72 - 0.78 | Seasonal specificity |
| Specialized rituals (Lễ Cúng) | 0.65 - 0.70 | Less common, broader matching |
| Rare rituals | 0.60 - 0.65 | Need to be detected when relevant |

### Tuning Guidelines

1. **Start Conservative**: Begin with 0.70 and adjust based on testing
2. **Monitor False Positives**: If users dismiss recommendations, increase threshold
3. **Monitor False Negatives**: If users don't see recommendations, decrease threshold
4. **A/B Testing**: Test different thresholds with user groups
5. **Seasonal Adjustment**: Adjust thresholds seasonally (e.g., higher for Tết during Lunar New Year)

### Example Thresholds

```json
{
  "id": "tet",
  "name": "Tết Nguyên Đán",
  "confidenceThreshold": 0.75,
  ...
}
```

---

## Validation and Testing

### Manifest Validation

The system automatically validates the manifest on startup:

1. **JSON Syntax**: Validates JSON structure
2. **Required Fields**: Checks all required fields are present
3. **Catalog Validation**: Verifies all product IDs exist in catalog
4. **ID Uniqueness**: Ensures ritual IDs are unique

### Manual Validation Steps

1. **Validate JSON**:
   ```bash
   # Use a JSON validator or your IDE
   # Check for syntax errors
   ```

2. **Check Product IDs**:
   ```sql
   -- Verify products exist in database
   SELECT * FROM Products WHERE Id IN ('product-id-1', 'product-id-2')
   ```

3. **Test Pattern Matching**:
   - Use the API endpoint: `POST /api/recommendations/analyze`
   - Send test action sequences
   - Verify the ritual is detected

4. **Test Recommendations**:
   - Simulate user actions matching the pattern
   - Verify recommendations are generated
   - Check missing items are correct

### Testing Checklist

- [ ] JSON is valid
- [ ] All product IDs exist in catalog
- [ ] Ritual ID is unique
- [ ] Action sequence is logical
- [ ] Required items are complete
- [ ] Confidence threshold is appropriate
- [ ] Cultural significance is accurate
- [ ] Sources are cited
- [ ] Pattern matching works correctly
- [ ] Recommendations are generated
- [ ] Missing items are correct

---

## Frontend Integration

### What Frontend Developers Need to Do

When a new ritual is added to the manifest, frontend developers need to:

1. **Update Recommendation Display**:
   - Ensure the UI can display the new ritual name
   - Add any ritual-specific styling or icons

2. **Update Dismissal Tracking**:
   - Ensure dismissals are tracked for the new ritual ID
   - Update user preference storage

3. **Update Explanation Generation**:
   - Ensure Gemini API receives the new ritual context
   - Test explanation generation for the new ritual

4. **Update Testing**:
   - Add test cases for the new ritual pattern
   - Test end-to-end flow with the new ritual

### Frontend API Integration

The frontend calls these endpoints:

**1. Analyze Actions (Backend)**:
```
POST /api/recommendations/analyze
Content-Type: application/json

{
  "actionSequence": [
    {
      "type": "ViewProduct",
      "productCategoryId": "00000000-0000-0000-0000-000000000001",
      "timestamp": "2025-12-20T10:00:00Z"
    }
  ]
}

Response:
{
  "ritualId": "your-ritual-id",
  "ritualName": "Your Ritual Name",
  "confidenceScore": 0.75,
  "missingItems": [...],
  "matchingMetadata": {...},
  "systemReport": {...}
}
```

**2. Generate Explanation (Frontend)**:
```
POST /api/explanations/generate
Content-Type: application/json

{
  "ritualId": "your-ritual-id",
  "ritualName": "Your Ritual Name",
  "confidenceScore": 0.75,
  "missingItems": [...],
  "matchingMetadata": {...},
  "systemReport": {...}
}

Response:
{
  "ritualName": "Your Ritual Name",
  "culturalContext": "Generated explanation...",
  "itemExplanations": [
    {
      "productId": "...",
      "productName": "...",
      "whyNeeded": "...",
      "traditionalUsage": "..."
    }
  ],
  "sources": ["Source 1"],
  "generatedBy": "gemini"
}
```

**3. Track Dismissals**:
```
POST /api/preferences/dismiss-ritual
Content-Type: application/json

{
  "ritualId": "your-ritual-id"
}
```

### Frontend Display Example

```html
<div class="ritual-recommendation">
  <h3>{{ recommendation.ritualName }}</h3>
  <p class="confidence">Confidence: {{ recommendation.confidenceScore * 100 }}%</p>
  
  <div class="explanation">
    {{ explanation.culturalContext }}
  </div>
  
  <div class="missing-items">
    <h4>Recommended Items:</h4>
    <ul>
      <li v-for="item in explanation.itemExplanations" :key="item.productId">
        <strong>{{ item.productName }}</strong>: {{ item.whyNeeded }}
      </li>
    </ul>
  </div>
  
  <button @click="dismissRitual">Dismiss</button>
</div>
```

---

## Examples

### Complete Example: Lễ Cúng Thần Tài (Wealth God Worship)

```json
{
  "id": "le-cung-than-tai",
  "name": "Lễ Cúng Thần Tài",
  "actionSequencePattern": [
    {
      "type": "BrowseCategory",
      "productCategoryId": "00000000-0000-0000-0000-000000000007",
      "metadata": null
    },
    {
      "type": "ViewProduct",
      "productCategoryId": "00000000-0000-0000-0000-000000000007",
      "metadata": null
    },
    {
      "type": "AddToCart",
      "productCategoryId": null,
      "metadata": null
    }
  ],
  "requiredItems": [
    {
      "categoryId": "00000000-0000-0000-0000-000000000007",
      "productIds": [
        "00000000-0000-0000-0000-000000000701",
        "00000000-0000-0000-0000-000000000702",
        "00000000-0000-0000-0000-000000000703"
      ]
    }
  ],
  "confidenceThreshold": 0.7,
  "culturalSignificance": "Lễ Cúng Thần Tài là lễ hội để cầu mong tài lộc và may mắn trong kinh doanh. Thường được tổ chức vào ngày mùng 10 tháng Giêng hoặc ngày mùng 1 tháng Giêng. Các vật phẩm cúng bao gồm hoa, quả, bánh, và các đồ vàng.",
  "sources": [
    "Vietnamese Wealth God Worship",
    "Traditional Beliefs"
  ]
}
```

### Complete Example: Tết Trung Thu (Mid-Autumn Festival)

```json
{
  "id": "tet-trung-thu",
  "name": "Tết Trung Thu",
  "actionSequencePattern": [
    {
      "type": "ViewProduct",
      "productCategoryId": "00000000-0000-0000-0000-000000000008",
      "metadata": null
    },
    {
      "type": "BrowseCategory",
      "productCategoryId": "00000000-0000-0000-0000-000000000008",
      "metadata": null
    },
    {
      "type": "AddToCart",
      "productCategoryId": null,
      "metadata": null
    }
  ],
  "requiredItems": [
    {
      "categoryId": "00000000-0000-0000-0000-000000000008",
      "productIds": [
        "00000000-0000-0000-0000-000000000801",
        "00000000-0000-0000-0000-000000000802",
        "00000000-0000-0000-0000-000000000803"
      ]
    }
  ],
  "confidenceThreshold": 0.72,
  "culturalSignificance": "Tết Trung Thu là lễ hội truyền thống của trẻ em Việt Nam, diễn ra vào ngày rằm tháng 8 âm lịch. Đây là dịp để gia đình tụ tập, trẻ em được tặng đèn lồng, bánh trung thu, và các đồ chơi truyền thống.",
  "sources": [
    "Vietnamese Mid-Autumn Festival",
    "Children's Traditions"
  ]
}
```

---

## Troubleshooting

### Issue: Ritual not being detected

**Possible Causes**:
1. Confidence threshold too high
2. Action sequence doesn't match user behavior
3. Product IDs not in catalog

**Solutions**:
1. Lower confidence threshold by 0.05-0.10
2. Review and adjust action sequence
3. Verify product IDs exist in catalog

### Issue: Too many false positives

**Possible Causes**:
1. Confidence threshold too low
2. Action sequence too generic
3. Overlapping patterns with other rituals

**Solutions**:
1. Increase confidence threshold by 0.05-0.10
2. Make action sequence more specific
3. Review other rituals for conflicts

### Issue: Manifest validation fails

**Possible Causes**:
1. Invalid JSON syntax
2. Missing required fields
3. Product IDs don't exist in catalog

**Solutions**:
1. Validate JSON using a JSON validator
2. Check all required fields are present
3. Verify product IDs in database

---

## Best Practices

1. **Cultural Accuracy**: Ensure ritual descriptions are culturally accurate
2. **Product Completeness**: Include all traditionally required items
3. **Threshold Tuning**: Start conservative and adjust based on user feedback
4. **Testing**: Thoroughly test new rituals before deployment
5. **Documentation**: Document any custom metadata or special handling
6. **Versioning**: Keep track of manifest changes in version control
7. **Monitoring**: Monitor recommendation accuracy and user feedback

---

## Related Documentation

- [API Documentation](./API_DOCUMENTATION.md)
- [BE-AI Integration Guide](./BE_AI_INTEGRATION_GUIDE.md)
- [FE-AI Integration Guide](./FE_AI_INTEGRATION_GUIDE.md)
- [Design Document](./design.md)
- [Requirements Document](./requirements.md)
