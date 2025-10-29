# VietCommerce.Core - Auto Generated Files

This document lists all files and directories that were created by the PowerShell automation script.  
The script ensures:

- ✅ Correct folder structure under **VietCommerce.Core**
- ✅ Files are created with namespace matching their folder
- ✅ Existing files are not overwritten
- ✅ Each file contains a minimal valid C# template

---

## 📂 Entities

- Entities\Common\ITenantEntity.cs
- Entities\Users\UserAddress.cs
- Entities\Products\ProductPrice.cs
- Entities\Products\InventoryMovement.cs
- Entities\Marketing\Campaign.cs
- Entities\Marketing\Promotion.cs
- Entities\Marketing\PromotionProduct.cs
- Entities\System\AuditLog.cs

## 📂 Enums

- Enums\Common\Status.cs
- Enums\Common\AddressType.cs
- Enums\Users\UserStatus.cs
- Enums\Customers\CustomerStatus.cs
- Enums\Products\ProductStatus.cs
- Enums\Products\PriceType.cs
- Enums\Products\InventoryMovementType.cs
- Enums\Orders\CartStatus.cs
- Enums\Orders\ShippingStatus.cs
- Enums\Payments\PaymentMethodType.cs
- Enums\Payments\TransactionType.cs
- Enums\Marketing\CampaignStatus.cs
- Enums\Marketing\CampaignType.cs
- Enums\Marketing\PromotionType.cs
- Enums\Marketing\PromotionStatus.cs
- Enums\Notifications\NotificationType.cs

## 📂 Helpers

- Helpers\PriceCalculationHelper.cs
- Helpers\InventoryHelper.cs
- Helpers\TenantHelper.cs
- Helpers\DateTimeHelper.cs

## 📂 Models

- Models\PriceRange.cs
- Models\InventoryAlert.cs
- Models\CampaignSummary.cs

## 📂 Exceptions

- Common\Exceptions\TenantException.cs
- Common\Exceptions\InventoryException.cs

## 📂 Constants

- Common\Constants\InventoryConstants.cs
- Common\Constants\CampaignConstants.cs
- Common\Constants\PricingConstants.cs
- Common\Constants\SystemConstants.cs

## 📂 Utils

- Common\Utils\TenantContextHelper.cs
- Common\Utils\AuditHelper.cs

## 📂 Extensions

- Common\Extensions\DecimalExtensions.cs
- Common\Extensions\EntityExtensions.cs

## 📂 Attributes

- Common\Attributes\TenantFilterAttribute.cs
- Common\Attributes\AuditableAttribute.cs

---

⚠️ Note:  
This README is generated for documentation purposes.  
The PowerShell script only creates files if they do not already exist.
#25/10/25
create mode 100644 VietCommerce.Api/Apifilelist.txt
 create mode 100644 VietCommerce.Api/Controllers/CartController.cs
 create mode 100644 VietCommerce.Api/Controllers/ProductController.cs
 create mode 100644 VietCommerce.Api/Services/CartService.cs
 create mode 100644 VietCommerce.Api/Services/Interfaces/ICacheService.cs
 create mode 100644 VietCommerce.Api/Services/Interfaces/ICartService.cs
 create mode 100644 VietCommerce.Api/Services/Interfaces/IProductService.cs
 create mode 100644 VietCommerce.Api/Services/PermissionCacheInvalidationService.cs
 create mode 100644 VietCommerce.Api/Services/ProductService.cs
 create mode 100644 VietCommerce.Api/Services/RedisCacheService.cs
 create mode 100644 VietCommerce.Application/Extension/ProductExtensions.cs
 create mode 100644 VietCommerce.Application/Mappings/CartMappingProfile.cs
 create mode 100644 VietCommerce.Application/Mappings/ProductMappingProfile.cs
 create mode 100644 VietCommerce.Core/DTOs/Cart/AddToCartResponseDto.cs
 create mode 100644 VietCommerce.Core/DTOs/Cart/CartDto.cs
 create mode 100644 VietCommerce.Core/DTOs/Cart/CartItemDto.cs
 create mode 100644 VietCommerce.Core/DTOs/Cart/CartSummaryDto.cs
 create mode 100644 VietCommerce.Core/DTOs/Cart/ClearCartResponseDto.cs
 create mode 100644 VietCommerce.Core/DTOs/Cart/GetCartResponseDto.cs
 create mode 100644 VietCommerce.Core/DTOs/Cart/UpdateCartItemResponseDto.cs
 create mode 100644 VietCommerce.Core/DTOs/Products/ProductCreateDto.cs
 create mode 100644 VietCommerce.Core/DTOs/Products/ProductDetailDto.cs
 create mode 100644 VietCommerce.Core/DTOs/Products/ProductListDto.cs
 create mode 100644 VietCommerce.Core/Entities/Core_Entities.txt
 create mode 100644 VietCommerce.Core/Helpers/SlugHelper.cs
 create mode 100644 VietCommerce.Core/Models/CacheInvalidationMessage.cs
 create mode 100644 VietCommerce.Core/Models/SessionData.cs
 rename VietCommerce_Folders.txt => VietCommerce.Core/VietCommerce_Folders.txt (100%)
 create mode 100644 VietCommerce.Data/Datafile_list.txt
 create mode 100644 VietCommerce.Data/Migrations/20251019081724_UpdateWithMoreProductfolderEntitiesAddProductFeaturesAndStats.Designer.cs
 create mode 100644 VietCommerce.Data/Migrations/20251019081724_UpdateWithMoreProductfolderEntitiesAddProductFeaturesAndStats.cs
 create mode 100644 VietCommerce.Data/Migrations/20251019153740_addTenantId.Designer.cs
 create mode 100644 VietCommerce.Data/Migrations/20251019153740_addTenantId.cs
 create mode 100644 VietCommerce.Data/Migrations/20251019175420_AddTenantRelationToInventory.Designer.cs
 create mode 100644 VietCommerce.Data/Migrations/20251019175420_AddTenantRelationToInventory.cs
 create mode 100644 VietCommerce.Data/Migrations/20251022130604_UpdateProductEntitiesFixed.Designer.cs
 create mode 100644 VietCommerce.Data/Migrations/20251022130604_UpdateProductEntitiesFixed.cs
 create mode 100644 VietCommerce.Data/Migrations/20251022145930_FixUserRoleCompositeKey.Designer.cs
 create mode 100644 VietCommerce.Data/Migrations/20251022145930_FixUserRoleCompositeKey.cs
 create mode 100644 VietCommerce.Data/Migrations/20251023115753_Add_IsMain_ProductImageEntities.Designer.cs
 create mode 100644 VietCommerce.Data/Migrations/20251023115753_Add_IsMain_ProductImageEntities.cs
 create mode 100644 VietCommerce.Data/Migrations/20251025114954_Fix_CustomerForeignKey.Designer.cs
 create mode 100644 VietCommerce.Data/Migrations/20251025114954_Fix_CustomerForeignKey.cs
 create mode 100644 VietCommerce.Data/Repositories/CartRepository.cs
 create mode 100644 VietCommerce.Data/Repositories/Interfaces/ICartRepository.cs
 create mode 100644 VietCommerce.Data/Seeds/CartOrderPermissionSeed.cs
 create mode 100644 VietCommerce.Data/Seeds/Seeders/CartOrderPermissionSeeder.cs
 create mode 100644 VietCommerce.Data/Seeds/Seeders/ProductAnalyticsSeeder.cs
 create mode 100644 VietCommerce.Tests/Base/TestBase.cs
 create mode 100644 VietCommerce.Tests/Repositories/CartRepositoryTests.cs
 create mode 100644 VietCommerce.Tests/Repositories/ProductRepositoryTests.cs
 create mode 100644 VietCommerce.Tests/SeedData/TestDataSeeder.cs
PM> 