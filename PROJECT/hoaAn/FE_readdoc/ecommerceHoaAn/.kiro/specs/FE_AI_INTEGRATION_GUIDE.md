# FE-AI Integration Guide: Sequential Ritual Recommendation System

## Overview

FE-AI (Frontend Advisor) là thành phần chịu trách nhiệm gọi Gemini API để tạo giải thích tự nhiên cho các gợi ý sản phẩm. FE-AI nhận dữ liệu từ BE-AI và biến nó thành những lời giải thích dễ hiểu cho người dùng.

---

## 🔄 Data Flow: BE-AI → FE-AI → User

```
┌─────────────────────────────────────────────────────────────────┐
│ 1. USER ACTION (Frontend)                                       │
│    - Xem sản phẩm, thêm vào giỏ, duyệt danh mục                │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 2. BE-AI ANALYSIS (Backend)                                     │
│    POST /api/recommendations/analyze                            │
│    Input: actionSequence                                        │
│    Output: RecommendationPayload {                              │
│      ritualId, ritualName, confidenceScore,                     │
│      missingItems, matchingMetadata, systemReport               │
│    }                                                             │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 3. FE-AI EXPLANATION (Frontend)                                 │
│    POST /api/explanations/generate                              │
│    Input: RecommendationPayload                                 │
│    → Call Gemini API                                            │
│    Output: ExplanationPayload {                                 │
│      ritualName, culturalContext,                               │
│      itemExplanations[], sources[], generatedBy                 │
│    }                                                             │
└────────────────────┬────────────────────────────────────────────┘
                     │
                     ▼
┌─────────────────────────────────────────────────────────────────┐
│ 4. DISPLAY TO USER (Frontend UI)                                │
│    - Hiển thị gợi ý sản phẩm                                    │
│    - Hiển thị lý do theo phong tục                              │
│    - Hiển thị confidence score                                  │
│    - Cho phép dismiss/disable ritual                            │
└─────────────────────────────────────────────────────────────────┘
```

---

## 📋 FE-AI Responsibilities

### 1. **Receive RecommendationPayload from BE-AI**

**Endpoint:** `POST /api/recommendations/analyze`

**Response từ BE-AI:**
```json
{
  "ritualId": "day-thang-ritual",
  "ritualName": "Đầy Tháng",
  "confidenceScore": 0.92,
  "missingItems": [
    {
      "id": "product-123",
      "name": "Bộ Tam Sên",
      "category": "Ritual Items"
    },
    {
      "id": "product-456",
      "name": "Heo Quay",
      "category": "Food Items"
    }
  ],
  "matchingMetadata": {
    "matchedSequenceLength": 3,
    "totalSequenceLength": 4,
    "matchedActionIndices": [0, 1, 2]
  },
  "systemReport": {
    "matchedPattern": ["ViewMamCung", "AddToy", "ViewNguQua"],
    "matchingSteps": [
      "Matched action 0: ViewMamCung",
      "Matched action 1: AddToy",
      "Matched action 2: ViewNguQua"
    ],
    "reasonsForMissingItems": {
      "product-123": "Bộ Tam Sên là bộ ba vị thần bảo vệ gia đình",
      "product-456": "Heo Quay là lễ vật truyền thống không thể thiếu"
    }
  }
}
```

---

### 2. **Call Gemini API to Generate Explanation**

**Endpoint:** `POST /api/explanations/generate`

**Request từ FE-AI:**
```json
{
  "ritualId": "day-thang-ritual",
  "ritualName": "Đầy Tháng",
  "culturalSignificance": "Lễ Đầy Tháng là nghi lễ quan trọng trong văn hóa Việt, tổ chức khi bé tròn 1 tháng tuổi",
  "missingItems": [
    {
      "productId": "product-123",
      "productName": "Bộ Tam Sên",
      "category": "Ritual Items"
    },
    {
      "productId": "product-456",
      "productName": "Heo Quay",
      "category": "Food Items"
    }
  ],
  "confidenceScore": 0.92
}
```

**Gemini API Call (inside FE-AI):**
```
Prompt to Gemini:
"Bạn là một chuyên gia về phong tục Việt. Hãy giải thích tại sao những sản phẩm sau đây là cần thiết cho lễ Đầy Tháng:
- Bộ Tam Sên
- Heo Quay

Hãy cung cấp:
1. Tên lễ lễ: Đầy Tháng
2. Bối cảnh văn hóa: Lễ Đầy Tháng là nghi lễ quan trọng...
3. Cho mỗi sản phẩm:
   - Tên sản phẩm
   - Tại sao nó cần thiết
   - Cách sử dụng truyền thống
4. Nguồn tham khảo

Trả lời bằng tiếng Việt, ngắn gọn và dễ hiểu."
```

**Response từ Gemini:**
```
Lễ Đầy Tháng là một nghi lễ quan trọng trong văn hóa Việt, tổ chức khi bé tròn 1 tháng tuổi để cảm ơn các vị thần bảo vệ gia đình.

Bộ Tam Sên:
- Tại sao cần thiết: Bộ Tam Sên (Phật, Thánh, Tổ) là biểu tượng của ba vị thần bảo vệ gia đình. Đây là lễ vật không thể thiếu trong bất kỳ lễ lễ nào.
- Cách sử dụng: Đặt trên mâm cúng ở vị trí trung tâm, hướng về phía trước.

Heo Quay:
- Tại sao cần thiết: Heo Quay là lễ vật truyền thống, biểu tượng của sự sung túc và may mắn. Nó là phần không thể thiếu của mâm cúng.
- Cách sử dụng: Đặt trên mâm cúng, thường ở phía trước bên phải.

Nguồn tham khảo: Phong tục Việt cổ truyền, Tục lệ gia đình Việt Nam.
```

---

### 3. **Format Response as ExplanationPayload**

**Response từ FE-AI:**
```json
{
  "ritualName": "Đầy Tháng",
  "culturalContext": "Lễ Đầy Tháng là một nghi lễ quan trọng trong văn hóa Việt, tổ chức khi bé tròn 1 tháng tuổi để cảm ơn các vị thần bảo vệ gia đình.",
  "itemExplanations": [
    {
      "productId": "product-123",
      "productName": "Bộ Tam Sên",
      "whyNeeded": "Bộ Tam Sên (Phật, Thánh, Tổ) là biểu tượng của ba vị thần bảo vệ gia đình. Đây là lễ vật không thể thiếu trong bất kỳ lễ lễ nào.",
      "traditionalUsage": "Đặt trên mâm cúng ở vị trí trung tâm, hướng về phía trước."
    },
    {
      "productId": "product-456",
      "productName": "Heo Quay",
      "whyNeeded": "Heo Quay là lễ vật truyền thống, biểu tượng của sự sung túc và may mắn. Nó là phần không thể thiếu của mâm cúng.",
      "traditionalUsage": "Đặt trên mâm cúng, thường ở phía trước bên phải."
    }
  ],
  "sources": [
    "Phong tục Việt cổ truyền",
    "Tục lệ gia đình Việt Nam"
  ],
  "generatedBy": "gemini"
}
```

---

## 🛠️ FE-AI Implementation Checklist

### Files FE-AI cần tạo/cập nhật:

#### 1. **Backend API Endpoint** (C# - VietCommerce.Api)
```
VietCommerce.Api/Controllers/ExplanationController.cs
- POST /api/explanations/generate
  - Input: RecommendationPayload
  - Call GeminiExplanationService
  - Return ExplanationPayload
```

#### 2. **Gemini Service** (C# - VietCommerce.Application)
```
VietCommerce.Application/Services/Services/GeminiExplanationService.cs
- Interface: IGeminiExplanationService
- Method: GenerateExplanation(RecommendationPayload)
  - Build prompt with ritual context
  - Call Gemini API with VITE_GEMINI_API_KEY
  - Parse response
  - Return ExplanationPayload
- Method: GetFallbackExplanation(RecommendationPayload)
  - Return template explanation if API fails
```

#### 3. **DTOs** (C# - VietCommerce.Core)
```
VietCommerce.Core/DTOs/Rituals/ExplanationPayloadDto.cs
- ritualName: string
- culturalContext: string
- itemExplanations: ItemExplanationDto[]
- sources: string[]
- generatedBy: string ("gemini" | "fallback")

VietCommerce.Core/DTOs/Rituals/ItemExplanationDto.cs
- productId: string
- productName: string
- whyNeeded: string
- traditionalUsage: string
```

#### 4. **Frontend Integration** (TypeScript/React - Frontend)
```
src/services/ritualRecommendationService.ts
- Function: callBeAiAnalysis(actionSequence)
  - POST /api/recommendations/analyze
  - Return RecommendationPayload

- Function: callFeAiExplanation(payload)
  - POST /api/explanations/generate
  - Return ExplanationPayload

src/components/RitualRecommendation.tsx
- Display RecommendationPayload + ExplanationPayload
- Show ritual name, confidence score
- Show item explanations
- Show dismiss/disable buttons
```

---

## 📝 Environment Variables FE-AI cần

**Backend (.env hoặc appsettings.json):**
```
GEMINI_API_KEY=AIzaSyBCAUM9DHJjeILypYzSEtUY76W-jmXQsqU
GEMINI_API_ENDPOINT=https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent
```

**Frontend (.env.local):**
```
VITE_GEMINI_API_KEY=AIzaSyBCAUM9DHJjeILypYzSEtUY76W-jmXQsqU
VITE_API_BASE_URL=http://localhost:5000/api
```

---

## 🔐 Error Handling FE-AI cần xử lý

### 1. **Gemini API Timeout**
```csharp
try {
  response = await geminiClient.GenerateContent(prompt);
} catch (TimeoutException) {
  return GetFallbackExplanation(payload);
}
```

### 2. **Gemini API Rate Limit**
```csharp
try {
  response = await geminiClient.GenerateContent(prompt);
} catch (RateLimitException) {
  // Queue request, retry with exponential backoff
  await Task.Delay(TimeSpan.FromSeconds(Math.Pow(2, retryCount)));
  return await GenerateExplanation(payload); // Retry
}
```

### 3. **Invalid Gemini Response**
```csharp
try {
  explanation = ParseGeminiResponse(response);
} catch (JsonException) {
  return GetFallbackExplanation(payload);
}
```

### 4. **Missing Required Fields**
```csharp
if (string.IsNullOrEmpty(payload.RitualName) || 
    payload.MissingItems == null || 
    payload.MissingItems.Count == 0) {
  throw new ArgumentException("Invalid RecommendationPayload");
}
```

---

## 📊 Fallback Template (khi Gemini API fail)

```json
{
  "ritualName": "{ritualName}",
  "culturalContext": "Đây là một lễ lễ truyền thống Việt Nam. Các sản phẩm dưới đây được khuyến nghị dựa trên phong tục.",
  "itemExplanations": [
    {
      "productId": "{productId}",
      "productName": "{productName}",
      "whyNeeded": "Sản phẩm này là một phần truyền thống của lễ lễ này.",
      "traditionalUsage": "Sử dụng theo phong tục Việt Nam."
    }
  ],
  "sources": ["Phong tục Việt Nam"],
  "generatedBy": "fallback"
}
```

---

## 🔗 Integration Points

### BE-AI → FE-AI Communication

**1. BE-AI cung cấp cho FE-AI:**
- ✅ RecommendationPayload (ritual name, missing items, confidence score)
- ✅ systemReport (matching logic, reasons)
- ✅ culturalSignificance (từ Ritual Manifest)

**2. FE-AI cần từ BE-AI:**
- ✅ Ritual Manifest (để biết cultural significance)
- ✅ Product Catalog (để lấy product details)
- ✅ Action Sequence (để hiểu context)

**3. FE-AI trả về cho Frontend:**
- ✅ ExplanationPayload (human-readable explanation)
- ✅ Confidence score
- ✅ Item reasons
- ✅ Cultural references

---

## 🧪 Testing FE-AI

### Unit Tests
```csharp
[Test]
public void GenerateExplanation_WithValidPayload_ReturnsExplanationPayload()
{
  // Arrange
  var payload = new RecommendationPayload { ... };
  
  // Act
  var result = _geminiService.GenerateExplanation(payload);
  
  // Assert
  Assert.NotNull(result);
  Assert.NotEmpty(result.ItemExplanations);
  Assert.Equal("gemini", result.GeneratedBy);
}

[Test]
public void GenerateExplanation_WithApiFailure_ReturnsFallbackExplanation()
{
  // Arrange
  _geminiClient.Setup(x => x.GenerateContent(It.IsAny<string>()))
    .ThrowsAsync(new TimeoutException());
  
  // Act
  var result = _geminiService.GenerateExplanation(payload);
  
  // Assert
  Assert.Equal("fallback", result.GeneratedBy);
}
```

### Integration Tests
```csharp
[Test]
public async Task EndToEnd_ActionSequence_ReturnsExplanation()
{
  // 1. Call BE-AI
  var recommendation = await _beAiClient.Analyze(actionSequence);
  
  // 2. Call FE-AI
  var explanation = await _feAiClient.GenerateExplanation(recommendation);
  
  // 3. Verify
  Assert.NotNull(explanation);
  Assert.Equal(recommendation.RitualName, explanation.RitualName);
  Assert.Equal(recommendation.MissingItems.Count, explanation.ItemExplanations.Count);
}
```

---

## 📞 Communication Protocol

### Request/Response Format

**FE-AI Request to Gemini:**
```
POST https://generativelanguage.googleapis.com/v1beta/models/gemini-pro:generateContent
Authorization: Bearer {GEMINI_API_KEY}
Content-Type: application/json

{
  "contents": [{
    "parts": [{
      "text": "Prompt về lễ lễ và sản phẩm..."
    }]
  }]
}
```

**Gemini Response:**
```json
{
  "candidates": [{
    "content": {
      "parts": [{
        "text": "Giải thích về lễ lễ..."
      }]
    }
  }]
}
```

---

## ✅ Checklist for FE-AI Implementation

- [ ] Create ExplanationController.cs
- [ ] Create GeminiExplanationService.cs
- [ ] Create ExplanationPayloadDto.cs
- [ ] Create ItemExplanationDto.cs
- [ ] Configure Gemini API key in appsettings.json
- [ ] Implement error handling (timeout, rate limit, invalid response)
- [ ] Implement fallback template
- [ ] Write unit tests for GeminiExplanationService
- [ ] Write integration tests for end-to-end flow
- [ ] Create frontend service to call /api/explanations/generate
- [ ] Create React component to display ExplanationPayload
- [ ] Test with real Gemini API
- [ ] Document API endpoints in Swagger

---

## 🎯 Success Criteria

✅ FE-AI receives RecommendationPayload from BE-AI
✅ FE-AI calls Gemini API with proper context
✅ FE-AI returns ExplanationPayload with human-readable text
✅ FE-AI handles API failures gracefully with fallback
✅ Frontend displays explanation to user
✅ All tests pass (unit + integration)
✅ Confidence score and cultural references are included

