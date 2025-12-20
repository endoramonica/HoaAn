# BE-AI Integration Guide: Sequential Ritual Recommendation System

## Overview

BE-AI (Backend Strategist) là thành phần chịu trách nhiệm:
1. **Định nghĩa Ritual Patterns** - Các mẫu lễ lễ Việt Nam
2. **Phân tích Action Sequences** - Phát hiện người dùng đang chuẩn bị lễ gì
3. **Trả về Recommendation Payload** - Dữ liệu cho FE-AI xử lý

---

## 🏗️ BE-AI Architecture

```
┌──────────────────────────────────────────────────────────────┐
│ 1. RITUAL MANIFEST (JSON Configuration)                      │
│    - Định nghĩa các lễ lễ Việt Nam                           │
│    - Action sequences cho mỗi lễ                             │
│    - Required items cho mỗi lễ                               │
│    - Confidence thresholds                                   │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────────────────────────┐
│ 2. RITUAL MANIFEST LOADER                                    │
│    - Load manifest từ JSON file                              │
│    - Validate ritual data                                    │
│    - Index patterns cho efficient lookup                     │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────────────────────────┐
│ 3. SEQUENTIAL PATTERN MATCHER                                │
│    - Implement PrefixSpan-inspired algorithm                 │
│    - Match action sequences against patterns                 │
│    - Calculate confidence scores                             │
│    - Return MatchResult                                      │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────────────────────────┐
│ 4. RECOMMENDATION SERVICE                                    │
│    - Get missing items from ritual                           │
│    - Lookup products in catalog                              │
│    - Generate system report                                  │
│    - Return RecommendationPayload                            │
└────────────────────┬─────────────────────────────────────────┘
                     │
                     ▼
┌──────────────────────────────────────────────────────────────┐
│ 5. API ENDPOINT                                              │
│    POST /api/recommendations/analyze                         │
│    - Accept action sequence from frontend                    │
│    - Return RecommendationPayload                            │
└──────────────────────────────────────────────────────────────┘
```

---

## 📋 BE-AI Responsibilities

### 1. **Define Ritual Manifest (ritual-manifest.json)**

**Location:** `VietCommerce.Api/wwwroot/data/ritual-manifest.json`

**Structure:**
```json
{
  "rituals": [
    {
      "id": "day-thang-ritual",
      "name": "Đầy Tháng",
      "description": "Lễ Đầy Tháng - Nghi lễ khi bé tròn 1 tháng tuổi",
      "culturalSignificance": "Lễ Đầy Tháng là nghi lễ quan trọng trong văn hóa Việt, tổ chức khi bé tròn 1 tháng tuổi để cảm ơn các vị thần bảo vệ gia đình.",
      "sources": [
        "Phong tục Việt cổ truyền",
        "Tục lệ gia đình Việt Nam"
      ],
      "actionSequencePattern": [
        {
          "type": "ViewProduct",
          "productCategoryId": "ritual-items",
          "metadata": { "keyword": "mâm cúng" }
        },
        {
          "type": "AddToCart",
          "productCategoryId": "ritual-items"
        },
        {
          "type": "BrowseCategory",
          "productCategoryId": "food-items",
          "metadata": { "keyword": "heo quay" }
        }
      ],
      "requiredItems": [
        {
          "categoryId": "ritual-items",
          "productIds": ["product-123", "product-124", "product-125"],
          "description": "Bộ Tam Sên"
        },
        {
          "categoryId": "food-items",
          "productIds": ["product-456", "product-457"],
          "description": "Heo Quay"
        },
        {
          "categoryId": "fruits",
          "productIds": ["product-789"],
          "description": "Ngũ Quả"
        }
      ],
      "confidenceThreshold": 0.75
    },
    {
      "id": "tet-ritual",
      "name": "Tết",
      "description": "Lễ Tết Nguyên Đán",
      "culturalSignificance": "Tết là ngày lễ quan trọng nhất trong năm, kỷ niệm sự bắt đầu của một năm mới.",
      "sources": ["Phong tục Tết Việt Nam"],
      "actionSequencePattern": [
        {
          "type": "ViewProduct",
          "productCategoryId": "tet-decorations"
        },
        {
          "type": "BrowseCategory",
          "productCategoryId": "tet-gifts"
        },
        {
          "type": "AddToCart",
          "productCategoryId": "tet-gifts"
        }
      ],
      "requiredItems": [
        {
          "categoryId": "tet-decorations",
          "productIds": ["product-1001"],
          "description": "Hoa Đào"
        },
        {
          "categoryId": "tet-gifts",
          "productIds": ["product-1002", "product-1003"],
          "description": "Quà Tết"
        }
      ],
      "confidenceThreshold": 0.70
    }
  ]
}
```

---

### 2. **Implement RitualManifest Loader**

**File:** `VietCommerce.Application/Services/Services/RitualManifestLoader.cs`

**Responsibilities:**
- Load manifest từ JSON file
- Validate ritual data
- Index patterns cho efficient lookup
- Cache manifest in memory

**Code Structure:**
```csharp
public interface IRitualManifestLoader
{
    Task LoadManifestAsync(string manifestPath);
    RitualManifest GetManifest();
    Ritual GetRitualById(string ritualId);
    IEnumerable<Ritual> GetAllRituals();
}

public class RitualManifestLoader : IRitualManifestLoader
{
    private RitualManifest _manifest;
    private readonly ILogger<RitualManifestLoader> _logger;
    
    public async Task LoadManifestAsync(string manifestPath)
    {
        // 1. Read JSON file
        // 2. Deserialize to RitualManifest
        // 3. Validate all required items exist in catalog
        // 4. Index patterns by action types
        // 5. Cache in memory
    }
    
    public RitualManifest GetManifest() => _manifest;
    
    public Ritual GetRitualById(string ritualId) 
        => _manifest.Rituals.FirstOrDefault(r => r.Id == ritualId);
}
```

---

### 3. **Implement Sequential Pattern Matcher**

**File:** `VietCommerce.Application/Services/Services/SequentialPatternMatcher.cs`

**Algorithm: PrefixSpan-Inspired**

```csharp
public interface ISequentialPatternMatcher
{
    MatchResult MatchPattern(List<Action> actionSequence);
    void LoadManifest(RitualManifest manifest);
}

public class SequentialPatternMatcher : ISequentialPatternMatcher
{
    private RitualManifest _manifest;
    
    public MatchResult MatchPattern(List<Action> actionSequence)
    {
        // 1. Iterate through all rituals in manifest
        // 2. For each ritual:
        //    a. Try to match action sequence against pattern
        //    b. Calculate confidence score based on:
        //       - Number of matched actions
        //       - Order of actions
        //       - Exact vs partial matches
        // 3. Return best match (highest confidence)
        
        var bestMatch = new MatchResult { Matched = false };
        
        foreach (var ritual in _manifest.Rituals)
        {
            var matchResult = MatchRitualPattern(actionSequence, ritual);
            
            if (matchResult.ConfidenceScore > bestMatch.ConfidenceScore &&
                matchResult.ConfidenceScore >= ritual.ConfidenceThreshold)
            {
                bestMatch = matchResult;
            }
        }
        
        return bestMatch;
    }
    
    private MatchResult MatchRitualPattern(List<Action> sequence, Ritual ritual)
    {
        // PrefixSpan-inspired matching:
        // 1. Find longest common subsequence
        // 2. Calculate confidence = matched_length / pattern_length
        // 3. Return MatchResult with metadata
    }
}
```

**Confidence Score Calculation:**
```
confidence = (matched_actions / total_pattern_actions) * 
             (sequence_order_match_score) * 
             (category_match_score)

Example:
- Pattern: [ViewMamCung, AddToy, ViewNguQua]
- Sequence: [ViewMamCung, AddToy, ViewNguQua, AddToCart]
- Matched: 3/3 actions in correct order
- Confidence: (3/3) * 1.0 * 1.0 = 1.0 (100%)
```

---

### 4. **Implement Recommendation Service**

**File:** `VietCommerce.Application/Services/Services/RecommendationService.cs`

**Responsibilities:**
- Get missing items from ritual
- Lookup products in catalog
- Generate system report
- Log recommendations

**Code Structure:**
```csharp
public interface IRecommendationService
{
    Task<RecommendationPayload> GenerateRecommendation(
        List<Action> actionSequence, 
        List<CartItem> cartItems);
    
    Task<List<Product>> GetMissingItems(Ritual ritual, List<CartItem> cartItems);
    
    Task LogRecommendation(RecommendationPayload payload);
}

public class RecommendationService : IRecommendationService
{
    private readonly ISequentialPatternMatcher _patternMatcher;
    private readonly IProductRepository _productRepository;
    private readonly IRecommendationLogRepository _logRepository;
    
    public async Task<RecommendationPayload> GenerateRecommendation(
        List<Action> actionSequence, 
        List<CartItem> cartItems)
    {
        // 1. Match pattern
        var matchResult = _patternMatcher.MatchPattern(actionSequence);
        
        if (!matchResult.Matched)
        {
            return new RecommendationPayload { Matched = false };
        }
        
        // 2. Get missing items
        var ritual = _patternMatcher.GetRitual(matchResult.RitualId);
        var missingItems = await GetMissingItems(ritual, cartItems);
        
        // 3. Generate system report
        var systemReport = GenerateSystemReport(matchResult, missingItems);
        
        // 4. Create payload
        var payload = new RecommendationPayload
        {
            RitualId = matchResult.RitualId,
            RitualName = ritual.Name,
            ConfidenceScore = matchResult.ConfidenceScore,
            MissingItems = missingItems,
            MatchingMetadata = matchResult.Metadata,
            SystemReport = systemReport
        };
        
        // 5. Log recommendation
        await LogRecommendation(payload);
        
        return payload;
    }
    
    public async Task<List<Product>> GetMissingItems(
        Ritual ritual, 
        List<CartItem> cartItems)
    {
        var cartProductIds = cartItems.Select(c => c.ProductId).ToList();
        var missingItems = new List<Product>();
        
        foreach (var requiredItem in ritual.RequiredItems)
        {
            foreach (var productId in requiredItem.ProductIds)
            {
                if (!cartProductIds.Contains(productId))
                {
                    var product = await _productRepository.GetByIdAsync(productId);
                    if (product != null)
                    {
                        missingItems.Add(product);
                    }
                }
            }
        }
        
        return missingItems;
    }
    
    private SystemReport GenerateSystemReport(
        MatchResult matchResult, 
        List<Product> missingItems)
    {
        return new SystemReport
        {
            MatchedPattern = matchResult.MatchedPattern,
            MatchingSteps = matchResult.MatchingSteps,
            ReasonsForMissingItems = missingItems.ToDictionary(
                p => p.Id,
                p => $"{p.Name} là lễ vật truyền thống không thể thiếu"
            )
        };
    }
}
```

---

### 5. **Create API Endpoint**

**File:** `VietCommerce.Api/Controllers/RecommendationController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
public class RecommendationController : ControllerBase
{
    private readonly IRecommendationService _recommendationService;
    private readonly IActionTrackingService _actionTrackingService;
    
    [HttpPost("analyze")]
    public async Task<IActionResult> AnalyzeActionSequence(
        [FromBody] AnalyzeRecommendationRequest request)
    {
        try
        {
            // 1. Get action sequence from request
            var actionSequence = await _actionTrackingService
                .GetActionSequence(request.UserId, request.SessionId);
            
            // 2. Get cart items
            var cartItems = await _cartService.GetCartItems(request.UserId);
            
            // 3. Generate recommendation
            var payload = await _recommendationService
                .GenerateRecommendation(actionSequence, cartItems);
            
            // 4. Return payload
            return Ok(payload);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing recommendation");
            return StatusCode(500, new { error = "Internal server error" });
        }
    }
}

public class AnalyzeRecommendationRequest
{
    public string UserId { get; set; }
    public string SessionId { get; set; }
}
```

---

## 📊 Data Models BE-AI cần

### 1. **RitualManifest.cs**
```csharp
public class RitualManifest
{
    public List<Ritual> Rituals { get; set; }
}

public class Ritual
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string CulturalSignificance { get; set; }
    public List<string> Sources { get; set; }
    public List<ActionType> ActionSequencePattern { get; set; }
    public List<RequiredItemGroup> RequiredItems { get; set; }
    public double ConfidenceThreshold { get; set; }
}

public class ActionType
{
    public string Type { get; set; } // ViewProduct, AddToCart, BrowseCategory
    public string ProductCategoryId { get; set; }
    public Dictionary<string, object> Metadata { get; set; }
}

public class RequiredItemGroup
{
    public string CategoryId { get; set; }
    public List<string> ProductIds { get; set; }
    public string Description { get; set; }
}
```

### 2. **MatchResult.cs**
```csharp
public class MatchResult
{
    public bool Matched { get; set; }
    public string RitualId { get; set; }
    public string RitualName { get; set; }
    public double ConfidenceScore { get; set; }
    public List<Action> MatchedActions { get; set; }
    public List<ActionType> MatchedPattern { get; set; }
    public MatchingMetadata Metadata { get; set; }
}

public class MatchingMetadata
{
    public int MatchedSequenceLength { get; set; }
    public int TotalSequenceLength { get; set; }
    public List<int> MatchedActionIndices { get; set; }
}
```

### 3. **RecommendationPayload.cs**
```csharp
public class RecommendationPayload
{
    public bool Matched { get; set; }
    public string RitualId { get; set; }
    public string RitualName { get; set; }
    public double ConfidenceScore { get; set; }
    public List<Product> MissingItems { get; set; }
    public MatchingMetadata MatchingMetadata { get; set; }
    public SystemReport SystemReport { get; set; }
}

public class SystemReport
{
    public List<ActionType> MatchedPattern { get; set; }
    public List<string> MatchingSteps { get; set; }
    public Dictionary<string, string> ReasonsForMissingItems { get; set; }
}
```

---

## 🔗 Integration Points

### BE-AI provides to FE-AI:
- ✅ RecommendationPayload (ritual name, missing items, confidence)
- ✅ SystemReport (matching logic, reasons)
- ✅ CulturalSignificance (from Ritual Manifest)
- ✅ Sources (cultural references)

### BE-AI needs from other components:
- ✅ Action Sequence (from ActionTrackingService)
- ✅ Cart Items (from CartService)
- ✅ Product Catalog (from ProductRepository)
- ✅ Ritual Manifest (from RitualManifestLoader)

---

## 🧪 Testing BE-AI

### Unit Tests
```csharp
[Test]
public void MatchPattern_WithValidSequence_ReturnsMatchResult()
{
    // Arrange
    var actionSequence = new List<Action>
    {
        new Action { Type = "ViewProduct", CategoryId = "ritual-items" },
        new Action { Type = "AddToCart", CategoryId = "ritual-items" },
        new Action { Type = "BrowseCategory", CategoryId = "food-items" }
    };
    
    // Act
    var result = _patternMatcher.MatchPattern(actionSequence);
    
    // Assert
    Assert.True(result.Matched);
    Assert.Equal("day-thang-ritual", result.RitualId);
    Assert.GreaterOrEqual(result.ConfidenceScore, 0.75);
}

[Test]
public async Task GenerateRecommendation_WithMatchedPattern_ReturnsMissingItems()
{
    // Arrange
    var actionSequence = /* matched sequence */;
    var cartItems = new List<CartItem> { /* some items */ };
    
    // Act
    var payload = await _recommendationService
        .GenerateRecommendation(actionSequence, cartItems);
    
    // Assert
    Assert.True(payload.Matched);
    Assert.NotEmpty(payload.MissingItems);
    Assert.NotNull(payload.SystemReport);
}
```

---

## ✅ Checklist for BE-AI Implementation

- [ ] Create ritual-manifest.json with Vietnamese rituals
- [ ] Create RitualManifest.cs data model
- [ ] Create RitualManifestLoader.cs
- [ ] Create SequentialPatternMatcher.cs with PrefixSpan algorithm
- [ ] Create RecommendationService.cs
- [ ] Create RecommendationController.cs
- [ ] Create MatchResult.cs DTO
- [ ] Create RecommendationPayload.cs DTO
- [ ] Create SystemReport.cs DTO
- [ ] Implement error handling
- [ ] Write unit tests for pattern matching
- [ ] Write unit tests for recommendation generation
- [ ] Write integration tests
- [ ] Document API endpoints in Swagger
- [ ] Test with sample action sequences

---

## 🎯 Success Criteria

✅ Ritual Manifest loads successfully on startup
✅ Pattern matching works with sample action sequences
✅ Confidence scores calculated correctly
✅ Missing items identified accurately
✅ System report generated with matching logic
✅ API endpoint returns RecommendationPayload
✅ All tests pass (unit + integration)
✅ Error handling works for edge cases

